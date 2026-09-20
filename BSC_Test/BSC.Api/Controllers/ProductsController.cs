using BSC.BusinessLogic.Models;
using BSC.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSC.Api.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpPost("{productId:int}/inventory-adjustments")]
    [Authorize(Roles = "Administrator,AdministrativeStaff")]
    public async Task<ActionResult<ProductDto>> AdjustInventory(
        int productId,
        AdjustInventoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await productService.AdjustInventoryAsync(
                productId,
                request,
                cancellationToken);

            return product is null
                ? NotFound()
                : Ok(product);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,AdministrativeStaff")]
    public async Task<ActionResult<ProductDto>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await productService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { productId = product.ProductId }, product);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{productId:int}")]
    [Authorize(Roles = "Administrator,AdministrativeStaff")]
    public async Task<ActionResult<ProductDto>> Update(
        int productId,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await productService.UpdateAsync(
                productId,
                request,
                cancellationToken);

            return product is null
                ? NotFound()
                : Ok(product);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetActive(
        CancellationToken cancellationToken)
    {
        var products = await productService.GetActiveAsync(cancellationToken);
        return Ok(products);
    }

    [HttpGet("{productId:int}")]
    public async Task<ActionResult<ProductDto>> GetById(
        int productId,
        CancellationToken cancellationToken)
    {
        var product = await productService.GetByIdAsync(productId, cancellationToken);

        return product is null
            ? NotFound()
            : Ok(product);
    }

    [HttpGet("stock")]
    public async Task<ActionResult<IReadOnlyList<StockReportDto>>> GetStockReport(
        CancellationToken cancellationToken)
    {
        var report = await productService.GetStockReportAsync(cancellationToken);
        return Ok(report);
    }
}
