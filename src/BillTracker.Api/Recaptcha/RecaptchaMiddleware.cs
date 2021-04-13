using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BillTracker.Api.Recaptcha
{
    internal class RecaptchaMiddleware
    {
        private const string RecaptchaEndpoint = "https://www.google.com/recaptcha/api/siteverify";

        private readonly RequestDelegate _next;

        public RecaptchaMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IHttpClientFactory httpClientFactory, RecaptchaConfiguration configuration)
        {
            if (configuration.UseRecaptcha &&
                !await VerifyRecaptcha(httpContext, httpClientFactory, configuration))
            {
                return;
            }

            await _next(httpContext);
        }

        private static async Task<bool> VerifyRecaptcha(
            HttpContext httpContext,
            IHttpClientFactory httpClientFactory,
            RecaptchaConfiguration configuration)
        {
            var responseToken = httpContext.Request.Cookies["g-recaptcha-response"];
            if (string.IsNullOrEmpty(responseToken))
            {
                httpContext.Response.StatusCode = 400;
                httpContext.Response.ContentType = "text/plain";
                await httpContext.Response.WriteAsync("'g-recaptcha-response' cookie is missing");
                return false;
            }

            var client = httpClientFactory.CreateClient();
            var formContent = new MultipartFormDataContent
            {
                { new StringContent(configuration.Secret), "secret" },
                { new StringContent(responseToken), "response" },
            };

            var response = await client.PostAsync(RecaptchaEndpoint, formContent);
            var result = await response.Content.ReadAsAsync<RecaptchaVerifyResult>();

            if (!result.Success)
            {
                httpContext.Response.StatusCode = 429;
            }

            return result.Success;
        }
    }
}
