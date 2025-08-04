namespace Common.Libraries.EventStore.EF.TestApi
{
   //command
    public class Create
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid SpouseId { get; set; }

        public override string ToString() => $"CreateUser {Id}";
    }
}
