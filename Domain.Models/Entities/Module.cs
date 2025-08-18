using Service.Contracts;
using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Entities;

public class Module : BaseModel
{
    public DateTime Starts { get; set; }
    public DateTime Ends { get; set; }

    public Course course { get; set; }
    public int CourseId { get; set; }

    public ICollection<Activity> activities { get; set; }
   
}
