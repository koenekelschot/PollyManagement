using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using PollyManagement.ServiceCollection;

namespace PollyManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddPollyPolicy("Ideal-Policy", policy);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        private readonly AsyncCircuitBreakerPolicy policy = Policy.Handle<Exception>()
            .AdvancedCircuitBreakerAsync(failureThreshold: 0.5,
                samplingDuration: TimeSpan.FromMinutes(2),
                minimumThroughput: 2,
                durationOfBreak: TimeSpan.FromMinutes(5),
                onBreak: (Exception e, TimeSpan span) =>
                {
                    //Policy did break
                },
                onReset: () =>
                {
                    //Policy did reset
                });
    }
}
