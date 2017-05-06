using CorpoGameApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CorpoGameApp.Data 
{
    public class DbInitialization
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();
        }
    }
}