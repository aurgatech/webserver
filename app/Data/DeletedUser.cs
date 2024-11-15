using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace aurga.Data
{
    
    [Table("DeletedUsers")]
    public class DeletedUser
    {
        [Key]
        public long Id { get; set; }
        public string UID { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
        public string Password { get; set; }

        [Column("CreatedAt")]
        public long Created { get; set; }
        public long DeletedAt { get; set; }
    }
}
