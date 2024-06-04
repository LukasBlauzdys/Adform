namespace Squares.Models
{
  /// <summary>
  /// Database settings class
  /// </summary>
  public class SquaresDatabaseSettings
  {
    /// <summary>
    /// Database connection string
    /// </summary>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// Database name
    /// </summary>
    public string DatabaseName { get; set; } = null!;
  }
}