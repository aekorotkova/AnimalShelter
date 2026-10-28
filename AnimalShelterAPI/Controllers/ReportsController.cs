using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using AnimalShelterAPI.Models;

namespace AnimalShelterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        [HttpGet("species")]
        [ProducesResponseType(typeof(List<SpeciesStatDto>), 200)]
        public IActionResult GetSpeciesStatistics()
        {
            var list = new List<SpeciesStatDto>();

            string query = @"
                SELECT s.SpeciesName,
                       COUNT(a.AnimalID) AS TotalCount,
                       SUM(CASE WHEN a.IsAdopted = 0 THEN 1 ELSE 0 END) AS InShelterCount,
                       SUM(CASE WHEN a.IsAdopted = 1 THEN 1 ELSE 0 END) AS AdoptedCount
                FROM Species_kursovoi s
                LEFT JOIN Animals_kursovoi a ON s.SpeciesID = a.SpeciesID
                GROUP BY s.SpeciesName
                ORDER BY s.SpeciesName";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SpeciesStatDto
                        {
                            SpeciesName = reader["SpeciesName"].ToString(),
                            TotalCount = Convert.ToInt32(reader["TotalCount"]),
                            InShelterCount = reader["InShelterCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["InShelterCount"]),
                            AdoptedCount = reader["AdoptedCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AdoptedCount"])
                        });
                    }
                }
            }

            return Ok(list);
        }

        [HttpGet("in-shelter")]
        [ProducesResponseType(200)]
        public IActionResult GetInShelterCount()
        {
            string query = "SELECT COUNT(*) FROM Animals_kursovoi WHERE IsAdopted = 0";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return Ok(new { animalsInShelter = count });
            }
        }
    }
}