using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddSingleton(_ => Configuration);
            services.AddMvc();
            services.AddScoped<Controllers.ServiceFacade.Controller, Controllers.ServiceFacade.Controller>();
            services.AddScoped<Data.Context, Data.SqlServer.SqlServerContext>();
            //services.AddScoped<Data.Context, Data.SQLite.SQLiteContext>();
            services.AddScoped<Domain.IRepository, Data.Repository>();
            services.AddScoped<Domain.Service, Domain.Service>();
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

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
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
