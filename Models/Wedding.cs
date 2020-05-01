using System;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get; set; }
        [Required(ErrorMessage="Wedder One is required to schedule a wedding")]
        public string Wedder1 {get; set; }
        [Required(ErrorMessage="Wedder Two is required to schedule a wedding")]
        public string Wedder2 {get; set; }
        [Required(ErrorMessage="Date of wedding is required")]
        public DateTime Date {get; set; }
        [NotMapped]
        public bool CheckDate
        {
            get
            {
                DateTime today = DateTime.Today;
                if(Date.Date < today.Date)
                    return false;
                return true;
            }
        }

        [Required(ErrorMessage="Venue address is required")]
        public String Address {get; set; }
        public int UserId{get; set; }
        public User Creator {get; set; }
        public List<Guest>  Guests {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set; } = DateTime.Now;
    }
}