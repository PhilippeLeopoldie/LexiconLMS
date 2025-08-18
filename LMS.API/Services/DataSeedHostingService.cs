using Bogus;
using LMS.Infractructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Services;

public class DataSeedHostingService : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IConfiguration configuration;
    private readonly ILogger<DataSeedHostingService> logger;
    private UserManager<ApplicationUser> userManager = null!;
    private RoleManager<IdentityRole> roleManager = null!;
    private const string TeacherRole = "Teacher";
    private const string StudentRole = "Student";

    public DataSeedHostingService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<DataSeedHostingService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if (!env.IsDevelopment()) return;

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (await context.Users.AnyAsync(cancellationToken)) return;

        userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        ArgumentNullException.ThrowIfNull(roleManager, nameof(roleManager));
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));

        try
        {
            await AddRolesAsync([TeacherRole, StudentRole]);
            await AddDemoUsersAsync();
            await AddUsersAsync(20);
            await AddActivityTypesAsync(context);
            logger.LogInformation("Seed complete");
        }
        catch (Exception ex)
        {
            var message = ex.Message;
            logger.LogError("Data seed fail with error: {message}", message);
            throw;
        }
    }

    private async Task AddActivityTypesAsync(ApplicationDbContext context)
    {
        var activityTypesToSeed = new List<Tuple<string, string>>
        {
            new("Föreläsning","En lärarledd presentation som introducerar nya ämnen, teorier eller koncept för studenterna" ),
            new("E-learning", "En aktivitet där studenter får tillgång till digitalt kursmaterial, såsom videor, interaktiva moduler eller läsmaterial, för självstudier i egen takt"),
            new("Övningspass", "En handledd session där studenter arbetar med praktiska uppgifter eller problem för att tillämpa kunskapen från föreläsningar och e-learning."),
            new("Inlämningsuppgift", "En aktivitet där studenter ska lämna in ett skriftligt eller digitalt arbete för bedömning.")
        };

        foreach (var (activityTypeName, activityTypeDescription) in activityTypesToSeed)
        {
            if (!await context.ActivityTypes.AnyAsync(at => at.Name.Equals(activityTypeName)))
            {
                var newActivityType = new ActivityType
                {
                    Name = activityTypeName,
                    Description = activityTypeDescription
                };
                context.ActivityTypes.Add(newActivityType);
                logger.LogInformation(message: "Adding activity type: {activityTypeName}", activityTypeName);
            }
        }

        await context.SaveChangesAsync();
    }

    private async Task AddRolesAsync(string[] roleNames)
    {
        foreach (string roleName in roleNames)
        {
            if (await roleManager.RoleExistsAsync(roleName)) continue;
            var role = new IdentityRole { Name = roleName };
            var res = await roleManager.CreateAsync(role);

            if (!res.Succeeded) throw new Exception(string.Join("\n", res.Errors));
        }
    }
    private async Task AddDemoUsersAsync()
    {
        var teacher = new ApplicationUser
        {
            UserName = "teacher@test.com",
            Email = "teacher@test.com"
        };

        var student = new ApplicationUser
        {
            UserName = "student@test.com",
            Email = "student@test.com"
        };

        await AddUserToDb([teacher, student]);

        var teacherRoleResult = await userManager.AddToRoleAsync(teacher, TeacherRole);
        if (!teacherRoleResult.Succeeded) throw new Exception(string.Join("\n", teacherRoleResult.Errors));

        var studentRoleResult = await userManager.AddToRoleAsync(student, StudentRole);
        if (!studentRoleResult.Succeeded) throw new Exception(string.Join("\n", studentRoleResult.Errors));
    }

    private async Task AddUsersAsync(int nrOfUsers)
    {
        var faker = new Faker<ApplicationUser>("sv").Rules((f, e) =>
        {
            e.Email = f.Person.Email;
            e.UserName = f.Person.Email;
        });

        await AddUserToDb(faker.Generate(nrOfUsers));
    }

    private async Task AddUserToDb(IEnumerable<ApplicationUser> users)
    {
        var passWord = configuration["password"];
        ArgumentNullException.ThrowIfNull(passWord, nameof(passWord));

        foreach (var user in users)
        {
            var result = await userManager.CreateAsync(user, passWord);
            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
        }
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
