namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    IActivityRepository ActivityRepository { get; }
    IModuleRepository ModuleRepository { get; }
    ICourseRepository CourseRepository { get; }
    Task CompleteAsync();
}