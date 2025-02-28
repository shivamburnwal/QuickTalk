using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.Data.Implementations;
using QuickTalk.Api.Data.Interfaces;
using QuickTalk.Api.Repositories.Implementations;
using QuickTalk.Api.Repositories.Interfaces;
using QuickTalk.Api.Services;
using QuickTalk.Api.Services.Implementations;
using QuickTalk.Api.Services.Interfaces;
using QuickTalk.Api.Services.Others;

namespace QuickTalk.Api.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCorsService(configuration);             // Add CORS
            services.AddDatabaseService(configuration);         // Add Database
            services.AddJwtAuthentication(configuration);       // Add JWT Authentication

            // Add Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });

            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddSwaggerService();                      // Add Swagger

            // Register Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IChatroomsService, ChatroomsService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IUsersService, UsersService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<AuthorizationService>();
            services.AddScoped<DatabaseService>();

            return services;
        }

        private static IServiceCollection AddCorsService(this IServiceCollection services, IConfiguration configuration)
        {
            var corsOrigins = configuration.GetSection("CorsOrigins").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins(corsOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            return services;
        }

        private static IServiceCollection AddDatabaseService(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string is not configured.");

            services.AddDbContext<ChatDbContext>(options => options.UseSqlServer(connectionString));


            // Register Repositories
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IChatroomsRepository, ChatroomsRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUserChatroomsRepository, UserChatroomsRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();


            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
                        ),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }

        private static IServiceCollection AddSwaggerService(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
