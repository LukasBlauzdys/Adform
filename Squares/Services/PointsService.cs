using MongoDB.Driver;
using Squares.Interfaces;
using Squares.Models;

namespace Squares.Services
{
  public class PointsService : IPointsService
  {
    private readonly ILogger<PointsService> _logger;
    private readonly PointsContext _PointsCollection;

    /// <summary>
    /// Initialize new instance of PointsService class
    /// </summary>
    public PointsService(PointsContext context, ILogger<PointsService> logger)
    {
      _PointsCollection = context;
      _logger = logger;
    }

    /// <summary>
    /// Service for getting the full list of points
    /// </summary>
    /// <returns>List of all the points in DataBase</returns>
    public async Task<List<Point>> GetPointsAsync()
    {
      _logger.LogInformation("Retrieving all possible points from the database");
      try
      {
        var points = await _PointsCollection.Points.Find(FilterDefinition<Point>.Empty).ToListAsync();
        if (points == null)
        {
          return null;
        }
        _logger.LogInformation($"Successfully retrieved {points.Count} points");

        return points;
      }
      catch
      {
        throw;
      }
    }

    /// <summary>
    /// Gets specific point by their ID
    /// </summary>
    /// <param name="id">BSON ID for specific point</param>
    /// <returns>Singular point coordinates</returns>
    public async Task<Point> GetPointByIdAsync(string id)
    {
      _logger.LogInformation($"Retrieving point with ID {id} from the database");
      try
      {
        var point = await _PointsCollection.Points.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (point == null)
        {
          return null;
        }
        _logger.LogInformation($"Successfully retrieved Point: {point.ToString()} from database");
        return point;
      }
      catch
      {
        throw;
      }
    }

    /// <summary>
    /// Adds points to the database
    /// </summary>
    /// <param name="point">list of points to add</param>
    public async Task CreatePointAsync(List<Point> point)
    {
      _logger.LogInformation($"Inserting a List of Points to the database");
      try
      {
        await _PointsCollection.Points.InsertManyAsync(point);
        _logger.LogInformation("Successfully inserted Points to database");
      }
      catch
      {
        throw;
      }
    }

    /// <summary>
    /// Updates specific point to new coordinates
    /// </summary>
    /// <param name="id">ID of a point to update</param>
    /// <param name="point">New coordinates</param>
    public async Task UpdatePointAsync(string id, Point point)
    {
      _logger.LogInformation($"Updating Point with ID {id} to new coordinates: {point.ToString()}");
      try
      {
        await _PointsCollection.Points.ReplaceOneAsync(p => p.Id == id, point);
        _logger.LogInformation("Successfully updated Point");
      }
      catch
      {
        throw;
      }
    }

    /// <summary>
    /// Deletes specific point by their ID
    /// </summary>
    /// <param name="id">BSON ID</param>
    public async Task DeletePointAsync(string id)
    {
      _logger.LogInformation($"Deleting Point with ID {id}");
      try
      {
        await _PointsCollection.Points.DeleteOneAsync(p => p.Id == id);
      }
      catch
      {
        throw;
      }
    }

    /// <summary>
    /// Counts the number of squares that can be formed with the given points.
    /// </summary>
    /// <param name="points">The list of points.</param>
    /// <returns>The number of squares.</returns>
    public HashSet<string> CountSquares(List<Point> points)
    {
      // Remove duplicate points and ensure there are at least 4 points
      points = points.Distinct().ToList();
      if (points.Count < 4)
      {
        return new HashSet<string>();
      }

      // Create a HashSet for efficient lookup
      HashSet<(int, int)> pointSet = points.Select(p => (p.X, p.Y)).ToHashSet();

      var uniqueSquares = new HashSet<string>();

      for (int i = 0; i < points.Count; i++)
      {
        for (int j = i + 1; j < points.Count; j++)
        {
          Point p1 = points[i];
          Point p2 = points[j];

          // Calculate potential square vertices (2 other possible points from given Points)
          int dx = p2.X - p1.X;
          int dy = p2.Y - p1.Y;

          Point p3 = new Point(p1.X + dy, p1.Y - dx);
          Point p4 = new Point(p2.X + dy, p2.Y - dx);

          if (pointSet.Contains((p3.X, p3.Y)) && pointSet.Contains((p4.X, p4.Y)))
          {
            var squarePoints = new List<Point> { p1, p2, p3, p4 };
            if (IsSquare(squarePoints))
            {
              squarePoints.Sort((a, b) => a.X == b.X ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));
              string squareKey = string.Join(",", squarePoints.Select(p => $"\"x\": {p.X},\"y\": {p.Y}"));
              if (!uniqueSquares.Contains(squareKey))
              {
                uniqueSquares.Add(squareKey);
              }
            }
          }
        }
      }
      return uniqueSquares;
    }

    /// <summary>
    /// Private method to check if 4 points can create a square
    /// </summary>
    /// <param name="points">4 points to check if they create a square</param>
    /// <returns>true if 4 points creates a square, false if they don't</returns>
    /// code example from https://www.geeksforgeeks.org/how-to-check-if-given-four-points-form-a-square-or-not-in-javascript/
    private bool IsSquare(List<Point> points)
    {
      if (points.Count != 4)
        return false;

      // Calculate the squared distances between each pair of points
      double d1 = CalculateDistance(points[0], points[1]);
      double d2 = CalculateDistance(points[0], points[2]);
      double d3 = CalculateDistance(points[0], points[3]);
      double d4 = CalculateDistance(points[1], points[2]);
      double d5 = CalculateDistance(points[1], points[3]);
      double d6 = CalculateDistance(points[2], points[3]);

      // Identify the four sides and the two diagonals
      double[] distances = new double[] { d1, d2, d3, d4, d5, d6 };
      Array.Sort(distances);

      // Check the conditions for a square
      return distances[0] > 0 &&
             distances[0] == distances[1] &&
             distances[1] == distances[2] &&
             distances[2] == distances[3] &&
             distances[4] == distances[5] &&
             distances[4] == 2 * distances[0];
    }

    /// <summary>
    /// calculates squared distance between 2 points
    /// </summary>
    private double CalculateDistance(Point p1, Point p2)
    {
      var dx = p1.X - p2.X;
      var dy = p1.Y - p2.Y;
      return (dx * dx + dy * dy);
    }


  }
}
