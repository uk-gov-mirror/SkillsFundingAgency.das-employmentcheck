using System;
using System.Threading.Tasks;
using SFA.DAS.EmploymentCheck.Domain.Models;

namespace SFA.DAS.EmploymentCheck.Application.Interfaces
{
    public interface IHmrcService
    {
        string GenerateAuthRedirectUrl(string redirectUrl);

        Task<HmrcTokenResponse> GetAuthenticationToken(string redirectUrl, string accessCode);
        Task<HmrcEmploymentResponse> GetEmploymentStatus(string empRef, string nino, DateTime fromDate, DateTime toDate);
        Task<HmrcTokenResponse> GetOgdAuthenticationToken();
    }
}
