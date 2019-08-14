using AspWinService.Model;
using AspWinService.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace AspWinService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup));

            services.AddSingleton(typeof(ClientInfoService));
            services.AddSingleton(typeof(TrayClientService));
            services.AddSingleton(typeof(DownloadService));
            services.AddSingleton(typeof(RuntimeVersionDetectorService));
            services.AddSingleton(typeof(UpdateProcessorService));
            services.AddSingleton(typeof(ManifestService));
            services.AddSingleton(typeof(ProgressService));


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("MyPolicy");

            app.UseMvc();

            RunServiceClient();

            CreateInstallationConfig();
        }

        private static void RunServiceClient()
        {
            var servicePath = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            var ngClientPath = Path.Combine(servicePath, "..", "NgClient", "asp-win-service-ng-client.exe");
            ProcessExtensions.StartProcessAsCurrentUser(ngClientPath);
        }

        private static void CreateInstallationConfig()
        {
            if (!File.Exists(Constants.InstalledClientsFileName))
            {
                File.WriteAllText(Constants.InstalledClientsFileName, JsonConvert.SerializeObject(new ClientInfo[] { }));
            }
        }
    }
}
