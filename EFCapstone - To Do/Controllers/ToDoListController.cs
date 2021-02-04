using EFCapstone___To_Do.DAL_Models;
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
    //1) Complete log in with ASP Identity


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

        public IActionResult Search(string searchString)
        {
            var viewModel = new TaskListViewModel();
            var tasks = _toDoListDBContext.Tasks.Where(task => task.UserID == _currentUser.CurrentUser.UserID).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                var taskViewModelList = tasks
                    .Select(taskDAL => new TaskItem
                    {
                        Description = taskDAL.Description,
                        DueDate = taskDAL.DueDate,
                        TaskID = taskDAL.taskID,
                        UserID = taskDAL.UserID,
                        IsDone = taskDAL.IsDone
                    })
                    .Where(task => task.Description.Contains(searchString)).ToList();

                viewModel.Tasks = taskViewModelList;
                viewModel.UserAccount = _currentUser.CurrentUser.Email;

                return View("TaskList", viewModel);
            }

            return ListItems();
        }


        public IActionResult RegisterUser()
        {
            return View();
        }

        public IActionResult SortByDueDate()
        {
            var viewModel = new TaskListViewModel();
            var tasks = _toDoListDBContext.Tasks.Where(task => task.UserID == _currentUser.CurrentUser.UserID).ToList();

            var taskViewModelList = tasks
                .Select(taskDAL => new TaskItem { 
                    Description = taskDAL.Description, 
                    DueDate = taskDAL.DueDate, 
                    TaskID = taskDAL.taskID, 
                    UserID = taskDAL.UserID, 
                    IsDone = taskDAL.IsDone })
                .OrderBy(task => task.DueDate).ToList();

            viewModel.Tasks = taskViewModelList;
            viewModel.UserAccount = _currentUser.CurrentUser.Email;

            return View("TaskList", viewModel);

        }

        public IActionResult SortByCompletionStatus()
        {
            var viewModel = new TaskListViewModel();
            var tasks = _toDoListDBContext.Tasks.Where(task => task.UserID == _currentUser.CurrentUser.UserID).ToList();

            var taskViewModelList = tasks
                .Select(taskDAL => new TaskItem
                {
                    Description = taskDAL.Description,
                    DueDate = taskDAL.DueDate,
                    TaskID = taskDAL.taskID,
                    UserID = taskDAL.UserID,
                    IsDone = taskDAL.IsDone
                })
                .OrderBy(task => task.IsDone).ToList();

            viewModel.Tasks = taskViewModelList;
            viewModel.UserAccount = _currentUser.CurrentUser.Email;

            return View("TaskList", viewModel);

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
                    _currentUser.loggedIn = true;
                    return View(); 
                }

            }

            return View("LogIn", model);
        }


        public IActionResult AddTask()
        {

            if (_currentUser.loggedIn == false)
            {
                var errorModel = new ErrorPageViewModel();
                return View("ErrorPage", errorModel);
            }


            var model = new AddTaskViewModel();
            model.DueDate = DateTime.Now;
            model.UserAccount = _currentUser.CurrentUser.Email;
            return View(model);
        }

        public IActionResult PushTaskToSQL(AddTaskViewModel model)
        {
            var newTask = new TasksDAL();
            newTask.Description = model.Description;
            newTask.DueDate = model.DueDate;
            newTask.IsDone = model.IsDone;

            UsersDAL userDAL = _toDoListDBContext.Users
                .Where(user => user.Email == _currentUser.CurrentUser.Email).FirstOrDefault();

            newTask.UserID = userDAL.userID; //FOREIGN KEY SET UP

            _toDoListDBContext.Tasks.Add(newTask);
            _toDoListDBContext.SaveChanges();

            return ListItems();

        }



        //ONLY GET TO THIS ACTION AFTER ADDING A TASK
        public IActionResult TaskList(AddTaskViewModel model)
        {

            //TO ACCESS TASK LIST VIEW FROM THE HEADER LINK(rather than routing from ADD TASK VIEW
            if (_currentUser.loggedIn == false)
            {
                var errorModel = new ErrorPageViewModel();
                return View("ErrorPage", errorModel);
            }

            return ListItems();
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
            return ListItems();
        }

        public IActionResult DeleteItem(int id) 
        {
            
            var taskItem = GetTaskWhereIDIsFirstORDefault(id);
            var taskDAL = _toDoListDBContext.Tasks
                .First(taskDAL => taskDAL.taskID == taskItem.TaskID);

            _toDoListDBContext.Tasks.Remove(taskDAL);
            _toDoListDBContext.SaveChanges();

            //WE WANT TO CREATE A NEW LIST OF TASKS TO DISPLAY AFTER DELETING
            return ListItems();
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

        private ViewResult ListItems()
        {
            var viewModel = new TaskListViewModel();
            var tasks = _toDoListDBContext.Tasks.Where(task => task.UserID == _currentUser.CurrentUser.UserID).ToList();

            var taskViewModelList = tasks
                .Select(taskDAL => new TaskItem { Description = taskDAL.Description, DueDate = taskDAL.DueDate, TaskID = taskDAL.taskID, UserID = taskDAL.UserID, IsDone = taskDAL.IsDone}).ToList();

            viewModel.Tasks = taskViewModelList;
            viewModel.UserAccount = _currentUser.CurrentUser.Email;

            return View("TaskList", viewModel);
        }


    }
}
