using LeS_License_Registry_API.Data;
using LeS_License_Registry_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Debugging;


namespace LeS_License_Registry_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Error().WriteTo.Console().WriteTo.File("logs/LeSLicenseRegistry.txt",
                   rollingInterval: RollingInterval.Day).CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        public static void CreateHostBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var configuration = builder.Configuration;

            builder.Services.AddScoped<TokenService>();
            // Add DbContext using Connection String from appsettings.json
            builder.Services.AddDbContext<LesLicenseRegistryContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]??"");

            // Configure Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });
            // Add Authorization
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost5174",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5174")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
                options.AddPolicy("Allowtest.lighthouse-esolutions.com",
                    policy =>
                    {
                        policy.WithOrigins("https://test.lighthouse-esolutions.com")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });
            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseCors("AllowAll");
            app.UseSerilogRequestLogging();
            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
