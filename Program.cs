using LiteDB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Diagnostics;
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

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}