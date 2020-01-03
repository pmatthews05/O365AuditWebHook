using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AuditWebHook.Entities;
using AuditWebHook.Queue;
using AuditWebHook.WebHooks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AuditWebHook
{
    public static class O365AuditWebHook
    {
        private static CloudQueue AuditContentUriQueue = null;

        [FunctionName("AuditWebHook")]
        public static async Task<HttpResponseMessage> AuditWebHookTrigger([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Audit webhook triggered");
            string stringvalue = await req.Content.ReadAsStringAsync();
            log.LogInformation($"Req.Content {stringvalue}");
            try
            {
                log.LogInformation("Getting validation code");
                dynamic data = await req.Content.ReadAsAsync<object>();
                string validationToken = data.validationCode.ToString();
                log.LogInformation($"Validation Token: {validationToken} received");
                HttpResponseMessage response = req.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(validationToken);
                return response;
            }
            catch (Exception)
            {
                log.LogInformation("No ValidationCode, therefore process WebHook as content");
            }

            log.LogInformation($"Audit Logs triggered the webhook");
            string content = await req.Content.ReadAsStringAsync();
            log.LogInformation($"Received following payload: {content}");

            List<AuditContentEntity> auditContents = JsonConvert.DeserializeObject<List<AuditContentEntity>>(content);

            foreach (var auditcontent in auditContents)
            {
                if (AuditContentUriQueue == null)
                {
                    string cloudStorageAccountConnectionString = System.Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cloudStorageAccountConnectionString);
                    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                    AuditContentUriQueue = queueClient.GetQueueReference("auditcontenturi");
                    await AuditContentUriQueue.CreateIfNotExistsAsync();
                  }

                log.LogInformation($"Content Queue Message: {auditcontent.ContentUri}");
                AuditContentQueue acq = new AuditContentQueue
                {
                    ContentType = auditcontent.ContentType,
                    ContentUri = auditcontent.ContentUri,
                    TenantID = auditcontent.TenantId
                };
                string message = JsonConvert.SerializeObject(acq);
                log.LogInformation($"Adding a message to the queue. Message content: {message}");
                await AuditContentUriQueue.AddMessageAsync(new CloudQueueMessage(message));
                log.LogInformation($"Message added");
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [FunctionName("AuditContentUri")]
        public static async Task AuditContentUri([QueueTrigger("auditcontenturi", Connection = "AzureWebJobsStorage")]AuditContentQueue auditContentQueue, ExecutionContext exCtx, ILogger log)
        {

            log.LogInformation($"Reading in Audit ContentQueue for : {auditContentQueue.ContentType} Tenant: {auditContentQueue.TenantID} ContentUri:{auditContentQueue.ContentUri} ");

            //GetLogs
                                   
           
            }
        }
    }
}
