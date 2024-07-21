using AspireApp1.ApiService.Models;
using AspireApp1.ApiService.Services;
using Microsoft.AspNetCore.Mvc;
namespace AspireApp1.ApiService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class DistanceController : ControllerBase
{
    private readonly IDistanceCalculatorService _distanceCalculatorService;
    public DistanceController(IDistanceCalculatorService distanceCalculatorService)
    {
       _distanceCalculatorService = distanceCalculatorService;
    }

    [HttpGet()]
    public IActionResult GetShortestPath([FromQuery] string source, [FromQuery] string dest)
    {
        try
        {
            var dto = _distanceCalculatorService.FindShortestPath(source, dest);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
       
    }
}
