using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private Lazy<ICourseService> courseService;
    private Lazy<IActivityService> activityService;
    private Lazy<IActivityTypeService> activityTypeService;
    private Lazy<IDocumentService> documentService;
    private Lazy<IUserService> userService;
    private Lazy<IModuleService> moduleService;
    private Lazy<IDashboardService> dashboardService;
    public IAuthService AuthService => authService.Value;
    public ICourseService CourseService => courseService.Value;
    public IActivityService ActivityService => activityService.Value;
    public IActivityTypeService ActivityTypeService => activityTypeService.Value;
    public IDocumentService DocumentService => documentService.Value;
    public IUserService UserService => userService.Value;
    public IModuleService ModuleService => moduleService.Value;
    public IDashboardService DashboardService => dashboardService.Value;


    public ServiceManager(
        Lazy<IAuthService> authService,
        Lazy<ICourseService> courseService,
        Lazy<IActivityService> activityService,
        Lazy<IActivityTypeService> activityTypeService,
        Lazy<IDocumentService> documentService,
        Lazy<IUserService> userService,
        Lazy<IModuleService> moduleService,
        Lazy<IDashboardService> dashboardService)
    {
        this.authService = authService;
        this.courseService = courseService;
        this.activityService = activityService;
        this.activityTypeService = activityTypeService;
        this.documentService = documentService;
        this.userService = userService;
        this.moduleService = moduleService;
        this.dashboardService = dashboardService;
    }
}
