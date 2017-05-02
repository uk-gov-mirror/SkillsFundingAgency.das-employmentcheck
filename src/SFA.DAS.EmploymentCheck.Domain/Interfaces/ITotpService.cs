namespace SFA.DAS.EmploymentCheck.Domain.Interfaces
{
    public interface ITotpService
    {
        string GetCode(string timeValue = "");
    }
}