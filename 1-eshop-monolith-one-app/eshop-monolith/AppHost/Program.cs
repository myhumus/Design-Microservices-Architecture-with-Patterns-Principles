try
{
    Console.WriteLine("AppHost starting...");
    var builder = DistributedApplication.CreateBuilder(args);
    Console.WriteLine("Builder created successfully.");

    builder
        .AddProject<Projects.WebApp>("webapp")
        .WithUrlForEndpoint("https", url => url.DisplayText = "EShop WebApp (HTTPS)")
        .WithUrlForEndpoint("http", url => url.DisplayText = "EShop WebApp (HTTP)");

    Console.WriteLine("WebApp project added successfully.");

    var app = builder.Build();
    Console.WriteLine("Application built successfully. Starting...");

    app.Run();

    Console.WriteLine("Application completed.");
}
catch (Exception ex)
{
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
    }
    throw;
}
