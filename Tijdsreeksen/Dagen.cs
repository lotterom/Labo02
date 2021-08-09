using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace TijdsReeken
{
    public static class Dagen
    {
        [FunctionName("GetDagen")]
        public static async Task<IActionResult> GetDagen(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "days")] HttpRequest req, ILogger log)
        {
            //variable om dagen door te geven
            List<string> days = new List<string>();

            using (SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("connectionstring")))
            {
                //open connectie
                await conn.OpenAsync();
                //geven connectie door aan het sqlcommand zo weet hij via welke connectie hij de db moet benaderen.
                using(SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;

                    //nu maken we het sqlcommand
                    string sql = "select distinct DagVanDeWeek from tbl_bezoekers";
                    command.CommandText = sql;

                    //data ophalen (sql reader)
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        days.Add(reader["DagVanDeWeek"].ToString());
                    }
                }

            }
            //geef statuscode terug
            return new OkObjectResult(days);
        }
    }
}
