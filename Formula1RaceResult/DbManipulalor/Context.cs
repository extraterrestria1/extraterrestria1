using System.Data.Entity;

using Formula1RaceResult.Interfaces;



namespace Formula1RaceResult.DbManipulalor
{



    public class GrandPrixResultsContext : DbContext
    {

        public GrandPrixResultsContext(string connectionString) : base(connectionString) { }

        public DbSet<GrandPrixResults> GrandPrixResults { get; set; }

        public DbSet<GrandPrix> GrandPrix { get; set; }

    }



}
