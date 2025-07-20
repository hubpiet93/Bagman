TITLE: Setting Redis Connection String in WebApplicationFactory for C#
DESCRIPTION: This snippet demonstrates how to extend WebApplicationFactory to set a Redis connection string before creating the TestServer. It overrides the ConfigureWebHost method to use the connection string from a Redis container.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/examples/aspnet.md#2025-04-22_snippet_0

LANGUAGE: csharp
CODE:
```
private sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseSetting("ConnectionStrings:RedisCache", _redisContainer.GetConnectionString());
  }
}
```

----------------------------------------

TITLE: Pinning Docker Image Version in Testcontainers
DESCRIPTION: Demonstrates how to override and pin a specific Docker image version. This is a recommended practice regardless of whether using generic or module container builders.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/best_practices.md#2025-04-22_snippet_6

LANGUAGE: csharp
CODE:
```
_builder.WithImage("alpine:3.20.0")
```

----------------------------------------

TITLE: Running Hello World Container with Testcontainers in C#
DESCRIPTION: Complete example showing how to configure, start, and interact with a Docker container using Testcontainers for .NET. The code demonstrates creating a container, binding ports, setting wait strategies, and making HTTP requests to the container.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/index.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
// Create a new instance of a container.
var container = new ContainerBuilder()
  // Set the image for the container to "testcontainers/helloworld:1.2.0".
  .WithImage("testcontainers/helloworld:1.2.0")
  // Bind port 8080 of the container to a random port on the host.
  .WithPortBinding(8080, true)
  // Wait until the HTTP endpoint of the container is available.
  .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8080)))
  // Build the container configuration.
  .Build();

// Start the container.
await container.StartAsync()
  .ConfigureAwait(false);

// Create a new instance of HttpClient to send HTTP requests.
using var httpClient = new HttpClient();

// Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
var requestUri = new UriBuilder(Uri.UriSchemeHttp, container.Hostname, container.GetMappedPublicPort(8080), "uuid").Uri;

// Send an HTTP GET request to the specified URI and retrieve the response as a string.
var guid = await httpClient.GetStringAsync(requestUri)
  .ConfigureAwait(false);

// Ensure that the retrieved UUID is a valid GUID.
Debug.Assert(Guid.TryParse(guid, out _));
```

----------------------------------------

TITLE: Chaining Multiple Wait Strategies in Testcontainers .NET
DESCRIPTION: This snippet demonstrates how to chain multiple wait strategies together, including port availability, file existence, and custom operations. It also shows how to add a custom wait strategy.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/wait_strategies.md#2025-04-22_snippet_0

LANGUAGE: csharp
CODE:
```
_ = Wait.ForUnixContainer()
  .UntilPortIsAvailable(80)
  .UntilFileExists("/tmp/foo")
  .UntilFileExists("/tmp/bar")
  .UntilOperationIsSucceeded(() => true, 1)
  .AddCustomWaitStrategy(new MyCustomWaitStrategy());
```

----------------------------------------

TITLE: Setting Network Aliases for Container-to-Container Communication
DESCRIPTION: Shows how to configure network aliases for containers to enable reliable container-to-container communication. This is preferred over using IP addresses.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/best_practices.md#2025-04-22_snippet_3

LANGUAGE: csharp
CODE:
```
_builder.WithNetworkAliases(string)
```

----------------------------------------

TITLE: Configuring Docker Containers with Testcontainers in C#
DESCRIPTION: This snippet showcases the use of builder methods in Testcontainers for .NET to configure a Docker container. These methods enable setting container dependencies, Docker endpoints, labels, commands, and network settings among others. The configuration is used to automate container lifecycle management within .NET applications.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_11

LANGUAGE: C#
CODE:
```
"`DependsOn`                   | Sets the dependent resource to resolve and create or start before starting this container configuration.\n`WithDockerEndpoint`          | Sets the Docker daemon socket to connect to.\n`WithAutoRemove`              | Will remove the stopped container automatically, similar to `--rm`.\n`WithCleanUp`                 | Will remove the container automatically after all tests have been run.\n`WithLabel`                   | Applies metadata to the container e.g. `-l`, `--label \"testcontainers=awesome\"`.\n`WithImage`                   | Specifies an image for which to create the container.\n`WithImagePullPolicy`         | Specifies an image pull policy to determine when an image is pulled e.g. <code>--pull \"always\" &vert; \"missing\" &vert; \"never\"</code>.\n`WithName`                    | Sets the container name e.g. `--name \"testcontainers\"`.\n`WithHostname`                | Sets the container hostname e.g. `--hostname \"testcontainers\"`.\n`WithMacAddress`              | Sets the container MAC address e.g. `--mac-address \"00:80:41:ae:fd:7e\"`.\n`WithWorkingDirectory`        | Specifies or overrides the `WORKDIR` for the instruction sets.\n`WithEntrypoint`              | Specifies or overrides the `ENTRYPOINT` that runs the executable.\n`WithCommand`                 | Specifies or overrides the `COMMAND` instruction provided in the Dockerfile.\n`WithEnvironment`             | Sets an environment variable in the container e.g. `-e`, `--env \"MAGIC_NUMBER=42\"`.\n`WithExposedPort`             | Exposes a port inside the container e.g. `--expose \"80\"`.\n`WithPortBinding`             | Publishes a container port to the host e.g. `-p`, `--publish \"80:80\"`.\n`WithResourceMapping`         | Copies a file or any binary content into the created container even before it is started.\n`WithBindMount`               | Binds a path of a file or directory into the container e.g. `-v`, `--volume \".:/tmp\"`.\n`WithVolumeMount`             | Mounts a managed volume into the container e.g. `--mount \"type=volume,source=my-vol,destination=/tmp\"`.\n`WithTmpfsMount`              | Mounts a temporary volume into the container e.g. `--mount \"type=tmpfs,destination=/tmp\"`.\n`WithNetwork`                 | Assigns a network to the container e.g. `--network \"bridge\"`.\n`WithNetworkAliases`          | Assigns a network-scoped aliases to the container e.g. `--network-alias \"alias\"`.\n`WithExtraHost`               | Adds a custom host-to-IP mapping to the container's `/etc/hosts` respectively `%WINDIR%\\system32\\drivers\\etc\\hosts` e.g. `--add-host \"host.testcontainers.internal:172.17.0.2\"`.\n`WithPrivileged`              | Sets the `--privileged` flag.\n`WithOutputConsumer`          | Redirects `stdout` and `stderr` to capture the container output.\n`WithWaitStrategy`            | Sets the wait strategy to complete the container start and indicates when it is ready.\n`WithStartupCallback`         | Sets the startup callback to invoke after the container start.\n`WithCreateParameterModifier` | Allows low level modifications of the Docker container create parameter."
```

----------------------------------------

TITLE: Mapping Resources to Containers in Testcontainers
DESCRIPTION: Demonstrates how to copy dependent files to a container before it starts. This is the recommended approach instead of mounting local host paths.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/best_practices.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
_container.WithResourceMapping(string, string)
```

----------------------------------------

TITLE: Waiting for Docker Container Health Check in Testcontainers .NET
DESCRIPTION: This example shows how to use Docker's HEALTHCHECK feature as a wait strategy in Testcontainers .NET.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/wait_strategies.md#2025-04-22_snippet_5

LANGUAGE: csharp
CODE:
```
_ = new ContainerBuilder()
  .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy());
```

----------------------------------------

TITLE: Using PostgreSqlContainer in xUnit.net Tests
DESCRIPTION: This C# code demonstrates how to use a PostgreSqlContainer in xUnit.net tests. It implements IAsyncLifetime to manage the container lifecycle, starting it before tests and disposing of it after.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/postgres.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.PostgreSql.Tests/PostgreSqlContainerTest.cs:UsePostgreSqlContainer"
```

----------------------------------------

TITLE: Building a Docker Image with Testcontainers for .NET
DESCRIPTION: Creates and builds a Docker image from a Dockerfile located in the solution directory, demonstrating the basic usage of ImageFromDockerfileBuilder.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_image.md#2025-04-22_snippet_0

LANGUAGE: csharp
CODE:
```
var futureImage = new ImageFromDockerfileBuilder()
  .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
  .WithDockerfile("Dockerfile")
  .Build();

