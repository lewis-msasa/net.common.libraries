using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Specification
{
    public interface ISpecEntity
    {
    }
    public interface  HasId
    {
        public int Id { set; get; }
    }
    public interface ISpecDto
    {

    }
}
