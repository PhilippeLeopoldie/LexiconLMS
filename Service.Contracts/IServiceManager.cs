namespace Service.Contracts;
public interface IServiceManager
{
    IAuthService AuthService { get; }
    IActivityService ActivityService { get; }
    IActivityTypeService ActivityTypeService { get; }
    IDocumentService DocumentService { get; }
}