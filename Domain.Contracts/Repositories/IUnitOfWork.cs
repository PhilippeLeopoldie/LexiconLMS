namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    IActivityRepository ActivityRepository { get; }
    IModuleRepository ModuleRepository { get; }
    Task CompleteAsync();
}