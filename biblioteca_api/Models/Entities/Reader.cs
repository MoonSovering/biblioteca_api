namespace biblioteca_api.Models.Entities
{
    public class Reader : User
    {
        public string MembershipNumber { get; set; }
        public DateTime? MembershipExpiration { get; set; }
    }
}
