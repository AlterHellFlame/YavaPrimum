using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaHire.Core.DataBase.Models;

namespace YavaHire.Core.DataBase
{
    public class YavaHireDBContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-LOAQH83;Database=YavaHireDB;
				Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");
        }

        public DbSet<HR> HRs { get; set; }
        public DbSet<PersonalOfficer> PersonalOfficers { get; set; }
    }
}
