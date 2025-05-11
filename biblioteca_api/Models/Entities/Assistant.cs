namespace biblioteca_api.Models.Entities
{
    public class Assistant : User
    {
        public string Department { get; set; }
        public DateTime HireDate { get; set; }
    }
}