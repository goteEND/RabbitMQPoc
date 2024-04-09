namespace Order.Services.Interfaces;

public interface IResponseService
{
    Task<string> OrderConfirmation();
}