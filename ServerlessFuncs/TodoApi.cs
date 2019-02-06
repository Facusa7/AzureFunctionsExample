
using System.Collections.Generic;
using System.IO;
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
    }
}