await futureImage.CreateAsync()
  .ConfigureAwait(false);
```

----------------------------------------

TITLE: Using the Event Hubs Container in xUnit Tests
DESCRIPTION: Example showing how to use the Event Hubs container in an xUnit test, demonstrating container lifecycle management with IAsyncLifetime and actual test implementation.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/eventhubs.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.EventHubs.Tests/EventHubsContainerTest.cs:UseEventHubsContainer"
```

----------------------------------------

TITLE: Waiting for HTTP 200 OK on Port 80 in Testcontainers .NET
DESCRIPTION: This snippet demonstrates how to configure a wait strategy that checks for an HTTP 200 OK response on the root path of port 80.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/wait_strategies.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
_ = Wait.ForUnixContainer()
  .UntilHttpRequestIsSucceeded(request => request
    .ForPath("/"));
```

----------------------------------------

TITLE: Using Azure Service Bus Container in xUnit Tests
DESCRIPTION: Example test class demonstrating how to use the Azure Service Bus container with xUnit.net's IAsyncLifetime interface for container lifecycle management.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/servicebus.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.ServiceBus.Tests/ServiceBusContainerTest.cs:UseServiceBusContainer"
```

----------------------------------------

TITLE: Copying a File to a Container using Testcontainers
DESCRIPTION: Shows how to copy a single file from the host filesystem into a Docker container using the WithResourceMapping method with a FileInfo object.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_3

LANGUAGE: csharp
CODE:
```
_ = new ContainerBuilder()
  .WithResourceMapping(new FileInfo("appsettings.json"), "/app/");
```

----------------------------------------

TITLE: Using RabbitMQ Container in Tests
DESCRIPTION: Example showing how to implement RabbitMQ container tests using xUnit.net's IAsyncLifetime interface. Demonstrates container lifecycle management with initialization and disposal.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/rabbitmq.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.RabbitMq.Tests/RabbitMqContainerTest.cs:UseRabbitMqContainer"
```

----------------------------------------

TITLE: NGINX Container Example with HTTP Status Verification
DESCRIPTION: A complete example showing how to create, configure, and start an NGINX container with a random port binding, then verify its operation by making an HTTP request and checking the status code.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_9

LANGUAGE: csharp
CODE:
```
const ushort HttpPort = 80;

var nginxContainer = new ContainerBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .WithImage("nginx")
  .WithPortBinding(HttpPort, true)
  .Build();

await nginxContainer.StartAsync()
  .ConfigureAwait(false);

using var httpClient = new HttpClient();
httpClient.BaseAddress = new UriBuilder("http", nginxContainer.Hostname, nginxContainer.GetMappedPublicPort(HttpPort)).Uri;

var httpResponseMessage = await httpClient.GetAsync(string.Empty)
  .ConfigureAwait(false);

Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
```

----------------------------------------

TITLE: Basic Host-Container Communication in C#
DESCRIPTION: Demonstrates how to create a network connection to a container using hostname and mapped port. Uses UriBuilder to construct the connection string safely.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_network.md#2025-04-22_snippet_0

LANGUAGE: csharp
CODE:
```
_ = new UriBuilder("tcp", _container.Hostname, _container.GetMappedPublicPort(2375));
```

----------------------------------------

TITLE: Enabling Container Reuse in Testcontainers .NET
DESCRIPTION: Demonstrates how to enable resource reuse for a container using the ContainerBuilder. When enabled, the container will be retained after tests for future reuse instead of being disposed.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/resource_reuse.md#2025-04-22_snippet_0

LANGUAGE: csharp
CODE:
```
_ = new ContainerBuilder()
  .WithReuse(true);
```

----------------------------------------

TITLE: Using Qdrant Container in xUnit Tests
DESCRIPTION: Example of how to create and use a Qdrant container in an xUnit test class by implementing IAsyncLifetime to manage container lifecycle.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/qdrant.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Qdrant.Tests/QdrantDefaultContainerTest.cs:UseQdrantContainer"
```

----------------------------------------

TITLE: Retrieving Mapped Port from Testcontainer
DESCRIPTION: Shows how to retrieve the dynamically assigned public port that maps to a specific container port. This is used after configuring random port binding.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/best_practices.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
_container.GetMappedPublicPort(ushort)
```

----------------------------------------

TITLE: Creating an Azure Event Hubs Container Instance
DESCRIPTION: Sample code showing how to create and configure an Event Hubs container instance using Testcontainers. This sets up a containerized Event Hubs emulator for isolated testing.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/eventhubs.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.EventHubs.Tests/EventHubsContainerTest.cs:CreateEventHubsContainer"
```

----------------------------------------

TITLE: Creating Azure Service Bus Container Instance in C#
DESCRIPTION: Example showing how to create and configure an Azure Service Bus container instance using Testcontainers.NET.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/servicebus.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.ServiceBus.Tests/ServiceBusContainerTest.cs:CreateServiceBusContainer"
```

----------------------------------------

TITLE: Using Random Port Binding in Testcontainers
DESCRIPTION: Demonstrates how to configure random port binding for a container and retrieve the mapped public port. This avoids port conflicts that can occur with static port assignments.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/best_practices.md#2025-04-22_snippet_0

LANGUAGE: csharp
CODE:
```
_builder.WithPortBinding(ushort, true)
```

----------------------------------------

TITLE: Initializing and Starting Dependencies in C# using IAsyncLifetime
DESCRIPTION: This snippet demonstrates the use of IAsyncLifetime.InitializeAsync to create and start all dependencies before any test run, including the Docker network, Microsoft SQL Server container, and the weather forecast application container.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/examples/aspnet.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
await Image.InitializeAsync()
  .ConfigureAwait(false);

await _weatherForecastNetwork.CreateAsync()
  .ConfigureAwait(false);

await _msSqlContainer.StartAsync()
  .ConfigureAwait(false);

await _weatherForecastContainer.StartAsync()
  .ConfigureAwait(false);
```

----------------------------------------

TITLE: Setting Container Memory Limit Using Docker API in Testcontainers .NET
DESCRIPTION: Demonstrates how to set a container's memory limit to 2GB using the WithCreateParameterModifier method. This example shows direct modification of the Docker API parameters when Testcontainers' standard API doesn't provide the needed functionality.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/low_level_api_access.md#2025-04-22_snippet_0

LANGUAGE: csharp
CODE:
```
const long TwoGB = 2L * 1024 * 1024 * 1024;
_ = new ContainerBuilder()
  .WithCreateParameterModifier(parameterModifier => parameterModifier.HostConfig.Memory = TwoGB);
```

----------------------------------------

TITLE: Using MSSQL Container in xUnit Tests
DESCRIPTION: Example test class implementing IAsyncLifetime to manage the MSSQL container lifecycle. The test connects to the database using the generated connection string and verifies proper functioning.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/mssql.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.MsSql.Tests/MsSqlContainerTest.cs:UseMsSqlContainer"
```

----------------------------------------

TITLE: Configuring Container Startup Commands in .NET Testcontainers
DESCRIPTION: Demonstrates how to override the default ENTRYPOINT and CMD of a Docker container using Testcontainers. This example configures an NGINX container to only test its configuration file instead of starting the service.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_0

LANGUAGE: csharp
CODE:
```
_ = new ContainerBuilder()
  .WithEntrypoint("nginx")
  .WithCommand("-t");
```

----------------------------------------

TITLE: MongoDB Container Usage Example
DESCRIPTION: Example showing how to use the MongoDB container in a test scenario, implementing IAsyncLifetime for container lifecycle management.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/mongodb.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.MongoDb.Tests/MongoDbContainerTest.cs:UseMongoDbContainer"
```

----------------------------------------

