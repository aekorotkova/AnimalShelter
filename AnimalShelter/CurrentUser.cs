namespace AnimalShelter
{
    public static class CurrentUser
    {
        public static string Login { get; set; }
        public static string Role { get; set; }

        public static bool IsAdmin => Role == "Администратор";
        public static bool IsEmployee => Role == "Сотрудник";
    }
}