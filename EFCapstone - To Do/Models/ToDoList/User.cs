using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.Models.ToDoList
{
    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public int UserID { get; set; } //don't think i needed to add this
    }
}
