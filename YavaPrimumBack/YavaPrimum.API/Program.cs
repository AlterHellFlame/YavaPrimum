using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.Interfaces;
using YavaPrimum.Core.Services;
using YavaPrimum.API.Notify;
using Microsoft.AspNetCore.SignalR;

namespace YavaPrimum.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();

            builder.Services.AddSingleton<IUserIdProvider>(provider =>
            {
                // Создаем scope, чтобы получить scoped зависимости
                var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
                    return new UserIdProvider(jwtProvider);
                }
            });


            builder.Services.AddDbContext<YavaPrimumDBContext>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICandidateService, CandidateService>();
            builder.Services.AddScoped<ITasksService, TaskService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<ICountryService, CountryService>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<INotificationsService, NotificationsService>();

            builder.Services.AddScoped<IJwtProvider, JwtProvider>();

            builder.Services.AddSingleton<CodeVerificationService>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtProvider.JwtKey))
                    };

                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies[JwtProvider.CookiesName];
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseStaticFiles();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax,
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always,
            });

            app.UseCors(x => {
                x.AllowAnyHeader(); x.AllowAnyOrigin();
                x.WithOrigins("https://localhost:4200").AllowCredentials(); 
                x.WithOrigins("https://localhost:7247").AllowCredentials();
                x.AllowAnyMethod();
            });

            app.MapHub<NotifyHub>("/chat");


            app.MapControllers();

            app.Run();
        }
    }
}



