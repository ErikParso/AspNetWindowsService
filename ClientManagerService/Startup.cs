using ClientManagerService.Exceptions;
using ClientManagerService.Model;
using ClientManagerService.Services;
using ClientManagerService.Services.QueuedTasksService;
using ClientManagerService.SignalR;
using ClientManagerService.SignalR.Rpc;
using ClientManagerService.SignalR.RpcHubs;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace ClientManagerService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
                options.Filters.Add(new HttpResponseExceptionFilter()));

            services.AddSignalR();
            services.AddMediatR(typeof(Startup));

            services.AddSingleton(typeof(ClientInfoService));
            services.AddSingleton(typeof(TrayClientService));
            services.AddScoped(typeof(DownloadService));
            services.AddSingleton(typeof(RuntimeVersionDetectorService));
            services.AddSingleton(typeof(UpdateProcessorService));
            services.AddSingleton(typeof(ManifestService));
            services.AddSingleton(typeof(ProgressService));
            services.AddScoped(typeof(CheckNewVersionService));
            services.AddScoped(typeof(RedirectService));
            services.AddSingleton(typeof(CurrentUserService));
            services.AddSingleton(typeof(LinkService));
            services.AddSingleton(typeof(ProxyService));
            services.AddScoped(typeof(CredentialService));
            services.AddSingleton(typeof(RpcHub<RpcLoginRequest, RpcLoginResponse>));
            services.AddSingleton(typeof(RpcHub<AcceptCertificateRpcRequest, AcceptCertificateRpcResponse>));
            services.AddScoped(typeof(CertificateValidationService));
            services.AddScoped(typeof(CertificateValidator));
            services.AddSingleton(typeof(ClientConfigUpdateService));
            services.AddSingleton(typeof(ClientNameService));
            services.AddScoped(typeof(ClientAutoUpgradeService));
            services.AddScoped(typeof(QueuedHostedService));
            services.AddSingleton(typeof(ClientLockService));
            services.AddScoped(typeof(ConnectionInfoService));
            services.AddSingleton(typeof(ClientManagerSettingsService));

            // Background services
            services.AddHostedService<ClientAutoUpgradeService>();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins("http://localhost:4200")
                    .AllowCredentials();
            }));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Client Manager Service", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/clientmanagerlog-{Date}.txt", LogLevel.Information);

            app.UseDeveloperExceptionPage();

            app.UseHsts();

            app.UseCors("MyPolicy");

            app.UseRouting();

            app.UseEndpoints(c =>
            {
                c.MapControllers();
                c.MapHub<ProgressHub>("/progresshub");
                c.MapHub<AutoActualizationHub>("/autoactualizationhub");
                c.MapHub<RpcHub<RpcLoginRequest, RpcLoginResponse>>("/loginrpc");
                c.MapHub<RpcHub<AcceptCertificateRpcRequest, AcceptCertificateRpcResponse>>("/acceptcert");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Client Manager Service V1");
            });

            CreateInstallationConfig();
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
