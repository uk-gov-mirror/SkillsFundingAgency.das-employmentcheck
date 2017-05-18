using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.EmploymentCheck.Application.Services.DASProviderEvents
{
    public class SubmissionEvents
    {
        IPaymentsEventsApiConfiguration _config;
        IPaymentsEventsApiClient _client;

        public SubmissionEvents()
        {
            _config = new PaymentsEventsApiConfiguration
            {
                ClientToken = "YOUR_JWT_TOKEN",
                ApiBaseUrl = "https://some-server/"
            };
            _client = new PaymentsEventsApiClient(_config);
        }

        protected int CurrentMaxEvent()
        {
            //TODO: Get max event id from database
            return 1234;
        }

        public async Task<Provider.Events.Api.Types.PageOfResults<SubmissionEvent>> GetEvents(int fromId, int page)
        { 
            return await _client.GetSubmissionEvents(sinceEventId: CurrentMaxEvent());
        }
    }
}
