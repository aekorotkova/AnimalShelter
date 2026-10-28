using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using AnimalShelterAPI.Models;

namespace AnimalShelterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdoptionsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<AdoptionDto>), 200)]
        public IActionResult GetAll()
        {
            var list = new List<AdoptionDto>();

            string query = @"
                SELECT a.AdoptionID, an.Nickname AS AnimalNickname,
                       v.LastName + ' ' + v.FirstName AS VisitorName,
                       v.Phone AS VisitorPhone,
                       e.LastName + ' ' + e.FirstName AS EmployeeName,
                       a.AdoptionDate, a.Notes, a.NextContactDate
                FROM Adoptions_kursovoi a
                JOIN Animals_kursovoi an ON a.AnimalID = an.AnimalID
                JOIN Visitors_kursovoi v ON a.VisitorID = v.VisitorID
                JOIN Employees_kursovoi e ON a.EmployeeID = e.EmployeeID
                ORDER BY a.AdoptionID DESC";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AdoptionDto
                        {
                            AdoptionID = (int)reader["AdoptionID"],
                            AnimalNickname = reader["AnimalNickname"].ToString(),
                            VisitorName = reader["VisitorName"].ToString(),
                            VisitorPhone = reader["VisitorPhone"] == DBNull.Value ? "" : reader["VisitorPhone"].ToString(),
                            EmployeeName = reader["EmployeeName"].ToString(),
                            AdoptionDate = (DateTime)reader["AdoptionDate"],
                            Notes = reader["Notes"] == DBNull.Value ? "" : reader["Notes"].ToString(),
                            NextContactDate = reader["NextContactDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["NextContactDate"]
                        });
                    }
                }
            }

            return Ok(list);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult Create([FromBody] AdoptionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.AnimalID <= 0 || dto.VisitorID <= 0 || dto.EmployeeID <= 0)
                return BadRequest(new { message = "Не указаны обязательные ID (животное, посетитель, сотрудник)" });

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                string insertQuery = @"
                    INSERT INTO Adoptions_kursovoi (AnimalID, VisitorID, EmployeeID, AdoptionDate, Notes, NextContactDate)
                    VALUES (@AnimalID, @VisitorID, @EmployeeID, @AdoptionDate, @Notes, @NextContactDate);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int newId;
                using (var cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@AnimalID", dto.AnimalID);
                    cmd.Parameters.AddWithValue("@VisitorID", dto.VisitorID);
                    cmd.Parameters.AddWithValue("@EmployeeID", dto.EmployeeID);
                    cmd.Parameters.AddWithValue("@AdoptionDate", dto.AdoptionDate);
                    cmd.Parameters.AddWithValue("@Notes", (object)dto.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NextContactDate", (object)dto.NextContactDate ?? DBNull.Value);
                    newId = (int)cmd.ExecuteScalar();
                }

                string updateQuery = "UPDATE Animals_kursovoi SET IsAdopted = 1 WHERE AnimalID = @AnimalID";
                using (var cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@AnimalID", dto.AnimalID);
                    cmd.ExecuteNonQuery();
                }

                return CreatedAtAction(nameof(GetAll), new { id = newId }, new { id = newId, message = "Усыновление оформлено" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int id)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                int animalId;
                string selectQuery = "SELECT AnimalID FROM Adoptions_kursovoi WHERE AdoptionID = @Id";
                using (var cmd = new SqlCommand(selectQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    var result = cmd.ExecuteScalar();
                    if (result == null)
                        return NotFound(new { message = "Усыновление не найдено" });
                    animalId = (int)result;
                }

                using (var cmd = new SqlCommand("DELETE FROM Adoptions_kursovoi WHERE AdoptionID = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand("UPDATE Animals_kursovoi SET IsAdopted = 0 WHERE AnimalID = @AnimalID", conn))
                {
                    cmd.Parameters.AddWithValue("@AnimalID", animalId);
                    cmd.ExecuteNonQuery();
                }
            }

            return NoContent();
        }
    }
}