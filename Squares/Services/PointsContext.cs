using MongoDB.Driver;
using Squares.Models;

namespace Squares.Services
{
  public class PointsContext
  {
    private readonly IMongoDatabase _database;

    public PointsContext(IMongoDatabase database)
    {
      _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    /// <summary>
    /// Gets the collection of points from MongoDB
    /// </summary>
    public IMongoCollection<Point> Points => _database.GetCollection<Point>("Points");

  }
}