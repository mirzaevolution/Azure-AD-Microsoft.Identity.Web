using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using M1.Api.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
namespace M1.Web
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
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var tripleDESOption = new TripleDESApiOption();
            var aesOption = new AESApiOption();

            var tripleDESSection =  Configuration.GetSection("TripleDESApi");
            var aesSection = Configuration.GetSection("AESApi");


            tripleDESSection.Bind(tripleDESOption);
            aesSection.Bind(aesOption);

            services.AddOptions<AESApiOption>().Bind(aesSection);
            services.AddOptions<TripleDESApiOption>().Bind(tripleDESSection);

            services.AddHttpClient();
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration,subscribeToOpenIdConnectMiddlewareDiagnosticsEvents: true)
                .EnableTokenAcquisitionToCallDownstreamApi(new[]{
                        tripleDESOption.Scope,
                        aesOption.Scope
                    }).AddDistributedTokenCaches();
            services.Configure<CookieAuthenticationOptions>(options =>
            {
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
                options.ReturnUrlParameter = "returnUrl";
                options.AccessDeniedPath = "/unauthorized";
            });
            services.AddDistributedMemoryCache();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue &&
                    context.Request.Path.Value.Equals("/unauthorized", StringComparison.InvariantCultureIgnoreCase))
                {
                    await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("401 - Unauthorized"));
                }
                else if (context.Request.Path.HasValue &&
                    context.Request.Path.Value.Equals("/error", StringComparison.InvariantCultureIgnoreCase))
                {
                    await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("500 - Internal Server Error"));
                }
                else
                {
                    await next();
                }
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            
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
