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

namespace ChooseYourGame
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<ChooseYourGameContext>(options =>
                //options.UseInMemoryDatabase("DefaultConnection")
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
            );
            services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ChooseYourGameContext>()
            .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options => options.LoginPath = "/Home/LogIn");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<ChooseYourGameContext>();
                    // alterar para migration
                    //context.Database.EnsureCreated();
                    context.Database.Migrate();

                    //Construct                   
                    if (!context.Users.Any())
                    {
                        context.Users.Add(new IdentityUser
                        {
                            Email = "dev",
                            PasswordHash = "1",
                            UserName = "Leozinho580",
                        });
                        context.SaveChanges();
                    }
                    if (!context.Profiles.Any())
                    {
                        context.Profiles.Add(new Profile
                        {
                            UserId = context.Users.First().Id,
                            Lastname = "Bevilacqua",
                            Name = "Leonardo"
                        });
                        context.SaveChanges();
                    }
                    if (!context.Blogs.Any())
                    {
                        context.Blogs.Add(new Blog
                        {
                            BlogText = "lorem",
                            CreationTime = DateTime.Now,
                            Description = "Texto",
                            ProfileId = context.Profiles.First().Id,
                            Title = "Titulo teste"
                        });
                        context.SaveChanges();
                    }
                }
            }
            // else app.UseExceptionHandler("/Home/Error");



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
