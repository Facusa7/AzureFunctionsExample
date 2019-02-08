
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace ServerlessFuncs
{
    public static class TodoApi
    {
        private static List<Todo> items = new List<Todo>(); //For now

        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")]HttpRequest req, TraceWriter log)
        {
            log.Info("Creating a new TODO list item");

            var requestBody = new StreamReader(req.Body).ReadToEnd();
            var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

            var todo = new Todo() { TaskDescription = input.TaskDescription };
            items.Add(todo); //Save in the DB

            return new OkObjectResult(todo);
        }

        [FunctionName("GetTodos")]
        public static IActionResult GetTodos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")]HttpRequest req, TraceWriter log)
        {
            log.Info("Getting todo list items");
            return new OkObjectResult(items);
        }

        [FunctionName("GetTodoById")]
        public static IActionResult GetTodoById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")]HttpRequest req, TraceWriter log, string id)
        {
            log.Info("Getting todo item by id");

            var todo = items.FirstOrDefault(t => t.Id == id);
            
            if (todo == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(todo);
        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")]HttpRequest req, TraceWriter log, string id)
        {
            log.Info("Updating TODO item");

            var todo = items.FirstOrDefault(t => t.Id == id);
            
            if (todo == null)
            {
                return new NotFoundResult();
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);

            todo.IsCompleted = updated.IsCompleted;
            if (!string.IsNullOrWhiteSpace(updated.TaskDescription))
            {
                todo.TaskDescription = updated.TaskDescription;
            }

            return new OkObjectResult(todo);
        }

        [FunctionName("DeleteTodo")]
        public static IActionResult DeleteTodo([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")]HttpRequest req, TraceWriter log, string id)
        {
            log.Info("Deleting todo item by id");

            var todo = items.FirstOrDefault(t => t.Id == id);
            
            if (todo == null)
            {
                return new NotFoundResult();
            }

            items.Remove(todo);

            return new OkResult();
        }
    }
}