TITLE: Creating and Starting a Testcontainers Module Container in C#
DESCRIPTION: Basic C# code snippet demonstrating how to create and start a pre-configured container using a Testcontainers module. This shows the standard pattern used across all modules.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/index.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
var moduleNameContainer = new ModuleNameBuilder().Build();
await moduleNameContainer.StartAsync();
```

----------------------------------------

TITLE: Using a Pulsar Container in xUnit Tests
DESCRIPTION: Example showing how to use a Pulsar container within an xUnit test, including container lifecycle management with IAsyncLifetime.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/pulsar.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Pulsar.Tests/PulsarContainerTest.cs:UsePulsarContainer"
```

----------------------------------------

TITLE: Configuring Wait Strategy Timeout in Testcontainers .NET
DESCRIPTION: This example shows how to configure a wait strategy with a timeout. It waits for a specific log message to appear and cancels the readiness check after one minute.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/wait_strategies.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
_ = Wait.ForUnixContainer()
  .UntilMessageIsLogged("Server started", o => o.WithTimeout(TimeSpan.FromMinutes(1)));
```

----------------------------------------

TITLE: Basic .NET Dockerfile for Testcontainers
DESCRIPTION: A Dockerfile that builds and publishes a .NET 6.0 application, designed to work with the build context provided by Testcontainers.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_image.md#2025-04-22_snippet_2

LANGUAGE: Dockerfile
CODE:
```
FROM mcr.microsoft.com/dotnet/sdk:6.0
ARG SLN_FILE_PATH="WeatherForecast.sln"
COPY . .
RUN dotnet restore $SLN_FILE_PATH
RUN dotnet publish $SLN_FILE_PATH --configuration Release --framework net6.0 --output app
ENTRYPOINT ["dotnet", "/app/WeatherForecast.dll"]
```

----------------------------------------

TITLE: Accessing Container Hostname in Testcontainers
DESCRIPTION: Demonstrates how to access the container hostname property instead of using localhost. This is the recommended approach for connecting to containers from the test host.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/best_practices.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
_container.Hostname
```

----------------------------------------

TITLE: Reusing Existing MSSQL Container with Service Bus Emulator
DESCRIPTION: Code example showing how to configure the Service Bus container builder to use an existing MSSQL container instance instead of creating a new one.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/servicebus.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.ServiceBus.Tests/ServiceBusContainerTest.cs:ReuseExistingMsSqlContainer"
```

----------------------------------------

TITLE: Creating MSSQL Container Instance in C#
DESCRIPTION: Code snippet demonstrating how to create a Microsoft SQL Server container instance using Testcontainers.NET. It configures the container with a password and exposes the default MSSQL port.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/mssql.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.MsSql.Tests/MsSqlContainerTest.cs:CreateMsSqlContainer"
```

----------------------------------------

TITLE: Running Shared Redis Container Tests in C#
DESCRIPTION: Example of running tests with a shared Redis container, demonstrating that the container state is preserved between tests.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/test_frameworks/xunit_net.md#2025-04-22_snippet_5

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest`2.cs:RunTests"
```

----------------------------------------

TITLE: Custom Network Communication Between Containers
DESCRIPTION: Shows how to create a custom network for container-to-container communication using NetworkBuilder. Demonstrates setting up two containers that communicate through a network alias.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_network.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
const string MagicNumber = "42";

const string MagicNumberHost = "deep-thought";

const ushort MagicNumberPort = 80;

var network = new NetworkBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .Build();

var deepThoughtContainer = new ContainerBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .WithImage("alpine")
  .WithEnvironment("MAGIC_NUMBER", MagicNumber)
  .WithEntrypoint("/bin/sh", "-c")
  .WithCommand($"while true; do echo \"$MAGIC_NUMBER\" | nc -l -p {MagicNumberPort}; done")
  .WithNetwork(network)
  .WithNetworkAliases(MagicNumberHost)
  .Build();

var ultimateQuestionContainer = new ContainerBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .WithImage("alpine")
  .WithEntrypoint("top")
  .WithNetwork(network)
  .Build();

await network.CreateAsync()
  .ConfigureAwait(false);

await Task.WhenAll(deepThoughtContainer.StartAsync(), ultimateQuestionContainer.StartAsync())
  .ConfigureAwait(false);

var execResult = await ultimateQuestionContainer.ExecAsync(new[] { "nc", MagicNumberHost, MagicNumberPort.ToString(CultureInfo.InvariantCulture) })
  .ConfigureAwait(false);

Assert.Equal(MagicNumber, execResult.Stdout.Trim());
```

----------------------------------------

TITLE: Configuring Network Reuse with Fixed Name
DESCRIPTION: Demonstrates how to configure network reuse by setting a fixed network name instead of the default random name, which is necessary for proper resource reuse functionality.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/resource_reuse.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
_ = new NetworkBuilder()
  .WithReuse(true)
  .WithName("WeatherForecast");
```

----------------------------------------

TITLE: Configuring WeatherForecastContainer with Dependencies in C#
DESCRIPTION: This snippet shows the configuration of WeatherForecastContainer class, setting up the Docker network, Microsoft SQL Server container, and the weather forecast application container with necessary environment variables and wait strategy.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/examples/aspnet.md#2025-04-22_snippet_3

LANGUAGE: csharp
CODE:
```
const string weatherForecastStorage = "weatherForecastStorage";

const string connectionString = $"server={weatherForecastStorage};user id={MsSqlBuilder.DefaultUsername};password={MsSqlBuilder.DefaultPassword};database={MsSqlBuilder.DefaultDatabase}";

_weatherForecastNetwork = new NetworkBuilder()
  .Build();

_msSqlContainer = new MsSqlBuilder()
  .WithNetwork(_weatherForecastNetwork)
  .WithNetworkAliases(weatherForecastStorage)
  .Build();

_weatherForecastContainer = new ContainerBuilder()
  .WithImage(Image)
  .WithNetwork(_weatherForecastNetwork)
  .WithPortBinding(WeatherForecastImage.HttpsPort, true)
  .WithEnvironment("ASPNETCORE_URLS", "https://+")
  .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", WeatherForecastImage.CertificateFilePath)
  .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", WeatherForecastImage.CertificatePassword)
  .WithEnvironment("ConnectionStrings__DefaultConnection", connectionString)
  .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(WeatherForecastImage.HttpsPort))
  .Build();
```

----------------------------------------

TITLE: Configuring Docker Compose for Testcontainers Tests
DESCRIPTION: Docker Compose configuration for running .NET tests inside a container with the Docker Wormhole pattern. Includes volume mounting for the Docker socket and comments about additional environment variables needed for Docker Desktop.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/dind/index.md#2025-04-22_snippet_1

LANGUAGE: yaml
CODE:
```
version: "3"
services:
  docker_compose_test:
    build:
      dockerfile: Dockerfile
      context: .
    entrypoint: dotnet
    command: test
    # Uncomment the lines below in the case of Docker Desktop (see note above).
    # TESTCONTAINERS_HOST_OVERRIDE is not needed in the case of Docker Engine.
    # environment:
    #   - TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal
    volumes:
      - /var/run/docker.sock.raw:/var/run/docker.sock
```

----------------------------------------

TITLE: Creating MongoDB Container Instance
DESCRIPTION: C# code demonstrating how to create and configure a MongoDB container instance using Testcontainers.NET. Shows the container setup with basic configuration.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/mongodb.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.MongoDb.Tests/MongoDbContainerTest.cs:CreateMongoDbContainer"
```

----------------------------------------

TITLE: Waiting for Multiple HTTP Status Codes in Testcontainers .NET
DESCRIPTION: This example shows how to wait for either an HTTP 200 OK or 301 Moved Permanently response on the root path.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/wait_strategies.md#2025-04-22_snippet_3

LANGUAGE: csharp
CODE:
```
_ = Wait.ForUnixContainer()
  .UntilHttpRequestIsSucceeded(request => request
    .ForPath("/")
    .ForStatusCode(HttpStatusCode.OK)
    .ForStatusCode(HttpStatusCode.MovedPermanently));
