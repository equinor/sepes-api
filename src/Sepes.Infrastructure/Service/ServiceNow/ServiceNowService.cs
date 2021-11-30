using AutoMapper;
using Sepes.Common.Dto.ServiceNow;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.ServiceNow.Interface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Sepes.Infrastructure.Service.ServiceNow
{
    public class ServiceNowService : IServiceNowService
    {
        public SepesDbContext _db;
        public IMapper _mapper;
        public ServiceNowService (SepesDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        public void ReportIncident(ReportIssueDto reportIssueDto)
        {
            var baseUrlServiceNow = "https://equinortest.service-now.com";

            //var test = new HttpRequestMessage();
            //test.RequestUri = new Uri(baseUrlServiceNow);

            //WebRequest 

            //TODO: Get ServiceNOW token

            try
            {
                var oAuthData = new ServiceNowAuthData
                {
                    client_id = "9e24a2f1f6966740415accd7f7daad49",
                    client_secret = "",
                    grant_type = "",
                    password = "",
                    username = ""
                };

                var url = "https://equinortest.service-now.com/oauth_token.do";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";

                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";

                //Get response with token
                var response = httpRequest.GetResponse();

                if (response != null)
                {
                    try
                    {
                        var urlServiceNow = "https://equinortest.service-now.com/api/x_stasa_robotproce/robotprocessing/incident";

                        var httpRequestServiceNow = (HttpWebRequest)WebRequest.Create(urlServiceNow);
                        httpRequestServiceNow.Method = "PUT";
                        httpRequestServiceNow.Accept = "application/json";
                        httpRequestServiceNow.ContentType = "application/json";
                        httpRequestServiceNow.Headers.Add("Authorization", "bearer" + response.ToString());

                        httpRequestServiceNow.GetResponse();
                    }
                    catch
                    {
                        throw new Exception("Could not report incident to service now");
                        //TODO handle error
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not get token needed to report incident to service now", e.InnerException);
                //TODO handle error
            }

            


            //using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(data);
            //}

            //var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();
            //}

            //Console.WriteLine(httpResponse.StatusCode);

            //TODO: Use token with a serviceNow request to create an issue

            throw new NotImplementedException();
        }
    }
}
