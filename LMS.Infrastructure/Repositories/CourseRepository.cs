using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class CourseRepository(ApplicationDbContext context) : RepositoryBase<Course>(context), ICourseRepository
{
    public async Task<Course?> GetCourseByIdAsync(int id, bool trackChanges = false) =>
        await FindByCondition(course => course.Id.Equals(id), trackChanges).FirstOrDefaultAsync();

    public async Task<IEnumerable<Course>> GetAllCoursesAsync(bool trackChanges = false) =>
        await FindAll(trackChanges).ToListAsync();
}
