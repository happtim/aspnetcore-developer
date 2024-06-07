<Query Kind="Statements">
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
</Query>

// Create a logger factory
var loggerFactory = LoggerFactory.Create(builder =>
{
	builder
		.AddConsole();
});

// Get a logger instance
var logger = loggerFactory.CreateLogger<UserQuery>();

// Log some messages
logger.LogTrace("This is a trace message.");
logger.LogDebug("This is a debug message.");
logger.LogInformation("This is an information message.");
logger.LogWarning("This is a warning message.");
logger.LogError("This is an error message.");
logger.LogCritical("This is a critical message.");