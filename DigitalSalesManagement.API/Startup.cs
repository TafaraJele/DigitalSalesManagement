using DigitalSalesManagement.Abstractions;
using DigitalSalesManagement.Abstractions.Repositories;
using DigitalSalesManagement.Abstractions.Services;
using DigitalSalesManagement.Core.Services;
using DigitalSalesManagement.Infrastructure.Data.SQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DigitalSalesManagement.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().Enrich.WithExceptionDetails().ReadFrom.Configuration(configuration).CreateLogger();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            }
           );
            services.AddControllers().AddXmlSerializerFormatters();
            services.AddControllers().AddNewtonsoftJson();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DigitalSalesManagement.API", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.Configure<ApplicationSettings>(this.Configuration.GetSection("ApplicationSettings"));

            services.AddTransient<IAgentRepository, AgentRepository>();
            services.AddTransient<IAgentCommissionRepository, AgentCommissionRepository>();
            services.AddTransient<ICommisionPlanRepository, CommisionPlanRepository>();
            services.AddTransient<IAgentCalculatedCommissionRepository, AgentCalculatedCommissionRepository>();
            services.AddTransient<IDigitialSalesApplication, DigitialSalesApplication>();
            services.AddTransient<IAgentCommissionApprovalRepository, AgentCommissionApprovalRepository>();

            services.AddDbContext<SQLDBContext>(opts => opts.UseSqlServer(Configuration["ConnectionString:DSADB"]));

            //add httpclient
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            string virtualDirectory = Configuration.GetValue<string>("ApplicationSettings:VirtualDirectory");
           
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint(virtualDirectory +"/swagger/v1/swagger.json", "DigitalSalesManagement.API v1"));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddSerilog();

           // app.UseHttpsRedirection();
           
            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
