using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MluviiBot
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            RegisterBotDependencies();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime,
            ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => Configuration);

            var credentialProvider = new StaticCredentialProvider(
                Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value,
                Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppPasswordKey)?.Value);

            services.AddAuthentication(
                    options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }
                )
                .AddBotAuthentication(credentialProvider);

            services.AddSingleton(typeof(ICredentialProvider), credentialProvider);

            services.AddMvc(options => { options.Filters.Add(typeof(TrustServiceUrlAttribute)); });

            
        }
        
        private void RegisterBotDependencies()
        {
            Conversation.UpdateContainer(builder =>
            {
                // Update the container to use the right MicorosftAppCredentials based on
                // Identity set by BotAuthentication
                var appCredentials = new MicrosoftAppCredentials(
                    Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value,
                    Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppPasswordKey)?.Value);
                builder.Register(c => appCredentials)
                    .AsSelf()
                    .InstancePerLifetimeScope();
                builder.RegisterModule<MluviiBotModule>();
            });
        }
    }
}