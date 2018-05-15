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
            services.AddDbContext<ChooseYourGameContext>(options =>
                //options.UseInMemoryDatabase("DefaultConnection")
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // else app.UseExceptionHandler("/Home/Error");

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ChooseYourGameContext>();
                // alterar para migration
                context.Database.EnsureCreated();

                //Construct

                if (!context.Accesses.Any())
                {
                    context.Accesses.Add(new Access
                    {
                        Description = "1"
                    });
                    context.SaveChanges();
                }
                if (!context.Users.Any())
                {
                    context.Users.Add(new User
                    {
                        AccessId = context.Accesses.First().Id,
                        EMail = "1",
                        Password = "1"
                    });
                    context.SaveChanges();
                }
                if (!context.Profiles.Any())
                {
                    context.Profiles.Add(new Profile
                    {
                        UserId = context.Users.First().Id,
                        Lastname = "Bevilacqua",
                        Name = "Leonardo",
                        Username = "Leozinho580"
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
                        ProfileId = context.Profiles.First().UserId,
                        Title = "Titulo teste"
                    });
                    context.SaveChanges();
                }
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
