using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace projectMvc.Models.models
{
        public class User : IdentityUser
        {
            [Required, StringLength(100)]
            public string FullName { get; set; }

            [Required]
            public string Role { get; set; } = "Customer";

            public List<Order> Orders { get; set; } = new List<Order>();
            public List<TableReservation> TableReservations { get; set; } = new List<TableReservation>();
        }
    }

            
