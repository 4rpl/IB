using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IdeaMarket.DbModels
{
    public class MarketContext : DbContext
    {
        public DbSet<Idea> Ideas { get; set; }
    }

    public class Idea
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
    }
}