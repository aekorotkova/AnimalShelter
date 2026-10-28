using System;

namespace AnimalShelter
{
    public class Adoption
    {
        public int AdoptionID { get; set; }
        public int AnimalID { get; set; }
        public string AnimalNickname { get; set; }
        public int VisitorID { get; set; }
        public string VisitorName { get; set; }
        public string VisitorPhone { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime AdoptionDate { get; set; }
        public string Notes { get; set; }
        public DateTime? NextContactDate { get; set; }
    }
}