using CompanyEmployees.Extensions;
using Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using CompanyEmployees.ActionFilters;
using Entities.DataTransferObjects;
using Repository.DataShaping;

namespace CompanyEmployees
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat
                (
                Directory.GetCurrentDirectory(),
                "/nlog.config")
                );
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime.
        //Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();
            services.CongigureIISIntegration();
            services.ConfigureLoggerService();
            services.ConfigureRepositoryManager();
            services.ConfigureSqlContext(Configuration);
            services.ConfigureAutoMapper();
            services.AddControllers();
            services.AddScoped<ValidateEmployeForCompanyExistsAttribute>();
            services.AddScoped<ValidationFilterAttribute>();
            services.AddScoped<IDataShaper<EmployeeDto>,DataShaper<EmployeeDto>>();
            services.Configure<ApiBehaviorOptions>(options => {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
                config.CacheProfiles.Add("120SecodsDuration", new CacheProfile
                {
                    Duration = 120
                });

            }).AddNewtonsoftJson()
                .AddXmlSerializerFormatters()
                .AddCustomCSVFormater();
            services.ConfigureVersioning();
            services.ConfigureResponseCashing();
            services.ConfigureHttpCacheHeaders();
        }

        // This method gets called by the runtime.
        //Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env, 
            ILoggerManager logger
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.ConfigureExceptionHandler(logger);
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All
                }
            );

            app.UseResponseCaching();
            app.UseHttpCacheHeaders();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
