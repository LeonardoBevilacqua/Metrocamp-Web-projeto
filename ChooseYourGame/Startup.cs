using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ChooseYourGame.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace ChooseYourGame
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IHostingEnvironment _CurrentEnvironment { get; set; }
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _CurrentEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization();

            var connection = _CurrentEnvironment.IsDevelopment() ?
            Configuration.GetConnectionString("DefaultConnection") :
            Configuration.GetConnectionString("ReleaseConnection");

            services.AddDbContext<ChooseYourGameContext>(options =>
                options.UseMySql(connection)
            );
            services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ChooseYourGameContext>()
            .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options => options.LoginPath = "/Home/LogIn");
            services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireUppercase = false;
                }
            );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
        UserManager<IdentityUser> userManager, ChooseYourGameContext context,
        RoleManager<IdentityRole> roleManager)
        {
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("pt-Br"),
                SupportedCultures = { new CultureInfo("pt-Br") },
                SupportedUICultures = { new CultureInfo("pt-Br") }
            });

            context.Database.Migrate();
            // context.Database.EnsureCreated();

            if (_CurrentEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");
            }

            //Construct                   
            if (!context.Users.Any())
            {
                IdentityDataInitializer.SeedData(userManager, roleManager);

                context.Profiles.Add(new Profile
                {
                    Name = "Admin",
                    Lastname = "CYG",
                    UserId = context.Users.FirstOrDefault().Id
                });
                context.SaveChangesAsync();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }


    }
}
