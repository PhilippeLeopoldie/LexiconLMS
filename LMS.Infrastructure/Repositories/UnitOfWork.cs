using Domain.Contracts.Repositories;
using LMS.Infrastructure.Data;

namespace LMS.Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;
    private readonly Lazy<IActivityRepository> activityRepository;
    private readonly Lazy<IModuleRepository> moduleRepository;
    private readonly Lazy<ICourseRepository> courseRepository;


    public UnitOfWork(ApplicationDbContext context, Lazy<IActivityRepository> activityRepository, Lazy<IModuleRepository> moduleRepository, Lazy<ICourseRepository> courseRepository)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.activityRepository = activityRepository;
        this.moduleRepository = moduleRepository;
        this.courseRepository = courseRepository;
    }

    public IActivityRepository ActivityRepository => activityRepository.Value;
    public IModuleRepository ModuleRepository => moduleRepository.Value;
    public ICourseRepository CourseRepository => courseRepository.Value;

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}
