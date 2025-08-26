using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Shared.DTOs.UserDtos;

public class UserCreationDto
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
}
