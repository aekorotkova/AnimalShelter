using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using AnimalShelterAPI.Models;

namespace AnimalShelterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalsController : ControllerBase
    {
        // получить всех животных
        [HttpGet]
        [ProducesResponseType(typeof(List<AnimalDto>), 200)]
        public IActionResult GetAll()
        {
            var list = new List<AnimalDto>();

            string query = @"
                SELECT a.AnimalID, a.Nickname, s.SpeciesName, b.BreedName,
                       a.Age, g.GenderName, a.HealthStatus, a.AdmissionDate,
                       a.IsAdopted, a.PhotoPath
                FROM Animals_kursovoi a
                JOIN Species_kursovoi s ON a.SpeciesID = s.SpeciesID
                JOIN Breeds_kursovoi b ON a.BreedID = b.BreedID
                JOIN Gender_kursovoi g ON a.GenderID = g.GenderID
                ORDER BY a.AnimalID";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AnimalDto
                        {
                            AnimalID = (int)reader["AnimalID"],
                            Nickname = reader["Nickname"].ToString(),
                            SpeciesName = reader["SpeciesName"].ToString(),
                            BreedName = reader["BreedName"].ToString(),
                            Age = reader["Age"] == DBNull.Value ? (int?)null : (int)reader["Age"],
                            GenderName = reader["GenderName"].ToString(),
                            HealthStatus = reader["HealthStatus"] == DBNull.Value ? "" : reader["HealthStatus"].ToString(),
                            AdmissionDate = reader["AdmissionDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["AdmissionDate"],
                            IsAdopted = (bool)reader["IsAdopted"],
                            PhotoPath = reader["PhotoPath"] == DBNull.Value ? null : reader["PhotoPath"].ToString()
                        });
                    }
                }
            }

            return Ok(list);
        }

        // получить животное по id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AnimalDto), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetById(int id)
        {
            AnimalDto animal = null;

            string query = @"
                SELECT a.AnimalID, a.Nickname, s.SpeciesName, b.BreedName,
                       a.Age, g.GenderName, a.HealthStatus, a.AdmissionDate,
                       a.IsAdopted, a.PhotoPath
                FROM Animals_kursovoi a
                JOIN Species_kursovoi s ON a.SpeciesID = s.SpeciesID
                JOIN Breeds_kursovoi b ON a.BreedID = b.BreedID
                JOIN Gender_kursovoi g ON a.GenderID = g.GenderID
                WHERE a.AnimalID = @Id";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        animal = new AnimalDto
                        {
                            AnimalID = (int)reader["AnimalID"],
                            Nickname = reader["Nickname"].ToString(),
                            SpeciesName = reader["SpeciesName"].ToString(),
                            BreedName = reader["BreedName"].ToString(),
                            Age = reader["Age"] == DBNull.Value ? (int?)null : (int)reader["Age"],
                            GenderName = reader["GenderName"].ToString(),
                            HealthStatus = reader["HealthStatus"] == DBNull.Value ? "" : reader["HealthStatus"].ToString(),
                            AdmissionDate = reader["AdmissionDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["AdmissionDate"],
                            IsAdopted = (bool)reader["IsAdopted"],
                            PhotoPath = reader["PhotoPath"] == DBNull.Value ? null : reader["PhotoPath"].ToString()
                        };
                    }
                }
            }

            if (animal == null)
                return NotFound(new { message = "Животное не найдено" });

            return Ok(animal);
        }

        // добавить животное
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult Create([FromBody] AnimalCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Nickname))
                return BadRequest(new { message = "Кличка обязательна" });

            string query = @"
                INSERT INTO Animals_kursovoi 
                    (Nickname, SpeciesID, BreedID, Age, GenderID, HealthStatus, AdmissionDate, IsAdopted)
                VALUES 
                    (@Nickname, @SpeciesID, @BreedID, @Age, @GenderID, @HealthStatus, @AdmissionDate, 0);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int newId;
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Nickname", dto.Nickname);
                cmd.Parameters.AddWithValue("@SpeciesID", dto.SpeciesID);
                cmd.Parameters.AddWithValue("@BreedID", dto.BreedID);
                cmd.Parameters.AddWithValue("@Age", (object)dto.Age ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GenderID", dto.GenderID);
                cmd.Parameters.AddWithValue("@HealthStatus", (object)dto.HealthStatus ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AdmissionDate", (object)dto.AdmissionDate ?? DBNull.Value);

                conn.Open();
                newId = (int)cmd.ExecuteScalar();
            }

            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, message = "Животное добавлено" });
        }

        // обновить животное
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(int id, [FromBody] AnimalCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Nickname))
                return BadRequest(new { message = "Кличка обязательна" });

            string query = @"
                UPDATE Animals_kursovoi SET
                    Nickname = @Nickname,
                    SpeciesID = @SpeciesID,
                    BreedID = @BreedID,
                    Age = @Age,
                    GenderID = @GenderID,
                    HealthStatus = @HealthStatus,
                    AdmissionDate = @AdmissionDate
                WHERE AnimalID = @Id";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Nickname", dto.Nickname);
                cmd.Parameters.AddWithValue("@SpeciesID", dto.SpeciesID);
                cmd.Parameters.AddWithValue("@BreedID", dto.BreedID);
                cmd.Parameters.AddWithValue("@Age", (object)dto.Age ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GenderID", dto.GenderID);
                cmd.Parameters.AddWithValue("@HealthStatus", (object)dto.HealthStatus ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AdmissionDate", (object)dto.AdmissionDate ?? DBNull.Value);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                    return NotFound(new { message = "Животное не найдено" });
            }

            return NoContent();
        }

        // удалить животное
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int id)
        {
            string query = "DELETE FROM Animals_kursovoi WHERE AnimalID = @Id";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                    return NotFound(new { message = "Животное не найдено" });
            }

            return NoContent();
        }
    }
}