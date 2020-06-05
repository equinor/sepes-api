using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sepes.Infrastructure.Exceptions;
using Sepes.RestApi.Util;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Sepes.RestApi.Middelware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> log /* other dependencies */)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;  
            var requestId = ApplicationInsightsUtil.GetOperationId();

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
                   requestId, ex, HttpStatusCode.InternalServerError);

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

        public static JsonResponse CreateErrorMessageResult(string requestId, string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            var content = JsonConvert.SerializeObject(new
            {
                Message = message,
                RequestId = requestId
            });

            return new JsonResponse { Content = content, StatusCode = statusCode };
        }

        public static JsonResponse CreateExceptionMessageResult(string requestId, Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return CreateErrorMessageResult(requestId, ex.Message, statusCode);
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
