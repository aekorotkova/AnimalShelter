using System;

namespace AnimalShelter
{
    public class Animal
    {
        public int AnimalID { get; set; }
        public string Nickname { get; set; }
        public int SpeciesID { get; set; }
        public string SpeciesName { get; set; }
        public int BreedID { get; set; }
        public string BreedName { get; set; }
        public int? Age { get; set; }
        public int GenderID { get; set; }
        public string GenderName { get; set; }
        public string HealthStatus { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public bool IsAdopted { get; set; }

        public string PhotoPath { get; set; }

        public string PhotoFullPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PhotoPath)) return null;
                return System.IO.Path.Combine(PhotoHelper.PhotosFolder, PhotoPath);
            }
        }
    }
}