```

----------------------------------------

TITLE: Forwarding Container Logs to Console in Testcontainers
DESCRIPTION: Demonstrates how to continuously forward container logs to the console in real-time during container execution using an output consumer.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_8

LANGUAGE: csharp
CODE:
```
using IOutputConsumer outputConsumer = Consume.RedirectStdoutAndStderrToConsole();

_ = new ContainerBuilder()
  .WithOutputConsumer(outputConsumer);
```

----------------------------------------

TITLE: Creating a Pulsar Container Instance in C#
DESCRIPTION: Code snippet that demonstrates how to create an Apache Pulsar container instance using the Testcontainers.Pulsar library.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/pulsar.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Pulsar.Tests/PulsarContainerTest.cs:CreatePulsarContainer"
```

----------------------------------------

TITLE: Implementing Db2 Container Test in C#
DESCRIPTION: Example class showing how to create and use a Db2 container in a unit test. Implements IAsyncLifetime to manage container lifecycle, connecting to the database and performing a simple query.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/db2.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Db2.Tests/Db2ContainerTest.cs:UseDb2Container"
```

----------------------------------------

TITLE: Creating Non-Expiring Authentication Token
DESCRIPTION: Code demonstrating how to create an authentication token that never expires from a running Pulsar container.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/pulsar.md#2025-04-22_snippet_8

LANGUAGE: csharp
CODE:
```
var authToken = await container.CreateAuthenticationTokenAsync(Timeout.InfiniteTimeSpan)
    .ConfigureAwait(false);
```

----------------------------------------

TITLE: Using Cassandra Container with xUnit Test
DESCRIPTION: Example of how to create, configure, and use a Cassandra container in a xUnit test. The test demonstrates connecting to Cassandra and performing basic operations within the containerized environment.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/cassandra.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Cassandra.Tests/CassandraContainerTest.cs:UseCassandraContainer"
```

----------------------------------------

TITLE: Running PostgreSQL Container Tests in C#
DESCRIPTION: Example of running tests with a PostgreSQL container, demonstrating how to execute SQL queries against the containerized database.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/test_frameworks/xunit_net.md#2025-04-22_snippet_8

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Xunit.Tests/PostgreSqlContainer.cs:RunTests"
```

----------------------------------------

TITLE: Getting Pulsar Broker URL
DESCRIPTION: Code snippet that shows how to get the Pulsar broker URL from a running container instance.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/pulsar.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
string pulsarBrokerUrl = _pulsarContainer.GetPulsarBrokerUrl();
```

----------------------------------------

TITLE: Waiting for HTTP Status Code Range in Testcontainers .NET
DESCRIPTION: This snippet demonstrates how to use a predicate to wait for an HTTP status code within a specific range (2xx in this case).
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/wait_strategies.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
_ = Wait.ForUnixContainer()
  .UntilHttpRequestIsSucceeded(request => request
    .ForPath("/")
    .ForStatusCodeMatching(statusCode => statusCode >= HttpStatusCode.OK && statusCode < HttpStatusCode.MultipleChoices));
```

----------------------------------------

TITLE: Establishing Connection and Producing Message
DESCRIPTION: Example showing how to connect to the ActiveMQ Artemis container and produce a message.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/activemq.md#2025-04-22_snippet_5

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.ActiveMq.Tests/ArtemisContainerTest.cs:ArtemisContainerEstablishesConnection"
```

----------------------------------------

TITLE: MongoDB Replica Set Configuration
DESCRIPTION: C# code showing how to configure a MongoDB container to run as a single-node replica set instead of standalone instance.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/mongodb.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.MongoDb.Tests/MongoDbContainerTest.cs:ReplicaSetContainerConfiguration"
```

----------------------------------------

TITLE: Configuring ASP.NET Core Container with Environment Variables and Files
DESCRIPTION: Shows how to configure an ASP.NET Core container by setting environment variables for HTTPS URLs and certificate configuration, while also mapping a certificate file into the container.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
_ = new ContainerBuilder()
  .WithEnvironment("ASPNETCORE_URLS", "https://+")
  .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", "/app/certificate.crt")
  .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", "password")
  .WithResourceMapping("certificate.crt", "/app/");
```

----------------------------------------

TITLE: Configuring ActiveMQ Artemis Container with Custom Authentication
DESCRIPTION: Test configuration using custom authentication credentials for ActiveMQ Artemis container.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/activemq.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.ActiveMq.Tests/ArtemisContainerTest.cs:UseArtemisContainerCustomAuth"
```

----------------------------------------

TITLE: Retrieving Container Logs using GetLogsAsync in Testcontainers
DESCRIPTION: Shows how to retrieve stdout and stderr logs from a container after it has started. This approach is useful for post-test analysis of container output.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_7

LANGUAGE: csharp
CODE:
```
var (stdout, stderr) = await _container.GetLogsAsync();
```

----------------------------------------

TITLE: Configuring Bitbucket Pipelines for Testcontainers
DESCRIPTION: YAML configuration for Bitbucket Pipelines that sets up a .NET SDK environment with Docker support. Includes settings to disable Ryuk for Testcontainers compatibility and runs tests in the pipeline.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/cicd/index.md#2025-04-22_snippet_1

LANGUAGE: yaml
CODE:
```
image: mcr.microsoft.com/dotnet/sdk:8.0
options:
  docker: true
pipelines:
  default:
    - step:
      script:
        # Bitbucket Pipelines does not support Ryuk:
        # https://dotnet.testcontainers.org/api/resource_reaper/.
        - export TESTCONTAINERS_RYUK_DISABLED=true
        - dotnet test
      services:
        - docker
```

----------------------------------------

TITLE: Labeling Container Resources for Identification
DESCRIPTION: Shows how to add a distinct label to a reusable container to prevent hash collisions. This ensures unique identification of resources when reuse is enabled.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/resource_reuse.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
_ = new ContainerBuilder()
  .WithReuse(true)
  .WithLabel("reuse-id", "WeatherForecast");
```

----------------------------------------

TITLE: Custom TCP Server Container with Data Verification
DESCRIPTION: An advanced example creating a container that runs a simple TCP server in Alpine Linux. The container is configured to listen on a port and return a magic number, which is then verified by connecting to it with a TCP client.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_10

LANGUAGE: csharp
CODE:
```
const string MagicNumber = "42";

const ushort MagicNumberPort = 80;

var deepThoughtContainer = new ContainerBuilder()
  .WithName(Guid.NewGuid().ToString("D"))
  .WithImage("alpine")
  .WithExposedPort(MagicNumberPort)
  .WithPortBinding(MagicNumberPort, true)
  .WithEnvironment("MAGIC_NUMBER", MagicNumber)
  .WithEntrypoint("/bin/sh", "-c")
  .WithCommand($"while true; do echo \"$MAGIC_NUMBER\" | nc -l -p {MagicNumberPort}; done")
  .Build();

await deepThoughtContainer.StartAsync()
  .ConfigureAwait(false);

using var magicNumberClient = new TcpClient(deepThoughtContainer.Hostname, deepThoughtContainer.GetMappedPublicPort(MagicNumberPort));
using var magicNumberReader = new StreamReader(magicNumberClient.GetStream());

var magicNumber = await magicNumberReader.ReadLineAsync()
  .ConfigureAwait(false);

Assert.Equal(MagicNumber, magicNumber);
```

----------------------------------------

TITLE: Configuring Docker Host with Environment Variable in Testcontainers
DESCRIPTION: Sets the Docker host using an environment variable to connect to a remote Docker daemon running on tcp://docker:2375.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/custom_configuration/index.md#2025-04-22_snippet_0

LANGUAGE: properties
CODE:
```
DOCKER_HOST=tcp://docker:2375
```

----------------------------------------

TITLE: Configuring Docker Host with Properties File in Testcontainers
DESCRIPTION: Sets the Docker host using a properties file entry to connect to a remote Docker daemon running on tcp://docker:2375.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/custom_configuration/index.md#2025-04-22_snippet_1

LANGUAGE: properties
CODE:
```
docker.host=tcp://docker:2375
```

----------------------------------------

