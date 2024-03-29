using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Poll.Data;
using Poll.Services;
using Poll.Data.Repositories;
using Microsoft.AspNetCore.Http;

namespace Poll
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
            services.AddDbContext<AppDbContext>(dbCtxOption => {
                dbCtxOption.UseMySql(
                    Configuration.GetConnectionString("MariaDbConnectionString"),  
                    new MySqlServerVersion(new Version(Configuration["MysqlVersion"]))
                ).LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
            });

            services.AddAuthentication("Cookies")
                .AddCookie("Cookies", config =>
                {
                    config.LoginPath = "/home/login";
                    config.LogoutPath = "/home/logout";
                    config.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                                // Si la personne fais une requete le cookie va de nouveau valoir 60mn
                                config.SlidingExpiration = true;
                    config.Cookie.IsEssential = true;
                });

            services.AddScoped<ISurveyService, SurveyService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IVoteService, VoteService>();
    
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped<ISurveyRepository, SurveyRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();

            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{guid?}");
            });
        }
    }
}
