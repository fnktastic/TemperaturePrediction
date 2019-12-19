using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.Model;

namespace TemperaturePrediction.DataAccess
{
    public class Context : DbContext
    {
        public Context() : base("TempPredictionDb")
        {

        }

        public DbSet<SceneDto> Scenes { get; set; }
    }

    public class DataInitializer : DropCreateDatabaseIfModelChanges<Context>
    {
        public DataInitializer()
        {
            using (var context = new Context())
            {
                InitializeDatabase(context);
            }
        }
    }
}
