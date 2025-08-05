using Common.Libraries.EventSourcing;
using Common.Libraries.Services.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Libraries.EventStore.EF.TestApi
{
    public class UserDetails : IEntity, IHasAggregateName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public Guid SpouseId { get; set; }
        public string AggregateName { get; set; } = nameof(User);
    }
}
