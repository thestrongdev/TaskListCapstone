using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.DAL_Models
{
    public class TasksDAL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int taskID { get; set; }

        public string Description { get; set; }
        public string IsDone { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
    }
}
