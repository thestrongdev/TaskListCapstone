using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.Models.ToDoList
{
    public class TaskItem
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public string IsDone { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        public int TaskID { get; set; }
        public int UserID { get; set; }

    }
}
