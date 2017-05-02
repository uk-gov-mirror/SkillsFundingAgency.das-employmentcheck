using SFA.DAS.EmploymentCheck.Application.Configuration;
using SFA.DAS.EmploymentCheck.Domain.Interfaces;
using Simonbu11.Otp;
using Simonbu11.Otp.Totp;
using System;
using System.Globalization;

namespace SFA.DAS.EmploymentCheck.Infrastructure.Services
{
    public class TotpService : ITotpService
    {
        private readonly string  _ogdSecret;

        public TotpService(string ogdSecret)
        {
            _ogdSecret = ogdSecret;
        }

        public string GetCode(string timeValue = "")
        {
            var generator = new HmacSha512TotpGenerator(new TotpGeneratorSettings
            {
                SharedSecret = OtpSharedSecret.FromBase32String(_ogdSecret)
            });

            var time = string.IsNullOrEmpty(timeValue)
                ? DateTime.UtcNow
                : DateTime.ParseExact(timeValue, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None);
            var result = generator.Generate(time);

            return result;
        }

    }
}

