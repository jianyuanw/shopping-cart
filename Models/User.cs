using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required, MaxLength(30)]
        public string Username { get; set; }
        [Required, MaxLength(100)]
        public string Password { get; set; }
        [Required, MaxLength(50)]
        public string Salt { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
