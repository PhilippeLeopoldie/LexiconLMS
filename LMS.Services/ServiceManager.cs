using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private Lazy<ICourseService> courseService;
    private Lazy<IActivityService> activityService;

    public IAuthService AuthService => authService.Value;
    public ICourseService CourseService => courseService.Value;
    public IActivityService ActivityService => activityService.Value;

    public ServiceManager(
        Lazy<IAuthService> authService,
        Lazy<ICourseService> courseService, 
        Lazy<IActivityService> activityService)
    {
        this.authService = authService;
        this.courseService = courseService;
        this.activityService = activityService;
    }
}
