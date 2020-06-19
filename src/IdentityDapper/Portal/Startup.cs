using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityDapperCore.Models;
using IdentityDapperCore.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Portal
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
            services.AddTransient<IUserStore<ApplicationUser>, UserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();
            services.AddTransient<IUserRoleStore<ApplicationUser>, UserRoleStore>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options=>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireDigit = false;
                options.User.RequireUniqueEmail = true;
            
            }).AddRoles<ApplicationRole>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options=>
            {
                options.LoginPath = "/Secreto/login";
                options.Cookie.Name = "SecretoCK";
                options.AccessDeniedPath = "/Secreto/Deny";

            });

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
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
