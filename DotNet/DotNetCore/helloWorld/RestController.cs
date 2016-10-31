using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ConsoleApplication
{
    [Route("api/[controller]")]
    public class RestController : Controller
    {
        public RestController()
        {

        }

        //test with url http://localhost:5000/api/rest
        [HttpGet]
        public IEnumerable<Item> GetAll()
        {
            var items = new List<Item>()
            {
                new Item()
                {
                    NumberValue = 3.14,
                    StrValue = "Hello",
                    BooleanValue = true
                },
                new Item()
                {
                    NumberValue = 0.618,
                    StrValue = "World",
                    BooleanValue = false
                },
            };
            return items;
        }

        //test with url http://localhost:5000/api/rest/xxx
        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(string id)
        {
            var item = new Item()
            {
                NumberValue = 3.14,
                StrValue = "Hello",
                BooleanValue = true
            };
            return new ObjectResult(item);
        }
    }
}