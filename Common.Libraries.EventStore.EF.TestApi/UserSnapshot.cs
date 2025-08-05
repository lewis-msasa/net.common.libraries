using Common.Libraries.EventSourcing;
using Common.Libraries.Services.Dtos;
using Common.Libraries.Services.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Libraries.EventStore.EF.TestApi
{
    public class UserSnapshot : IEntity, ISnapshot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }

        public Guid AggregateId { get; set; }

        public int Version { get; set; }

        public string UserName { get; set; }

        public DateTime Timestamp { get; set; }
    }
    public class UserSnapshotDto : IDTO
    {
        public int RecordId { get; set; }

        public Guid AggregateId { get; set; }

        public int Version { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
