using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SFA.DAS.EmploymentCheck.AcceptanceTests
{
    public static class WebJobsBuilderExtensions
    {
        public static IHostBuilder ConfigureServices(this IHostBuilder builder, Action<IServiceCollection> configure)
        {
            return builder.ConfigureServices((_, s) => configure(s));
        }
    }
}