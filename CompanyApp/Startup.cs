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
            //подключаем конфиг из appsetting.json
            Configuration.Bind("Project", new Config());

            //подключаем нужный функционал приложения в качестве сервисов
            services.AddTransient<ITextFieldsRepository, EFTextFieldsRepository>();
            services.AddTransient<IServiceItemsRepository, EFServiceItemsRepository>();
            services.AddTransient<DataManager>();

            //подключаем контекст БД
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(Config.ConnectionString));

            //настраиваем identity систему
            services.AddIdentity<IdentityUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //настраиваем authentication cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "myCompanyAuth";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/account/login";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
            });

            //настраиваем политику авторизации для Admin area
            services.AddAuthorization(x =>
            {
                x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
            });

            //добавляем сервисы для контроллеров и представлений (MVC)
            services.AddControllersWithViews(x =>
            {
                x.Conventions.Add(new AdminAreaAuthorization("Admin", "AdminArea"));
            })
                //выставляем совместимость с asp.net core 3.0
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //!!! порядок регистрации middleware очень важен

            //в процессе разработки нам важно видеть какие именно ошибки
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            //подключаем поддержку статичных файлов в приложении (css, js и т.д.)
            app.UseStaticFiles();

            //подключаем систему маршрутизации
            app.UseRouting();

            //подключаем аутентификацию и авторизацию
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            //регистриуруем нужные нам маршруты (ендпоинты)
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
            //    // подключаем класс конфиг из appsettings.json
            //    Configuration.Bind("Project", new Config());

            //    //подключаем нужный функционал приложения в качестве сервисов
            //    services.AddTransient<ITextFieldsRepository, EFTextFieldsRepository>();
            //    services.AddTransient<IServiceItemsRepository, EFServiceItemsRepository>();
            //    services.AddTransient<DataManager>();

            //    //подключаемся к контексту бд.
            //    //ConnectionString - это строка подключения из appsetting.json
            //    services.AddDbContext<AppDbContext>(x => x.UseSqlServer(Config.ConnectionString));

            //    //настраиваем Identy систему - требования безопасности (пароль и т.д.)
            //    services.AddIdentity<IdentityUser, IdentityRole>(opts =>
            //     {
            //             //подтверждение своего emil -> приходит письмо на почту
            //             opts.User.RequireUniqueEmail = true;
            //             // минимальная длина пароля
            //             opts.Password.RequiredLength = 6;
            //         opts.Password.RequireNonAlphanumeric = false;
            //         opts.Password.RequireLowercase = false;
            //         opts.Password.RequireUppercase = false;
            //             // использовать обязательно цифры
            //             opts.Password.RequireDigit = false;
            //     }
            //    ).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //    //настраиваем authentication cookie
            //    services.ConfigureApplicationCookie(options =>
            //    {
            //        options.Cookie.Name = "myCompanyAuth";
            //        options.Cookie.HttpOnly = true;
            //        options.LoginPath = "/accont/login"; // сюда отправляем пользователя , чтобы он залогинился на сайте
            //            options.AccessDeniedPath = "/account/accessdenied";
            //        options.SlidingExpiration = true;
            //    });

            //    //настраиваем политику авторизации для Admin area
            //    services.AddAuthorization(x =>
            //    {
            //            // суть политики - мы требуем от пользователя роль АДМИН
            //            x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
            //    });

            //    //поддержка контроллеров и представлений
            //    services.AddControllersWithViews(x =>
            //    {
            //            // для области admin передаем политику AdminArea
            //            x.Conventions.Add(new AdminAreaAuthorization("Admin", "AdminArea"));
            //    })
            //            //выставляем совместимось с asp.net core 3.0
            //            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider();
            //}


            //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            //{
            //    //                  !!Порядок регистрации middleware очень важен!!

            //    //Если у нас идет процесс разработки сайта
            //    if (env.IsDevelopment())
            //    {
            //        //нам важно знать какие у нас возникают ошибки - подробная информация для разрабов
            //        app.UseDeveloperExceptionPage();
            //    }
            //    else
            //    {
            //        app.UseExceptionHandler("/Home/Error");
            //        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //        app.UseHsts();
            //    }
            //    app.UseHttpsRedirection();

            //    // подключаем поддрежку статичных файлов в приложении (css,js ...) --> папка wwwroot
            //    app.UseStaticFiles();

            //    // Добалвяем маршрутизацию
            //    app.UseRouting();

            //    //подключаем аутентификацию и авторизацию
            //    app.UseAuthorization();
            //    app.UseCookiePolicy();
            //    app.UseAuthentication();

            //    // регистрация необходимых маршрутов
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
