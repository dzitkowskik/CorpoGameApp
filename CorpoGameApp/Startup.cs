using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CorpoGameApp.Data;
using CorpoGameApp.Models;
using CorpoGameApp.Services;
using CorpoGameApp.Properties;
using System;
using Hangfire;
using CorpoGameApp.Logic;
using CorpoGameApp.Hubs;
using Microsoft.AspNetCore.Identity;

namespace CorpoGameApp
{
    public class Startup
    {
        /* Migration from .net core 1.x to 2.x
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();

            _environment = env;
        }

        public IConfigurationRoot Configuration { get; }
        */

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private IHostingEnvironment _environment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbConnectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(dbConnectionString));
            
            // Identity settings
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = false;      
            });

            // Additional Libraries
            services.AddHangfire(config => config.UseSqlServerStorage(dbConnectionString));
            services.AddSignalR();

            // MVC
            services.AddMvc();

            // Add Game Settings
            services.Configure<GameSettings>(Configuration.GetSection("GameSettings"));
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            // Add application services.
            services.AddTransient<IGameServices, GameServices>();
            services.AddTransient<IPlayerQueueService, PlayerQueueService>();
            services.AddTransient<IPlayerServices, PlayerServices>();
            services.AddTransient<IStatisticsServices, StatisticsServices>();
            services.AddTransient<IEmailServices, EmailServices>();
            services.AddTransient<IGameLogic, GameLogic>();
            services.AddSingleton<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory, 
            ApplicationDbContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddAzureWebAppDiagnostics();

            var logger = loggerFactory.CreateLogger("Startup.Configure");

            if (env.IsDevelopment())
            {
                logger.LogInformation("Using development environment");
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                logger.LogInformation("Using production environment");
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseHangfireServer();
            app.UseWebSockets();
            app.UseSignalR(routes =>
            {
                routes.MapHub<GameQueueHub>("gameQueueHub");
            });
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Game}/{action=Index}");
            });
        }
    }
}
