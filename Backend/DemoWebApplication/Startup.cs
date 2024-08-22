using DemoWebApplication.Middleware;
using Serilog;
using DemoWebApplication.Service.ServiceImplementation;
using DemoWebApplication.Service.ServiceInterface;
using Microsoft.AspNetCore.Identity;
using DemoWebApplication.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using DemoWebApplication.Utils;
using Microsoft.AspNetCore.Authorization;
using DemoWebApplication.Helpers;
using DemoWebApplication.Controllers;
using System.Diagnostics.CodeAnalysis;

namespace DemoWebApplication
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {

            Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .MinimumLevel.Debug()
                            .WriteTo.Console()
                            .CreateLogger();

            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSession();

            //new instance of  service is created for a scope or logical operation (method)
            services.AddScoped<IUserInterface, UserIntefaceImpl>();
            services.AddScoped<IShopInterface, ShopInterfaceImpl>();
            services.AddScoped<IAuthServiceInterface, AuthServiceImpl>();

            //new instance of service is created for each http request
            services.AddTransient<AuthServiceImpl>();

            //services.AddScoped<TokenServiceImpl>();

            //single instance of service is created and used for the entire application
            //services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();

            services.AddHttpContextAccessor();

            services.AddDbContext<ApplicationDbContext>();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        RequireExpirationTime = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero,
                    };
                    options.Events = new()
                    {
                        OnMessageReceived = context =>
                        {
                            var request = context.HttpContext.Request;
                            var cookies = request.Cookies;
                            if(context.HttpContext.Request.Cookies.TryGetValue("JwtToken",out var cookie))
                            {
                                context.Token = cookie;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });



            services.AddIdentity<Person, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

       
            services.AddMvc(options => options.EnableEndpointRouting = false);

        

            services.AddCors(options =>
            {
                options.AddPolicy("myPolicy", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Users\i44375\source\repos\DemoWebApplication\keys"))
                .SetApplicationName("unique");

         
            services.AddAutoMapper(typeof(Startup));

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseCors("myPolicy");// always above auth 

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseCors(policy => policy.AllowAnyHeader().AllowAnyHeader().AllowAnyOrigin());

            //app.UseMyCustomMiddleware();
            //app.UseMiddleware<JwtTokenMiddleware>();
            //app.UseMiddleware<JwtAuthMiddleware>();
            //app.UseMiddleware<ExceptionMiddleware>();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
           
        }
    }
}
