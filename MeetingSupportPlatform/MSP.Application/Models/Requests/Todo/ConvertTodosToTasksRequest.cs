using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSP.Application.Models.Requests.Todo
{
    public class ConvertTodosToTasksRequest
    {
        public List<Guid> TodoIds { get; set; }
    }
}
