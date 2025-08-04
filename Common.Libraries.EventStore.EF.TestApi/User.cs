using Common.Libraries.EventSourcing;
using static System.Net.Mime.MediaTypeNames;

namespace Common.Libraries.EventStore.EF.TestApi
{
    public class User : AggregateRoot
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
              new UsernameChanged
              {
                 Username = name
              }
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
                    break;
                case UserCreated ev:
                    Id = ev.Id;
                    Spouse = UserId.FromGuid(ev.SpouseId);
                    Username = new Username(ev.Name);
                    break;
                

                
            }
        }
    }
}
