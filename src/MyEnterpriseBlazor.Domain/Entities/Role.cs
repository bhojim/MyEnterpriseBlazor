using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MyEnterpriseBlazor.Domain
{
    public class Role : IdentityRole<string>
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
