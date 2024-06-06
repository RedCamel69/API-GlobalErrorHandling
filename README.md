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
