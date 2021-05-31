using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sepes.Common.Exceptions;
using Sepes.Common.Interface;
using Sepes.Common.Util;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Sepes.RestApi.Middelware
{
    public class ErrorHandlingMiddleware
    {
        readonly RequestDelegate next;
        readonly IRequestIdService _requestIdService;

        public ErrorHandlingMiddleware(RequestDelegate next, IRequestIdService requestIdService)
        {
            this.next = next;
            _requestIdService = requestIdService;
        }

        public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> log /* other dependencies */)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;  
            var requestId = _requestIdService.GetRequestId();

            try
            {
                await next(context);
            }
            catch (NotFoundException ex)
            {
                LogHelper.LogError(log, ex, path, method);

                var result = JsonExceptionResultFactory.CreateExceptionMessageResult(
                   requestId, ex, HttpStatusCode.NotFound);

                await HandleExceptionAsync(context, result.Content, result.StatusCode);
            }
            catch (ForbiddenException ex)
            {
                LogHelper.LogError(log, ex, path, method);

                var result = JsonExceptionResultFactory.CreateExceptionMessageResult(
                  requestId, ex, HttpStatusCode.Forbidden);

                await HandleExceptionAsync(context, result.Content, result.StatusCode);
            }          
            catch (ArgumentException ex)
            {
                LogHelper.LogError(log, ex, path, method);

                var result = JsonExceptionResultFactory.CreateExceptionMessageResult(
                   requestId, ex, HttpStatusCode.BadRequest);

                await HandleExceptionAsync(context, result.Content, result.StatusCode);
            }
            catch (BadRequestException ex)
            {
                LogHelper.LogError(log, ex, path, method);

                var result = JsonExceptionResultFactory.CreateExceptionMessageResult(
                   requestId, ex, HttpStatusCode.BadRequest);

                await HandleExceptionAsync(context, result.Content, result.StatusCode);
            }
            catch (TaskCanceledException ex)
            {
                log.LogInformation($"Task was cancelled: {method} {path}, requestId: {requestId}");

                var result = JsonExceptionResultFactory.CreateExceptionMessageResult(
                   requestId, ex, HttpStatusCode.Continue);

                await HandleExceptionAsync(context, result.Content, result.StatusCode);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(log, ex, path, method);

                var result = JsonExceptionResultFactory.CreateExceptionMessageResult(
                    requestId, ex, HttpStatusCode.InternalServerError);

                await HandleExceptionAsync(context, result.Content, result.StatusCode);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, string error, HttpStatusCode status = HttpStatusCode.InternalServerError)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(error);
        }
    }

    public class JsonResponse
    {
        public string Content { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }

    static class JsonExceptionResultFactory
    {    
        
        public static JsonResponse CreateExceptionMessageResult(string requestId, Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            var userMessage = ex.Message;

            if (ex is CustomUserMessageException)
            {
                var exceptionConverted = ex as CustomUserMessageException;

                if (!String.IsNullOrWhiteSpace(exceptionConverted.UserFriendlyMessage))
                {
                    userMessage = exceptionConverted.UserFriendlyMessage;
                }

                if (exceptionConverted.StatusCode.HasValue)
                {
                    statusCode = exceptionConverted.StatusCode.Value;
                }
            }
            
            return CreateErrorMessageResult(requestId,userMessage , statusCode);
        }

        public static JsonResponse CreateErrorMessageResult(string requestId, string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            var content = JsonSerializerUtil.Serialize(new Common.Dto.ErrorResponse
            {
                Message = message,
                RequestId = requestId
            });

            return new JsonResponse { Content = content, StatusCode = statusCode };
        }
    }

    static class LogHelper
    {     
    
        public static void LogError(ILogger log, Exception ex, string where, string method, string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                log.LogError(ex, "{0}({1}) exception.", where, method);
            }
            else
            {
                log.LogError(ex, "{0}({1}={2}) exception.", where, method, id);
            }
        }
    }
}
