using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmploymentCheck.Domain.Models;
using SFA.DAS.EmploymentCheck.Application.Services;

namespace SFA.DAS.EmploymentCheck.Application.UnitTests.Services.HmrcService
{
    [TestFixture]
    public class WhenEmploymentCheckRequested: HmrcServiceTestBase
    {
        [Test]
        public async Task ThenEmployedStatusIsTrue()
        {

            var HmrcService = new SFA.DAS.EmploymentCheck.Application.Services.HmrcService(_configuration , _httpClientwrapper , _totpService);

            var employee = new HmrcEmploymentCheckRequest();
            employee.Empref = "123/AB12345";
            employee.FromDate = new DateTime(2017,05, 01);
            employee.Nino = "AB123456A";
                        
            var result = await HmrcService.GetEmploymentStatus(employee.Empref, employee.Nino, employee.FromDate, employee.ToDate);

            Assert.IsTrue(result.Employed);

        }

        [Test]
        public async Task ThenEmployedStatusIsFalse()
        {
            var employee = new HmrcEmploymentCheckRequest();
            employee.Empref = "123/AB12346";
            employee.FromDate = new DateTime(2017, 05, 01);
            employee.Nino = "AB123456A";
            
            var result = await _HmrcService.GetEmploymentStatus(employee.Empref, employee.Nino, employee.FromDate, employee.ToDate);

            Assert.IsFalse(result.Employed);
        }

        [Test]
        public async Task ThenEmployeeNotFound()
        {
            var employee = new HmrcEmploymentCheckRequest();
            employee.Empref = "123/AB12345";
            employee.FromDate = new DateTime(2017, 05, 01);
            employee.Nino = "ZZ654321Z";
            
            var result = await _HmrcService.GetEmploymentStatus(employee.Empref, employee.Nino, employee.FromDate, employee.ToDate);

            Assert.IsFalse(result.Employed);
        }
    }
}
