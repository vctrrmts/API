using Common.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Service.Dto
{
    public class MainUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
