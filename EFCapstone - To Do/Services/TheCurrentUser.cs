using EFCapstone___To_Do.Models.ToDoList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.Services
{
    public class TheCurrentUser : ICurrentUser
    {
        public User CurrentUser { get; } = new User();
        public bool loggedIn { get ; set ; }
    }
}
