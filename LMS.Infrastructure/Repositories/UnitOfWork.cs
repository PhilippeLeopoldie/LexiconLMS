using Domain.Contracts.Repositories;
using LMS.Infrastructure.Data;

namespace LMS.Infrastructure.Repositories;
public class UnitOfWork(ApplicationDbContext context,
                        Lazy<IActivityRepository> activityRepository,
                        Lazy<IModuleRepository> moduleRepository,
                        Lazy<ICourseRepository> courseRepository,
                        Lazy<IDocumentRepository> documentRepository,
                        Lazy<IActivityTypeRepository> activityTypeRepository) : IUnitOfWork
{
    private readonly ApplicationDbContext context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly Lazy<IActivityRepository> activityRepository = activityRepository;
    private readonly Lazy<IModuleRepository> moduleRepository = moduleRepository;
    private readonly Lazy<ICourseRepository> courseRepository = courseRepository;
    private readonly Lazy<IDocumentRepository> documentRepository = documentRepository;
    private readonly Lazy<IActivityTypeRepository> activityTypeRepository = activityTypeRepository;

    public IActivityRepository ActivityRepository => activityRepository.Value;
    public IActivityTypeRepository ActivityTypeRepository => activityTypeRepository.Value;
    public IModuleRepository ModuleRepository => moduleRepository.Value;
    public ICourseRepository CourseRepository => courseRepository.Value;

    public IDocumentRepository DocumentRepository => documentRepository.Value;


    public async Task CompleteAsync() => await context.SaveChangesAsync();
}
