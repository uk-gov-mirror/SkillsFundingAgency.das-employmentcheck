using System;
using System.Threading.Tasks;
using System.Web;
using SFA.DAS.EmploymentCheck.Application.Interfaces;
using SFA.DAS.EmploymentCheck.Application.Configuration;
using SFA.DAS.EmploymentCheck.Domain.Models;
using SFA.DAS.EmploymentCheck.Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace SFA.DAS.EmploymentCheck.Infrastructure.Services
{
    public class HmrcService: IHmrcService 
    {
        private readonly HmrcConfiguration _configuration;
        readonly IHttpClientWrapper _httpClientWrapper;
        //private readonly ITotpService _totpService;

        public HmrcService(HmrcConfiguration configuration, IHttpClientWrapper httpClientWrapper)
        {
            //_logger = logger;
            _configuration = configuration;
            _httpClientWrapper = httpClientWrapper;
            //_totpService = totpService;
        }

        public string GenerateAuthRedirectUrl(string redirectUrl)
        {
            var urlFriendlyRedirectUrl = HttpUtility.UrlEncode(redirectUrl);
            return $"{_configuration.BaseUrl}oauth/authorize?response_type=code&client_id={_configuration.ClientId}&scope={_configuration.Scope}&redirect_uri={urlFriendlyRedirectUrl}";
        }

        public async Task<HmrcTokenResponse> GetAuthenticationToken(string redirectUrl, string accessCode)
        {
            var urlFriendlyRedirectUrl = HttpUtility.UrlEncode(redirectUrl);

            var url = $"oauth/token?client_secret={_configuration.ClientSecret}&client_id={_configuration.ClientId}&grant_type=authorization_code&redirect_uri={urlFriendlyRedirectUrl}&code={accessCode}";

            var response = await _httpClientWrapper.SendMessage("", url);

            return JsonConvert.DeserializeObject<HmrcTokenResponse>(response);
        }

        public async Task<HmrcTokenResponse> GetOgdAuthenticationToken()
        {
            var code = _totpService.GetCode();
            var url = $"oauth/token?client_secret={code}&client_id={_configuration.OgdClientId}&grant_type=client_credentials&scopes=read:apprenticeship-levy";

            var response = await _httpClientWrapper.SendMessage("", url);

            return JsonConvert.DeserializeObject<HmrcTokenResponse>(response);
        }

        public async Task<HmrcEmploymentResponse> GetEmploymentStatus(string empRef, string nino, DateTime fromDate, DateTime toDate)
        {
            var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/employment-check?nino={HttpUtility.UrlEncode(nino)}&fromDate={fromDate.ToShortDateString()}&toDate={toDate.ToShortDateString()}";

            return await _httpClientWrapper.Get<HmrcEmploymentResponse>(_configuration.ServerToken, url);
        }
    }
}
