using NUnit.Framework;
using Moq;
using SFA.DAS.EmploymentCheck.Application.Interfaces;
using SFA.DAS.EmploymentCheck.Application.Services;
using SFA.DAS.EmploymentCheck.Application.Configuration;

namespace SFA.DAS.EmploymentCheck.Application.UnitTests.Services
{
    public abstract class HmrcServiceTestBase
    {
        //protected Mock<IHmrcService> HmrcService;
        protected HmrcConfiguration  _configuration;
        protected HttpClientWrapper _httpClientwrapper;
        protected TotpService _totpService;

        protected Application.Services.HmrcService _HmrcService;

        [SetUp]
        public void Arrange()
        {
            //HmrcService = new Mock<IHmrcService>();
            

            var HmrcConfig = new HmrcConfiguration();
            HmrcConfig.BaseUrl = "http://"; //TODO Find test url for fake HMRC end point
            HmrcConfig.OgdClientId = "SFA.DAS.EmploymentCheck.Application.UnitTests";
            HmrcConfig.Scope = "???"; //TODO find appropriate scope value
            HmrcConfig.ServerToken = ""; //TODO find appropriate server token value




            _httpClientwrapper = new HttpClientWrapper(HmrcConfig);
            _totpService = new TotpService(HmrcConfig.OgdSecret);

            _HmrcService = new SFA.DAS.EmploymentCheck.Application.Services.HmrcService(_configuration, _httpClientwrapper, _totpService);
        }
    }
}
