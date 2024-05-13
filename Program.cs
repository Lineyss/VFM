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

            foreach (var address in IPManager.getAddress())
            {
                builder.WebHost.UseUrls($"http://{address}:80", $"https://{address}:449");
            }

            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton(new LiteDbContext("LiteDb.db"));
            builder.Services.AddTransient<Services.AuthenticationManager>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthorization();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Auth/Login.html"; // —траница авторизации
                options.LogoutPath = "/Auth/Exit"; // —траница выхода

                options.SlidingExpiration = true; // ѕродлени€ сроков жизни cookie при каждом запросе

                options.Cookie.HttpOnly = true; // ”станавливаем флажок в браузере чтобы не было доступа у js

                options.Cookie.MaxAge = TimeSpan.FromDays(1); // ¬рем€ жизни cookie
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // указывает, будет ли валидироватьс€ издатель при валидации токена
                    ValidateIssuer = true,
                    // строка, представл€юща€ издател€
                    ValidIssuer = Jwt.ValidIssuer,
                    // будет ли валидироватьс€ потребитель токена
                    ValidateAudience = true,
                    // установка потребител€ токена
                    ValidAudience = Jwt.ValidAudience,
                    // будет ли валидироватьс€ врем€ существовани€
                    ValidateLifetime = true,
                    // установка ключа безопасности
                    IssuerSigningKey = Jwt.GetSymmetricSecurityKey(),
                    // валидаци€ ключа безопасности
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}