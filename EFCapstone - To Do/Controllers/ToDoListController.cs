using EFCapstone___To_Do.DAL_Models;
using EFCapstone___To_Do.Models.ToDoList;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCapstone___To_Do.Controllers
{
    //OUTSTANDING ITEMS
    //keep user logged in the whole process! - keeps routing me back to log in after clicking "add Task" in the header even though I've logged in already
    //note that after adding a task, I get routed to the task list, but list is showing empty...
    //Fix duedate function as it starts at year "0001"
    //add checkbox for isDone property
    //make it look pretty?


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
            var logInModel = new LogInViewModel();
            logInModel.Email = model.Email;
            logInModel.Password = model.Password;


            var viewModel = new AddTaskViewModel();

            foreach(var user in _toDoListDBContext.Users.ToList())
            {
                if(user.Email == model.Email)
                {
                    return View("AddTask", logInModel); //or should we return the task list???
                }
       
            }
                   
            var newUser = new UsersDAL();
            newUser.Email = model.Email;
            newUser.Password = model.Password;

            _toDoListDBContext.Users.Add(newUser);
            _toDoListDBContext.SaveChanges();

            return View("AddTask", viewModel);
         
        }

        public IActionResult AddTask(LogInViewModel model)
        {
       
            var viewModel = new AddTaskViewModel();
            var logInView = new LogInViewModel();
           

            //check if user is even logged in
            if(model.Email == null || model.Password == null)
            {
                return View("LogIn", logInView);
            }

            //the below is for if we link to addTask in the header
            if(viewModel.Description==null || viewModel.DueDate == null)
            {
                return View(viewModel);
            }

            var newTask = new TasksDAL();
            newTask.Description = viewModel.Description;
            newTask.DueDate = viewModel.DueDate;
            newTask.IsDone = viewModel.IsDone;

            UsersDAL userDAL = _toDoListDBContext.Users
                .Where(user => user.Email == model.Email).FirstOrDefault();

            newTask.UserID = userDAL.userID;

            _toDoListDBContext.Tasks.Add(newTask);
            _toDoListDBContext.SaveChanges();

            var viewTaskList = new TaskListViewModel();

            return View("TaskList", viewTaskList);
        }

       
        public IActionResult TaskList(LogInViewModel model)
        {
            //only show tasks that share that primary key/foreign key relationship

            var LogInView = new LogInViewModel();

            if (model.Email == null || model.Password == null)
            {
                return View("LogIn", LogInView);
            }

            var currentUser = new UsersDAL();
            currentUser.Email = model.Email;
            currentUser.Password = model.Password;
         

            var viewModel = new TaskListViewModel();
            var tasks = _toDoListDBContext.Tasks.Where(task => task.UserID == currentUser.userID).ToList();

            viewModel.Tasks = tasks.Select(tasksDAL => new TaskItem()
            {
                Description = tasksDAL.Description,
                DueDate = tasksDAL.DueDate,
                IsDone = tasksDAL.IsDone,
                TaskID = tasksDAL.taskID

            }).ToList();

            return View(viewModel);
        }

        public IActionResult DeleteItem(int id, int userID) 
        {
            var taskItem = GetTaskWhereIDIsFirstORDefault(id, userID);
            var taskDAL = _toDoListDBContext.Tasks
                .First(taskDAL => taskDAL.taskID == taskItem.TaskID);

            _toDoListDBContext.Tasks.Remove(taskDAL);
            _toDoListDBContext.SaveChanges();

            var viewModel = new TaskListViewModel();
            var tasks = _toDoListDBContext.Tasks.Where(task => task.UserID == userID).ToList();

            var taskViewModelList = tasks
                .Select(taskDAL => new TaskItem { Description = taskDAL.Description, DueDate = taskDAL.DueDate, TaskID = taskDAL.taskID, UserID = taskDAL.UserID }).ToList();

            viewModel.Tasks = taskViewModelList;

            return View();
        }

        private TaskItem GetTaskWhereIDIsFirstORDefault(int id, int userID)
        {
            TasksDAL taskDAL = _toDoListDBContext.Tasks
                .Where(task => task.taskID == id && task.UserID == userID)
                .FirstOrDefault();

            var task = new TaskItem();
            task.DueDate = taskDAL.DueDate;
            task.Description = taskDAL.Description;
            task.TaskID = taskDAL.taskID;
            task.UserID = taskDAL.UserID;

            return task;
        }

    }
}
