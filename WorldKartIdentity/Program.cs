using AutoMapper;
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
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

            builder.Services.AddEndpointsApiExplorer(); // needed for minimal APIs / endpoint discovery
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme)
                .AddBearerToken(IdentityConstants.BearerScheme);

            builder.Services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();          // generates the JSON at /swagger/v1/swagger.json
                app.UseSwaggerUI();

                //app.ApplyMigrations();
            }

            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    app.UseHsts();
            //}

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
