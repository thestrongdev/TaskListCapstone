using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.Models.ToDoList
{
    public class RegisterUserViewModel : User
    {

        [Required]
        public string PasswordConfirmation { get; set; }
    }
}
