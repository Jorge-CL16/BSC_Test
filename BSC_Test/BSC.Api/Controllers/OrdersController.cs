using System.Security.Claims;
using BSC.BusinessLogic.Models;
using BSC.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BSC.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Salesperson")]
    public async Task<ActionResult<OrderCreatedDto>> Create(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var salespersonId))
            return Unauthorized();

        try
        {
            var order = await orderService.CreateAsync(
                salespersonId,
                request,
                cancellationToken);

            return Ok(order);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (SqlException exception) when (exception.Number >= 50001)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Administrator,AdministrativeStaff")]
    public async Task<ActionResult<IReadOnlyList<OrderSummaryDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var orders = await orderService.GetSummariesAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpPost("{orderId:int}/cancel")]
    [Authorize(Roles = "Administrator,AdministrativeStaff")]
    public async Task<IActionResult> Cancel(
        int orderId,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var cancelledByUserId))
            return Unauthorized();

        try
        {
            await orderService.CancelAsync(
                orderId,
                cancelledByUserId,
                cancellationToken);

            return NoContent();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (SqlException exception) when (exception.Number >= 50011)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    private bool TryGetUserId(out int userId)
    {
        return int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out userId);
    }
}
