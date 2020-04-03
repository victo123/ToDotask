using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;


        public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Title = "Item1", Description = "Description 1", Expiry = DateTime.Now });
                _context.TodoItems.Add(new TodoItem { Title = "Item2", Description = "Description 2", Expiry = DateTime.Now.AddDays(1) });
                _context.TodoItems.Add(new TodoItem { Title = "Item3", Description = "Description 3", Expiry = DateTime.Now.AddDays(2) });
                _context.TodoItems.Add(new TodoItem { Title = "Item4", Description = "Description 4", Expiry = DateTime.Now.AddDays(7) });
                _context.SaveChanges();
            }
        }


        //Method GET: api/Todo
        // •	Get All Todo’s 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }


        //Method GET: api/Todo/2
        // •	Get Specific Todo 
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }


        //Method GET: api/Todo/GetIncomingToDo/1
        // 1 today 2 next day 3 current week
        // •	Get Incoming ToDo (for today/next day/current week)
        [HttpGet("GetIncomingToDo/{type}")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetIncomingToDo(long type)
        {
            List<TodoItem> todoItem = null;
            if (type == 1)
            {
                todoItem = await _context.TodoItems.Where(x => x.Expiry.Day == DateTime.Now.Day).ToListAsync();
            }else if (type == 2)
            {
                todoItem = await _context.TodoItems.Where(x => x.Expiry.Day <= DateTime.Now.AddDays(1).Day ).ToListAsync();
            }else if (type == 3)
            {
                todoItem = await _context.TodoItems.Where(x => x.Expiry.Day <= DateTime.Now.AddDays(7).Day ).ToListAsync();
            }

            if (todoItem == null || todoItem.Count == 0)
            {
                return NotFound();
            }

            return todoItem;
        }


        //Method POST: api/Todo
        // •	Create Todo  
        [HttpPost]
        public async Task<ActionResult> Submit([FromForm] TodoItem item)
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);
        }


        //Method PUT: api/Todo/5
        // •	Update Todo 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, [FromForm] TodoItem item)
        {
            if (item ==  null)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        //Method PUT: api/Todo/Todocompletecheck/id=5
        // •	Set Todo percent complete 
        [HttpPut("Todocompletecheck/{id}")]
        public async Task<IActionResult> Todo_percent_complete_check(long id)
        {
            var ItemUpdate = await _context.TodoItems.FindAsync(id);

            if (ItemUpdate == null)
            {
                return BadRequest();
            }

            ItemUpdate.Complete = 100; 
            ItemUpdate.IsComplete = true;

            _context.Entry(ItemUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(ItemUpdate);
        }

        //Method DELETE: api/Todo/5
        // •	Delete Todo 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return Ok(todoItem);
        }

        //Method GET: api/Todo/MarkCheck/true
        // •	Mark Todo as Done 
        [HttpGet("MarkCheck/{_istrue}")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> Mark_Todo_Done_Check(bool _istrue)
        {
            var todoItem = await _context.TodoItems.Where(x => x.IsComplete == _istrue).ToListAsync();

            if (todoItem == null || todoItem.Count == 0)
            {
                return NotFound();
            }

            return todoItem;

        }
    }
}