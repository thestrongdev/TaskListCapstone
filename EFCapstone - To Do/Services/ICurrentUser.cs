using EFCapstone___To_Do.Models.ToDoList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.Services
{
    public interface ICurrentUser
    {
        public User CurrentUser { get; }

        public bool loggedIn { get; set; }
    }
}
