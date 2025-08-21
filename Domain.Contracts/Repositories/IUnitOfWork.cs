namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    IActivityRepository ActivityRepository { get; }
    IActivityTypeRepository ActivityTypeRepository { get; }
    IModuleRepository ModuleRepository { get; }
    ICourseRepository CourseRepository { get; }
    IDocumentRepository DocumentRepository { get; }
    Task CompleteAsync();
}