using Service.Contracts;
using Services.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private Lazy<ICourseService> courseService;
    private Lazy<IActivityService> activityService;
    private Lazy<IActivityTypeService> activityTypeService;
    private Lazy<IDocumentService> documentService;
    private Lazy<IModuleService> moduleService;
    public IAuthService AuthService => authService.Value;
    public ICourseService CourseService => courseService.Value;
    public IActivityService ActivityService => activityService.Value;

    public IActivityTypeService ActivityTypeService => activityTypeService.Value;

    public IDocumentService DocumentService => documentService.Value;

    public IModuleService ModuleService => moduleService.Value;

    public ServiceManager(
        Lazy<IAuthService> authService,
        Lazy<ICourseService> courseService,
        Lazy<IActivityService> activityService,
        Lazy<IActivityTypeService> activityTypeService,
        Lazy<IDocumentService> documentService,
        Lazy<IModuleService> moduleService)
    {
        this.authService = authService;
        this.courseService = courseService;
        this.activityService = activityService;
        this.activityTypeService = activityTypeService;
        this.documentService = documentService;
        this.moduleService = moduleService;
    }
}
