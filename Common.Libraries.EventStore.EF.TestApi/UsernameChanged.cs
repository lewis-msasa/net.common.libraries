using System.Text.Json.Serialization;

namespace Common.Libraries.EventStore.EF.TestApi
{
    public class UsernameChanged
    {
        [JsonConstructor]
        public UsernameChanged(Guid Id,Username username) { 
            this.Id = Id;
            Username = username;

        }
        public Username Username { get; set; }
        public Guid Id { get; set; }
        public override string ToString()
          => $"{nameof(UsernameChanged)}";
    }
}