TITLE: Configuring Redis Container for Isolated Testing in C#
DESCRIPTION: Example of configuring a Redis container for isolated testing by inheriting from ContainerTest<TBuilderEntity, TContainerEntity> and overriding the Configure method.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/test_frameworks/xunit_net.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest`1.cs:ConfigureRedisContainer"
```

----------------------------------------

TITLE: Multi-stage Dockerfile with Resource Reaper Session Labels
DESCRIPTION: A multi-stage Dockerfile that properly labels each stage with Resource Reaper session IDs to ensure intermediate layers can be cleaned up after test execution.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_image.md#2025-04-22_snippet_3

LANGUAGE: Dockerfile
CODE:
```
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env-1
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env-2
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
```

----------------------------------------

TITLE: Installing Testcontainers.MsSql NuGet Package for C#
DESCRIPTION: This command installs the Testcontainers.MsSql NuGet package version 3.0.0, which provides pre-configured Microsoft SQL Server module for Testcontainers.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/examples/aspnet.md#2025-04-22_snippet_2

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.MsSql --version 3.0.0
```

----------------------------------------

TITLE: Configuring Event Hubs Container with Custom Azurite Instance
DESCRIPTION: Example showing how to configure an Event Hubs container to use an existing Azurite container instance instead of creating a new one automatically.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/eventhubs.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.EventHubs.Tests/EventHubsContainerTest.cs:ReuseExistingAzuriteContainer"
```

----------------------------------------

TITLE: Enabling Token Authentication for Pulsar
DESCRIPTION: Code that demonstrates how to configure a Pulsar container with token authentication enabled.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/pulsar.md#2025-04-22_snippet_6

LANGUAGE: csharp
CODE:
```
PulsarContainer _pulsarContainer = PulsarBuilder().WithTokenAuthentication().Build();
```

----------------------------------------

TITLE: Adding WithPassword Method to PostgreSqlBuilder
DESCRIPTION: Implements a WithPassword method in the PostgreSqlBuilder class to set the password and corresponding environment variable.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/index.md#2025-04-22_snippet_7

LANGUAGE: csharp
CODE:
```
public PostgreSqlBuilder WithPassword(string password)
{
  return Merge(DockerResourceConfiguration, new PostgreSqlConfiguration(password: password)).WithEnvironment("POSTGRES_PASSWORD", password);
}
```

----------------------------------------

TITLE: Validating Password Configuration in PostgreSqlContainer
DESCRIPTION: Overrides the Validate method in the PostgreSqlContainer class to ensure that a password is provided and not empty.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/index.md#2025-04-22_snippet_9

LANGUAGE: csharp
CODE:
```
protected override void Validate()
{
  base.Validate();

  _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(PostgreSqlConfiguration.Password))
    .NotNull()
    .NotEmpty();
}
```

----------------------------------------

TITLE: Configuring TLS for Qdrant Container
DESCRIPTION: Code snippet showing how to configure a Qdrant container with TLS using a self-signed certificate and private key.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/qdrant.md#2025-04-22_snippet_5

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Qdrant.Tests/QdrantSecureContainerTest.cs:ConfigureQdrantContainerCertificate"
```

----------------------------------------

TITLE: Exposing Host Ports Configuration
DESCRIPTION: Shows how to expose host ports to containers using TestcontainersSettings. Enables containers to access services running on the test host.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_network.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
await TestcontainersSettings.ExposeHostPortsAsync(8080)
  .ConfigureAwait(false);
```

----------------------------------------

TITLE: Installing and Running WeatherForecast Tests with Testcontainers
DESCRIPTION: Console commands to clone the repository and run tests for the WeatherForecast example. Requires git-lfs and executes tests in Release configuration. Note that one test requires Chrome version 106 for Selenium testing.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/examples/WeatherForecast/README.md#2025-04-22_snippet_0

LANGUAGE: console
CODE:
```
git lfs version
git clone --branch develop git@github.com:testcontainers/testcontainers-dotnet.git
cd ./testcontainers-dotnet/examples/WeatherForecast/
dotnet test WeatherForecast.sln --configuration=Release
```

----------------------------------------

TITLE: Container to Host Communication
DESCRIPTION: Demonstrates how to send a request from a container to the test host using the special hostname 'host.testcontainers.internal'.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_network.md#2025-04-22_snippet_3

LANGUAGE: csharp
CODE:
```
_ = await curlContainer.ExecAsync(new[] { "curl", "http://host.testcontainers.internal:8080" })
  .ConfigureAwait(false);
```

----------------------------------------

TITLE: Canceling Container Start with Timeout in Testcontainers
DESCRIPTION: Demonstrates how to implement a timeout when starting a container by using a CancellationTokenSource that automatically cancels after a specified duration.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_6

LANGUAGE: csharp
CODE:
```
using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
await _container.StartAsync(timeoutCts.Token);
```

----------------------------------------

TITLE: Cloning and Running Testcontainers Flyway Example
DESCRIPTION: Console commands to clone the testcontainers-dotnet repository, navigate to the Flyway example directory, and run the tests in Release configuration.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/examples/Flyway/README.md#2025-04-22_snippet_0

LANGUAGE: console
CODE:
```
git clone --branch develop git@github.com:testcontainers/testcontainers-dotnet.git
cd ./testcontainers-dotnet/examples/Flyway/
dotnet test Flyway.sln --configuration=Release
```

----------------------------------------

TITLE: Installing Testcontainers.PostgreSql NuGet Package
DESCRIPTION: This command adds the Testcontainers.PostgreSql package to a .NET project using the dotnet CLI.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/postgres.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.PostgreSql
```

----------------------------------------

TITLE: Creating Neo4j Container Instance in C#
DESCRIPTION: C# code snippet demonstrating how to create a Neo4j container instance using Testcontainers. This example is likely part of a test setup using xUnit.net's IAsyncLifetime interface.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/neo4j.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Neo4j.Tests/Neo4jContainerTest.cs:CreateNeo4jContainer"
```

----------------------------------------

TITLE: Generating Connection String in PostgreSqlContainer
DESCRIPTION: Implements a GetConnectionString method in the PostgreSqlContainer class to construct a connection string using the configured values.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/index.md#2025-04-22_snippet_8

LANGUAGE: csharp
CODE:
```
public string GetConnectionString()
{
  var properties = new Dictionary<string, string>();
  properties.Add("Host", Hostname);
  properties.Add("Port", GetMappedPublicPort(5432).ToString());
  properties.Add("Database", "postgres");
  properties.Add("Username", "postgres");
  properties.Add("Password", _configuration.Password);
  return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
}
```

----------------------------------------

TITLE: Configuring PostgreSQL Container for Testing in C#
DESCRIPTION: Example of configuring a PostgreSQL container for testing by inheriting from DbContainerTest or DbContainerFixture and overriding the Configure method.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/test_frameworks/xunit_net.md#2025-04-22_snippet_6

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Xunit.Tests/PostgreSqlContainer.cs:ConfigurePostgreSqlContainer"
```

----------------------------------------

TITLE: Listing Docker Contexts with PowerShell
DESCRIPTION: PowerShell command to list available Docker contexts that can be used with Testcontainers, showing context names and their Docker endpoints.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/custom_configuration/index.md#2025-04-22_snippet_2

LANGUAGE: powershell
CODE:
```
PS C:\Sources\dotnet\testcontainers-dotnet> docker context ls
NAME   DESCRIPTION   DOCKER ENDPOINT           ERROR
tcc                  tcp://127.0.0.1:60706/0
```

----------------------------------------

TITLE: Installing Testcontainers Module with .NET CLI
DESCRIPTION: Command for adding a Testcontainers module to a .NET project using the dotnet CLI. This is the first step to use any Testcontainers module in a test project.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/index.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.ModuleName
```

----------------------------------------

TITLE: Copying a Directory to a Container using Testcontainers
DESCRIPTION: Demonstrates how to copy an entire directory from the host filesystem into a Docker container using the WithResourceMapping method with a DirectoryInfo object.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
_ = new ContainerBuilder()
  .WithResourceMapping(new DirectoryInfo("."), "/app/");
