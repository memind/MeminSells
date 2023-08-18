using MeminSells.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeminSells.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName);
    }

}
