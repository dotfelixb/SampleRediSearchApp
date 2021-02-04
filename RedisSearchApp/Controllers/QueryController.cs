using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NRediSearch;
using RedisSearchApp.Model;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace RedisSearchApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _redisDb;
        private readonly Client _client;

        public QueryController(IConnectionMultiplexer connectionMultiplexer)
        {
            var key = "idx:movies";

            _connectionMultiplexer = connectionMultiplexer;
            _redisDb = _connectionMultiplexer.GetDatabase();
            _client = new Client(key, _redisDb);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string q)
        {
            var query = new Query(q)
                    .ReturnFields("v")
                    //.Limit(0, 1)
                    ;

            var res = await _client.SearchAsync(query);

            var values = res.Documents
                // be sure the field 'v' is in the result set
                .Where(d => d.HasProperty("v"))
                .Select(d => JsonConvert.DeserializeObject<Movie>(d["v"]) );

            return Ok(new { values });
        }
    }
}