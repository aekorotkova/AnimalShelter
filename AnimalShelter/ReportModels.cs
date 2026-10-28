using System;

namespace AnimalShelter
{
    public class AnimalInShelterReport
    {
        public int AnimalID { get; set; }
        public string Nickname { get; set; }
        public string SpeciesName { get; set; }
        public string BreedName { get; set; }
        public int? Age { get; set; }
        public string GenderName { get; set; }
        public string HealthStatus { get; set; }
        public DateTime? AdmissionDate { get; set; }
    }

    public class AdoptedAnimalReport
    {
        public int AnimalID { get; set; }
        public string Nickname { get; set; }
        public string SpeciesName { get; set; }
        public string BreedName { get; set; }
        public string VisitorName { get; set; }
        public string EmployeeName { get; set; }
        public DateTime AdoptionDate { get; set; }
    }

    public class SpeciesStatReport
    {
        public string SpeciesName { get; set; }
        public int TotalCount { get; set; }
        public int InShelterCount { get; set; }
        public int AdoptedCount { get; set; }
    }

    public class AdoptionStatReport
    {
        public int AdoptionID { get; set; }
        public string AnimalNickname { get; set; }
        public string VisitorName { get; set; }
        public string EmployeeName { get; set; }
        public DateTime AdoptionDate { get; set; }
    }
}