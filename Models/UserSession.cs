
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.Models
{
    public class UserSession
    {
        public int Id { set; get; }
        public string Session { set; get; }
        public string UserId { set; get; }
        public virtual User User { set; get; }
    }
}
