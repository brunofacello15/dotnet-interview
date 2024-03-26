using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/todolists/{idTodoList}/todoitems")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }
        //POST api/todolists/{idTodoList}/todoitems
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(long idTodoList, TodoItem todoItem)
        {
            var todoList = await _context.TodoList.FindAsync(idTodoList);

            if (todoList == null)
            {
                return NotFound();
            }

            if (todoList.Items == null)
            {
                todoList.Items = new List<TodoItem>();
            }

            todoList.Items.Add(todoItem);
            _context.Update(todoList);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetTodoItem", new { idTodoList = idTodoList, idTodoItem = todoItem.Id }, todoItem);


        }

        //GET api/todolists/{idTodoList}/todoitems/{idTodoItem}
        [HttpGet("{idTodoItem}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long idTodoList, long idTodoItem)
        {
            if (_context.TodoItem == null)
            {
                return NotFound();
            }
            var todoList = await _context.TodoList.Include(t => t.Items)
                                                  .FirstOrDefaultAsync(t => t.Id == idTodoList);
            if (todoList == null)
            {
                return BadRequest();
            }
            if (todoList.Items.IsNullOrEmpty())
            {
                return NotFound();
            }
            var todoItem = todoList.Items.FirstOrDefault(i => i.Id == idTodoItem);
            if (todoItem == null)
            {
                return NotFound();
            }
            return Ok(todoItem);
        }

        //PUT api/todolists/{idTodoList}/todoitems/{idTodoItem}
        [HttpPut("{idTodoItem}")]
        public async Task<ActionResult> PutTodoItem(long idTodoList, long idTodoItem, TodoItem todoItem)
        {
            if (idTodoItem != todoItem.Id)
            {
                return BadRequest();
            }
            var todoList = await _context.TodoList.Include(t => t.Items)
                                                  .FirstOrDefaultAsync(i=> i.Id==idTodoList);
            if (todoList == null || todoList.Items.IsNullOrEmpty())
            {
                return NotFound();
            }
            var todoItemExisting = todoList.Items.FirstOrDefault(i => i.Id == idTodoItem);
            if (todoItemExisting == null)
            {
                return BadRequest();
            }
            todoItemExisting.Name = todoItem.Name;
            todoItemExisting.State= todoItem.State;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.TodoItem?.Any(i => i.Id == idTodoItem) ?? false)
                {
                    throw;
                }
                else
                {
                    return NotFound();
                }
            }
            return NoContent();
        }

        //DELETE  api/todolists/{idTodoList}/todoitems/{idTodoItem}
        [HttpDelete("{idTodoItem}")]
        public async Task<ActionResult> DeleteTodoItem(long idTodoList, long idTodoItem)
        {
            if (_context.TodoItem == null)
            {
                return NotFound();
            }
            var todoList = await _context.TodoList.FindAsync(idTodoList);
            if (todoList == null)
            {
                return NotFound();
            }
            var todoItem = await _context.TodoItem.FindAsync(idTodoItem);
            if (todoItem == null)
            {
                return NotFound();
            }
            if (todoList.Items.Find(i => i.Id == idTodoItem) == null)
            {
                return BadRequest();
            }
            _context.TodoItem.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
