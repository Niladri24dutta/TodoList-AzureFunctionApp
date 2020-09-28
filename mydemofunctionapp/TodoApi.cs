using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using System.Linq;

namespace mydemofunctionapp
{
    public static class TodoApi
    {
        static List<Todo> items = new List<Todo>();
        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            TraceWriter log)
        {
            log.Info("Create todo item");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);
            var todo = new Todo { TaskDescription = input.TaskDescription };
            items.Add(todo);
            return new OkObjectResult(todo);
        }

        [FunctionName("GetTodos")]
        public static async Task<IActionResult> GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos")] HttpRequest req,
            TraceWriter log)
        {
            log.Info("Get all todo items");

            return new OkObjectResult(items);
        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
            TraceWriter log,string id)
        {
            log.Info("Get Todo by Id");
            var todo = items.FirstOrDefault(x => x.Id == id);
            if (todo == null)
            {
                return new NotFoundResult();
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);
            log.Info("Update todo items");
            todo.IsCompleted = updated.IsCompleted;
            if (!string.IsNullOrEmpty(updated.TaskDescription))
            {
                todo.TaskDescription = updated.TaskDescription;
            }
      
            return new OkObjectResult(todo);
        }

        [FunctionName("DeleteTodo")]
        public static async Task<IActionResult> DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
            TraceWriter log,string id)
        {
            log.Info("Delete todo item");
            var todo = items.FirstOrDefault(x => x.Id == id);
            if (todo == null)
            {
                return new NotFoundResult();
            }
            items.Remove(todo);
            return new OkResult();
        }

        [FunctionName("GetTodoById")]
        public static async Task<IActionResult> GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
            TraceWriter log,string id)
        {
            log.Info("Get Todo by Id");
            var todo = items.FirstOrDefault(x => x.Id == id);
            if(todo == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(todo);
        }
    }
}
