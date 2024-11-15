using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aurga.Data
{
    [Table("Users")]
    public class User
    {
        [Key]
        public long Id { get; set; }
        public string UID { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
        public string Password { get; set; }
        public string EmailHash { get; set; }
        public string PasswordHash { get; set; }

        [Column("CreatedAt")]
        private long Created { get; set; }
        [Column("VisitedAt")]
        private long Visited { get; set; }
        public string VisitedIP { get; set; }

        public bool Activated { get; set; }

        [NotMapped]
        public DateTime CreatedAt
        {
            get { return DateTimeOffset.FromUnixTimeSeconds(Created).DateTime; }
            set { Created = new DateTimeOffset(value).ToUnixTimeSeconds(); }
        }

        [NotMapped]
        public DateTime VisitedAt
        {
            get { return DateTimeOffset.FromUnixTimeSeconds(Visited).DateTime; }
            set { Visited = new DateTimeOffset(value).ToUnixTimeSeconds(); }
        }
    }
}
