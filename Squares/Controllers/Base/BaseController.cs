using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net;

namespace Squares.Controllers.Base
{
  /// <summary>
  /// Base controller class to implement easy exception handling
  /// </summary>
  public abstract class BaseController : ControllerBase
  {
    private readonly ILogger<BaseController> _logger;

    /// <summary>
    /// base controller for creating logger
    /// </summary>
    protected BaseController(ILogger<BaseController> logger)
    {
      _logger = logger;
    }

    /// <summary>
    /// Helper method to handle multiple types of exceptions
    /// </summary>
    /// <param name="ex">any type of exception</param>
    protected ActionResult HandleException(Exception ex)
    {
      switch (ex)
      {
        case TimeoutException:
          _logger.LogError(ex, "The request timed out. Please try again later.");
          break;
        case MongoConnectionException:
          _logger.LogError(ex, "There was a connection issue. Please try again later.");
          break;
        case MongoCommandException:
          _logger.LogError(ex, "A database error occurred. Please try again later.");
          break;
        default:
          _logger.LogError(ex, "An unexpected error occurred. Please try again later.");
          break;
      }

      var userFriendlyMessage = ex switch
      {
        TimeoutException => "The request timed out. Please try again later.",
        MongoConnectionException => "There was a connection issue. Please try again later.",
        MongoCommandException => "A database error occurred. Please try again later.",
        _ => "An unexpected error occurred. Please try again later.",
      };

      var statusCode = ex switch
      {
        TimeoutException => HttpStatusCode.GatewayTimeout,
        MongoConnectionException => HttpStatusCode.InternalServerError,
        MongoCommandException => HttpStatusCode.InternalServerError,
        _ => HttpStatusCode.InternalServerError,
      };

      var result = new
      {
        message = userFriendlyMessage,
        statusCode = (int)statusCode
      };

      return StatusCode((int)statusCode, result);
    }
  }

}
