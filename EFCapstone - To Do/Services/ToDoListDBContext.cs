using EFCapstone___To_Do.DAL_Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.Models.ToDoList
{
    public class ToDoListDBContext : DbContext
    {
        public ToDoListDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<UsersDAL> Users { get; set; }
        public DbSet<TasksDAL> Tasks { get; set; }

    }
}
