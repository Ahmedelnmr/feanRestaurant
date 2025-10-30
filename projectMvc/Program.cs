using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using projectMvc.Models; // ???? ???? Mycontext ? User

namespace projectMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1?? Add services
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // ????? ???? ?????? ???? ??? ?????
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddDbContext<Mycontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            // 2?? Add DbContext + Identity
            builder.Services.AddDbContext<Mycontext>(); // ??? ???? Identity ???? ?????
            builder.Services.AddIdentity<Models.models.User, IdentityRole>()
                .AddEntityFrameworkStores<Mycontext>()
                .AddDefaultTokenProviders();

            // 3?? Build app
            var app = builder.Build();

            // 4?? Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            // ? ?? ???? ????? ????:
            app.UseAuthentication();  // ???? ???? ??? Authorization
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=login}/{id?}");

            app.Run();
        }
    }
}
