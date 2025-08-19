using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Shared.DTOs.ModuleDtos;

public record ModuleDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateTime StartsAt { get; init; }
    public required DateTime EndsAt { get; init; }

    public int CourseId { get; init; }

    public IEnumerable<Activity> Activities { get; init; }
}
