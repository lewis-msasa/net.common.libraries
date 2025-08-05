namespace Common.Libraries.EventStore.EF.TestApi.Controllers
{
    public class CreateRequest
    {
        public string Name { get; set; }
    }
    public class ChangeNameRequest
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
