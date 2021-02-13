using EFCapstone___To_Do.Models.ToDoList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.Services
{
    public class Validation
    {
        public static bool ValidatePW(RegisterUserViewModel user)
        {
            if(user.Password!=null && user.PasswordConfirmation != null && user.Password == user.PasswordConfirmation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
