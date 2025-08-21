using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private Lazy<IActivityService> activityService;
    private Lazy<IActivityTypeService> activityTypeService;
    private Lazy<IDocumentService> documentService;
    public IAuthService AuthService => authService.Value;

    public IActivityService ActivityService => activityService.Value;

    public IActivityTypeService ActivityTypeService => activityTypeService.Value;

    public IDocumentService DocumentService => documentService.Value;

    public ServiceManager(
        Lazy<IAuthService> authService,
        Lazy<IActivityService> activityService,
        Lazy<IActivityTypeService> activityTypeService,
        Lazy<IDocumentService> documentService)
    {
        this.authService = authService;
        this.activityService = activityService;
        this.activityTypeService = activityTypeService;
        this.documentService = documentService;
    }
}
