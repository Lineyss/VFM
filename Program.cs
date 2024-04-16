using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using VFM.Models;
using VFM.Services;

namespace VFM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost
                .UseKestrel(options =>
                {
                    foreach (var address in IPManager.getAddress())
                    {
                        options.Listen(address, 443, listenOptions =>
                        {
                            // Указание пути к SSL сертификату и, при необходимости, пароля к нему
                            listenOptions.UseHttps("myCert.pfx", "Password123312");
                        });
                    }
                });

            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton(new LiteDbContext("LiteDb.db"));

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy =>
                {
                    policy.RequireClaim("isAdmin", "True");
                });
                options.AddPolicy("create", policy =>
                {
                    policy.RequireClaim("createF", "True");
                });
                options.AddPolicy("delete", policy =>
                {
                    policy.RequireClaim("deleteF", "True");
                });
                options.AddPolicy("updateName", policy =>
                {
                    policy.RequireClaim("updateNameF", "True");
                });
                options.AddPolicy("download", policy =>
                {
                    policy.RequireClaim("downloadF", "True");
                });
                options.AddPolicy("upload", policy =>
                {
                    policy.RequireClaim("uploadF", "True");
                });
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Auth/Login.html"; // Страница авторизации
                options.LogoutPath = "/Auth/Exit"; // Страница выхода

                options.SlidingExpiration = true; // Продления сроков жизни cookie при каждом запросе

                options.Cookie.HttpOnly = true; // Устанавливаем флажок в браузере чтобы не было доступа у js
                 
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Для использования HTTPS

                options.Cookie.SameSite = SameSiteMode.None; // Для предотвращения csrf атак

                options.Cookie.MaxAge = TimeSpan.FromDays(1); // Время жизни cookie
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // указывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    // строка, представляющая издателя
                    ValidIssuer = Jwt.ValidIssuer,
                    // будет ли валидироваться потребитель токена
                    ValidateAudience = true,
                    // установка потребителя токена
                    ValidAudience = Jwt.ValidAudience,
                    // будет ли валидироваться время существования
                    ValidateLifetime = true,
                    // установка ключа безопасности
                    IssuerSigningKey = Jwt.GetSymmetricSecurityKey(),
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true,
                };
            });

            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHsts();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();

            File.WriteAllText("Путь до твоего файла", "Зашифрованный текст");
        }
    }
}