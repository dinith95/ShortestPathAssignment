using AspireApp1.ApiService.Models;
using AspireApp1.ApiService.Services;
using Microsoft.AspNetCore.Mvc;
namespace AspireApp1.ApiService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class DistanceController : ControllerBase
{
    private readonly DistanceCalculatorService _distanceCalculatorService;
    public DistanceController()
    {
        _distanceCalculatorService = new DistanceCalculatorService();
    }

    [HttpGet()]
    public IActionResult GetShortestPath([FromQuery] string source, [FromQuery] string dest)
    {
        var dto = _distanceCalculatorService.FindShortestPath(source, dest);
        return Ok(dto);
    }
}
