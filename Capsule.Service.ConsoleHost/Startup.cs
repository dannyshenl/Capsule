using Capsule.Service.ConsoleHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace XBMall.Server.Image
{
    class Startup : StartupBase
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CapsuleOptions>(_configuration);
            services.AddSingleton<CapsuleDispatch>();
        }

        public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            var typeOfICapsuleInstance = typeof(ICapsuleInstance);
            var typeOfICapsuleInstances = typeof(Program).Assembly.GetTypes().Where(x => x != typeOfICapsuleInstance && typeOfICapsuleInstance.IsAssignableFrom(x));
            foreach (var instanceOfICapsuleInstance in typeOfICapsuleInstances)
            {
                services.AddSingleton(typeof(ICapsuleInstance), instanceOfICapsuleInstance);
            }
            return base.CreateServiceProvider(services);
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<CapsuleMiddleware>();
        }
    }
}
