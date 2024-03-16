using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain
{
    public class RefreshToken
    {
        public string Id { get; set; } = default!;
        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
