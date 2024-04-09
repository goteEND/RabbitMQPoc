using Order.Models;

namespace Order.Services.Interfaces;

public interface IMessageService
{
    void MessageOrder(UserOrder userOrder);

}