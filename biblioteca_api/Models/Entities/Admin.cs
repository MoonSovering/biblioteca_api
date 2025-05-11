namespace biblioteca_api.Models.Entities
{
    public class Admin : User
    {
        public string AdminLevel { get; set; }
        public bool CanManageUsers { get; set; } = true;
    }
}