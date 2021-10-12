var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<SchoolContext>(
        (s, o) => o
            .UseSqlite("Data Source=uni.db")
            .UseLoggerFactory(s.GetRequiredService<ILoggerFactory>()))
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .ConfigureResolverCompiler(c => c.AddService<SchoolContext>())
    .ModifyOptions(o => o.DefaultResolverStrategy = ExecutionStrategy.Serial)
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .EnsureDataIsSeeded()
    .InitializeOnStartup()
    .SetPagingOptions(new HotChocolate.Types.Pagination.PagingOptions { IncludeTotalCount = true }) // Added
    .AddDiagnosticEventListener<MyExecutionEventListener>(); // Added

var app = builder.Build();

app.MapGraphQL();

app.Run();

// Added:
public class MyExecutionEventListener : HotChocolate.Execution.Instrumentation.ExecutionDiagnosticEventListener
{
    private readonly ILogger<MyExecutionEventListener> _logger;

    public MyExecutionEventListener(ILogger<MyExecutionEventListener> logger)
        => _logger = logger;

    // this is invoked at the start of the `ExecuteRequest` operation
    public override IDisposable ExecuteRequest(IRequestContext context)
    {
        var start = DateTime.UtcNow;

        return new RequestScope(start, _logger);
    }
}

public class RequestScope : IDisposable
{
    private readonly ILogger _logger;
    private readonly DateTime _start;

    public RequestScope(DateTime start, ILogger logger)
    {
        _start = start;
        _logger = logger;
    }

    // this is invoked at the end of the `ExecuteRequest` operation
    public void Dispose()
    {
        var end = DateTime.UtcNow;
        var elapsed = end - _start;

        _logger.LogInformation("Request finished after {Ticks} ticks", elapsed.Ticks);
    }
}
