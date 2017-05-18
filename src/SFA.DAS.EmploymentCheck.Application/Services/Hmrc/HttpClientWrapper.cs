using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using System.Net.Http;
using System.Net.Http.Headers;
using SFA.DAS.EmploymentCheck.Application.Configuration;
using SFA.DAS.EmploymentCheck.Application.Interfaces;

namespace SFA.DAS.EmploymentCheck.Application.Services
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        //private readonly ILogger _logger;
        private readonly HmrcConfiguration _configuration;

        public HttpClientWrapper(HmrcConfiguration configuration) //ILogger logger, EmployerApprenticeshipsServiceConfiguration configuration)
        {
            //_logger = logger;
            //TODO Logging
            _configuration = configuration;
        }

        public async Task<string> SendMessage<T>(T content, string url)
        {
            try
            {
                using (var httpClient = CreateHttpClient())
                {

                    var serializeObject = JsonConvert.SerializeObject(content);
                    var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(serializeObject, Encoding.UTF8, "application/json")
                    });

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                //_logger.Error(ex);
            }

            return null;
        }

        public async Task<T> Get<T>(string authToken, string url)
        {
            try
            {
                using (var httpClient = CreateHttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json"));
                    var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));

                    return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                //_logger.Error(ex);
            }

            return default(T);
        }

        private HttpClient CreateHttpClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri(_configuration.BaseUrl)

            };
        }
    }
}