```

----------------------------------------

TITLE: Installing Testcontainers.MsSql Package via NuGet
DESCRIPTION: Command to add the Testcontainers.MsSql NuGet package to your .NET project, which provides the MSSQL container implementation for Testcontainers.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/mssql.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.MsSql
```

----------------------------------------

TITLE: Resolving Solution Directory for Dockerfile Context
DESCRIPTION: Example showing how to detect the directory containing the solution file by traversing up the directory tree from the executing assembly.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_image.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
_ = new ImageFromDockerfileBuilder()
  .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
  .WithDockerfile("Dockerfile");
```

----------------------------------------

TITLE: Configuring Azure Service Bus Emulator with Custom Configuration
DESCRIPTION: Example showing how to provide a custom JSON configuration file to the Azure Service Bus emulator container.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/servicebus.md#2025-04-22_snippet_5

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.ServiceBus.Tests/ServiceBusContainerTest.cs:UseCustomConfiguration"
```

----------------------------------------

TITLE: Specifying NuGet Package References for PostgreSQL Tests
DESCRIPTION: This XML snippet shows the package references required for running PostgreSQL container tests, including Testcontainers.PostgreSql and xunit dependencies.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/postgres.md#2025-04-22_snippet_2

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.PostgreSql.Tests/Testcontainers.PostgreSql.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: Package References for MongoDB Testing
DESCRIPTION: XML configuration showing the required NuGet package references for MongoDB container testing.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/mongodb.md#2025-04-22_snippet_3

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.MongoDb.Tests/Testcontainers.MongoDb.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: Injecting Shared Redis Container Fixture in C#
DESCRIPTION: Example of injecting a shared Redis container fixture into a test class by implementing IClassFixture<TFixture>.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/test_frameworks/xunit_net.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest`2.cs:InjectContainerFixture"
```

----------------------------------------

TITLE: Configuring Qdrant Client for TLS Verification
DESCRIPTION: Two-part example demonstrating how to configure a Qdrant client to validate the TLS certificate using its thumbprint.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/qdrant.md#2025-04-22_snippet_6

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Qdrant.Tests/QdrantSecureContainerTest.cs:ConfigureQdrantClientCertificate-1"

--8<-- "tests/Testcontainers.Qdrant.Tests/QdrantSecureContainerTest.cs:ConfigureQdrantClientCertificate-2"
```

----------------------------------------

TITLE: Required NuGet Package References for Db2 Tests
DESCRIPTION: XML package references needed for Db2 container tests. These include the IBM.Data.Db2 client libraries and testing frameworks.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/db2.md#2025-04-22_snippet_2

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.Db2.Tests/Testcontainers.Db2.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: Required NuGet Package References for Qdrant Tests
DESCRIPTION: XML snippet showing the required package references for the Qdrant test project, including dependencies needed to interact with Qdrant containers.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/qdrant.md#2025-04-22_snippet_2

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.Qdrant.Tests/Testcontainers.Qdrant.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: PostgreSQL Module Initialization
DESCRIPTION: Implementation of the Init method to configure PostgreSQL-specific settings including Docker image and wait strategy.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/index.md#2025-04-22_snippet_5

LANGUAGE: csharp
CODE:
```
protected override PostgreSqlBuilder Init()
{
  var waitStrategy = Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready");
  return base.Init().WithImage("postgres:15.1").WithPortBinding(5432, true).WithWaitStrategy(waitStrategy);
}
```

----------------------------------------

TITLE: Reading a File from a Container using Testcontainers
DESCRIPTION: Shows how to read a file from a running container into a byte array and then optionally save it to the host filesystem using the ReadFileAsync method.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_5

LANGUAGE: csharp
CODE:
```
var readBytes = await container.ReadFileAsync("/app/appsettings.json")
  .ConfigureAwait(false);

await File.WriteAllBytesAsync("appsettings.json", readBytes)
  .ConfigureAwait(false);
```

----------------------------------------

TITLE: Required NuGet Package References
DESCRIPTION: XML configuration showing required NuGet package references for the test project.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/activemq.md#2025-04-22_snippet_6

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.ActiveMq.Tests/Testcontainers.ActiveMq.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: Creating Authentication Token with Expiration
DESCRIPTION: Code showing how to create an authentication token with a specified expiration time from a running Pulsar container.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/pulsar.md#2025-04-22_snippet_7

LANGUAGE: csharp
CODE:
```
var authToken = await container.CreateAuthenticationTokenAsync(TimeSpan.FromHours(1))
    .ConfigureAwait(false);
```

----------------------------------------

TITLE: Configuring API Key for Qdrant Container
DESCRIPTION: Code snippet demonstrating how to configure an API key for a Qdrant container to enable authentication.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/qdrant.md#2025-04-22_snippet_3

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Qdrant.Tests/QdrantSecureContainerTest.cs:ConfigureQdrantContainerApiKey"
```

----------------------------------------

TITLE: Installing Testcontainers.EventHubs NuGet Package
DESCRIPTION: Command to add the Testcontainers.EventHubs package to a .NET project using the dotnet CLI.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/eventhubs.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.EventHubs
```

----------------------------------------

TITLE: Accessing Docker API for Advanced Container Configuration
DESCRIPTION: Shows how to use the CreateParameterModifier to access underlying Docker API properties for specific configurations not exposed by Testcontainers API.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/best_practices.md#2025-04-22_snippet_5

LANGUAGE: csharp
CODE:
```
_builder.WithCreateParameterModifier(Action<TCreateResourceEntity>)
```

----------------------------------------

TITLE: Dockerfile with Explicit User Permissions
DESCRIPTION: Example Dockerfile showing how to properly set permissions when switching user context, addressing a known issue in Testcontainers for .NET.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_image.md#2025-04-22_snippet_5

LANGUAGE: Dockerfile
CODE:
```
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
RUN chown app:app .
USER app
```

----------------------------------------

TITLE: Installing Testcontainers.ActiveMq NuGet Package
DESCRIPTION: Command to add the Testcontainers.ActiveMq package to a .NET project using the dotnet CLI.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/activemq.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.ActiveMq
```

----------------------------------------

TITLE: Installing Testcontainers.Elasticsearch NuGet Package
DESCRIPTION: Command to add the Testcontainers.Elasticsearch package to your .NET project using the dotnet CLI.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/elasticsearch.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.Elasticsearch
```

----------------------------------------

TITLE: Configuring Base ActiveMQ Artemis Container Test
DESCRIPTION: Base test class setup for ActiveMQ Artemis container using IAsyncLifetime interface.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/activemq.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.ActiveMq.Tests/ArtemisContainerTest.cs:UseArtemisContainer"
}
```

----------------------------------------

TITLE: Getting Pulsar HTTP Service URL
DESCRIPTION: Code snippet that shows how to get the Pulsar HTTP service URL from a running container instance.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/pulsar.md#2025-04-22_snippet_5

LANGUAGE: csharp
CODE:
```
string pulsarServiceUrl = _pulsarContainer.GetHttpServiceUrl();
```

----------------------------------------

TITLE: Installing Testcontainers.Cassandra NuGet Package
DESCRIPTION: Command to add the Testcontainers.Cassandra package to a .NET project using the dotnet CLI.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/cassandra.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.Cassandra
```

----------------------------------------

TITLE: Adding Redis Configuration Source in WebApplicationFactory for C#
DESCRIPTION: This snippet shows a more complex approach to configure Redis in WebApplicationFactory. It implements IConfigurationSource and ConfigurationProvider to automatically start the Redis service before creating the TestServer.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/examples/aspnet.md#2025-04-22_snippet_1

LANGUAGE: csharp
CODE:
```
private sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureAppConfiguration(configure =>
    {
      configure.Add(new RedisConfigurationSource());
    });
  }
}

private sealed class RedisConfigurationSource : IConfigurationSource
{
  public IConfigurationProvider Build(IConfigurationBuilder builder)
  {
    return new RedisConfigurationProvider();
  }
}

private sealed class RedisConfigurationProvider : ConfigurationProvider
{
  private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

