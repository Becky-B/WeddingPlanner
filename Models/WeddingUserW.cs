using System;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class WeddingUserW
    {
        public User user {get; set; }
        public List<Wedding> Allweddings {get; set; }
        public Wedding wedding {get; set; }
    }
}