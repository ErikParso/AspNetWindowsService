using AspWinService.Model;
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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

            app.UseMvc();

            RunServiceClient();

            CreateInstallationConfig();
        }

        private static void RunServiceClient()
        {
            var servicePath = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            var clientPath = Path.Combine(servicePath, "..", "Client", "AspWinServiceClient.exe");
            var ngClientPath = Path.Combine(servicePath, "..", "NgClient", "asp-win-service-ng-client.exe");
            ProcessExtensions.StartProcessAsCurrentUser(clientPath);
            ProcessExtensions.StartProcessAsCurrentUser(ngClientPath);
        }

        private static void CreateInstallationConfig()
        {
            if (!File.Exists(Constants.installedClientsFile))
            {
                File.WriteAllText(Constants.installedClientsFile, JsonConvert.SerializeObject(new ClientInstallationInfo[] { }));
            }
        }
    }
}
