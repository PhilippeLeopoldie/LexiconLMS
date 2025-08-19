using Domain.Contracts.Repositories;
using LMS.Infrastructure.Data;

namespace LMS.Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;
    private readonly Lazy<IActivityRepository> activityRepository;
    private readonly Lazy<IModuleRepository> moduleRepository;


    public UnitOfWork(ApplicationDbContext context, Lazy<IActivityRepository> activityRepository, Lazy<IModuleRepository> moduleRepository)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.activityRepository = activityRepository;
        this.moduleRepository = moduleRepository;
    }

    public IActivityRepository ActivityRepository => activityRepository.Value;
    public IModuleRepository ModuleRepository => moduleRepository.Value;

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}
