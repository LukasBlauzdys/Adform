using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Squares.Models
{
  /// <summary>
  /// Points class that stores x and y coordinates
  /// </summary>
  public class Point
  {
    /// <summary>
    /// Constructor for easily constructing points with providing 2 values of x and y coordinate
    /// </summary>
    /// <param name="x">Coordinate of an x in 2D space</param>
    /// <param name="y">Coordinate of a y point in 2D space</param>
    public Point(int x, int y)
    {
      X = x;
      Y = y;
    }

    /// <summary>
    /// Stores identifier for point
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    internal string? Id { get; set; }

    /// <summary>
    /// Stores X coordinate of a point
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Stores Y coordinate of a point
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// overriding to string to easily output X and Y coordinates
    /// </summary>
    /// <returns>Point x and y coordinates</returns>
    public override string ToString()
    {
      return $"X:{this.X}, Y:{this.Y}";
    }
  }
}