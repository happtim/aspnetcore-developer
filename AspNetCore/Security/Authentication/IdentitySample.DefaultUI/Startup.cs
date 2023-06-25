// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using IdentitySample.DefaultUI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentitySample.DefaultUI
{
    public class BadDude : IUserConfirmation<ApplicationUser>
    {
        public Task<bool> IsConfirmedAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            return Task.FromResult(false);
        }
    }

    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(
                options => options.ConfigureWarnings(b => b.Log(CoreEventId.ManyServiceProvidersCreatedWarning))
                    .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("IdentitySample.DefaultUI")));

            services.AddMvc().AddNewtonsoftJson();
            services.AddIdentity
            services.AddDefaultIdentity<ApplicationUser>(o => {
                o.SignIn.RequireConfirmedAccount = false; //是否需要电子邮件确认。 默认为 false。
                o.Password.RequireNonAlphanumeric = false; //密码是否必须包含非字母数字字符。 默认为 true。
                o.Password.RequireUppercase = false;//密码是否必须包含大写 ASCII 字符。 默认为 true。
                o.Password.RequireDigit = false; //密码是否必须包含数字。 默认为 true。
                o.Password.RequireLowercase = false; //密码是否必须包含小写 ASCII 字符。 默认为 true。
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDatabaseDeveloperPageExceptionFilter();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
