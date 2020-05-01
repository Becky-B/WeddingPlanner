using System;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Guest
    {
        public int GuestId {get; set; }
        public int WeddingId {get; set;}
        public int UserId {get; set; }
        public Wedding Wedding {get; set; }
        public User User {get; set; }
        
    }
}