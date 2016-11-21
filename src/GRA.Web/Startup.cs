using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            services.AddScoped<Controllers.ServiceFacade.Controller, Controllers.ServiceFacade.Controller>();

            // database
            services.AddScoped<Data.Context, Data.SqlServer.SqlServerContext>();
            //services.AddScoped<Data.Context, Data.SQLite.SQLiteContext>();

            // services
            services.AddScoped<Domain.Service.ChallengeService, Domain.Service.ChallengeService>();
            services.AddScoped<Domain.Service.ConfigurationService, Domain.Service.ConfigurationService>();
            services.AddScoped<Domain.Service.SiteService, Domain.Service.SiteService>();

            // repositories
            services.AddScoped<Domain.Repository.IBranchRepository, Data.Repository.BranchRepository>();
            services.AddScoped<Domain.Repository.IChallengeRepository, Data.Repository.ChallengeRepository>();
            services.AddScoped<Domain.Repository.IChallengeTaskRepository, Data.Repository.ChallengeTaskRepository>();
            services.AddScoped<Domain.Repository.IProgramRepository, Data.Repository.ProgramRepository>();
            services.AddScoped<Domain.Repository.ISiteRepository, Data.Repository.SiteRepository>();
            services.AddScoped<Domain.Repository.ISystemRepository, Data.Repository.SystemRepository>();

            services.AddIdentity<Domain.Model.User, IdentityRole>()
                .AddEntityFrameworkStores<Data.Context>();
            services.AddAutoMapper();
            services.AddAuthorization(_ =>
            {
                _.AddPolicy(PolicyName.MissionControlAccess,
                    policy => policy.RequireClaim(ClaimType.Privilege, ClaimName.MissionControlUser));
            });
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
            app.UseIdentity();

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
