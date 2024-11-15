using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aurga.Data
{
    [Table("Devices")]
    public class Device
    {
        /// <summary>
        /// Device UID
        /// </summary>
        [Key]
        public string UID { get; set; }
        /// <summary>
        /// Account UID
        /// </summary>
        public string AUID { get; set; }
        public string? Name { get; set; }
        public int Model { get; set; }
        public string? Firmware { get; set; }
        public string? IP { get; set; }
        public string? MAC { get; set; }
        public required int Status { get; set; }
        public string? Location { get; set; }
        public string? Owner { get; set; }
        public string? Note { get; set; }

        private long Registered { get; set; }

        [NotMapped]
        public DateTime RegisteredAt
        {
            get { return DateTimeOffset.FromUnixTimeSeconds(Registered).DateTime; }
            set { Registered = new DateTimeOffset(value).ToUnixTimeSeconds(); }
        }
    }
}
