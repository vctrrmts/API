using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public ICollection<User> Users { get; set; } = default!;
    }
}
