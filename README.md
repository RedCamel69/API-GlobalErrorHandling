#API-GlobalErrorHandling

In versions of ASP.NET Core up to 7, the implementation of custom middleware was necessary for global error handling. 
However, with the introduction of ASP.NET Core 8, this process has been significantly simplified with the new IExceptionHandler interface.

This interface allows you to not only map exceptions to the appropriate HTTP status code, but also to log the details of the exception with a unique traceId. 
This traceId can be subsequently used for troubleshooting issues.


RFC 7807, also known as “Problem Details for HTTP APIs”, is a standard that provides a method for conveying machine-readable details of errors in HTTP responses for HTTP APIs.

 ASP.NET Core already provides support for it by just adding a few lines of code to your Program.cs file:
 
```
builder.Services.AddSingleton<IFilmRepository, FilmRepository>()
    .AddProblemDetails();

app.UseStatusCodePages();
app.UseExceptionHandler();

```

Here’s what those new lines do:

AddProblemDetails() registers the problem details middleware that will handle exceptions and return a problem details response.
UseStatusCodePages() adds a middleware that will return a problem details response for common HTTP status codes.
UseExceptionHandler() adds a middleware that will return a problem details response for unhandled exceptions.

Now, if you run the API again and invoke either of the endpoints, you’ll get a slightly more useful response:

```
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500
}
```

The added JSON payload contains a few useful properties:

type: A URI reference that identifies the problem type.
title: A short, human-readable summary of the problem type.
status: The HTTP status code generated by the origin server for this occurrence of the problem.


Building on this we can create a global exception handler that will:

Catch all unhandled exceptions and return a problem details response
Map each exception to the correct problem details response
Logs the exception details to our logging provider

With the new IExceptionHandler interface available starting with .NET 8, implementing this global exception handler is pretty straightforward:


```
  public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
  {
      public async ValueTask<bool> TryHandleAsync(
          HttpContext httpContext,
          Exception exception,
          CancellationToken cancellationToken)
      {
          var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

          logger.LogError(
              exception,
              "Could not process a request on machine {MachineName}. TraceId: {TraceId}",
              Environment.MachineName,
              traceId
          );

          var (statusCode, title) = MapException(exception);

          await Results.Problem(
              title: title,
              statusCode: statusCode,
              extensions: new Dictionary<string, object?>
              {
              {"traceId",  traceId}
              }
          ).ExecuteAsync(httpContext);

          return true;
      }


    private static (int StatusCode, string Title) MapException(Exception exception)
    {
      return exception switch
       {
         ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, exception.Message),
         ArgumentException => (StatusCodes.Status400BadRequest,exception.Message),
         _ => (StatusCodes.Status500InternalServerError, "Sorry, its not you , its us!")
       };
    }
  }

```


TryHandleAsync() is the method that will be invoked by the problem details middleware when any exception is thrown.

The first thing we do is capture a unique traceId that will be used to correlate the exception with the logs. We can get that either from the current activity or from the httpContext trace identifier

Then we log the exception details using the ILogger instance, making sure we include some important details like the machine name and the traceId.

Next, we use the MapException() method to map the exception to the correct status code and title.
Any ArgumentOutOfRangeException or ArgumentException will return a 400 status code and the exception message as the title since this will be useful for clients.
Any other exception will return a 500 status code and a generic title since we don’t want to reveal too much of our internal details to clients.

Finally, we use the Problem() helper method to create a problem details response with the correct status code, title, and traceId.

Notice also that we return true at the end of the method, which means we handled the exception and the request pipeline can stop here.


To register the exception handler, all you need to do is invoke the AddExceptionHandler() method in your Program.cs file:

```
            builder.Services.AddSingleton<IFilmRepository, FilmRepository>()
                .AddProblemDetails()
                .AddExceptionHandler<GlobalExceptionHandler>();
```

Now the response will look like 

```
{
	"type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
	"title": "We made a mistake but we are on it!",
	"status": 500,
	"traceId": "00-b6319ccd3d239807cfb4ca877e88ee19-cecb30a8cb7357a4-00"
}
```

Not only did the InvalidOperationException get mapped to a 500 status code and a generic title, but also a handy traceId was included in the response.
