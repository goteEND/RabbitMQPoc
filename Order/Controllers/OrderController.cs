using Microsoft.AspNetCore.Mvc;
using Order.Models;
using Order.Services.Interfaces;

namespace Order.Controllers;

[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IMessageService _orderService;
    private readonly IResponseService _responseService;

    public OrderController(IMessageService orderService, IResponseService responseService)
    {
        _orderService = orderService;
        _responseService = responseService;
    }

    [HttpPost]
    public async Task<IActionResult> SendOrder(UserOrder userOrder)
    {
        _orderService.MessageOrder(userOrder);
        var response = await _responseService.OrderConfirmation();
        return Ok(response);
    }
}