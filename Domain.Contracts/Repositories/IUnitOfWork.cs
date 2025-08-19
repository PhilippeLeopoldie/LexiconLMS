namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    IActivityRepository ActivityRepository { get; }
    Task CompleteAsync();
}