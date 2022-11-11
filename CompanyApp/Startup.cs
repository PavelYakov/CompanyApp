using CompanyApp.Domain;
using CompanyApp.Domain.Repositories.Abstract;
using CompanyApp.Domain.Repositories.EntityFramework;
using CompanyApp.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            //���������� ������ �� appsetting.json
            Configuration.Bind("Project", new Config());

            //���������� ������ ���������� ���������� � �������� ��������
            services.AddTransient<ITextFieldsRepository, EFTextFieldsRepository>();
            services.AddTransient<IServiceItemsRepository, EFServiceItemsRepository>();
            services.AddTransient<DataManager>();

            //���������� �������� ��
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(Config.ConnectionString));

            //����������� identity �������
            services.AddIdentity<IdentityUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //����������� authentication cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "myCompanyAuth";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/account/login";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
            });

            //����������� �������� ����������� ��� Admin area
            services.AddAuthorization(x =>
            {
                x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
            });

            //��������� ������� ��� ������������ � ������������� (MVC)
            services.AddControllersWithViews(x =>
            {
                x.Conventions.Add(new AdminAreaAuthorization("Admin", "AdminArea"));
            })
                //���������� ������������� � asp.net core 3.0
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //!!! ������� ����������� middleware ����� �����

            //� �������� ���������� ��� ����� ������ ����� ������ ������
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            //���������� ��������� ��������� ������ � ���������� (css, js � �.�.)
            app.UseStaticFiles();

            //���������� ������� �������������
            app.UseRouting();

            //���������� �������������� � �����������
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            //������������� ������ ��� �������� (���������)
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("admin", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            //public Startup(IConfiguration configuration)
            //{
            //    Configuration = configuration;
            //}

            //public IConfiguration Configuration { get; }


            //public void ConfigureServices(IServiceCollection services)
            //{
            //    // ���������� ����� ������ �� appsettings.json
            //    Configuration.Bind("Project", new Config());

            //    //���������� ������ ���������� ���������� � �������� ��������
            //    services.AddTransient<ITextFieldsRepository, EFTextFieldsRepository>();
            //    services.AddTransient<IServiceItemsRepository, EFServiceItemsRepository>();
            //    services.AddTransient<DataManager>();

            //    //������������ � ��������� ��.
            //    //ConnectionString - ��� ������ ����������� �� appsetting.json
            //    services.AddDbContext<AppDbContext>(x => x.UseSqlServer(Config.ConnectionString));

            //    //����������� Identy ������� - ���������� ������������ (������ � �.�.)
            //    services.AddIdentity<IdentityUser, IdentityRole>(opts =>
            //     {
            //             //������������� ������ emil -> �������� ������ �� �����
            //             opts.User.RequireUniqueEmail = true;
            //             // ����������� ����� ������
            //             opts.Password.RequiredLength = 6;
            //         opts.Password.RequireNonAlphanumeric = false;
            //         opts.Password.RequireLowercase = false;
            //         opts.Password.RequireUppercase = false;
            //             // ������������ ����������� �����
            //             opts.Password.RequireDigit = false;
            //     }
            //    ).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //    //����������� authentication cookie
            //    services.ConfigureApplicationCookie(options =>
            //    {
            //        options.Cookie.Name = "myCompanyAuth";
            //        options.Cookie.HttpOnly = true;
            //        options.LoginPath = "/accont/login"; // ���� ���������� ������������ , ����� �� ����������� �� �����
            //            options.AccessDeniedPath = "/account/accessdenied";
            //        options.SlidingExpiration = true;
            //    });

            //    //����������� �������� ����������� ��� Admin area
            //    services.AddAuthorization(x =>
            //    {
            //            // ���� �������� - �� ������� �� ������������ ���� �����
            //            x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
            //    });

            //    //��������� ������������ � �������������
            //    services.AddControllersWithViews(x =>
            //    {
            //            // ��� ������� admin �������� �������� AdminArea
            //            x.Conventions.Add(new AdminAreaAuthorization("Admin", "AdminArea"));
            //    })
            //            //���������� ������������ � asp.net core 3.0
            //            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider();
            //}


            //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            //{
            //    //                  !!������� ����������� middleware ����� �����!!

            //    //���� � ��� ���� ������� ���������� �����
            //    if (env.IsDevelopment())
            //    {
            //        //��� ����� ����� ����� � ��� ��������� ������ - ��������� ���������� ��� ��������
            //        app.UseDeveloperExceptionPage();
            //    }
            //    else
            //    {
            //        app.UseExceptionHandler("/Home/Error");
            //        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //        app.UseHsts();
            //    }
            //    app.UseHttpsRedirection();

            //    // ���������� ��������� ��������� ������ � ���������� (css,js ...) --> ����� wwwroot
            //    app.UseStaticFiles();

            //    // ��������� �������������
            //    app.UseRouting();

            //    //���������� �������������� � �����������
            //    app.UseAuthorization();
            //    app.UseCookiePolicy();
            //    app.UseAuthentication();

            //    // ����������� ����������� ���������
            //    app.UseEndpoints(endpoints =>
            //    {
            //            //endpoints.MapControllerRoute(
            //            //        name: "admin",
            //            //        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            //            //endpoints.MapControllerRoute(
            //            //   name: "default",
            //            //   pattern: "{controller=Home}/{action=Index}/{id?}");

            //            endpoints.MapControllerRoute("admin", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            //        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            //    });


        }
    }
}