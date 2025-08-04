using Common.Libraries.EventStore.EF.TestApi;
using Common.Libraries.EventStore.Projection;
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Repositories;

namespace Common.Libraries.EventStore.EF.TestApi
{
    public class UserDetailsProjectorProvider: IProjectorProvider<UserDetails>
    {

        public Projector<UserDetails> GetProjector()
        {
            return (repository, @event) =>
            {
                // You can use reflection, type matching, or pattern matching here
                if (@event is UserCreated evt)
                {
                    return async () =>
                    {
                        await Task.Run(() => repository.AddAsync(new UserDetails { SpouseId = evt.SpouseId, UserId = evt.Id, UserName = evt.Name }));
                       
                    };
                }
                else if(@event is UsernameChanged ev)
                {
                    return async () =>
                    {
                        var user = await repository.GetOneAsync(t => t.UserId == ev.Id);
                        user.UserName = ev.Username;
                        await Task.Run(() => repository.UpdateAsync(user));
                    };
                }
                throw new NotImplementedException();



            };
        }
    }
}

  

