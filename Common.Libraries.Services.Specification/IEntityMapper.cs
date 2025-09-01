using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Specification
{
    public interface IEntityMapper<T, TD> where T : class, ISpecEntity where TD : class, ISpecDto
    {
        TD Map(T entity);
        ICollection<TD> Map(ICollection<T> entities);
    }

}
