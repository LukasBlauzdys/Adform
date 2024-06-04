namespace Squares.Services
{
  /// <summary>
  /// Request timeout middleware
  /// </summary>
  public class TimeoutMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<TimeoutMiddleware> _logger;
    private readonly TimeSpan _timeout;

    /// <summary>
    /// Initializes a new instance of the TimeoutMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger to log timeout events.</param>
    /// <param name="timeout">The timeout duration for requests.</param>
    public TimeoutMiddleware(RequestDelegate next, ILogger<TimeoutMiddleware> logger, TimeSpan timeout)
    {
      _next = next ?? throw new ArgumentNullException(nameof(next));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _timeout = timeout;
    }

    /// <summary>
    /// Invokes the middleware that checks if any task is finished it could be the timeout task or response
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
      using (var cts = new CancellationTokenSource(_timeout))
      {
        var timeoutTask = Task.Delay(_timeout, cts.Token);
        var requestTask = _next(context);

        var completedTask = await Task.WhenAny(requestTask, timeoutTask);

        if (completedTask == timeoutTask)
        {
          context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
          context.Response.ContentType = "application/json";
          await context.Response.WriteAsync("{\"error\": \"Request timed out.\"}");
          _logger.LogWarning("Request timed out.");
        }
        else
        {
          cts.Cancel();
          await requestTask;
        }
      }
    }

  }
}