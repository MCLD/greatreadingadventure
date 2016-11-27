using AutoMapper;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace GRA.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            if (env.IsDevelopment())
            {
                Configuration["ConnectionStrings:DefaultConnection"] = @"Server=(localdb)\mssqllocaldb;Database=gra4;Trusted_Connection=True;MultipleActiveResultSets=true";
                //Configuration["ConnectionStrings:DefaultConnection"] = @"Filename=./gra4.db";
            }
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddSession(_ => _.IdleTimeout = System.TimeSpan.FromMinutes(30));
            services.AddSingleton(_ => Configuration);
            services.AddMvc();

            services.AddAuthorization(options =>
            {
                foreach (var permisisonName in Enum.GetValues(typeof(Domain.Model.Permission)))
                {
                    options.AddPolicy(permisisonName.ToString(),
                        _ => _.RequireClaim(ClaimType.Permission, permisisonName.ToString()));
                }
            });

            // service facades
            services.AddScoped<Controllers.ServiceFacade.Controller, Controllers.ServiceFacade.Controller>();
            services.AddScoped<Data.ServiceFacade.Repository, Data.ServiceFacade.Repository>();

            // database
            services.AddScoped<Data.Context, Data.SqlServer.SqlServerContext>();
            //services.AddScoped<Data.Context, Data.SQLite.SQLiteContext>();

            // utilities
            services.AddScoped<Security.Abstract.IPasswordHasher, Security.PasswordHasher>();

            // services
            services.AddScoped<Domain.Service.ActivityService, Domain.Service.ActivityService>();
            services.AddScoped<Domain.Service.ChallengeService, Domain.Service.ChallengeService>();
            services.AddScoped<Domain.Service.ConfigurationService, Domain.Service.ConfigurationService>();
            services.AddScoped<Domain.Service.SiteService, Domain.Service.SiteService>();
            services.AddScoped<Domain.Service.UserService, Domain.Service.UserService>();

            // repositories
            services.AddScoped<Domain.Repository.IBranchRepository, Data.Repository.BranchRepository>();
            services.AddScoped<Domain.Repository.IChallengeRepository, Data.Repository.ChallengeRepository>();
            services.AddScoped<Domain.Repository.IChallengeTaskRepository, Data.Repository.ChallengeTaskRepository>();
            services.AddScoped<Domain.Repository.IPointTranslationRepository, Data.Repository.PointTranslationRepository>();
            services.AddScoped<Domain.Repository.IProgramRepository, Data.Repository.ProgramRepository>();
            services.AddScoped<Domain.Repository.IRoleRepository, Data.Repository.RoleRepository>();
            services.AddScoped<Domain.Repository.ISiteRepository, Data.Repository.SiteRepository>();
            services.AddScoped<Domain.Repository.ISystemRepository, Data.Repository.SystemRepository>();
            services.AddScoped<Domain.Repository.IUserLogRepository, Data.Repository.UserLogRepository>();
            services.AddScoped<Domain.Repository.IUserRepository, Data.Repository.UserRepository>();

            services.AddAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.ApplicationServices.GetService<Data.Context>().Migrate();

            app.UseStaticFiles();

            app.UseSession();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = Controllers.Authentication.SchemeGRACookie,
                LoginPath = new PathString("/MissionControl/Login/"),
                AccessDeniedPath = new PathString("/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: null,
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: null,
                    template: "{sitePath}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" },
                    constraints: new { site = new RouteConstraints.SiteRouteConstraint() });
                routes.MapRoute(
                    name: null,
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
