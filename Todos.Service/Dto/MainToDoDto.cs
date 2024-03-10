using Common.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todos.Service.Dto
{
    public class MainToDoDto
    {
        public int Id { get; set; }
        public string Label { get; set; } = default!;
        public int OwnerId { get; set; }
        public bool IsDone { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
