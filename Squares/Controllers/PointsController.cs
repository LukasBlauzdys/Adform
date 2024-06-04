using Microsoft.AspNetCore.Mvc;
using Squares.Controllers.Base;
using Squares.Interfaces;
using Squares.Models;

namespace Squares.Controllers
{
  /// <summary>
  /// API controller class for managing points and counting squares
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]
  public class PointsController : BaseController
  {
    private readonly IPointsService _pointsService;
    private readonly ILogger<PointsController> _logger;

    /// <summary>
    /// Points service controller
    /// </summary>
    public PointsController(IPointsService pointsService, ILogger<PointsController> logger) : base(logger)
    {
      _pointsService = pointsService;
    }

    /// <summary>
    /// Retrieves all points from the database.
    /// </summary>
    /// <returns>A list of points.</returns>
    [HttpGet]
    public async Task<ActionResult<List<Point>>> GetPoints()
    {
      try
      {
        var points = await _pointsService.GetPointsAsync();
        if (points == null)
        {
          return NotFound();
        }
        return Ok(points);
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    /// <summary>
    /// Retrieves a specific point by its ID.
    /// </summary>
    /// <param name="id">The ID of the point.</param>
    /// <returns>The requested point.</returns>
    [HttpGet("{id:length(24)}", Name = "GetPoint")]
    public async Task<ActionResult<Point>> GetPoint(string id)
    {
      try
      {
        var point = await _pointsService.GetPointByIdAsync(id);
        if (point == null)
        {
          return NotFound();
        }

        return Ok(point);
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    /// <summary>
    /// Creates new points in the database.
    /// </summary>
    /// <param name="points">A list of points to create.</param>
    [HttpPost]
    public async Task<IActionResult> CreatePoint(List<Point> point)
    {
      try
      {
        await _pointsService.CreatePointAsync(point);
        return Ok(new { message = "Points successfully inserted" });
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    /// <summary>
    /// Updates an existing point in the database.
    /// </summary>
    /// <param name="id">The ID of the point to update.</param>
    /// <param name="point">The updated point object.</param>
    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> UpdatePoint(string id, Point point)
    {
      try
      {
        var existingPoint = await _pointsService.GetPointByIdAsync(id);
        if (existingPoint == null)
        {
          return NotFound();
        }

        point.GetType().GetProperty("_id").SetValue(point, id);
        await _pointsService.UpdatePointAsync(id, point);

        return NoContent();
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    /// <summary>
    /// Deletes a specific point by its ID.
    /// </summary>
    /// <param name="id">The ID of the point to delete.</param>
    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> DeletePoint(string id)
    {
      try
      {
        var point = await _pointsService.GetPointByIdAsync(id);
        if (point == null)
        {
          return NotFound();
        }

        await _pointsService.DeletePointAsync(id);

        return NoContent();
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    /// <summary>
    /// Deletes a specific point based on its coordinates.
    /// </summary>
    /// <param name="p">The point object with coordinates to delete.</param>
    [HttpDelete]
    public async Task<IActionResult> DeletePoint(Point p)
    {
      try
      {
        var points = await _pointsService.GetPointsAsync();
        var point = points.FirstOrDefault(point => point.X == p.X && point.Y == p.Y);
        if (point == null)
        {
          return NotFound();
        }

        await _pointsService.DeletePointAsync(point.Id);

        return NoContent();
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    /// <summary>
    /// Retrieves the count and detailed information of all squares formed by the points.
    /// </summary>
    /// <returns>The count and details of squares.</returns>
    [HttpGet("Squares")]
    public async Task<IActionResult> CountSquaresDetailed()
    {
      try
      {
        var points = await _pointsService.GetPointsAsync();
        if (points == null || points.Count == 0)
        {
          return NotFound("No points found to calculate squares.");
        }

        var squares = _pointsService.CountSquares(points);

        var response = new
        {
          Count = squares.Count,
          Squares = squares.Select(squareKey =>
          {
            var pointStrings = squareKey.Split(',');
            var points = new List<object>();

            for (int i = 0; i < pointStrings.Length; i += 2)
            {
              var xPart = pointStrings[i].Replace("\"x\": ", string.Empty).Trim();
              var yPart = pointStrings[i + 1].Replace("\"y\": ", string.Empty).Trim();

              points.Add(new
              {
                x = int.Parse(xPart),
                y = int.Parse(yPart)
              });
            }

            return new { Points = points };
          }).ToList()
        };

        return Ok(response);
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    /// <summary>
    /// Retrieves the count of all squares formed by the points.
    /// </summary>
    /// <returns>The count of squares.</returns>
    [HttpGet("CountSquares")]
    public async Task<IActionResult> CountSquares()
    {
      try
      {
        var points = await _pointsService.GetPointsAsync();
        if (points == null || points.Count == 0)
        {
          return NotFound("No points found to calculate squares.");
        }

        var squares = _pointsService.CountSquares(points);

        var response = new
        {
          Count = squares.Count
        };

        return Ok(response);
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }
  }
}
