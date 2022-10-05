using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace bmcmsproject.Models
{
    public class sampleProject_DBEntities : DbContext
    {
        public sampleProject_DBEntities() : base("sampleProject_DBEntities") { }

        public DbSet<bm_slider_cms> bm_slider { get; set; }

        public DbSet<web_tbl_care_user> care_users { get; set; }
        public DbSet<tba> tba { get; set; }

    }
}