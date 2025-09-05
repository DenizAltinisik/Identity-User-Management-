using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementAPI.Dto.Person
{
    public class AssignRoleDTO
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}