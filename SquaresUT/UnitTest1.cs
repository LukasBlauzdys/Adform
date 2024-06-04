using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using Squares.Models;
using Squares.Services;

namespace SquaresUT.Services
{
  [TestClass]
  public class PointsServiceTests
  {
    private Mock<IMongoCollection<Point>> _mockCollection;
    private Mock<IMongoDatabase> _mockDatabase;
    private Mock<PointsContext> _mockContext;
    private Mock<ILogger<PointsService>> _mockLogger;
    private PointsService _pointsService;

    /// <summary>
    /// Initializes different mocks to create a service
    /// </summary>
    [TestInitialize]
    public void SetUp()
    {
      _mockCollection = new Mock<IMongoCollection<Point>>();
      _mockDatabase = new Mock<IMongoDatabase>();
      _mockDatabase.Setup(db => db.GetCollection<Point>("Points", null))
        .Returns(_mockCollection.Object);
      _mockContext = new Mock<PointsContext>(_mockDatabase.Object);
      _mockLogger = new Mock<ILogger<PointsService>>();
      _pointsService = new PointsService(_mockContext.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Tests the GetPointsAsync method to ensure it returns the expected points list
    /// </summary>
    [TestMethod]
    public async Task GetPointsAsync_ExpectedBehavior()
    {
      // Arrange
      var points = new List<Point>
      {
        new Point(0, 0),
        new Point(1, 1)
      };
      var mockCursor = new Mock<IAsyncCursor<Point>>();

      mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(true)
        .ReturnsAsync(false);

      mockCursor.SetupSequence(_ => _.Current)
        .Returns(points)
        .Returns(Enumerable.Empty<Point>());

      _mockCollection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Point>>(), It.IsAny<FindOptions<Point, Point>>(), default))
        .ReturnsAsync(mockCursor.Object);

      // Act
      var result = await _pointsService.GetPointsAsync();

      // Assert
      Assert.AreEqual(2, result.Count);
      Assert.AreEqual(points[0].X, result[0].X);
      Assert.AreEqual(points[0].Y, result[0].Y);
      Assert.AreEqual(points[1].X, result[1].X);
      Assert.AreEqual(points[1].Y, result[1].Y);
    }

    /// <summary>
    /// Tests the GetPointByIdAsync method to ensure it throws a NullReferenceException when the ID is null
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public async Task GetPointByIdAsync_IdIsNull()
    {
      // Arrange
      string id = null;

      // Act
      var result = await _pointsService.GetPointByIdAsync(id);

      // Assert
      Assert.Fail();
    }

    /// <summary>
    /// Tests the GetPointByIdAsync method to ensure it returns the expected point for an existing ID
    /// </summary>
    [TestMethod]
    public async Task GetPointByIdAsync_ExistingId()
    {
      // Arrange
      string id = "665db816614252326ebcb636";
      var expectedPoint = new Point(0, 0);
      expectedPoint.Id = id;
      var mockCursor = new Mock<IAsyncCursor<Point>>();
      mockCursor.Setup(_ => _.Current).Returns(new List<Point> { expectedPoint });
      mockCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
      mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

      _mockCollection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Point>>(), It.IsAny<FindOptions<Point, Point>>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockCursor.Object);

      // Act
      var result = await _pointsService.GetPointByIdAsync(id);

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(expectedPoint.Id, result.Id);
      Assert.AreEqual(expectedPoint.X, result.X);
      Assert.AreEqual(expectedPoint.Y, result.Y);
    }

    /// <summary>
    /// Tests the CreatePointAsync method to ensure it inserts the provided points correctly
    /// </summary>
    [TestMethod]
    public async Task CreatePointAsync_StateUnderTest_ExpectedBehavior()
    {
      // Arrange
      var points = new List<Point>
      {
        new Point(0, 0),
        new Point(1, 1)
      };

      // Act
      await _pointsService.CreatePointAsync(points);

      // Assert
      _mockCollection.Verify(x => x.InsertManyAsync(points, null, default), Times.Once);
    }

    /// <summary>
    /// Tests the DeletePointAsync method to ensure it deletes the point with the specified ID
    /// </summary>
    [TestMethod]
    public async Task DeletePointAsync_StateUnderTest_ExpectedBehavior()
    {
      // Arrange
      string id = "665db816614252326ebcb636";

      _mockCollection.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Point>>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new DeleteResult.Acknowledged(1));

      // Act
      await _pointsService.DeletePointAsync(id);

