using LMS.Services;

namespace Service.Contracts;
public interface IServiceManager
{
    IAuthService AuthService { get; }
    ICourseService CourseService { get; }
    IActivityService ActivityService { get; }
}