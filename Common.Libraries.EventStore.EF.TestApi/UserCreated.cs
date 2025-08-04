namespace Common.Libraries.EventStore.EF.TestApi
{
    //event
    public class UserCreated
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid SpouseId { get; set; }

        public override string ToString()
            => $"{nameof(UserCreated)}";


    }
}
