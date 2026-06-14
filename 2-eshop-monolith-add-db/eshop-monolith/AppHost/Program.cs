try
{
    // Load environment variables from .env file
    var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (File.Exists(envFilePath))
    {
        Console.WriteLine($"Loading environment variables from {envFilePath}...");
        foreach (var line in File.ReadAllLines(envFilePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
                Console.WriteLine($"Loaded: {parts[0].Trim()}");
            }
        }
    }

    // Get PostgreSQL credentials from environment variables
    var postgresUsername = Environment.GetEnvironmentVariable("POSTGRES_USERNAME") ?? "postgres";
    var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ??
        throw new InvalidOperationException("POSTGRES_PASSWORD not found in environment variables or .env file");

    var builder = DistributedApplication.CreateBuilder(args);

    // Create parameter resources for credentials
    var usernameParam = builder.AddParameter("postgres-username", postgresUsername, secret: false);
    var passwordParam = builder.AddParameter("postgres-password", postgresPassword, secret: true);

    var postgres = builder
        .AddPostgres("postgres", usernameParam, passwordParam)
            .WithPgAdmin(pgAdmin => pgAdmin.WithUrlForEndpoint("http", url => url.DisplayText = "PostgreDB Browser"))
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent);

    var eshopDb = postgres.AddDatabase("eshopdb");

    builder
        .AddProject<Projects.WebApp>("webapp")
        .WithUrlForEndpoint("https", url => url.DisplayText = "EShop WebApp (HTTPS)")
        .WithUrlForEndpoint("http", url => url.DisplayText = "EShop WebApp (HTTP)")
        .WithReference(eshopDb)
        .WaitFor(eshopDb);

    builder.Build().Run();
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

