using AuditWebHook.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuditWebHook.Utilities
{
    public class RestAPI
    {
        public static async Task<AuditRestDataEntity> PostRestDataAsync(string restEndpoint, string token, string postData = null, int retryCount = 5, int delay = 500, string userAgent = null, ILogger Log = null)
        {
            AuditRestDataEntity restData = new AuditRestDataEntity();

            if (retryCount <= 0)
                throw new ArgumentException("Provide a retry count greater than zero.");

            if (delay <= 0)
                throw new ArgumentException("Provide a delay greater than zero.");

            int retryAttempts = 0;
            int backoffInterval = delay;
            
            var request = (HttpWebRequest)WebRequest.Create(restEndpoint);

            var tenant = System.Environment.GetEnvironmentVariable("Tenant", EnvironmentVariableTarget.Process);
            // set headers as appropriate
            request.Method = "POST";
            request.ContentLength = 0;
            request.UserAgent = string.IsNullOrEmpty(userAgent) ? $"NONISV|{tenant}|AuditLogs/1.0" : userAgent;
            request.Headers.Add("Authorization", "Bearer " + token);

            request.Accept = "application/json;";
            request.ContentType = "application/json;odata=verbose";

            if (!string.IsNullOrEmpty(postData))
            {
                request.ContentLength = postData.Length;
                byte[] byteData = Encoding.UTF8.GetBytes(postData);
                Stream requestDataStream = request.GetRequestStream();
                requestDataStream.Write(byteData, 0, byteData.Length);
            }

            while (retryAttempts < retryCount)
            {
                try
                {
                    using (WebResponse response = await request.GetResponseAsync())
                    {
                        restData.WebHeaderCollections = response.Headers;
                        using (Stream stream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                restData.RestResponse = await reader.ReadToEndAsync();
                            }
                        }
                    }
                    return restData;
                }
                catch (WebException webException)
                {
                    if (Log != null)
                    {
                        Log.LogInformation("There has been a webException error calling RESTAPI");
                        Log.LogInformation(webException.Message);
                    }
                    HttpWebResponse response = (HttpWebResponse)webException.Response;
                    if (Log != null)
                    {
                        Log.LogInformation(response.StatusCode.ToString());
                    }

                    int statusCode = (int)response.StatusCode;
                    if (statusCode == 429)
                    {
                        Thread.Sleep(backoffInterval);
                        retryAttempts++;
                        backoffInterval *= 2;
                    }
                    else
                    {
                        throw webException;
                    }
                }
            }

            return restData;
        }

        public static async Task<AuditRestDataEntity> GetRestDataAsync(string restEndpoint, string token, int retryCount = 10, int delay = 500, string userAgent = null)
        {

            AuditRestDataEntity restData = new AuditRestDataEntity();

            if (retryCount <= 0)
                throw new ArgumentException("Provide a retry count greater than zero.");

            if (delay <= 0)
                throw new ArgumentException("Provide a delay greater than zero.");

            int retryAttempts = 0;
            int backoffInterval = delay;

            var request = (HttpWebRequest)WebRequest.Create(restEndpoint);
            var tenant = System.Environment.GetEnvironmentVariable("Tenant", EnvironmentVariableTarget.Process);

            // set headers as appropriate
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + token);
            request.Accept = "application/json;";
            request.UserAgent = string.IsNullOrEmpty(userAgent) ? $"NONISV|{tenant}|AuditLogs/1.0" : userAgent;

            while (retryAttempts < retryCount)
            {
                try
                {
                    using (WebResponse response = await request.GetResponseAsync())
                    {
                        restData.WebHeaderCollections = response.Headers;
                        using (Stream stream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                restData.RestResponse = await reader.ReadToEndAsync();
                            }
                        }
                    }
                    return restData;
                }
                catch (WebException webException)
                {
                    HttpWebResponse response = (HttpWebResponse)webException.Response;
                    int statusCode = (int)response.StatusCode;
                    if (statusCode == 429)
                    {
                        Thread.Sleep(backoffInterval);
                        retryAttempts++;
                        backoffInterval *= 2;
                    }
                    else
                    {
                        throw webException;
                    }
                }
            }

            return restData;
        }
    }
}
