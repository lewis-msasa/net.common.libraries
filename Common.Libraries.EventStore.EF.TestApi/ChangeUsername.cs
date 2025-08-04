namespace Common.Libraries.EventStore.EF.TestApi
{
    public class ChangeUsername
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public override string ToString() => $"SetUsername {Id} {Name}";
    }
}
