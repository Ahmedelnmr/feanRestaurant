using projectMvc.Models.models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectMvc.Models.models
{
    public class TableReservation : BaseModel
    {
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public int TableNumber { get; set; }

        [Required]
        public DateTime ReservationTime { get; set; }

        public int GuestsCount { get; set; }
    }
}
