

namespace ECommerce.API.Models
{
    public class VisitedPath
    {
        public int Id { get; set; }
        public int IpProfileId { get; set; }
        public string Path { get; set; }
        public string Verb { get; set; }
        public string AppName { get; set; }
        public DateTime Created { get; set; }

    }
}
