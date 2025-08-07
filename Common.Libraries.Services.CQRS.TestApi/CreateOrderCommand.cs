namespace Common.Libraries.Services.CQRS.TestApi
{
    public class CreateOrderCommand : ICommand<Guid>
    {
        public string ProductId { get; set; } = default!;
        public int Quantity { get; set; }
    }
    public class DeleteOrderCommand : ICommand { }

    public class DeleteUserHandler : IRequestHandler<DeleteOrderCommand>
    {
        public Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            // perform delete
            return Task.CompletedTask;
        }
    }
}
