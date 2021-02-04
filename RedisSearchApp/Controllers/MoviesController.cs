using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisSearchApp.Model;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisSearchApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _redisDb;

        public MoviesController(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _redisDb = _connectionMultiplexer.GetDatabase();
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var key = MakeId(id);
            var exist = await _redisDb.KeyExistsAsync(key);

            if (!exist)
            {
                return NotFound();
            }

            var d = await _redisDb.HashGetAsync(key, "v");
            var movie = JsonConvert.DeserializeObject<Movie>(d);

            return Ok(new { payload = movie });
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Movie model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var stringifyedModel = JsonConvert.SerializeObject(model);

            await _redisDb.HashSetAsync($"movies:{model.Id}", new HashEntry[] {
                 new HashEntry("tl", model.Title)
                , new HashEntry ( "ry" , model.ReleaseYear)
                , new HashEntry ( "rt" , model.Rating)
                , new HashEntry ( "gn" , model.Genre)
                , new HashEntry ( "v", stringifyedModel  )
            });

            return CreatedAtAction(nameof(Get), new { id = model.Id });
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        public string MakeId(Guid id) => $"movies:{id}";
    }
}