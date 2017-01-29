using Microsoft.EntityFrameworkCore;

namespace CorpoGameApp.Data {

    public class DbInitialization
    {
        public static void Initialize(DbContextOptions<ApplicationDbContext> options)
        {
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.Migrate();
            }
        }
    }

}