  public override void Load()
  {
    // Until the asynchronous configuration provider is available,
    // we use the TaskFactory to spin up a new task that handles the work:
    // https://github.com/dotnet/runtime/issues/79193
    // https://github.com/dotnet/runtime/issues/36018
    TaskFactory.StartNew(LoadAsync)
      .Unwrap()
      .ConfigureAwait(false)
      .GetAwaiter()
      .GetResult();
  }

  public async Task LoadAsync()
  {
    var redisContainer = new RedisBuilder().Build();

    await redisContainer.StartAsync()
      .ConfigureAwait(false);

    Set("ConnectionStrings:RedisCache", redisContainer.GetConnectionString());
  }
}
```

----------------------------------------

TITLE: Configuring Qdrant Client with API Key
DESCRIPTION: Example showing how to configure a Qdrant client to include the API key in requests to the Qdrant container.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/qdrant.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Qdrant.Tests/QdrantSecureContainerTest.cs:ConfigureQdrantClientApiKey"
```

----------------------------------------

TITLE: Using Docker Compose for Documentation Preview
DESCRIPTION: Command to start a Docker container for previewing the documentation locally using Docker Compose. The documentation will be accessible at http://localhost:8000.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/contributing_docs.md#2025-04-22_snippet_1

LANGUAGE: shell
CODE:
```
docker compose up
```

----------------------------------------

TITLE: Passing Resource Reaper Session ID to Docker Build
DESCRIPTION: Demonstrates how to pass the Resource Reaper session ID as a build argument to ensure proper cleanup of intermediate layers in multi-stage builds.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_image.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
_ = new ImageFromDockerfileBuilder()
  .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"));
```

----------------------------------------

TITLE: Configuring Redis Container for Shared Testing in C#
DESCRIPTION: Example of configuring a Redis container for shared testing by inheriting from ContainerFixture<TBuilderEntity, TContainerEntity> and implementing IMessageSink.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/test_frameworks/xunit_net.md#2025-04-22_snippet_3

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest`2.cs:ConfigureRedisContainer"
```

----------------------------------------

TITLE: Configuring DbProviderFactory for PostgreSQL in C#
DESCRIPTION: Example of implementing the abstract DbProviderFactory property to resolve a compatible DbProviderFactory for PostgreSQL.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/test_frameworks/xunit_net.md#2025-04-22_snippet_7

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Xunit.Tests/PostgreSqlContainer.cs:ConfigureDbProviderFactory"
```

----------------------------------------

TITLE: Copying Byte Array Content to a Container File using Testcontainers
DESCRIPTION: Demonstrates how to create a file in a Docker container from a byte array in memory, useful for dynamically generated content or when file content is already available in memory.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/api/create_docker_container.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
_ = new ContainerBuilder()
  .WithResourceMapping(Encoding.Default.GetBytes("{}"), "/app/appsettings.json");
```

----------------------------------------

TITLE: GitHub Sponsorship Information in Markdown
DESCRIPTION: Describes the GitHub Sponsors program for Testcontainers, including the bounty system for issues, using Markdown syntax.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/CONTRIBUTING.md#2025-04-22_snippet_1

LANGUAGE: Markdown
CODE:
```
### GitHub Sponsorship

