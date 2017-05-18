using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmploymentCheck.Infrastructure.UnitTests.Data;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.EmploymentCheck.Infrastructure.Data;

namespace SFA.DAS.EmploymentCheck.Infrastructure.IntegrationTests.Data
{
    [TestFixture]
    public class WhenSubmissionEventsRepository : BaseDataTest
    {

        [Test]
        public async Task ThenSubssionEventsAreStored()
        {
            var repo = new SubmissionsRespository(_connectionString);
            var nino = "ab123456z";
            var eventid = 123;

            var submissionsEvent = new SubmissionEvent();
            submissionsEvent.Id = eventid;
            submissionsEvent.NiNumber = nino;
            submissionsEvent.IlrFileName = "IntegrationTest-"+TestContext.CurrentContext.Test.Name;
            submissionsEvent.Uln = 1234567;
            submissionsEvent.AcademicYear = "1617";
            submissionsEvent.ApprenticeshipId = 1;
            submissionsEvent.SubmittedDateTime = DateTime.Now;

            var submissionsPage = new PageOfResults<SubmissionEvent>();
            submissionsPage.Items = new SubmissionEvent[1];
            submissionsPage.Items[0] = submissionsEvent;
            submissionsPage.PageNumber = 1;
            submissionsPage.TotalNumberOfPages = 1;

            await repo.StoreSubmissionEvents<SubmissionEvent>(submissionsPage);

            var result = await repo.GetSubmissionEvent<SubmissionEvent>(eventid);
            
            Assert.That(result.Id, Is.EqualTo(eventid));
            
        }


    }
}
