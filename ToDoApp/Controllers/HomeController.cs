using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ToDoDbContext _context;

        public HomeController(ILogger<HomeController> logger, ToDoDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var ToDoList = _context.ToDo.OrderBy(t => t.Id ).ToList();
            
            return View(ToDoList);
        }

        public IActionResult DeleteToDo(int id)
        {
            try
            {
                _logger.LogInformation("User delete todo ID {Id}", id);
                var todoInDB = _context.ToDo.SingleOrDefault(x => x.Id == id);
                _context.ToDo.Remove(todoInDB);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo ID {Id}: {Message}", id, ex.Message);
            }
            return RedirectToAction("Index");
        }

        public IActionResult CheckToDo(int id)
        {
            try
            {
                var todoInDB = _context.ToDo.SingleOrDefault(x => x.Id == id);
                if (todoInDB.Check) todoInDB.Check = false;
                else todoInDB.Check = true;
                _logger.LogInformation("User update todo ID {Id} to {Check}", id, todoInDB.Check);
                _context.ToDo.Update(todoInDB);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo ID {Id}: {Message}", id, ex.Message);
            }
            return RedirectToAction("Index");
        }

        public IActionResult CreateEditToDo(int? id)
        {
            if (id != null)
            {
                var todoInDB = _context.ToDo.SingleOrDefault(x => x.Id == id);
                return View(todoInDB);
            }
            else
            {
                return View();
            }
        }

        public IActionResult CreateEditForm(ToDo model)
        {
            if (model.Id == 0)
            {
                try
                {
                    _logger.LogInformation("User add new todo ID: {Id} with description: {Description}", model.Id, model.Description);
                    _context.ToDo.Add(model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding todo ID {Id}: {Message}", model.Id, ex.Message);
                }
            }
            else
            {
                try
                {
                    _logger.LogInformation("User edit todo ID {Id} with description: {Description}", model.Id, model.Description);
                    _context.ToDo.Update(model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing todo ID {Id}: {Message}", model.Id, ex.Message);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
