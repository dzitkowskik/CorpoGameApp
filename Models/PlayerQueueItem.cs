using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorpoGameApp.Models
{
    public class PlayerQueueItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual Player Player { get; set; }

        public bool Played { get; set; }

        public virtual QueueItemState State { get; set; }
    }
}