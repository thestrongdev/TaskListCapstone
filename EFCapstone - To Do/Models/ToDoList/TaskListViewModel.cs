using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.Models.ToDoList
{
    public class TaskListViewModel : TaskItem
    {
        public List<TaskItem> Tasks { get; set; }
    }
}
