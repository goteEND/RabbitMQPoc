using Order.Models;

namespace Order.Services.Interfaces;

public interface IMessageService
{
    string MessageOrder(UserOrder userOrder);
    void Close();

}