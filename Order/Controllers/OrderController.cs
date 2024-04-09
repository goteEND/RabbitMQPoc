using Microsoft.AspNetCore.Mvc;
using Order.Models;
using Order.Services.Interfaces;

namespace Order.Controllers;

[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IMessageService _orderService;

    public OrderController(IMessageService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public IActionResult SendOrder(UserOrder userOrder)
    {
        var response = _orderService.MessageOrder(userOrder);
        return Ok(response);
    }
}