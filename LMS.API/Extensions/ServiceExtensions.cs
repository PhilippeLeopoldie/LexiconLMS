using LMS.Infrastructure.Data;
using LMS.Infrastructure.Repositories;
using LMS.Presentation;
using LMS.Services;
using LMS.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Services.Contracts;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LMS.API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            //Restrict access to your BlazorApp only!
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("https://localhost:7224")
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });

            //Can be used during development
            options.AddPolicy("AllowAll", policy =>
               policy.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());
        });
    }

    public static void ConfigureOpenApi(this IServiceCollection services) =>
       services.AddEndpointsApiExplorer()
               .AddSwaggerGen(setup =>
               {
                   setup.EnableAnnotations();
                   // Add parameter for role selection
                   setup.MapType<UserRole>(() => new OpenApiSchema
                   {
                       Type = "string",
                       Enum = Enum.GetNames(typeof(UserRole))
                           .Select(name => new OpenApiString(name))
                           .ToList<IOpenApiAny>()
                   });


                   setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                   {
                       In = ParameterLocation.Header,
                       Description = "Place to add JWT with Bearer",
                       Name = "Authorization",
                       Type = SecuritySchemeType.Http,
                       Scheme = "Bearer"
                   });

                   setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                   {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new List<string>()
                        }
                   });
               });

    public static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(opt =>
                {
                    opt.ReturnHttpNotAcceptable = true;
                    opt.Filters.Add(new ProducesAttribute("application/json"));

                })
                .AddNewtonsoftJson()
                .AddApplicationPart(typeof(AssemblyReference).Assembly);
    }

    public static void ConfigureSql(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ApplicationDbContext")
                ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")));
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IActivityTypeRepository, ActivityTypeRepository>();
        services.AddScoped<IModuleRepository, ModuleRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddLazy<IActivityRepository>();
        services.AddLazy<IActivityTypeRepository>();
        services.AddLazy<IModuleRepository>();
        services.AddLazy<ICourseRepository>();
        services.AddLazy<IDocumentRepository>();
        services.AddLazy<IUserRepository>();
    }

    public static void AddServiceLayer(this IServiceCollection services)
    {
        services.AddScoped<IServiceManager, ServiceManager>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IActivityTypeService, ActivityTypeService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IUserService, UserService>();

        services.AddLazy<IAuthService>();
        services.AddLazy<IActivityService>();
        services.AddLazy<IActivityTypeService>();
        services.AddLazy<IModuleService>();
        services.AddLazy<IDocumentService>();
        services.AddLazy<IUserService>();
    }
}
