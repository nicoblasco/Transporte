
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Transporte.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {

        }


        public System.Data.Entity.DbSet<Transporte.Models.Audit> Audits { get; set; }


        public System.Data.Entity.DbSet<Transporte.Models.Module> Modules { get; set; }

        public System.Data.Entity.DbSet<Transporte.Models.Permission> Permissions { get; set; }


        public System.Data.Entity.DbSet<Transporte.Models.Rol> Rols { get; set; }



        public System.Data.Entity.DbSet<Transporte.Models.Usuario> Usuarios { get; set; }


        public System.Data.Entity.DbSet<Transporte.Models.Window> Windows { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //modelBuilder.Entity<Act>()
            //.HasMany(c => c.)
            //.WithOptional()
            //.Map(m => m.MapKey("ClaimId"));

        }



        public System.Data.Entity.DbSet<Transporte.Models.Person> People { get; set; }

       



        public System.Data.Entity.DbSet<Transporte.Models.Setting> Settings { get; set; }





        public System.Data.Entity.DbSet<Transporte.Models.Country> Countries { get; set; }

        public System.Data.Entity.DbSet<Transporte.Models.Street> Streets { get; set; }

        public System.Data.Entity.DbSet<Transporte.Models.PersonType> PersonTypes { get; set; }

        public System.Data.Entity.DbSet<Transporte.Models.Transport> Transports { get; set; }

        public System.Data.Entity.DbSet<Transporte.Models.TransportType> TransportTypes { get; set; }
    }
}