using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Shared.Common;

public class RequestParams
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(2, 100)]
    public int PageSize { get; set; } = 5;
}

public class ModuleRequestParams : RequestParams
{
    public bool IncludeActivities { get; set; } = false;
}