Testcontainers is [in the GitHub Sponsors program](https://github.com/sponsors/testcontainers)!

This repository is supported by our sponsors, meaning that issues are eligible to have a 'bounty' attached to them by sponsors.

Please see [the bounty policy page](https://www.testcontainers.org/bounty) if you are interested, either as a sponsor or as a contributor.
```

----------------------------------------

TITLE: Configuring ActiveMQ Artemis Container Without Authentication
DESCRIPTION: Test configuration for ActiveMQ Artemis container without authentication settings.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/activemq.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.ActiveMq.Tests/ArtemisContainerTest.cs:UseArtemisContainerNoAuth"
```

----------------------------------------

TITLE: Running Isolated Redis Container Tests in C#
DESCRIPTION: Example of running tests with isolated Redis containers, demonstrating that each test gets a new container instance.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/test_frameworks/xunit_net.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Xunit.Tests/RedisContainerTest`1.cs:RunTests"
```

----------------------------------------

TITLE: Configuring ActiveMQ Artemis Container with Default Authentication
DESCRIPTION: Test configuration using default authentication credentials for ActiveMQ Artemis container.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/activemq.md#2025-04-22_snippet_3

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.ActiveMq.Tests/ArtemisContainerTest.cs:UseArtemisContainerDefaultAuth"
```

----------------------------------------

TITLE: Configuring Docker-in-Docker for GitLab CI/CD
DESCRIPTION: YAML configuration for enabling Docker-in-Docker service in GitLab CI/CD pipelines. Sets up the Docker service and configures the Docker host address for container access.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/cicd/index.md#2025-04-22_snippet_0

LANGUAGE: yaml
CODE:
```
services:
  - docker:dind
variables:
  DOCKER_HOST: tcp://docker:2375
```

----------------------------------------

TITLE: Implementing Password Property in PostgreSqlConfiguration
DESCRIPTION: Adds a Password property to the PostgreSqlConfiguration class, including constructors for individual and merged configurations.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/index.md#2025-04-22_snippet_6

LANGUAGE: csharp
CODE:
```
public PostgreSqlConfiguration(string password = null)
{
  Password = password;
}

public PostgreSqlConfiguration(PostgreSqlConfiguration oldValue, PostgreSqlConfiguration newValue)
  : base(oldValue, newValue)
{
  Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
}

public string Password { get; }
```

----------------------------------------

TITLE: Required Package References
DESCRIPTION: XML configuration showing the required NuGet package references for RabbitMQ container testing.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/rabbitmq.md#2025-04-22_snippet_2

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.RabbitMq.Tests/Testcontainers.RabbitMq.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: Installing Testcontainers NuGet Package in .NET
DESCRIPTION: Command to add the Testcontainers NuGet package to a .NET project using the dotnet CLI. This is the first step to start using Testcontainers in a .NET application.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/index.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers
```

----------------------------------------

TITLE: Enabling Pulsar Functions
DESCRIPTION: Code snippet showing how to enable Pulsar Functions when creating a Pulsar container instance.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/pulsar.md#2025-04-22_snippet_9

LANGUAGE: csharp
CODE:
```
PulsarContainer _pulsarContainer = PulsarBuilder().WithFunctions().Build();
```

----------------------------------------

TITLE: Required NuGet Package References for MSSQL Testing
DESCRIPTION: XML package references needed for MSSQL container testing, including dependencies for Testcontainers.MsSql, xUnit, and other testing utilities.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/mssql.md#2025-04-22_snippet_3

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.MsSql.Tests/Testcontainers.MsSql.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: Required NuGet Package References for Azure Service Bus Tests
DESCRIPTION: XML snippet showing the necessary package references needed for testing with Azure Service Bus emulator in a .NET project.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/servicebus.md#2025-04-22_snippet_3

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.ServiceBus.Tests/Testcontainers.ServiceBus.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: Installing Testcontainers .NET Template
DESCRIPTION: Commands to clone the repository and install the .NET template for creating new Testcontainers modules.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/index.md#2025-04-22_snippet_2

LANGUAGE: shell
CODE:
```
git clone --branch develop git@github.com:testcontainers/testcontainers-dotnet.git
cd ./testcontainers-dotnet/
dotnet new --install ./src/Templates
```

----------------------------------------

TITLE: Package References for Cassandra Tests
DESCRIPTION: XML snippet showing the required NuGet package references for running Cassandra container tests, including the necessary dependencies for the test project.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/cassandra.md#2025-04-22_snippet_2

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.Cassandra.Tests/Testcontainers.Cassandra.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: Installing Testcontainers.MongoDB NuGet Package
DESCRIPTION: Command to add the Testcontainers.MongoDB package to a .NET project using the dotnet CLI.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/mongodb.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.MongoDb
```

----------------------------------------

TITLE: Specifying NuGet Package References for Neo4j Tests
DESCRIPTION: XML snippet from a .NET project file showing the required NuGet package references for Neo4j Testcontainer tests.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/neo4j.md#2025-04-22_snippet_3

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.Neo4j.Tests/Testcontainers.Neo4j.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: Installing Testcontainers.Db2 NuGet Package
DESCRIPTION: Command to add the Testcontainers.Db2 package to a .NET project via NuGet. This package is required to create and manage Db2 containers for testing.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/db2.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.Db2
```

----------------------------------------

TITLE: Installing RabbitMQ Testcontainers Package
DESCRIPTION: Command to add the Testcontainers.RabbitMq NuGet package to a .NET project using the dotnet CLI.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/rabbitmq.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.RabbitMq
```

----------------------------------------

TITLE: Installing Testcontainers.Xunit Package via NuGet in .NET
DESCRIPTION: Command to add the Testcontainers.Xunit package to a .NET project using the dotnet CLI.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/test_frameworks/xunit_net.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.Xunit
```

----------------------------------------

TITLE: Installing Testcontainers.ServiceBus NuGet Package
DESCRIPTION: Command to add the Testcontainers.ServiceBus package to your .NET project using the dotnet CLI.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/servicebus.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.ServiceBus
```

----------------------------------------

TITLE: Installing Testcontainers.Neo4j NuGet Package
DESCRIPTION: Command to add the Testcontainers.Neo4j package to a .NET project using the dotnet CLI.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/neo4j.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.Neo4j
```

----------------------------------------

TITLE: Installing Testcontainers.Qdrant NuGet Package
DESCRIPTION: Command to add the Testcontainers.Qdrant package to a .NET project, which provides the necessary functionality to create and manage Qdrant containers for testing.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/qdrant.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.Qdrant
```

----------------------------------------

TITLE: Running Docker Tests with Sibling Containers (Docker CLI)
DESCRIPTION: Command to run .NET tests in a Docker container using the Docker Wormhole pattern, which mounts the Docker socket to allow container creation from within the test container.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/dind/index.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
docker run -v /var/run/docker.sock.raw:/var/run/docker.sock $IMAGE dotnet test
```

----------------------------------------

TITLE: Installing Testcontainers.Pulsar NuGet Package
DESCRIPTION: Command to add the Testcontainers.Pulsar package to a .NET project using the dotnet CLI.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/pulsar.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet add package Testcontainers.Pulsar
```

----------------------------------------

TITLE: Creating PostgreSQL Module
DESCRIPTION: CLI commands to create and add a new PostgreSQL module to the Testcontainers solution.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/index.md#2025-04-22_snippet_3

LANGUAGE: shell
CODE:
```
dotnet new tcm --name PostgreSql --official-module true --output ./src
dotnet sln add ./src/Testcontainers.PostgreSql/Testcontainers.PostgreSql.csproj
```

----------------------------------------

TITLE: Setting Docker Context with Environment Variable in Testcontainers
DESCRIPTION: Sets the Docker context to 'tcc' using an environment variable, directing Testcontainers to use the specified context for container operations.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/custom_configuration/index.md#2025-04-22_snippet_3

LANGUAGE: properties
CODE:
```
DOCKER_CONTEXT=tcc
```

----------------------------------------

TITLE: Setting Docker Context with Properties File in Testcontainers
DESCRIPTION: Sets the Docker context to 'tcc' using a properties file entry, directing Testcontainers to use the specified context for container operations.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/custom_configuration/index.md#2025-04-22_snippet_4

LANGUAGE: properties
CODE:
```
docker.context=tcc
```

----------------------------------------

TITLE: Package References for Pulsar Tests
DESCRIPTION: XML snippet showing the NuGet package dependencies required for testing with Testcontainers.Pulsar.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/pulsar.md#2025-04-22_snippet_3

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.Pulsar.Tests/Testcontainers.Pulsar.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: Required NuGet Package References for Event Hubs Testing
DESCRIPTION: XML snippet showing the necessary NuGet package references needed for testing with Testcontainers.EventHubs in a project file.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/eventhubs.md#2025-04-22_snippet_3

LANGUAGE: xml
CODE:
```
--8<-- "tests/Testcontainers.EventHubs.Tests/Testcontainers.EventHubs.Tests.csproj:PackageReferences"
```

----------------------------------------

TITLE: Running the Respawn Example with .NET Test Command
DESCRIPTION: Console commands to clone the testcontainers-dotnet repository, navigate to the Respawn example directory, and execute the tests using the dotnet CLI. This demonstrates how to run the example that uses Respawn to reset PostgreSQL database state between tests.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/examples/Respawn/README.md#2025-04-22_snippet_0

LANGUAGE: bash
CODE:
```
git clone --branch develop git@github.com:testcontainers/testcontainers-dotnet.git
cd ./testcontainers-dotnet/examples/Respawn/
dotnet test Respawn.sln --configuration=Release
```

----------------------------------------

TITLE: Building and Testing the Testcontainers-dotnet Project with Cake
DESCRIPTION: Command for building and testing the testcontainers-dotnet project using Cake build automation system. This command verifies that all tests are passing before submitting a contribution.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/contributing.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet cake
```

----------------------------------------

TITLE: Setting up Python Environment for MkDocs Documentation
DESCRIPTION: Instructions for setting up a Python virtual environment and installing dependencies for the Testcontainers .NET documentation. Requires Python 3.8.0 or higher to install dependencies and serve the documentation locally with auto-updates.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/contributing_docs.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
pip install -r requirements.txt
mkdocs serve
```

----------------------------------------

TITLE: Linking to Contribution Guidelines in Markdown
DESCRIPTION: Provides links to the main contributing guidelines and documentation for contributing documentation changes using Markdown syntax.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/CONTRIBUTING.md#2025-04-22_snippet_0

LANGUAGE: Markdown
CODE:
```
# Contributing

Please see the [main contributing guidelines](./docs/contributing.md).

There are additional docs describing [contributing documentation changes](./docs/contributing_docs.md).
```

----------------------------------------

TITLE: Using Neo4j Container in C# Test
DESCRIPTION: C# code snippet showing how to use a Neo4j container in a test scenario. This example demonstrates connecting to the Neo4j instance and performing operations.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/neo4j.md#2025-04-22_snippet_2

LANGUAGE: csharp
CODE:
```
--8<-- "tests/Testcontainers.Neo4j.Tests/Neo4jContainerTest.cs:UseNeo4jContainer"
```

----------------------------------------

TITLE: Specifying Python Package Dependencies for Testcontainers .NET Documentation
DESCRIPTION: This snippet lists the required Python packages and their versions for building the Testcontainers .NET documentation. It includes MkDocs, a Markdown plugin, and the Material theme for MkDocs.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/requirements.txt#2025-04-22_snippet_0

LANGUAGE: plaintext
CODE:
```
mkdocs==1.6.1
mkdocs-markdownextradata-plugin==0.2.6
mkdocs-material==8.5.11
```

----------------------------------------

TITLE: Installing and Using Testcontainers .NET Module Template
DESCRIPTION: Commands to install the Testcontainers .NET module template and scaffold a new module. The template needs to be installed first, then a new module can be created by specifying a name and output directory.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/src/Templates/README.md#2025-04-22_snippet_0

LANGUAGE: shell
CODE:
```
dotnet new --install ./src/Templates
```

LANGUAGE: shell
CODE:
```
dotnet new tcm --name ${module_name} --output ${output_directory}
```

----------------------------------------

TITLE: PostgreSQL Builder Configuration
DESCRIPTION: Implementation of the PostgreSQL builder class constructors and configuration property setup in C#.
SOURCE: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/docs/modules/index.md#2025-04-22_snippet_4

LANGUAGE: csharp
CODE:
```
public PostgreSqlBuilder()
  : this(new PostgreSqlConfiguration())
{
  DockerResourceConfiguration = Init().DockerResourceConfiguration;
}

private PostgreSqlBuilder(PostgreSqlConfiguration resourceConfiguration)
  : base(resourceConfiguration)
{
  DockerResourceConfiguration = resourceConfiguration;
}

protected override PostgreSqlConfiguration DockerResourceConfiguration { get; }
```