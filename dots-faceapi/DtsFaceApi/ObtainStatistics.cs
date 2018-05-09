using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Npgsql;

namespace DtsFaceApi
{
    public static class ObtainStatistics
    {
        [FunctionName("ObtainStatistics")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var result = await GetStats();

            var rsp = req.CreateResponse(HttpStatusCode.OK);
            rsp.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
            return rsp;
        }

        private static async Task<object> GetStats()
        {
            using (var con = new NpgsqlConnection("server=dots.mluvii.com;database=onop;user id=stats;password=Prdel00."))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
with st as (
  select
	s.session_id,
	date_trunc('month', entered_queue) as time_month,
	ended - started as length,
	(select stars from session_feedback f where s.session_id = f.session_id and f.client_temp_id is not null and f.stars > 0 limit 1) as stars,
	(select user_id from session_operator o where s.session_id = o.session_id limit 1) is not null as had_human_operator
  from session s
  where started is not null and ended is not null
)
select
  time_month,
  count(session_id) count,
  avg(length) average_length,
  avg(stars) average_stars,
  count(session_id) filter (where had_human_operator) count_with_human,
  count(session_id) filter (where not had_human_operator) count_without_human
from st
group by grouping sets ((), (time_month));
";

                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        var result = new List<object>();
                        while (rdr.Read())
                        {
                            result.Add(new
                            {
                                month = !rdr.IsDBNull(0) ? rdr.GetDateTime(0).ToString("yyyy-MM") : null,
                                count = rdr.GetValue(1),
                                averageLength = rdr.GetValue(2),
                                averageStars = rdr.GetValue(3),
                                countWithHuman = rdr.GetValue(4),
                                countWithoutHuman = rdr.GetValue(5)
                            });
                        }

                        return result;
                    }
                }
            }
        }
    }
}
