using LMS.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.Common;

public class RequestParams
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(2, 100)]
    public int PageSize { get; set; } = 20;

    public OrderByParams? OrderBy { get; set; }
    public string? SearchTerm { get; set; }
}

public class ModuleRequestParams : RequestParams
{
    public bool IncludeActivities { get; set; } = false;
}
