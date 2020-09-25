using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebCrawlerzada.Models;


namespace WebCrawlerzada.Data
{
    class DataContext : DbContext
    {
        public DbSet<Novel> Novels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(@"server=localhost;database=CrawlerDB;user=root;password=Gg896412753");
            base.OnConfiguring(optionsBuilder);
        }


        public void Start()
        {
            this.Database.EnsureCreated();
        }
    }
}
