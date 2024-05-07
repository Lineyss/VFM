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
                            // �������� ���� � SSL ����������� �, ��� �������������, ������ � ����
                            listenOptions.UseHttps("myCert.pfx", "Password123312");
                        });
                    }
                });

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
                options.LoginPath = "/Auth/Login.html"; // �������� �����������
                options.LogoutPath = "/Auth/Exit"; // �������� ������

                options.SlidingExpiration = true; // ��������� ������ ����� cookie ��� ������ �������

                options.Cookie.HttpOnly = true; // ������������� ������ � �������� ����� �� ���� ������� � js
                 
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ��� ������������� HTTPS

                options.Cookie.SameSite = SameSiteMode.None; // ��� �������������� csrf ����

                options.Cookie.MaxAge = TimeSpan.FromDays(1); // ����� ����� cookie
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // ���������, ����� �� �������������� �������� ��� ��������� ������
                    ValidateIssuer = true,
                    // ������, �������������� ��������
                    ValidIssuer = Jwt.ValidIssuer,
                    // ����� �� �������������� ����������� ������
                    ValidateAudience = true,
                    // ��������� ����������� ������
                    ValidAudience = Jwt.ValidAudience,
                    // ����� �� �������������� ����� �������������
                    ValidateLifetime = true,
                    // ��������� ����� ������������
                    IssuerSigningKey = Jwt.GetSymmetricSecurityKey(),
                    // ��������� ����� ������������
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
        }
    }
}