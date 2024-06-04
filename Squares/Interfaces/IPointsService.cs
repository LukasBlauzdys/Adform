using Squares.Models;

namespace Squares.Interfaces
{
  public interface IPointsService
  {
    Task<List<Point>> GetPointsAsync();
    Task<Point> GetPointByIdAsync(string id);
    Task CreatePointAsync(List<Point> point);
    Task UpdatePointAsync(string id, Point point);
    Task DeletePointAsync(string id);
    HashSet<string> CountSquares(List<Point> points);
  }
}