using Domain.Contracts.Repositories;
using LMS.Infrastructure.Data;

namespace LMS.Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;

    public IModuleRepository ModuleRepository { get;}


    public UnitOfWork(ApplicationDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));

        ModuleRepository = new ModuleRepository(context);

    }

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}
