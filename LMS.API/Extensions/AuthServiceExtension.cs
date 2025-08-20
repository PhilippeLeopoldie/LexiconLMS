using Domain.Models.Configurations;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LMS.API.Extensions;

public static class AuthServiceExtension
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration
                         .GetSection(JwtSettings.Section)
                         .Get<JwtSettings>()
                         ?? throw new InvalidOperationException("JwtSettings section is missing or invalid.");

        services.AddOptions<JwtSettings>()
                        .Bind(configuration.GetSection(JwtSettings.Section))
                        .Validate(config => !string.IsNullOrWhiteSpace(config.SecretKey), "SecretKey is required")
                        .ValidateDataAnnotations();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = jwtSettings.Issuer,
                   ValidAudience = jwtSettings.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
               };

               options.Events = new JwtBearerEvents
               {
                   OnChallenge = async context =>
                   {
                       context.HandleResponse();
                       context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                       context.Response.ContentType = "application/problem+json";
                       var problemDetails = new ProblemDetails
                       {
                           Status = StatusCodes.Status401Unauthorized,
                           Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
                           Title = "Authentication Required",
                           Detail = context.ErrorDescription ?? "You must be authenticated to access this resource. Please provide a valid token."
                       };
                       await context.Response.WriteAsJsonAsync(problemDetails);
                   },
                   OnForbidden = async context =>
                   {
                       context.Response.StatusCode = StatusCodes.Status403Forbidden;
                       context.Response.ContentType = "application/problem+json";
                       var problemDetails = new ProblemDetails
                       {
                           Status = StatusCodes.Status403Forbidden,
                           Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
                           Title = "Access Denied",
                           Detail = "You do not have the necessary permissions to perform this action."
                       };
                       await context.Response.WriteAsJsonAsync(problemDetails);
                   }
               };
           });
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUser>(opt =>
        {
            //Just during development!
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredLength = 3;

            opt.User.RequireUniqueEmail = true;
        })
               .AddRoles<IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();
    }
}
