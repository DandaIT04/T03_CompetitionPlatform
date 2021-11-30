using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Google.Apis.Auth.AspNetCore3;

namespace T03_CompetitionPlatform
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
            // Add a default in-memory implementation of distributed cache
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            services.AddHttpContextAccessor();

            // Add the session service
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            
            
            services.AddControllersWithViews();

            // This configures Google.Apis.Auth.AspNetCore3 for use in this app.
            services.AddAuthentication(options =>
            {
                // Login (Challenge) to be handled by Google OpenID Handler,
                options.DefaultChallengeScheme =
                GoogleOpenIdConnectDefaults.AuthenticationScheme;

                // Once a user is authenticated, the OAuth2 token info
                // is stored in cookies.
                options.DefaultScheme =
                CookieAuthenticationDefaults.AuthenticationScheme;
            })
           .AddCookie()
           .AddGoogleOpenIdConnect(options =>
           {
    // Credentials (stored in appsettings.json) to identify
    // the web app when performing Google authentication
    options.ClientId =
    Configuration["Authentication:Google:ClientId"];
               options.ClientSecret =
    Configuration["Authentication:Google:ClientSecret"];
           });
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
