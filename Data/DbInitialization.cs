using CorpoGameApp.Enums;
using CorpoGameApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CorpoGameApp.Data {

    public class DbInitialization
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();
        }

        public static async void Seed(ApplicationDbContext context)
        {
            if(!await context.QueueItemStates.AnyAsync(t => t.Id == (int)QueuedItemStateEnum.Queued))
            {
                await context.QueueItemStates.AddAsync(new QueueItemState()
                { 
                    Id = (int)QueuedItemStateEnum.Queued, 
                    Name = QueuedItemStateEnum.Queued.ToString() 
                });
            }

            if(!await context.QueueItemStates.AnyAsync(t => t.Id == (int)QueuedItemStateEnum.Playing))
            {
                await context.QueueItemStates.AddAsync(new QueueItemState()
                { 
                    Id = (int)QueuedItemStateEnum.Playing, 
                    Name = QueuedItemStateEnum.Playing.ToString() 
                });
            }
            
            if(!await context.QueueItemStates.AnyAsync(t => t.Id == (int)QueuedItemStateEnum.Finished))
            {
                await context.QueueItemStates.AddAsync(new QueueItemState()
                { 
                    Id = (int)QueuedItemStateEnum.Finished, 
                    Name = QueuedItemStateEnum.Finished.ToString() 
                });
            }

            await context.SaveChangesAsync();
        }
    }

}