      // Assert
      _mockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Point>>(), default), Times.Once);
    }

    /// <summary>
    /// Full 5x5 grid of Points that are equal to 50 squares (manually checked)
    /// 50 squares should be: (16 (1x1), 9(2x2), 4 (3x3), 1 (5x5),
    /// 9 (1x1 rotated clockwise (45 deg)), 1 (2x2 rotated clockwise (45 deg)),
    /// 4 smaller (rotated clockwise (~25deg)), 1 bigger (rotated clockwise (~25deg)),
    /// 4 smaller (rotated counterclockwise (~25deg)), 1 bigger (rotated counterclockwise (~25deg))
    /// </summary>
    [TestMethod]
    public void CountSquares_WithDifferentAngles()
    {
      // Arrange
      var points = new List<Point>
      {
        new Point ( 0,  0 ),
        new Point ( 1,  1 ),
        new Point ( 2,  2 ),
        new Point ( 3,  3 ),
        new Point ( 4,  4 ),
        new Point ( 0,  1 ),
        new Point ( 1,  0 ),
        new Point ( 2,  1 ),
        new Point ( 1,  2 ),
        new Point ( 3,  0 ),
        new Point ( 0,  3 ),
        new Point ( 3,  1 ),
        new Point ( 1,  3 ),
        new Point ( 4,  0 ),
        new Point ( 0,  4 ),
        new Point ( 4,  1 ),
        new Point ( 1,  4 ),
        new Point ( 2,  0 ),
        new Point ( 0,  2 ),
        new Point ( 2,  3 ),
        new Point ( 3,  2 ),
        new Point ( 4,  2 ),
        new Point ( 2,  4 ),
        new Point ( 3,  4 ),
        new Point( 4,  3 )
      };

      // Act
      var result = _pointsService.CountSquares(
        points);

      // Assert
      Assert.AreEqual(50, result.Count);
    }

    /// <summary>
    /// stress test for counting squares and for checking if after changes the result is still consistent
    /// </summary>
    [TestMethod]
    public void CountSquares_StressTest_10kPoints()
    {
      // Arrange
      // Generates constant 10000 points  
      int seed = 42;
      List<Point> points = GenerateRandomPoints(100 * 100, 100, 100, seed);

      // Act
      var result = _pointsService.CountSquares(
        points);

      // Assert
      Assert.AreEqual(1297337, result.Count);
    }

    /// <summary>
    /// Test counting squares if there is a trapezoid
    /// </summary>
    [TestMethod]
    public void CountSquares_Trapezoid()
    {
      // Arrange
      var points = new List<Point>
      {
        new Point ( 2,  0 ),
        new Point ( 1,  1 ),
        new Point ( 1,  3 ),
        new Point ( 2,  2 ),
      };

      // Act
      var result = _pointsService.CountSquares(
        points);

      // Assert
      Assert.AreEqual(0, result.Count);
    }

    /// <summary>
    /// Tests counting squares with different smaller trapezoid
    /// </summary>
    [TestMethod]
    public void CountSquares_TrapezoidSmall()
    {
      // Arrange
      var points = new List<Point>
      {
        new Point ( 1,1 ),
        new Point ( 1,0),
        new Point ( 0,0 ),
        new Point ( 2,  1 ),
      };

      // Act
      var result = _pointsService.CountSquares(
        points);

      // Assert
      Assert.AreEqual(0, result.Count);
    }

    /// <summary>
    /// Tests counting squares with rectangle
    /// </summary>
    [TestMethod]
    public void CountSquares_Rectangle()
    {
      // Arrange
      var points = new List<Point>
      {
        new Point ( 0,1 ),
        new Point (2,0),
        new Point ( 0,0 ),
        new Point ( 2,  1 ),
      };

      // Act
      var result = _pointsService.CountSquares(
        points);

      // Assert
      Assert.AreEqual(0, result.Count);
    }

    /// <summary>
    /// Tests counting squares around zero point ( with X and Y coords being negative and positive)
    /// </summary>
    [TestMethod]
    public void CountSquares_AroundZeroPoint()
    {
      // Arrange
      var points = new List<Point>
      {
        new Point ( -1,  -1 ),
        new Point ( 1,  -1 ),
        new Point ( 1,  1),
        new Point ( -1,  1 ),
      };

      // Act
      var result = _pointsService.CountSquares(
        points);

      // Assert
      Assert.AreEqual(1, result.Count);
    }

    /// <summary>
    /// Tests counting squares when all the coordinates are in minus
    /// </summary>
    [TestMethod]
    public void CountSquares_MinusPoints()
    {
      // Arrange
      var points = new List<Point>
      {
        new Point ( -1,  -1 ),
        new Point ( -2,-2 ),
        new Point ( -2,-1),
        new Point ( -1,  -2 ),
      };

      // Act
      var result = _pointsService.CountSquares(
        points);

      // Assert
      Assert.AreEqual(1, result.Count);
    }

    /// <summary>
    /// Tests counting rotated squares
    /// </summary>
    [TestMethod]
    public void CountSquares_Rotated()
    {
      // Arrange
      var points = new List<Point>
      {
        new Point ( -1,  -1 ),
        new Point ( 1,  -1 ),
        new Point ( 0,  0),
        new Point ( 0,  -2 ),
      };

      // Act
      var result = _pointsService.CountSquares(
        points);

      // Assert
      Assert.AreEqual(1, result.Count);
    }

    /// <summary>
    /// helper function to generate multiple random points in given grid
    /// </summary>
    /// <param name="numPoints">how many points to generate</param>
    /// <param name="maxX">maximum X coordinate</param>
    /// <param name="maxY">maximum Y coordinate</param>
    /// <param name="seed">Randomizer seed for consisted results</param>
    /// <returns></returns>
    private List<Point> GenerateRandomPoints(int numPoints, int maxX, int maxY, int seed)
    {
      Random rand = new Random(seed);
      HashSet<Point> points = new HashSet<Point>();

      while (points.Count < numPoints)
      {
        points.Add(new Point(rand.Next(maxX), rand.Next(maxY)));
      }

      return points.ToList();
    }
  }
}
