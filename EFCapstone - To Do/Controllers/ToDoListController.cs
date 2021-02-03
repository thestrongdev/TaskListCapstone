﻿using EFCapstone___To_Do.DAL_Models;
using EFCapstone___To_Do.Models.ToDoList;
using EFCapstone___To_Do.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.Controllers
{
    //OUTSTANDING ITEMS
    //make it look pretty?


    public class ToDoListController : Controller
    {
        private ToDoListDBContext _toDoListDBContext;
        private ICurrentUser _currentUser;

        public ToDoListController(ToDoListDBContext toDoListDBContext, ICurrentUser currentUser)
        {
            _toDoListDBContext = toDoListDBContext;
            _currentUser = currentUser;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RegisterUser()
        {
            return View();
        }

        public IActionResult ThankYou(RegisterUserViewModel model)
        {
            var newUser = new UsersDAL();
            newUser.Email = model.Email;
            newUser.Password = model.Password;

            _toDoListDBContext.Users.Add(newUser);
            _toDoListDBContext.SaveChanges();
            return View();
        }


        public IActionResult LogIn() //ASSUME WE CAN ONLY REACH THE LOG IN PAGE AFTER REGISTERING
        {

            return View();
         
        }

        public IActionResult SuccessfulLogIn(LogInViewModel model) 
        {
            foreach (var user in _toDoListDBContext.Users.ToList())
            {
                if (user.Email == model.Email)
                {
                    _currentUser.CurrentUser.Email = model.Email;
                    _currentUser.CurrentUser.Password = model.Password;
                    _currentUser.CurrentUser.UserID = user.userID;
                    return View(); 
                }

            }

            return View("LogIn", model);
        }


        public IActionResult AddTask()
        {
            var model = new AddTaskViewModel();
            model.DueDate = DateTime.Now;
            return View(model);
        }

       
        //ONLY GET TO THIS ACTION AFTER ADDING A TASK
        public IActionResult TaskList(AddTaskViewModel model)
        {
 
            //BELOW ADDS THE TASK SUBMITTED FROM THE ADD TASK VIEW
            var newTask = new TasksDAL();
            newTask.Description = model.Description;
            newTask.DueDate = model.DueDate;
            newTask.IsDone = model.IsDone;

            UsersDAL userDAL = _toDoListDBContext.Users
                .Where(user => user.Email == _currentUser.CurrentUser.Email).FirstOrDefault();

            newTask.UserID = userDAL.userID; //FOREIGN KEY SET UP

            _toDoListDBContext.Tasks.Add(newTask);
            _toDoListDBContext.SaveChanges();

            //ASSEMBLE THE LIST OF TASKS ASSOCIATED WITH THE USER'S ID
            //CAN ALSO BE PUT INTO FUNCTION
            var viewModel = new TaskListViewModel();
            var tasks = _toDoListDBContext.Tasks.Where(task => task.UserID == _currentUser.CurrentUser.UserID).ToList();

            viewModel.Tasks = tasks.Select(tasksDAL => new TaskItem()
            {
                Description = tasksDAL.Description,
                DueDate = tasksDAL.DueDate,
                IsDone = tasksDAL.IsDone,
                TaskID = tasksDAL.taskID

            }).ToList();

            return View(viewModel);
        }

        public IActionResult MarkComplete(int id)
        {

            //GET CORRECT ITEM FROM DATABASE
            var taskItem = GetTaskWhereIDIsFirstORDefault(id);
            var taskDAL = _toDoListDBContext.Tasks
                .First(taskDAL => taskDAL.taskID == taskItem.TaskID);

            taskDAL.IsDone = "YES";
            _toDoListDBContext.SaveChanges();

            //WE WANT TO CREATE A NEW LIST OF TASKS TO DISPLAY AFTER UPDATING STATUS ---PUT BELOW IN A FUNCTION!!!
            return ListItems(id);
        }

        public IActionResult DeleteItem(int id) 
        {
            
            var taskItem = GetTaskWhereIDIsFirstORDefault(id);
            var taskDAL = _toDoListDBContext.Tasks
                .First(taskDAL => taskDAL.taskID == taskItem.TaskID);

            _toDoListDBContext.Tasks.Remove(taskDAL);
            _toDoListDBContext.SaveChanges();

            //WE WANT TO CREATE A NEW LIST OF TASKS TO DISPLAY AFTER DELETING
            return ListItems(id);
        }



        //FUNCTIONS FOR REPEATED BLOCKS OF CODE
        private TaskItem GetTaskWhereIDIsFirstORDefault(int id)
        {
            TasksDAL taskDAL = _toDoListDBContext.Tasks
                .Where(task => task.taskID == id)
                .FirstOrDefault();

            var task = new TaskItem();
            task.DueDate = taskDAL.DueDate;
            task.Description = taskDAL.Description;
            task.TaskID = taskDAL.taskID;
            task.UserID = taskDAL.UserID;

            return task;
        }

        private ViewResult ListItems(int id)
        {
            var viewModel = new TaskListViewModel();
            var tasks = _toDoListDBContext.Tasks.Where(task => task.UserID == _currentUser.CurrentUser.UserID).ToList();

            var taskViewModelList = tasks
                .Select(taskDAL => new TaskItem { Description = taskDAL.Description, DueDate = taskDAL.DueDate, TaskID = taskDAL.taskID, UserID = taskDAL.UserID, IsDone = taskDAL.IsDone}).ToList();

            viewModel.Tasks = taskViewModelList;

            return View("TaskList", viewModel);
        }

    }
}
