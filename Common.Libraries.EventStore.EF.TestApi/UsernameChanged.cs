namespace Common.Libraries.EventStore.EF.TestApi
{
    public class UsernameChanged
    {
        public Username Username { get; set; }
        public Guid Id { get; set; }
        public override string ToString()
          => $"{nameof(UsernameChanged)}";
    }
}
