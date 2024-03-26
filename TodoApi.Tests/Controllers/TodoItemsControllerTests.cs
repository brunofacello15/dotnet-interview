using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApi.Controllers;
using TodoApi.Models;

namespace TodoApi.Tests.Controllers
{
    public class TodoItemsControllerTests
    {
        private DbContextOptions<TodoContext> DatabaseContextOptions()
        {
            return new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }
        private void PopulateDatabaseContext(TodoContext context)
        {
            TodoItem todoItem1 = new TodoItem { Id = 1, Name = "SubTask 1", State = false };
            TodoItem todoItem2 = new TodoItem { Id = 2, Name = "SubTask 2", State = false };
            context.TodoItem.Add(todoItem1);
            context.TodoItem.Add(todoItem2);
            context.TodoList.Add(new Models.TodoList { Id = 1, Name = "Task 1", Items = new List<TodoItem> { todoItem1, todoItem2 } });
            context.TodoList.Add(new Models.TodoList { Id = 2, Name = "Task 2", Items = new List<TodoItem>() });
            context.SaveChanges();
        }
        [Fact]
        public async Task GetTodoItem_WhenCalled_ReturnsTodoItem()
        {
            using (var context = new TodoContext(DatabaseContextOptions()))
            {
                PopulateDatabaseContext(context);

                var controller = new TodoItemsController(context);

                var result = await controller.GetTodoItem(1, 1);

                Assert.IsType<OkObjectResult>(result.Result);
                Assert.Equal(
                  1,
                    ((result.Result as OkObjectResult).Value as TodoItem).Id
                   );
            }
        }
        [Fact]
        public async Task GetTodoItem_WhenTodoListIsEmpty_ReturnsNotFound()
        {
            using (var context = new TodoContext(DatabaseContextOptions()))
            {
                PopulateDatabaseContext(context);

                var controller = new TodoItemsController(context);

                var result = await controller.GetTodoItem(2, 1);

                Assert.IsType<NotFoundResult>(result.Result);
            }
        }
        [Fact]
        public async Task PostTodoItem_WhenCalled_AddsTodoItem()
        {
            using (var context = new TodoContext(DatabaseContextOptions()))
            {
                PopulateDatabaseContext(context);

                var controller = new TodoItemsController(context);
                TodoItem todoItem3 = new TodoItem { Name = "Subtask 3", State = true };
                var result = await controller.PostTodoItem(2, todoItem3);

                Assert.IsType<CreatedAtActionResult>(result.Result);
                Assert.Equal(
                  3,
                  ((result.Result as CreatedAtActionResult).Value as TodoItem).Id
                );
            }
        }
        [Fact]
        public async Task PutTodoItem_WhenCalled_UpdatesTheTodoItem()
        {
            using (var context = new TodoContext(DatabaseContextOptions()))
            {
                PopulateDatabaseContext(context);

                var controller = new TodoItemsController(context);

                var todoItem = await context.TodoItem.Where(x => x.Id == 2).FirstAsync();
                var result = await controller.PutTodoItem(1, 2, todoItem);

                Assert.IsType<NoContentResult>(result);
            }
        }
        [Fact]
        public async Task PutTodoItem_WhenTodoListDoesntHaveTodoItem_ReturnBadRequest()
        {
            using (var context = new TodoContext(DatabaseContextOptions()))
            {
                PopulateDatabaseContext(context);

                var controller = new TodoItemsController(context);

                TodoItem todoItem4 = new TodoItem { Id = 4, Name = "Subtask 4", State = true };
                var result = await controller.PutTodoItem(1, 4, todoItem4);

                Assert.IsType<BadRequestResult>(result);
            }
        }
        [Fact]
        public async Task PutTodoItem_WhenTodoListIsEmpty_ReturnNotFound()
        {
            using (var context = new TodoContext(DatabaseContextOptions()))
            {
                PopulateDatabaseContext(context);

                var controller = new TodoItemsController(context);

                TodoItem todoItem4 = new TodoItem { Id = 4, Name = "Subtask 4", State = true };
                var result = await controller.PutTodoItem(2, 4, todoItem4);

                Assert.IsType<NotFoundResult>(result);
            }
        }
        [Fact]
        public async Task DeleteTodoItem_WhenCalled_RemovesTodoItem()
        {
            using (var context = new TodoContext(DatabaseContextOptions()))
            {
                PopulateDatabaseContext(context);

                var controller = new TodoItemsController(context);
                var result = await controller.DeleteTodoItem(1, 2);

                Assert.IsType<NoContentResult>(result);

            }
        }
        [Fact]
        public async Task DeleteTodoItem_WhenTodoListIsEmpty_ReturnsBadRequest()
        {
            using (var context = new TodoContext(DatabaseContextOptions()))
            {
                PopulateDatabaseContext(context);

                var controller = new TodoItemsController(context);
                var result = await controller.DeleteTodoItem(2, 2);

                Assert.IsType<BadRequestResult>(result);
            }
        }
    }
}
