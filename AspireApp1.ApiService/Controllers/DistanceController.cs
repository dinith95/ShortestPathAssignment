using AspireApp1.ApiService.Models;
using AspireApp1.ApiService.Services;
using Microsoft.AspNetCore.Mvc;
namespace AspireApp1.ApiService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class DistanceController : ControllerBase
{
    private readonly IDistanceCalculatorService _distanceCalculatorService;
    private readonly ILogger<DistanceController> _logger;
    public DistanceController(IDistanceCalculatorService distanceCalculatorService, ILogger<DistanceController> logger)
    {
        _distanceCalculatorService = distanceCalculatorService;
        _logger = logger;
    }

    [HttpGet()]
    public async Task<IActionResult> GetShortestPath([FromQuery] string source, [FromQuery] string dest)
    {
        try
        {
            var dto = await _distanceCalculatorService.FindShortestPath(source, dest);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while finding shortest path, ex: {ex}." , ex);
            return BadRequest(ex.Message);
        }
       
    }
}
