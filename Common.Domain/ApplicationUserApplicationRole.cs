using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain
{
    public class ApplicationUserApplicationRole
    {
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = default!;

        public int ApplicationUserRoleId { get; set; }
        public ApplicationUserRole ApplicationUserRole { get; set;} = default!;
    }
}
