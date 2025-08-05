using Common.Libraries.EventSourcing;
using static System.Net.Mime.MediaTypeNames;

namespace Common.Libraries.EventStore.EF.TestApi
{
    public class User : AggregateRoot<UserSnapshot>
    {
        public static User Create(UserId id, UserId spouseId, string username)
        {
            var user = new User();

            user.Apply(
                new UserCreated
                {
                    Id = id,
                    SpouseId = spouseId,
                    Name = username
                    
                   
                   
                }
            );
            return user;
        }
        public UserId Spouse { get; private set; }

        public Username Username { get; private set; }

        public void SetName(Username name)
          => Apply(
              new UsernameChanged(Id,name)
              
          );
        protected override void EnsureValidState()
        {
           
        }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case UsernameChanged e:
                    Username = new Username(e.Username);
                    //Id = e.Id;
                    break;
                case UserCreated ev:
                    Id = ev.Id;
                    Spouse = UserId.FromGuid(ev.SpouseId);
                    Username = new Username(ev.Name);
                    break;
                

                
            }
        }

        protected override void LoadSnapshot(UserSnapshot snapshot)
        {
            Username = new Username(snapshot.UserName);
            Version = snapshot.Version;
            Id = snapshot.AggregateId;

        }

        public override UserSnapshot CreateSnapShot() => new()
        {
            UserName = Username.Value,
            Version = Version,
            AggregateId = Id,
            Timestamp = DateTime.UtcNow

        };
    }
}
