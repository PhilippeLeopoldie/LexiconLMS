using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private Lazy<IActivityService> activityService;
    public IAuthService AuthService => authService.Value;

    public IActivityService ActivityService => activityService.Value;

    public ServiceManager(
        Lazy<IAuthService> authService,
        Lazy<IActivityService> activityService)
    {
        this.authService = authService;
        this.activityService = activityService;
    }
}
