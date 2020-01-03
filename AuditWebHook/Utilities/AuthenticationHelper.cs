using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuditWebHook.Utilities
{
    internal class AuthenticationHelper
    {
        internal static async Task<string> AcquireTokenForApplication()
        {
            var tenant = System.Environment.GetEnvironmentVariable("Tenant", EnvironmentVariableTarget.Process);
            var clientId = System.Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            var secret = System.Environment.GetEnvironmentVariable("Secret", EnvironmentVariableTarget.Process);
            var authorityUri = $"https://login.microsoftonline.com/{tenant}.onmicrosoft.com";
            var resourceUri = "https://manage.office.com";

            var microsoftToken = await GetTokenRetry(resourceUri, authorityUri, clientId, secret);

            return microsoftToken;
        }

        private static async Task<string> GetTokenRetry(string resourceUri, string authorityUri, string clientId, string secret, int retryCount = 5, int delay = 500)
        {
            var token = string.Empty;
            if (retryCount <= 0) throw new ArgumentException("Provide a retry count greater than zero.");
            if (delay <= 0) throw new ArgumentException("Provide a delay greater than zero.");

            int retryAttempts = 0;
            int backoffInterval = delay;

            while (retryAttempts < retryCount)
            {
                try
                {
                    AuthenticationContext authContext = new AuthenticationContext(authorityUri, false);
                    ClientCredential clientCred = new ClientCredential(clientId, secret);
                    var authenticationResult = await authContext.AcquireTokenAsync(resourceUri, clientCred);
                    token = authenticationResult.AccessToken;
                    return token;

                }
                catch (AuthenticationException)
                {
                    Thread.Sleep(backoffInterval);
                    retryAttempts++;
                    backoffInterval *= 2;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            throw new ApplicationException(string.Format("Maximum retry attempts {0}, has been attempted when getting Token", retryCount));
        }
    }
}
