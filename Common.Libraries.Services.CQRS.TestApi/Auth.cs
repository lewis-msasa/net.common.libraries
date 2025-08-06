namespace Common.Libraries.Services.CQRS.TestApi
{
    //public class CreateOrderPermissionStrategy : IPermissionStrategy<CreateOrderCommand>
    //{


    //    public CreateOrderPermissionStrategy()
    //    {

    //    }

    //    public Task<bool> IsAuthorizedAsync(CreateOrderCommand request)
    //    {
    //        // Example: check if user has a role
    //        var userRoles = new string[] { "Order.Creatory" };
    //        var isAuthorized = userRoles.Contains("Order.Creator");
    //        return Task.FromResult(isAuthorized);
    //    }
    //}
    public class CreateOrderPermissionStrategy : RoleBasedPermissionStrategy<CreateOrderCommand>
    {
        public CreateOrderPermissionStrategy() : base("Order.Creator") { }
    }

    public abstract class RoleBasedPermissionStrategy<TRequest> : IPermissionStrategy<TRequest>
    {
       
        private readonly string _requiredRole;

        protected RoleBasedPermissionStrategy(string requiredRole)
        {
            
            _requiredRole = requiredRole;
        }

        public Task<bool> IsAuthorizedAsync(TRequest request)
        {
            var userRoles = new string[] { "Order.Creator" };
            return Task.FromResult(userRoles.Contains(_requiredRole));
        }
    }


}
