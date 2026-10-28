namespace AnimalShelterAPI.Models
{
    // модель животного для ответа
    public class AnimalDto
    {
        public int AnimalID { get; set; }
        public string Nickname { get; set; }
        public string SpeciesName { get; set; }
        public string BreedName { get; set; }
        public int? Age { get; set; }
        public string GenderName { get; set; }
        public string HealthStatus { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public bool IsAdopted { get; set; }
        public string PhotoPath { get; set; }
    }

    // модель для добавления/обновления животного
    public class AnimalCreateDto
    {
        public string Nickname { get; set; }
        public int SpeciesID { get; set; }
        public int BreedID { get; set; }
        public int? Age { get; set; }
        public int GenderID { get; set; }
        public string HealthStatus { get; set; }
        public DateTime? AdmissionDate { get; set; }
    }

    // модель усыновления
    public class AdoptionDto
    {
        public int AdoptionID { get; set; }
        public string AnimalNickname { get; set; }
        public string VisitorName { get; set; }
        public string VisitorPhone { get; set; }
        public string EmployeeName { get; set; }
        public DateTime AdoptionDate { get; set; }
        public string Notes { get; set; }
        public DateTime? NextContactDate { get; set; }
    }

    // модель создания усыновления
    public class AdoptionCreateDto
    {
        public int AnimalID { get; set; }
        public int VisitorID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime AdoptionDate { get; set; }
        public string Notes { get; set; }
        public DateTime? NextContactDate { get; set; }
    }

    // статистика по видам
    public class SpeciesStatDto
    {
        public string SpeciesName { get; set; }
        public int TotalCount { get; set; }
        public int InShelterCount { get; set; }
        public int AdoptedCount { get; set; }
    }
}