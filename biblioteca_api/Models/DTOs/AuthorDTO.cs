namespace biblioteca_api.Models.DTOs.Author
{
    public class AuthorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Biography { get; set; }
    }
}