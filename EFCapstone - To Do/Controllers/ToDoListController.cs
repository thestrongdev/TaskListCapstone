using EFCapstone___To_Do.DAL_Models;
using EFCapstone___To_Do.Models.ToDoList;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.Controllers
{
    public class ToDoListController : Controller
    {
        private ToDoListDBContext _toDoListDBContext;

        public ToDoListController(ToDoListDBContext toDoListDBContext)
        {
            _toDoListDBContext = toDoListDBContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RegisterUser()
        {
            return View();
        }

        public IActionResult LogIn(RegisterUserViewModel model) 
        {
            //if database contains user email, return add task view
            //else if add them to database THEN return add task view

            var viewModel = new AddTaskViewModel();

            foreach(var user in _toDoListDBContext.Users.ToList())
            {
                if(user.Email == model.Email)
                {
                    return View("AddTask", viewModel);
                }
       
            }

            //UsersDAL userDAL = _toDoListDBContext.Users
            //    .Where(user => user.Email == model.Email).FirstOrDefault();


        
            var newUser = new UsersDAL();
            newUser.Email = model.Email;
            newUser.Password = model.Password;

            _toDoListDBContext.Users.Add(newUser);
            _toDoListDBContext.SaveChanges();

            return View("AddTask", viewModel);
         
        }

        public IActionResult AddTask(LogInViewModel model)
        {
            //how to add task to database with button
            //but also link to "add task" page with link from header
            //using this same action
            //NEED foreign key added to database for task table
            //need current user log in information for primary key foreign key relationship
            //when you add a task, also add the users primary key to table

            var viewModel = new AddTaskViewModel();

            var newTask = new TasksDAL();
            newTask.Description = viewModel.Description;
            newTask.DueDate = viewModel.DueDate;
            newTask.IsDone = viewModel.IsDone;

            return View();
        }

       
        public IActionResult TaskList()
        {
            //only show tasks that share that primary key/foreign key relationship
            return View();
        }

    }
}
