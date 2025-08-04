using Common.Libraries.EventSourcing;

namespace Common.Libraries.EventStore.EF.TestApi
{
    public class UserCommandService : ApplicationService<User>
    {
        public UserCommandService(IAggregateStore store) : base(store)
        {
            CreateWhen<Create>(
               cmd => UserId.FromGuid(cmd.Id),
               (cmd, id) => User.Create(
                   UserId.FromGuid(id),
                   UserId.FromGuid(cmd.SpouseId),
                   cmd.Name
               )
           );
            UpdateWhen<ChangeUsername>(
               cmd => UserId.FromGuid(cmd.Id),
               (usr, cmd)
                   => usr.SetName(Username.FromString(cmd.Name))
           );
        }
    }
}
