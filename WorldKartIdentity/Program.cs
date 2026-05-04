using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using WorldKartIdentity.Database;
using WorldKartIdentity.Extensions;

namespace WorldKartMaster
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            string id = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

            builder.Services.AddEndpointsApiExplorer(); // needed for minimal APIs / endpoint discovery
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddCookie(IdentityConstants.ApplicationScheme, options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(5);
                options.SlidingExpiration = true;
                options.LoginPath = "/Account/Login";
            })
            .AddCookie(IdentityConstants.ExternalScheme, options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(5);
                options.SlidingExpiration = true;
            })
            .AddGoogle(options =>
            {
                options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
                options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
                options.SaveTokens = true;
            })
            .AddBearerToken(IdentityConstants.BearerScheme);

            builder.Services.AddIdentityCore<User>(options =>
            {
                options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyz" +
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        "абвгдежзийклмнопрстуфхцчшщъьюяАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ" +
        "0123456789_-.@ ";
            })     //staro
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();


            //builder.Services.AddIdentity<User, IdentityRole>(options =>       //novo, no dava greshka
            //{
            //    options.SignIn.RequireConfirmedAccount = false;
            //})
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roleNames = { "Admin", "Users" };

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            //???? ?? ???????????
            var blockedIps = new List<string>{ "127.0.0.1", "192.168.1.5"};
            app.Use(async (context, next) =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString();

                if (ip != null && blockedIps.Contains(ip))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("????????!");
                    return;
                }

                await next();
            });



            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();          // generates the JSON at /swagger/v1/swagger.json
                app.UseSwaggerUI();
            }

            app.MapGet("users,/me", async (ClaimsPrincipal claims, ApplicationDbContext context) =>
            {
                string UserId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                return await context.Users.FindAsync(UserId);
            })
                .RequireAuthorization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //app.MapIdentityApi<User>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
