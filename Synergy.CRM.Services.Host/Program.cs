using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Synergy.Common.Aws.Extensions;

namespace Synergy.CRM.Services.Host
{
    internal class Program
    {
        public static void Main() => WebHost.CreateDefaultBuilder().UseDefaultSettings<Startup>().Build().Run();
    }
}
