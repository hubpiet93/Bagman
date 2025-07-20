TITLE: WCF API References
DESCRIPTION: Key .NET attributes used for defining service contracts and operations in Windows Communication Foundation (WCF) applications. These attributes are fundamental for exposing service functionality.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/wcf/getting-started-tutorial.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.ServiceModel.ServiceContractAttribute:
  Description: Specifies that an interface or a class defines a service contract in a Windows Communication Foundation (WCF) application.
  Namespace: System.ServiceModel

System.ServiceModel.OperationContractAttribute:
  Description: Specifies that a method defines an operation that is part of a service contract in a Windows Communication Foundation (WCF) application.
  Namespace: System.ServiceModel
```

----------------------------------------

TITLE: Example appsettings.json for HTTP Resilience Options
DESCRIPTION: Provides an example `appsettings.json` configuration file demonstrating how to define options for HTTP retry strategies, which can be dynamically reloaded by the application.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/resilience/http-resilience.md#_snippet_9

LANGUAGE: json
CODE:
```
{
  "RetryOptions": {
    "MaxRetryAttempts": 3,
    "Delay": "00:00:00.5",
    "BackoffType": "Exponential",
    "UseJitter": true
  }
}
```

----------------------------------------

TITLE: XmlDictionaryReaderQuotas API Reference
DESCRIPTION: Details the configurable quota values for XML dictionary readers, including their purpose and default settings.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/wcf/wcf-simplification-features.md#_snippet_2

LANGUAGE: APIDOC
CODE:
```
XmlDictionaryReaderQuotas:
  Description: Contains configurable quota values for XML dictionary readers which limit the amount of memory utilized by an encoder while creating a message.
  Properties:
    MaxArrayLength:
      Type: Int32.MaxValue (Default)
      Description: Gets and sets the maximum allowed array length. This quota limits the maximum size of an array of primitives that the XML reader returns, including byte arrays. This quota does not limit memory consumption in the XML reader itself, but in whatever component that is using the reader. For example, when the DataContractSerializer uses a reader secured with MaxArrayLength, it does not deserialize byte arrays larger than this quota.
    MaxBytesPerRead:
      Type: Int32.MaxValue (Default)
      Description: Gets and sets the maximum allowed bytes returned for each read. This quota limits the number of bytes that are read in a single Read operation when reading the element start tag and its attributes. (In non-streamed cases, the element name itself is not counted against the quota). Having too many XML attributes may use up disproportionate processing time because attribute names have to be checked for uniqueness. MaxBytesPerRead mitigates this threat.
    MaxDepth:
      Type: 128 nodes deep (Default)
      Description: This quota limits the maximum nesting depth of XML elements. MaxDepth interacts with MaxBytesPerRead: the reader always keeps data in memory for the current element and all of its ancestors, so the maximum memory consumption of the reader is proportional to the product of these two settings. When deserializing a deeply-nested object graph, the deserializer is forced to access the entire stack and throw an unrecoverable StackOverflowException. A direct correlation exists between XML nesting and object nesting for both the DataContractSerializer and the XmlSerializer. MaxDepth is used to mitigate this threat.
    MaxNameTableCharCount:
      Type: Int32.MaxValue (Default)
      Description: This quota limits the maximum number of characters allowed in a nametable. The nametable contains certain strings (such as namespaces and prefixes) that are encountered when processing an XML document. As these strings are buffered in memory, this quota is used to prevent excessive buffering when streaming is expected.
    MaxStringContentLength:
      Type: Int32.MaxValue (Default)
      Description: This quota limits the maximum string size that the XML reader returns. This quota does not limit memory consumption in the XML reader itself, but in the component that is using the reader. For example, when the DataContractSerializer uses a reader secured with MaxStringContentLength, it does not deserialize strings larger than this quota.
```

----------------------------------------

TITLE: WCF vs. ASP.NET Web API Feature Comparison
DESCRIPTION: Compares the major features of WCF and ASP.NET Web API, highlighting their differences in transport protocols, message encodings, supported standards, message exchange patterns, and client accessibility to guide technology selection for service development.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/wcf/wcf-and-aspnet-web-api.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
WCF Features:
- Enables building services that support multiple transport protocols (HTTP, TCP, UDP, and custom transports) and allows switching between them.
- Enables building services that support multiple encodings (Text, MTOM, and Binary) of the same message type and allows switching between them.
- Supports building services with WS-* standards like Reliable Messaging, Transactions, Message Security.
- Supports Request-Reply, One Way, and Duplex message exchange patterns.
- WCF SOAP services can be described in WSDL allowing automated tools to generate client proxies even for services with complex schemas.
- Ships with the .NET Framework.
- Provides some support for writing REST-style services, but ASP.NET Web API offers more complete support and future REST feature improvements.
- Recommended for existing WCF services that need to expose additional REST endpoints (using System.ServiceModel.WebHttpBinding).

ASP.NET Web API Features:
- HTTP only. First-class programming model for HTTP.
- More suitable for access from various browsers, mobile devices etc enabling wide reach.
- Enables building Web APIs that support wide variety of media types including XML, JSON etc.
- Uses basic protocol and formats such as HTTP, WebSockets, SSL, JSON, and XML. There is no support for higher level protocols such as Reliable Messaging or Transactions.
- HTTP is request/response but additional patterns can be supported through SignalR and WebSockets integration.
- There is a variety of ways to describe a Web API ranging from auto-generated HTML help page describing snippets to structured metadata for OData integrated APIs.
- Ships with .NET Framework but is open-source and is also available out-of-band as independent download.
- Ideal platform for building RESTful applications on the .NET Framework.
- Recommended for creating and designing new REST-style services.
```

----------------------------------------

TITLE: Parameters and Arguments
DESCRIPTION: Learn about language support for defining parameters and passing arguments to functions, methods, and properties. It includes information about how to pass by reference.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/language-reference/index.md#_snippet_36

LANGUAGE: APIDOC
CODE:
```
Parameters and Arguments:
  Description: Language support for defining parameters and passing arguments to functions, methods, and properties, including pass by reference.
```

----------------------------------------

TITLE: APIDOC: Book Class Reference
DESCRIPTION: Detailed API documentation for the `Book` class, including its constructors, properties, and methods, along with their descriptions, parameters, and return types.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/fundamentals/tutorials/inheritance.md#_snippet_11

LANGUAGE: APIDOC
CODE:
```
Class: Book : Publication
  Description: Represents a book, inheriting from Publication and adding specific book-related properties and behaviors.

  Constructors:
    Book(string title, string publisher, string author)
      Description: Initializes a new instance of the Book class without an ISBN. Uses constructor chaining to call the full constructor with a null ISBN.
      Parameters:
        title (string): The title of the book. Passed to the base Publication constructor.
        publisher (string): The publisher of the book. Passed to the base Publication constructor.
        author (string): The author of the book. Stored in the Author property.

    Book(string title, string publisher, string author, string isbn)
      Description: Initializes a new instance of the Book class with all details, including ISBN. Calls the base Publication constructor.
      Parameters:
        title (string): The title of the book. Passed to the base Publication constructor.
        publisher (string): The publisher of the book. Passed to the base Publication constructor.
        author (string): The author of the book. Stored in the Author property.
        isbn (string): The International Standard Book Number (10- or 13-digit). Stored in the ISBN auto-property.

  Properties:
    ISBN (string) [read-only]
      Description: Gets the International Standard Book Number of the book. This is a unique 10- or 13-digit number supplied during construction.
    Author (string) [read-only]
      Description: Gets the author's name for the book. Supplied during construction.
    Price (decimal) [read-only externally, settable internally]
      Description: Gets the price of the book. Set via the SetPrice method.
    Currency (string) [read-only externally, settable internally]
      Description: Gets the three-digit ISO currency symbol for the book's price (e.g., "USD"). Set via the SetPrice method.

  Methods:
    SetPrice(decimal price, string currency) : void
      Description: Sets the price and currency for the book.
      Parameters:
        price (decimal): The price value.
        currency (string): The three-digit ISO currency symbol.

    ToString() : string [override]
      Description: Returns a string that represents the current Book object. Overrides Publication.ToString().

    Equals(object obj) : bool [override]
      Description: Determines whether the specified object is equal to the current Book object. Equality is based on the ISBN property. Overrides Object.Equals().
      Parameters:
        obj (object): The object to compare with the current object.
      Returns:
        bool: true if the specified object is equal to the current object; otherwise, false.

    GetHashCode() : int [override]
      Description: Serves as the default hash function. Returns a hash code consistent with the Equals method, based on the ISBN property. Overrides Object.GetHashCode().
      Returns:
        int: A hash code for the current object.
```

----------------------------------------

TITLE: Define In-Memory API Resources for IdentityServer in C#
DESCRIPTION: This C# method, `GetApis`, provides an `IEnumerable<ApiResource>` collection to define API resources that IdentityServer will protect. It specifies 'orders' and 'basket' as API scopes, requiring IdentityServer-managed access tokens for calls to these services.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/authentication-and-authorization.md#_snippet_2

LANGUAGE: csharp
CODE:
```
public static IEnumerable<ApiResource> GetApis()
{
    return new List<ApiResource>
    {
        new ApiScope("orders", "Orders Service"),
        new ApiScope("basket", "Basket Service"),
        new ApiScope("webhooks", "Webhooks registration Service")
    };
}
```

----------------------------------------

TITLE: F# MiniCsvProvider User API Example
DESCRIPTION: Demonstrates how users would interact with the `MiniCsvProvider` to load data from a CSV file and access its properties. It shows instantiation of the provided type and iteration over data rows to extract specific fields like 'Time'.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/tutorials/type-providers/creating-a-type-provider.md#_snippet_38

LANGUAGE: F#
CODE:
```
let info = new MiniCsv<"info.csv">()
for row in info.Data do
let time = row.Time
printfn "${float time}"
```

----------------------------------------

TITLE: Microsoft.DotNet.ApiCompat.Tool Commands and Options Reference
DESCRIPTION: Detailed reference for the command-line interface of the Microsoft.DotNet.ApiCompat.Tool, outlining available commands and options for both general use and specific assembly or package comparisons.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/apicompat/global-tool.md#_snippet_2

LANGUAGE: APIDOC
CODE:
```
Commands:
  package | --package <PACKAGE_FILE>
    Description: Validates the compatibility of package assets. If unspecified, the tool compares assemblies.
    Parameters:
      <PACKAGE_FILE>: Specifies the path to the package file.

Options:
  General Options:
    --version
      Description: Shows version information.
    --generate-suppression-file
      Description: Generates a compatibility suppression file.
    --preserve-unnecessary-suppressions
      Description: Preserves unnecessary suppressions when regenerating the suppression file.
      Details: When an existing suppression file is regenerated, its content is read, deserialized into a set of suppressions, and then stored in a list. Some of the suppressions might no longer be necessary if the incompatibility has been fixed. When the suppressions are serialized back to disk, you can choose to keep *all* the existing (deserialized) expressions by specifying this option.
    --permit-unnecessary-suppressions
      Description: Permits unnecessary suppressions in the suppression file.
    --suppression-file <FILE>
      Description: Specifies the path to one or more suppression files to read from.
    --suppression-output-file <FILE>
      Description: Specifies the path to a suppression file to write to when `--generate-suppression-file` is specified. If unspecified, the first `--suppression-file` path is used.
    --noWarn <NOWARN_STRING>
      Description: Specifies the diagnostic IDs to suppress.
      Example: "$(NoWarn);PKV0001"
    --respect-internals
      Description: Checks both `internal` and `public` APIs.
    --roslyn-assemblies-path <FILE>
      Description: Specifies the path to the directory that contains the Microsoft.CodeAnalysis assemblies you want to use. You only need to set this property if you want to test with a newer compiler than what's in the SDK.
    -v, --verbosity [high|low|normal]
      Description: Controls the log level verbosity.
      Allowed Values: high, normal, low
      Default: normal
    --enable-rule-cannot-change-parameter-name
      Description: Enables the rule that checks whether parameter names have changed in public methods.
    --enable-rule-attributes-must-match
      Description: Enables the rule that checks if attributes match.
    --exclude-attributes-file <FILE>
      Description: Specifies the path to one or more files that contain attributes to exclude in DocId format.

  Assembly-Specific Options:
    -l, --left, --left-assembly <PATH>
      Description: Specifies the path to one or more assemblies that serve as the *left side* to compare. This option is required when comparing assemblies.
    -r, --right, --right-assembly <PATH>
      Description: Specifies the path to one or more assemblies that serve as the *right side* to compare. This option is required when comparing assemblies.
    --strict-mode
      Description: Performs API compatibility checks in strict mode.
```

----------------------------------------

TITLE: Making Authenticated API POST Request in C#
DESCRIPTION: This C# method, `CreateOrderAsync`, demonstrates how to make an authenticated POST request to an API. It retrieves an access token using `_identityService.GetAuthTokenAsync()` and includes it in the request headers via `_requestProvider.PostAsync()`, ensuring the API can only be accessed with a valid token.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/authentication-and-authorization.md#_snippet_16

LANGUAGE: csharp
CODE:
```
public async Task CreateOrderAsync(Models.Orders.Order newOrder)
{
    var authToken = await _identityService.GetAuthTokenAsync().ConfigureAwait(false);

    if (string.IsNullOrEmpty(authToken))
    {
        return;
    }

    var uri = $"{UriHelper.CombineUri(_settingsService.GatewayOrdersEndpointBase, ApiUrlBase)}?api-version=1.0";

    var success = await _requestProvider.PostAsync(uri, newOrder, authToken, "x-requestid").ConfigureAwait(false);
}
```

----------------------------------------

TITLE: IHostTask::Start Method API Reference
DESCRIPTION: Detailed API documentation for the IHostTask::Start method, including its C++ syntax, return types, and descriptions of possible HRESULT values.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/hosting/ihosttask-start-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
IHostTask::Start Method
  Method Signature:
    HRESULT Start ();

  Return Values:
    HRESULT:
      S_OK: Start returned successfully.
      E_FAIL: An unknown catastrophic failure occurred. When a method returns E_FAIL, the common language runtime (CLR) is no longer usable within the process. Subsequent calls to hosting methods return HOST_E_CLRNOTAVAILABLE.
```

----------------------------------------

TITLE: C# Example: Demonstrating IL3052 Warning with CorRuntimeHost
DESCRIPTION: This C# example illustrates how using `CorRuntimeHost` triggers the IL3052 warning when compiling with Native AOT. It defines a `CorRuntimeHost` class with `Guid`, `ComImport`, and `ClassInterface` attributes, and instantiates it to show the unsupported COM interop.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/deploying/native-aot/warnings/il3052.md#_snippet_0

LANGUAGE: csharp
CODE:
```
using System.Runtime.InteropServices;

// AOT analysis warning IL3052: CorRuntimeHost.CorRuntimeHost(): COM interop is not supported
// with full ahead of time compilation.
new CorRuntimeHost();

[Guid("CB2F6723-AB3A-11D2-9C40-00C04FA30A3E")]
[ComImport]
[ClassInterface(ClassInterfaceType.None)]
public class CorRuntimeHost
{
}
```

----------------------------------------

TITLE: Example appsettings.json for SupportOptions
DESCRIPTION: This JSON snippet represents an example appsettings.json file, demonstrating how configuration for the SupportOptions class would be structured. It includes a 'SupportOptions' section with 'SupportEmail' and 'MaxRetries' properties, which will be bound to the SupportOptions class.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/extensions/options-library-authors.md#_snippet_5

LANGUAGE: json
CODE:
```
{
  "SupportOptions": {
    "SupportEmail": "support@example.com",
    "MaxRetries": 5
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

----------------------------------------

TITLE: CreateALink Function API Details
DESCRIPTION: Comprehensive API documentation for the `CreateALink` function, detailing its purpose, parameters with descriptions, and the required library for its successful execution.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/alink/createalink-function.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Function: CreateALink
API Name: CreateALink
API Location: alink.dll
API Type: DLLExport
Description: Creates an instance of the Assembly Linker and sets a pointer to the specified interface.

Parameters:
  riid: The physical name of one of the Assembly Linker interfaces.
  ppInterface: The location that on successful completion contains a pointer to the `riid` interface.

Requirements:
  Library: alink.dll
```

----------------------------------------

TITLE: Making Authenticated API DELETE Request in C#
DESCRIPTION: This C# method, `ClearBasketAsync`, shows how to make an authenticated DELETE request to an IdentityServer protected API. It retrieves the access token from `IIdentityService` and includes it in the call to `DeleteBasketAsync`, ensuring the request is authorized.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/authentication-and-authorization.md#_snippet_17

LANGUAGE: csharp
CODE:
```
public async Task ClearBasketAsync()
{
    var authToken = await _identityService.GetAuthTokenAsync().ConfigureAwait(false);

    if (string.IsNullOrEmpty(authToken))
    {
        return;
    }

    await GetBasketClient().DeleteBasketAsync(new DeleteBasketRequest(), CreateAuthenticationHeaders(authToken))
        .ConfigureAwait(false);
}
```

----------------------------------------

TITLE: F# Functions for Raising Exceptions (APIDOC)
DESCRIPTION: This API documentation outlines common F# functions used to raise exceptions. It details their syntax, purpose, and the specific exception types they throw, guiding developers on appropriate usage for different error scenarios.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/style-guide/conventions.md#_snippet_9

LANGUAGE: APIDOC
CODE:
```
Function: nullArg
  Syntax: nullArg "argumentName"
  Purpose: Raises a System.ArgumentNullException with the specified argument name.

Function: invalidArg
  Syntax: invalidArg "argumentName" "message"
  Purpose: Raises a System.ArgumentException with a specified argument name and message.

Function: invalidOp
  Syntax: invalidOp "message"
  Purpose: Raises a System.InvalidOperationException with the specified message.

Function: raise
  Syntax: raise (ExceptionType("message"))
  Purpose: General-purpose mechanism for throwing exceptions.

Function: failwith
  Syntax: failwith "message"
  Purpose: Raises a System.Exception with the specified message.

Function: failwithf
  Syntax: failwithf "format string" argForFormatString
  Purpose: Raises a System.Exception with a message determined by the format string and its inputs.
```

----------------------------------------

TITLE: Handle Basket Update on Server-Side API
DESCRIPTION: The `Post` method in `BasketController` (server-side API) receives the `CustomerBasket` data from the client. It uses a `RedisBasketRepository` to persist the basket data to a Redis cache and returns the updated basket with a success HTTP status code.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/accessing-remote-data.md#_snippet_8

LANGUAGE: csharp
CODE:
```
[HttpPost]
public async Task<IActionResult> Post([FromBody] CustomerBasket value)
{
    var basket = await _repository.UpdateBasketAsync(value);
    return Ok(basket);
}
```

----------------------------------------

TITLE: FunctionTailcall3WithInfo Callback API Documentation
DESCRIPTION: Detailed API documentation for the `FunctionTailcall3WithInfo` callback function, including its C++ syntax, parameters, and critical implementation remarks for .NET Framework profiling.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/functiontailcall3withinfo-function.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
FunctionTailcall3WithInfo Function:
  Syntax:
    void __stdcall FunctionTailcall3WithInfo(
                   [in] FunctionIDOrClientID functionIDOrClientID,
                   [in] COR_PRF_ELT_INFO eltInfo);
  Parameters:
    functionIDOrClientID:
      Type: FunctionIDOrClientID
      Direction: [in]
      Description: The identifier of the currently executing function that is about to make a tail call.
    eltInfo:
      Type: COR_PRF_ELT_INFO
      Direction: [in]
      Description: An opaque handle that represents information about a given stack frame. This handle is valid only during the callback to which it is passed.
  Remarks:
    Purpose: Notifies the profiler as functions are called, allowing inspection of the stack frame using ICorProfilerInfo3::GetFunctionTailcall3Info.
    Requirements:
      - COR_PRF_ENABLE_FRAME_INFO flag must be set.
      - Profiler must use ICorProfilerInfo::SetEventMask to set event flags.
      - Profiler must use ICorProfilerInfo3::SetEnterLeaveFunctionHooks3WithInfo to register implementation.
    Implementation Details:
      - Must use __declspec(naked) storage-class attribute.
      - Must save all used registers (including FPU) on entry.
      - Must restore the stack on exit by popping off all parameters.
      - Should not block (delays garbage collection).
      - Should not attempt garbage collection (stack may not be GC-friendly).
      - Must not call into managed code or cause managed memory allocation.
  Requirements:
    Platforms: See System Requirements.
    Header: CorProf.idl
    Library: CorGuids.lib
    .NET Framework Versions: .NET Framework 2.0 and later.
```

----------------------------------------

TITLE: Get Text Content from Table Cell using UI Automation
DESCRIPTION: Illustrates how to extract the plain text content from a specific table cell using UI Automation GridPattern and TextPattern APIs. This example focuses on retrieving the direct text value of a cell.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/ui-automation/textpattern-and-embedded-objects-overview.md#_snippet_6

LANGUAGE: APIDOC
CODE:
```
Method Called: System.Windows.Automation.GridPattern.GetItem with parameters of (1,1).
Result: Returns the System.Windows.Automation.AutomationElement representing the content of the table cell; in this case, the element is a text control.

Method Called: System.Windows.Automation.TextPattern.RangeFromChild where System.Windows.Automation.AutomationElement is the object returned by the previous GetItem method.
Result: Returns "Y".
```

----------------------------------------

TITLE: Overriding VisitUsingDirective in C#
DESCRIPTION: This snippet overrides the `VisitUsingDirective` method within the `UsingCollector` class. It checks if a `using` directive's name starts with 'System.' and, if not, adds the directive to the `Usings` collection. This method is automatically called by the `CSharpSyntaxWalker` for every `using` directive encountered.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_13

LANGUAGE: C#
CODE:
```
public override void VisitUsingDirective(UsingDirectiveSyntax node)
{
    Console.WriteLine($"\tVisitUsingDirective called with {node.Name}.");
    if (node.Name.ToString() != "System" &&
        !node.Name.ToString().StartsWith("System."))
    {
        Console.WriteLine($"\t\tSuccess. Adding {node.Name}.");
        this.Usings.Add(node);
    }

    base.VisitUsingDirective(node);
}
```

----------------------------------------

TITLE: EmitManifest Method API Documentation
DESCRIPTION: Comprehensive API documentation for the EmitManifest method, detailing its purpose, parameters, return values, and header requirements.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/alink/emitmanifest-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Method: EmitManifest
Description: Emits the final manifest. Call this method after importing all other files and setting all options. Do not call this method for unbound modules.
Parameters:
  AssemblyID: ID of the assembly.
  pdwReserveSize: Receives the size to reserve in the assembly file, retrieved from StrongNameSignatureSize Function.
  ptkManifest: Optionally receives the assembly manifest token.
Return Value:
  S_OK: if the method succeeds.
Requirements:
  alink.h
See also:
  IALink Interface
  IALink2 Interface
  ALink API
```

----------------------------------------

TITLE: Creating Syntax Tree in C# Roslyn
DESCRIPTION: This code snippet demonstrates how to parse the `programText` constant into a `SyntaxTree` using `CSharpSyntaxTree.ParseText` and then retrieve the root node of the tree, which is a `CompilationUnitSyntax`.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_2

LANGUAGE: C#
CODE:
```
SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
```

----------------------------------------

TITLE: Invoking New Subcommands and Options
DESCRIPTION: Provides examples of command-line input for the newly added `quotes` subcommand and its children: `read`, `add`, and `delete`. It demonstrates how to use their respective options and arguments, including multi-argument options.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/standard/commandline/get-started-tutorial.md#_snippet_27

LANGUAGE: console
CODE:
```
scl quotes read --file sampleQuotes.txt --delay 40 --fgcolor red --light-mode
scl quotes add "Hello world!" "Nancy Davolio"
scl quotes delete --search-terms David "You can do" Antoine "Perfection is achieved"
```

----------------------------------------

TITLE: Declaring Test Compilation in C#
DESCRIPTION: This snippet declares a `Compilation` object named `testCompilation` and initializes it by calling `CreateTestCompilation()`. This serves as a placeholder for the compilation that will be analyzed and transformed by the `TypeInferenceRewriter`.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-transformation.md#_snippet_12

LANGUAGE: C#
CODE:
```
Compilation testCompilation = CreateTestCompilation();
```

----------------------------------------

TITLE: APIDOC: System.Diagnostics.ActivityListener Class and Callbacks
DESCRIPTION: This section provides detailed API documentation for the System.Diagnostics.ActivityListener class and its key callbacks. It describes the purpose and behavior of ShouldListenTo, Sample, ActivityStarted, and ActivityStopped methods, explaining how they interact with Activity objects and ActivitySource.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/diagnostics/distributed-tracing-collection-walkthroughs.md#_snippet_17

LANGUAGE: APIDOC
CODE:
```
System.Diagnostics.ActivityListener:
  Description: Used to receive callbacks during the lifetime of an Activity.

  Methods/Callbacks:
    ShouldListenTo(source: ActivitySource):
      Description: Invoked once for each ActivitySource in the process. Return true if you are interested in performing sampling or being notified about start/stop events for Activities produced by this source.

    Sample(options: ref ActivityCreationOptions<ActivityContext>):
      Description: By default ActivitySource.StartActivity does not create an Activity object unless some ActivityListener indicates it should be sampled. Returning ActivitySamplingResult.AllDataAndRecorded indicates that the Activity should be created, Activity.IsAllDataRequested should be set to true, and Activity.ActivityTraceFlags will have the Recorded flag set. IsAllDataRequested can be observed by the instrumented code as a hint that a listener wants to ensure that auxiliary Activity information such as Tags and Events are populated. The Recorded flag is encoded in the W3C TraceContext ID and is a hint to other processes involved in the distributed trace that this trace should be sampled.

    ActivityStarted(activity: Activity):
      Description: Called when an Activity is started. These callbacks provide an opportunity to record relevant information about the Activity or potentially to modify it. When an Activity has just started, much of the data may still be incomplete and it will be populated before the Activity stops.

    ActivityStopped(activity: Activity):
      Description: Called when an Activity is stopped. These callbacks provide an opportunity to record relevant information about the Activity or potentially to modify it.

    AddActivityListener(listener: System.Diagnostics.ActivityListener):
      Description: Initiates invoking the callbacks.

    Dispose():
      Description: Call to stop the flow of callbacks. Be aware that in multi-threaded code, callback notifications in progress could be received while Dispose() is running or even shortly after it has returned.
```

----------------------------------------

TITLE: Example Output for Background Server-Client Mode
DESCRIPTION: Illustrates the console output when starting both the dotnet-coverage server and clients in background mode, returning control to the user immediately.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/additional-tools/dotnet-coverage.md#_snippet_23

LANGUAGE: console
CODE:
```
D:\serverexample\server> dotnet-coverage collect --session-id serverdemo --server-mode --background
D:\serverexample\server> dotnet-coverage connect --background serverdemo ConsoleApplication.exe World
D:\serverexample\server> dotnet-coverage connect --background serverdemo WpfApplication.exe
D:\serverexample\server> dotnet-coverage shutdown serverdemo
D:\serverexample\server>
```

----------------------------------------

TITLE: CA1721 Violation Example in VB.NET
DESCRIPTION: This VB.NET example demonstrates a violation of rule CA1721, showing a property named 'Title' and a method named 'GetTitle' within the same class. This naming convention conflict is identified by the rule, encouraging consistent API naming.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/code-analysis/quality-rules/ca1721.md#_snippet_2

LANGUAGE: VB
CODE:
```
Public Class Book
    Public Property Title As String
    Public Function GetTitle() As String ' CA1721
        Return Title
    End Function
End Class
```

----------------------------------------

TITLE: APIDOC: Visual Basic XML Documentation Tag <example> Definition
DESCRIPTION: Formal documentation for the Visual Basic XML documentation `<example>` tag, detailing its purpose, parameters, and usage considerations.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/visual-basic/language-reference/xmldoc/example.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Tag Name: <example>
Purpose: Specifies an example for the member.
Parameters:
  - Name: description
    Type: string
    Description: A description of the code sample.
Remarks:
  - The <example> tag lets you specify an example of how to use a method or other library member.
  - This commonly involves using the <code> tag.
  - Compile with -doc to process documentation comments to a file.
```

----------------------------------------

TITLE: IALink.ImportFile2 Method API Documentation
DESCRIPTION: Detailed API documentation for the `IALink.ImportFile2` method, including its purpose, parameters, return value, and requirements.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/alink/importfile2-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
IALink.ImportFile2 Method:
  Description: Imports assemblies and unbound modules. This method is like ImportFile Method, but works even if the file being imported does not exist on disk.
  Parameters:
    pszFilename: Name of file to be imported.
    pszTargetName: Optional output file name that can be used to rename the file as it is linked into the assembly.
    pAssemblyScopeIn: Optional scope IMetaDataAssemblyImport Interface interface.
    fSmartImport: If TRUE, ImportTypes is used, otherwise importing must be performed manually.
    pImportToken: Receives the ID for the file or assembly.
    ppAssemblyScope: Receives the IMetaDataAssemblyImport Interface interface. NULL if the file is not an assembly.
    pdwCountOfScopes: Receives the found of files and/or scopes imported.
  Return Value: Returns S_OK if the method succeeds.
  Requirements: Requires alink.h.
  See also:
    - IALink Interface
    - IALink2 Interface
    - ALink API
```

----------------------------------------

TITLE: Define ExampleClient for HTTP Requests
DESCRIPTION: This class defines `ExampleClient`, which encapsulates HTTP request logic. Its constructor accepts an `HttpClient` instance, and it provides a method `GetCommentsAsync` to fetch data from a specific API endpoint.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/resilience/http-resilience.md#_snippet_3

LANGUAGE: csharp
CODE:
```
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

public class ExampleClient
{
    private readonly HttpClient _httpClient;

    public ExampleClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync()
    {
        var response = await _httpClient.GetAsync("/comments");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<Comment>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}
```

----------------------------------------

TITLE: Example Server Output for Server-Client Mode
DESCRIPTION: Displays the console output on the server side when dotnet-coverage is started in server mode, showing its idle state awaiting client connections.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/additional-tools/dotnet-coverage.md#_snippet_21

LANGUAGE: console
CODE:
```
D:\serverexample\server> dotnet-coverage collect --session-id serverdemo --server-mode
SessionId: serverdemo
// Server will be in idle state and wait for connect and shutdown commands
Code coverage results: output.coverage.
D:\serverexample\server>
```

----------------------------------------

TITLE: APIDOC: SpawnInstance Function Details
DESCRIPTION: Comprehensive API documentation for the `SpawnInstance` function, detailing its purpose, parameters, return values, and remarks for unmanaged API usage, particularly with WMI.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/wmi/spawninstance.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
SpawnInstance Function:
  Purpose: Creates a new instance of a class.
  Syntax:
    HRESULT SpawnInstance (
       [in] int                  vFunc,
       [in] IWbemClassObject*    ptr,
       [in] LONG                 lFlags,
       [out] IWbemClassObject**  ppNewInstance);
  Parameters:
    vFunc [in] (int): This parameter is unused.
    ptr [in] (IWbemClassObject*): A pointer to an IWbemClassObject instance.
    lFlags [in] (LONG): Reserved. This parameter must be 0.
    ppNewInstance [out] (IWbemClassObject**): Receives the pointer to the new instance of the class. If an error occurs, a new object is not returned, and ppNewInstance is left unmodified.
  Return Values:
    WBEM_E_INCOMPLETE_CLASS (0x80041020): `ptr` is not a valid class definition and cannot spawn new instances. Either it is incomplete or it has not been registered with Windows Management by calling PutClassWmi.
    WBEM_E_OUT_OF_MEMORY (0x80041006): Not enough memory is available to complete the operation.
    WBEM_E_INVALID_PARAMETER (0x80041008): `ppNewClass` is `null`.
    WBEM_S_NO_ERROR (0): The function call was successful.
  Remarks:
    This function wraps a call to the IWbemClassObject::SpawnInstance method.
    `ptr` must be a class definition obtained from Windows Management. (Note that spawning an instance from an instance is supported but the returned instance is empty.) You then use this class definition to create new instances. A call to the PutInstanceWmi function is required if you intend to write the instance to Windows Management.
    The new object returned in `ppNewClass` automatically becomes a subclass of the current object. This behavior cannot be overridden. There is no other method by which subclasses (derived classes) can be created.
```

----------------------------------------

TITLE: NetTcpBinding Configuration Settings
DESCRIPTION: Example configuration snippet for NetTcpBinding, showing various timeout, buffer, connection, and security settings. Modifying these settings while sharing a port can contribute to 'AddressAlreadyInUseException'.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/wcf/wcf-troubleshooting-quickstart.md#_snippet_5

LANGUAGE: xml
CODE:
```
<bindings>
  <netTcpBinding>
    <binding closeTimeout="00:01:00" openTimeout="00:01:00" receiveTimeout="00:10:00" sendTimeout="00:01:00" transactionFlow="false" transferMode="Buffered" transactionProtocol="OleTransactions" hostNameComparisonMode="StrongWildcard" listenBacklog="10" maxBufferPoolSize="524288" maxBufferSize="65536" maxConnections="11" maxReceivedMessageSize="65536">
      <readerQuotas maxDepth="32" maxStringContentLength="8192" maxArrayLength="16384" maxBytesPerRead="4096" maxNameTableCharCount="16384"/>
      <reliableSession ordered="true" inactivityTimeout="00:10:00" enabled="false"/>
      <security mode="Transport">
        <transport clientCredentialType="Windows" protectionLevel="EncryptAndSign"/>
      </security>
    </binding>
  </netTcpBinding>
</bindings>
```

----------------------------------------

TITLE: API Documentation for IHostTaskManager::CreateTask Method
DESCRIPTION: Detailed documentation for the IHostTaskManager::CreateTask method, including parameters, return values, remarks, and requirements.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/hosting/ihosttaskmanager-createtask-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Parameters:
  stacksize [in] DWORD: The requested size, in bytes, of the requested stack, or 0 (zero) for the default size.
  pStartAddress [in] LPTHREAD_START_ROUTINE: A pointer to the function the task is to execute.
  pParameter [in] PVOID: A pointer to the user data to be passed to the function, or null if the function takes no parameters.
  ppTask [out] IHostTask**: A pointer to the address of an IHostTask instance created by the host, or null if the task cannot be created. The task remains in a suspended state until it is explicitly started by a call to IHostTask::Start.

Return Value (HRESULT):
  S_OK: CreateTask returned successfully.
  HOST_E_CLRNOTAVAILABLE: The common language runtime (CLR) has not been loaded into a process, or the CLR is in a state in which it cannot run managed code or process the call successfully.
  HOST_E_TIMEOUT: The call timed out.
  HOST_E_NOT_OWNER: The caller does not own the lock.
  HOST_E_ABANDONED: An event was canceled while a blocked thread or fiber was waiting on it.
  E_FAIL: An unknown catastrophic failure occurred. When a method returns E_FAIL, the CLR is no longer usable within the process. Subsequent calls to hosting methods return HOST_E_CLRNOTAVAILABLE.
  E_OUTOFMEMORY: Not enough memory was available to create the requested task.

Remarks:
  The CLR calls CreateTask to request that the host create a new task. The host returns an interface pointer to an IHostTask instance. The returned task must remain suspended until it is explicitly started by a call to IHostTask::Start.

Requirements:
  Platforms: See System Requirements.
  Header: MSCorEE.h
  Library: Included as a resource in MSCorEE.dll
  .NET Framework Versions: .NET Framework 2.0+
```

----------------------------------------

TITLE: Declaring Storage for C# Using Directives
DESCRIPTION: This code adds a public read-only list, `Usings`, to the `UsingCollector` class. This list will store `UsingDirectiveSyntax` objects that are identified and collected during the syntax tree traversal by the `CSharpSyntaxWalker`.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_12

LANGUAGE: C#
CODE:
```
public readonly List<UsingDirectiveSyntax> Usings = new List<UsingDirectiveSyntax>();
```

----------------------------------------

TITLE: IALink.Init Method API Reference
DESCRIPTION: Detailed API documentation for the IALink.Init method, including parameters, their types, descriptions, return value, and requirements.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/alink/init-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
IALink.Init(pDispenser, pErrorHandler)
  pDispenser:
    Type: IMetaDataDispenserEx*
    Description: Pointer to the metadata dispenser.
  pErrorHandler:
    Type: IMetaDataError*
    Description: Pointer to an optional error handling interface.
  Returns:
    Type: HRESULT
    Description: S_OK if the method succeeds.
  Requirements: alink.h
```

----------------------------------------

TITLE: IHostMAlloc::Free Method API Documentation
DESCRIPTION: Detailed API documentation for the IHostMAlloc::Free method, including its purpose, parameters, return values, and specific remarks regarding its usage and error conditions.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/hosting/ihostmalloc-free-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
IHostMAlloc::Free Method

Purpose:
Frees memory that was allocated by using the Alloc function.

Syntax:
HRESULT Free (
    [in] void* pMem
);

Parameters:
pMem: [in] void*
    A pointer to the memory to be freed.

Return Value:
HRESULT | Description
--------|------------
S_OK | `Free` returned successfully.
HOST_E_CLRNOTAVAILABLE | The common language runtime (CLR) has not been loaded into a process, or the CLR is in a state in which it cannot run managed code or process the call successfully.
HOST_E_TIMEOUT | The call timed out.
HOST_E_NOT_OWNER | The caller does not own the lock.
HOST_E_ABANDONED | An event was canceled while a blocked thread or fiber was waiting on it.
E_FAIL | An unknown catastrophic failure occurred. When a method returns E_FAIL, the CLR is no longer usable within the process. Subsequent calls to hosting methods return HOST_E_CLRNOTAVAILABLE.
HOST_E_INVALIDOPERATION | An attempt was made to free memory that was not allocated through the host.

Remarks:
If the `pMem` parameter refers to a region of memory that was not allocated by using a call to `Alloc`, the host should return HOST_E_INVALIDOPERATION.

Requirements:
Platforms: See System Requirements.
Header: MSCorEE.h
Library: Included as a resource in MSCorEE.dll
.NET Framework Versions: .NET Framework 2.0+
```

----------------------------------------

TITLE: Get an ILogger from DI in ASP.NET Minimal APIs
DESCRIPTION: This example demonstrates how to obtain an `ILogger` object within a hosted application using ASP.NET Minimal APIs. It shows the use of a primary constructor in C# 12 to inject `ILogger<T>` into a service, which is then used for logging within a request handler. The DI container automatically provides the correct `ILogger` instance.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/extensions/logging.md#_snippet_5

LANGUAGE: csharp
CODE:
```
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ExampleHandler>();
var app = builder.Build();

app.MapGet("/", (ExampleHandler handler) => handler.HandleRequest());

app.Run();

public class ExampleHandler(ILogger<ExampleHandler> logger)
{
    public IResult HandleRequest()
    {
        logger.LogInformation("Request received and handled by ExampleHandler.");
        return Results.Ok("Hello from ExampleHandler!");
    }
}
```

----------------------------------------

TITLE: ICorProfilerInfo Interface Method Reference
DESCRIPTION: Detailed documentation for the methods available in the ICorProfilerInfo interface, including their purpose, parameters, and notes on obsolescence and recommended alternatives.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/icorprofilerinfo-interface.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
ICorProfilerInfo Interface:
  Methods:
    BeginInprocDebugging:
      Description: Initializes in-process debugging support. This method is obsolete in .NET Framework version 2.0.
    EndInprocDebugging:
      Description: Shuts down an in-process debugging session. This method is obsolete in .NET Framework version 2.0.
    ForceGC:
      Description: Forces garbage collection to occur within the runtime.
    GetAppDomainInfo:
      Description: Gets information about the specified application domain.
    GetAssemblyInfo:
      Description: Gets information about the specified assembly.
    GetClassFromObject:
      Description: Gets the ClassID of an

 object, given its ObjectID.
    GetClassFromToken:
      Description: Gets the ID of the class, given the metadata token. This method is obsolete in .NET Framework version 2.0. Use the ICorProfilerInfo2::GetClassFromTokenAndTypeArgs method instead.
    GetClassIDInfo:
      Description: Gets the parent module and the metadata token for the specified class.
    GetCodeInfo:
      Description: Gets the extent of native code associated with the specified function ID. This method is obsolete. Use the ICorProfilerInfo2::GetCodeInfo2 method instead.
    GetCurrentThreadID:
      Description: Gets the ID of the current thread, if it is a managed thread.
    GetEventMask:
      Description: Gets the current event categories for which the profiler wants to receive event notifications from the CLR.
    GetFunctionFromIP:
      Description: Maps a managed code instruction pointer to a FunctionID.
    GetFunctionFromToken:
      Description: Gets the ID of a function. This method is obsolete in .NET Framework version 2.0. Use the ICorProfilerInfo2::GetFunctionFromTokenAndTypeArgs method instead.
    GetFunctionInfo:
      Description: Gets the parent class and metadata token for the specified function.
    GetHandleFromThread:
      Description: Maps the ID of a thread to a Win32 thread handle.
    GetILFunctionBody:
      Description: Gets a pointer to the body of a method in common intermediate language (CIL) code, starting at its header.
    GetILFunctionBodyAllocator:
      Description: Gets an interface that provides a method to allocate memory to be used for swapping out the body of a method in CIL code.
    GetILToNativeMapping:
      Description: Gets a map from CIL offsets to native offsets for the code contained in the specified function.
    GetInprocInspectionInterface:
      Description: Gets an object that can be queried for an ICorDebugProcess interface. This method is obsolete in .NET Framework version 2.0.
    GetInprocInspectionIThisThread:
      Description: Gets an object that can be queried for the ICorDebugThread interface. This method is obsolete in .NET Framework version 2.0.
    GetModuleInfo:
      Description: Given a module ID, returns the file name of the module and the ID of the module's parent assembly.
    GetModuleMetaData:
      Description: Gets a metadata interface instance that maps to the specified module.
    GetObjectSize:
      Description: Gets the size of a specified object.
    GetThreadContext:
      Description: Gets the context identity currently associated with the specified thread.
    GetThreadInfo:
      Description: Gets the current Win32 thread identity for the specified thread.
    GetTokenAndMetadataFromFunction:
      Description: Gets the metadata token and an instance of the metadata interface that can be used against the token for the specified function.
    IsArrayClass:
      Description: Determines whether the specified class is an array class.
    SetEnterLeaveFunctionHooks:
      Description: Specifies profiler-implemented functions to be called on "enter", "leave", and "tailcall" hooks of managed functions.
```

----------------------------------------

TITLE: Overriding VisitLocalDeclarationStatement in C#
DESCRIPTION: This snippet overrides the `VisitLocalDeclarationStatement` method from `CSharpSyntaxRewriter`. This method is invoked when the rewriter encounters a local variable declaration statement, providing the entry point for custom transformation logic on such nodes.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-transformation.md#_snippet_7

LANGUAGE: C#
CODE:
```
public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
{

}
```

----------------------------------------

TITLE: APIDOC: System.Net.ServicePoint Methods Mapping
DESCRIPTION: This table maps methods of the old System.Net.ServicePoint API to their equivalent or alternative APIs, along with notes on usage or lack of direct equivalents.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/networking/http/httpclient-migrate-from-httpwebrequest.md#_snippet_10

LANGUAGE: APIDOC
CODE:
```
ServicePoint Methods Mapping:
- Old API: CloseConnectionGroup
  New API: No equivalent
  Notes: No workaround
- Old API: SetTcpKeepAlive
  New API: No direct equivalent API
  Notes: Usage of SocketsHttpHandler and ConnectCallback.
```

----------------------------------------

TITLE: Setting Text Colors with 'read' Subcommand
DESCRIPTION: Demonstrates how to customize the text display colors using the `--fgcolor` and `--light-mode` options with the `read` subcommand. This example sets the foreground color to red and enables light mode for the background.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/standard/commandline/get-started-tutorial.md#_snippet_24

LANGUAGE: console
CODE:
```
scl read --file sampleQuotes.txt --fgcolor red --light-mode
```

----------------------------------------

TITLE: ICorProfilerCallback4::ReJITCompilationStarted API Documentation
DESCRIPTION: Comprehensive API documentation for the ICorProfilerCallback4::ReJITCompilationStarted method, detailing its purpose, parameters with their types and descriptions, return values, and important usage remarks regarding class constructors and multi-threading.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/icorprofilercallback4-rejitcompilationstarted-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Method: ReJITCompilationStarted
Purpose: Notifies the profiler that the just-in-time (JIT) compiler has started to recompile a function.

Syntax:
  HRESULT ReJITCompilationStarted(
    [in] FunctionID functionId,
    [in] ReJITID    rejitId,
    [in] BOOL       fIsSafeToBlock
  )

Parameters:
  functionId: [in] FunctionID
    The ID of the function that the JIT compiler has started to recompile.
  rejitId: [in] ReJITID
    The recompilation ID of the new version of the function.
  fIsSafeToBlock: [in] BOOL
    true to indicate that blocking may cause the runtime to wait for the calling thread to return from this callback; false to indicate that blocking will not affect the operation of the runtime. A value of true does not harm the runtime, but can affect the profiling results.

Return Type: HRESULT

Remarks:
  It is possible to receive more than one pair of ReJITCompilationStarted and ReJITCompilationFinished method calls for each function because of the way the runtime handles class constructors. For example, the runtime starts to recompile method A, but the class constructor for class B needs to be run. Therefore, the runtime recompiles the constructor for class B and runs it. While the constructor is running, it makes a call to method A, which causes method A to be recompiled again. In this scenario, the first recompilation of method A is halted. However, both attempts to recompile method A are reported with JIT recompilation events.

  Profilers must support the sequence of JIT recompilation callbacks in cases where two threads are simultaneously making callbacks. For example, thread A calls ReJITCompilationStarted; however, before thread A calls ReJITCompilationFinished, thread B calls ICorProfilerCallback::ExceptionSearchFunctionEnter with the function ID from the ReJITCompilationStarted callback for thread A. It might appear that the function ID should not yet be valid because a call to ReJITCompilationFinished had not yet been received by the profiler. However, in this case, the function ID is valid.

Requirements:
  Platforms: See System Requirements.
  Header: CorProf.idl, CorProf.h
  Library: CorGuids.lib
  .NET Framework Versions: .NET Framework 4.5+
```

----------------------------------------

TITLE: Add Validation Rules to ValidatableObject<T> Properties in C#
DESCRIPTION: Shows how to add validation rules to the Validations collection of ValidatableObject<T> instances. This example adds the IsNotNullOrEmptyRule<T> to UserName and Password properties, along with custom validation messages.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/validation.md#_snippet_5

LANGUAGE: C#
CODE:
```
private void AddValidations()
{
    UserName.Validations.Add(new IsNotNullOrEmptyRule<string> 
    {
        ValidationMessage = "A username is required." 
    });

    Password.Validations.Add(new IsNotNullOrEmptyRule<string> 
    {
        ValidationMessage = "A password is required." 
    });
}
```

----------------------------------------

TITLE: Iterating Syntax Trees and Initializing Rewriter in C#
DESCRIPTION: This snippet iterates through each `SyntaxTree` within the `testCompilation`. For each tree, it retrieves its `SemanticModel` and then instantiates a `TypeInferenceRewriter`, passing the `SemanticModel` to its constructor. This prepares the rewriter for processing each individual syntax tree.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-transformation.md#_snippet_13

LANGUAGE: C#
CODE:
```
foreach (SyntaxTree tree in testCompilation.SyntaxTrees)
{
    SemanticModel model = testCompilation.GetSemanticModel(tree);

    TypeInferenceRewriter rewriter = new TypeInferenceRewriter(model);
}
```

----------------------------------------

TITLE: List All Azure Resource Groups
DESCRIPTION: Shows how to retrieve and iterate through all resource groups associated with a specific Azure subscription. The example uses `GetAllAsync()` on the `ResourceGroupCollection` to get a list of resource groups and prints their names.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/azure/sdk/resource-management.md#_snippet_14

LANGUAGE: csharp
CODE:
```
// First, initialize the ArmClient and get the default subscription
ArmClient client = new ArmClient(new DefaultAzureCredential());
SubscriptionResource subscription = await client.GetDefaultSubscriptionAsync();
// Now we get a ResourceGroup collection for that subscription
ResourceGroupCollection resourceGroupCollection = subscription.GetResourceGroups();
// With GetAllAsync(), we can get a list of the resources in the collection
await foreach (ResourceGroupResource resourceGroup in resourceGroupCollection)
{
    Console.WriteLine(resourceGroup.Data.Name);
}
```

----------------------------------------

TITLE: Workflow Foundation Pick Activity API Overview
DESCRIPTION: Documentation for the Workflow Foundation Pick activity and its associated types, including PickBranch, Trigger, and Action, detailing their purpose, behavior, and interaction within a workflow.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/windows-workflow-foundation/pick-activity.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.Activities.Statements.Pick:
  Description: Models a set of event triggers followed by their corresponding handlers.
  Contains: Collection of System.Activities.Statements.PickBranch activities.
  Behavior:
    - Triggers for all branches execute in parallel.
    - When one trigger completes, its corresponding action is executed.
    - All other triggers are canceled upon completion of one trigger.
  SimilarTo: System.Workflow.Activities.ListenActivity (.NET Framework 3.5)

System.Activities.Statements.PickBranch:
  Description: A pairing between a Trigger activity and an Action activity within a Pick activity.
  Properties:
    - Trigger (System.Activities.Statements.PickBranch.Trigger): The activity that initiates the branch.
    - Action (System.Activities.Statements.PickBranch.Action): The activity executed when the Trigger completes.

System.Activities.Statements.PickBranch.Trigger:
  Description: The initiating part of a PickBranch.
  BestPractice:
    - Should contain minimal logic, ideally just enough to receive an event.
    - Avoid introducing idle points if possible.
  Examples:
    - Custom activity (e.g., "Read input")
    - System.Activities.Statements.Delay
    - System.ServiceModel.Activities.Receive

System.Activities.Statements.PickBranch.Action:
  Description: The processing part of a PickBranch, executed after its Trigger completes.
  BestPractice:
    - Should contain all the processing logic for the event received by the Trigger.
  Examples:
    - Writing a greeting/timeout message to console.
    - System.ServiceModel.Activities.SendReply
    - Other business logic.

Commonly Referenced Activities:
  System.Activities.Statements.Delay: An activity often used as a Trigger.
  System.ServiceModel.Activities.Receive: A WF built-in messaging activity commonly used in the Trigger.
  System.ServiceModel.Activities.SendReply: A WF built-in messaging activity that should ideally be placed in the Action.
  System.Workflow.Activities.ListenActivity: The .NET Framework 3.5 equivalent to System.Activities.Statements.Pick.
```

----------------------------------------

TITLE: Configure IdentityServer Services in .NET
DESCRIPTION: This C# code demonstrates how to add and configure IdentityServer services in a .NET application's `Program.cs` file. It sets up cookie lifetime, event raising, and integrates with in-memory API scopes, API resources, clients, and ASP.NET Core Identity. It also includes a developer signing credential for non-production environments.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/authentication-and-authorization.md#_snippet_1

LANGUAGE: csharp
CODE:
```
builder.Services.AddIdentityServer(options =>
    {
        options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
    
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
    
        // TODO: Remove this line in production.
        options.KeyManagement.Enabled = false;
    })
    .AddInMemoryIdentityResources(Config.GetResources())
    .AddInMemoryApiScopes(Config.GetApiScopes())
    .AddInMemoryApiResources(Config.GetApis())
    .AddInMemoryClients(Config.GetClients(builder.Configuration))
    .AddAspNetIdentity<ApplicationUser>()
    // TODO: Not recommended for production - you need to store your key material somewhere secure
    .AddDeveloperSigningCredential();
```

----------------------------------------

TITLE: ICorProfilerInfo7::ApplyMetaData Method API Documentation
DESCRIPTION: Comprehensive API documentation for the `ICorProfilerInfo7::ApplyMetaData` method, including its C++ syntax, parameters, and detailed remarks on supported metadata types and usage constraints across different .NET versions.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/icorprofilerinfo7-applymetadata-method.md#_snippet_0

LANGUAGE: cpp
CODE:
```
HRESULT ApplyMetaData(
        [in] ModuleID moduleID
);
```

LANGUAGE: APIDOC
CODE:
```
Method: ICorProfilerInfo7::ApplyMetaData

Supported Versions:
  .NET Framework 4.6.1 and later
  .NET Core 3.0 and later (with extended support)

Parameters:
  moduleID:
    Type: ModuleID
    Direction: [in]
    Description: The identifier of the module whose metadata was changed.

Remarks:
  - Call this method if metadata changes are made after the ModuleLoadFinished callback, before using the new metadata.
  - Supports adding the following types of metadata:
    - AssemblyRef records (created by IMetaDataAssemblyEmit::DefineAssemblyRef)
    - TypeRef records (created by IMetaDataEmit::DefineTypeRefByName)
    - TypeSpec records (created by IMetaDataEmit::GetTokenFromTypeSpec)
    - MemberRef records (created by IMetaDataEmit::DefineMemberRef)
    - MemberSpec records (created by IMetaDataEmit2::DefineMethodSpec)
    - UserString records (created by IMetaDataEmit::DefineUserString)
  - Starting with .NET Core 3.0, also supports:
    - TypeDef records (created by IMetaDataEmit::DefineTypeDef)
    - MethodDef records (created by IMetaDataEmit::DefineMethod)
      - Note: Adding virtual methods to an existing type is not supported. Virtual methods must be added before the ModuleLoadFinished callback.
```

----------------------------------------

TITLE: Declaring Program Text Constant in C#
DESCRIPTION: This snippet declares a constant string variable, `programText`, which holds the source code of a basic 'Hello World!' C# program. This string serves as the input for building the syntax tree.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_1

LANGUAGE: C#
CODE:
```
const string programText =
    @"using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;

            namespace HelloWorld
            {
                class Program
                {
                    static void Main(string[] args)
                    {
                        Console.WriteLine("Hello, World!");
                    }
                }
            }";
```

----------------------------------------

TITLE: ICorProfilerInfo2::DoStackSnapshot Method API Details
DESCRIPTION: Detailed API documentation for the ICorProfilerInfo2::DoStackSnapshot method, including parameter descriptions, remarks, and information on synchronous stack walks.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/icorprofilerinfo2-dostacksnapshot-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
ICorProfilerInfo2::DoStackSnapshot Method:
  Parameters:
    thread:
      Type: ThreadID
      Direction: [in]
      Description: The ID of the target thread. Passing null yields a snapshot of the current thread. If a ThreadID of a different thread is passed, the common language runtime (CLR) suspends that thread, performs the snapshot, and resumes.
    callback:
      Type: StackSnapshotCallback *
      Direction: [in]
      Description: A pointer to the implementation of the StackSnapshotCallback method, which is called by the CLR to provide the profiler with information on each managed frame and each run of unmanaged frames. The StackSnapshotCallback method is implemented by the profiler writer.
    infoFlags:
      Type: ULONG32
      Direction: [in]
      Description: A value of the COR_PRF_SNAPSHOT_INFO enumeration, which specifies the amount of data to be passed back for each frame by StackSnapshotCallback.
    clientData:
      Type: void *
      Direction: [in]
      Description: A pointer to the client data, which is passed straight through to the StackSnapshotCallback callback function.
    context:
      Type: BYTE[]
      Direction: [in, size_is(contextSize), length_is(contextSize)]
      Description: A pointer to a Win32 CONTEXT structure, which is used to seed the stack walk. The Win32 CONTEXT structure contains values of the CPU registers and represents the state of the CPU at a particular moment in time. The seed helps the CLR determine where to begin the stack walk, if the top of the stack is unmanaged helper code; otherwise, the seed is ignored. A seed must be supplied for an asynchronous walk. If you are doing a synchronous walk, no seed is necessary. The context parameter is valid only if the COR_PRF_SNAPSHOT_CONTEXT flag was passed in the infoFlags parameter.
    contextSize:
      Type: ULONG32
      Direction: [in]
      Description: The size of the CONTEXT structure, which is referenced by the context parameter.
  Remarks:
    Passing null for thread yields a snapshot of the current thread. Snapshots can be taken of other threads only if the target thread is suspended at the time. When the profiler wants to walk the stack, it calls DoStackSnapshot. Before the CLR returns from that call, it calls your StackSnapshotCallback several times, once for each managed frame (or run of unmanaged frames) on the stack. When unmanaged frames are encountered, you must walk them yourself. The order in which the stack is walked is the reverse of how the frames were pushed onto the stack: leaf (last-pushed) frame first, main (first-pushed) frame last. For more information about how to program the profiler to walk managed stacks, see Profiler Stack Walking in the .NET Framework 2.0: Basics and Beyond. A stack walk can be synchronous or asynchronous, as explained in the following sections.
  Synchronous Stack Walk:
    A synchronous stack walk involves walking the stack of the current thread in response to a callback. It does not require seeding or suspending. You make a synchronous call when, in response to the CLR calling one of your profiler's ICorProfilerCallback (or ICorProfilerCallback2) methods, you call DoStackSnapshot to walk the stack of the current thread. This is useful when you want to see what the stack looks like at a notification such as ICorProfilerCallback::ObjectAllocated. You just call DoStackSnapshot from within your ICorProfilerCallback method, passing null in the context and thread parameters.
```

----------------------------------------

TITLE: Visual Basic Get Property: Basic Usage Example
DESCRIPTION: This example demonstrates a simple usage of the 'Get' statement within a property definition to retrieve and display the value of a property.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/visual-basic/language-reference/statements/get-statement.md#_snippet_3

LANGUAGE: Visual Basic
CODE:
```
Public Class ExampleClass
    Private _name As String = "Default Name"

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    Public Sub DisplayName()
        Console.WriteLine("The name is: " & Me.Name)
    End Sub
End Class

Module Module1
    Sub Main()
        Dim myObject As New ExampleClass()
        Console.WriteLine("Initial Name: " & myObject.Name) ' Uses Get
        myObject.Name = "New Name" ' Uses Set
        Console.WriteLine("Updated Name: " & myObject.Name) ' Uses Get
    End Sub
End Module
```

----------------------------------------

TITLE: INavigationService Interface Methods
DESCRIPTION: Detailed descriptions of the methods provided by the `INavigationService` interface, outlining their purpose, parameters, and behavior within the navigation system.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/navigation.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
INavigationService Interface:
  InitializeAsync():
    Purpose: Performs navigation to one of two pages when the app is launched.
  NavigateToAsync(route: string, routeParameters: IDictionary<string, object> = null):
    Purpose: Performs hierarchical navigation to a specified page using a registered navigation route. Can optionally pass named route parameters to use for processing on the destination page.
    Parameters:
      route: string - The navigation route.
      routeParameters: IDictionary<string, object> - Optional named route parameters.
  PopAsync():
    Purpose: Removes the current page from the navigation stack.
```

----------------------------------------

TITLE: XML Example: Subscribing to Workflow Instance 'Started' State
DESCRIPTION: This example demonstrates how to configure <workflowInstanceQueries> to subscribe specifically to the 'Started' state of a workflow instance, allowing tracking of when a workflow begins execution.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/configure-apps/file-schema/windows-workflow-foundation/workflowinstancequeries.md#_snippet_1

LANGUAGE: xml
CODE:
```
<workflowInstanceQueries>  
    <workflowInstanceQuery>  
      <states>  
        <state name="Started"/>  
      </states>  
    </workflowInstanceQuery>  
</workflowInstanceQueries>
```

----------------------------------------

TITLE: C# ASP.NET Core: Restricting Controller Access with Authorize Attribute
DESCRIPTION: Applying the `[Authorize]` attribute to an ASP.NET Core controller or action restricts access to authenticated users only. If an unauthenticated or unauthorized user attempts to access a protected resource, the API framework will return a `401 (unauthorized)` HTTP status code. This is a fundamental mechanism for securing web APIs.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/authentication-and-authorization.md#_snippet_14

LANGUAGE: csharp
CODE:
```
[Authorize]
public sealed class BasketController : Controller
{
    // Omitted for brevity
}
```

----------------------------------------

TITLE: apicompat Package-Specific Options
DESCRIPTION: Options applicable when comparing packages with the `apicompat` tool, detailing various flags for baseline validation, strict mode, and assembly references.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/apicompat/global-tool.md#_snippet_3

LANGUAGE: APIDOC
CODE:
```
Options:
  --baseline-package:
    Description: Specifies the path to a baseline package to validate the current package against.
  --enable-strict-mode-for-compatible-tfms:
    Description: Validates API compatibility in strict mode for contract and implementation assemblies for all compatible target frameworks. The default is true.
  --enable-strict-mode-for-compatible-frameworks-in-package:
    Description: Validates API compatibility in strict mode for assemblies that are compatible based on their target framework.
  --enable-strict-mode-for-baseline-validation:
    Description: Validates API compatibility in strict mode for package baseline checks.
  --package-assembly-references:
    Description: Specifies the paths to assembly references or their underlying directories for a specific target framework in the package. Separate multiple values with a comma.
  --baseline-package-assembly-references:
    Description: Specifies the paths to assembly references or their underlying directories for a specific target framework in the baseline package. Separate multiple values with a comma.
  --baseline-package-frameworks-to-ignore:
    Description: Specifies the set of target frameworks to ignore from the baseline package. The framework string must exactly match the folder name in the baseline package.
```

----------------------------------------

TITLE: CreateDebuggingInterfaceFromVersion API Documentation
DESCRIPTION: Detailed API documentation for the CreateDebuggingInterfaceFromVersion function, including parameter descriptions, possible return values, and general remarks about its usage and requirements.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/debugging/createdebugginginterfacefromversion-function-for-silverlight.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Function: CreateDebuggingInterfaceFromVersion
Purpose: Accepts a common language runtime (CLR) version string that is returned from the CreateVersionStringFromModule function, and returns a corresponding debugger interface (typically, ICorDebug).

Parameters:
  szDebuggeeVersion:
    Type: [in] LPCWSTR
    Description: Version string of the CLR in the target debuggee, which is returned by the CreateVersionStringFromModule function.
  ppCordb:
    Type: [out] IUnknown**
    Description: Pointer to a pointer to a COM object (IUnknown). This object will be cast to an ICorDebug object before it is returned.

Return Value:
  S_OK: ppCordb references a valid object that implements the ICorDebug interface.
  E_INVALIDARG: Either szDebuggeeVersion or ppCordb is null.
  CORDBG_E_DEBUG_COMPONENT_MISSING: A component that is necessary for CLR debugging cannot be located. Either _mscordbi.dll_ or _mscordaccore.dll_ was not found in the same directory as the target CoreCLR.dll.
  CORDBG_E_INCOMPATIBLE_PROTOCOL: Either mscordbi.dll or mscordaccore.dll is not the same version as the target CoreCLR.dll.
  E_FAIL (or other E_ return codes): Unable to return an ICorDebug interface.

Remarks:
  The interface that is returned provides the facilities for attaching to a CLR in a target process and debugging the managed code that the CLR is running.

Requirements:
  Platforms: See System Requirements.
  Header: dbgshim.h
  Library: dbgshim.dll
  .NET Framework Versions: 3.5 SP1
```

----------------------------------------

TITLE: FunctionTailcall3 API Reference
DESCRIPTION: Detailed API documentation for the `FunctionTailcall3` callback function, including its purpose, parameters, and critical implementation requirements for .NET Framework profiling.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/functiontailcall3-function.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Function: FunctionTailcall3
Description: Notifies the profiler that the currently executing function is about to perform a tail call to another function.

API Details:
  api_name: FunctionTailcall3
  api_location: mscorwks.dll
  api_type: COM
  f1_keywords: FunctionTailcall3
  helpviewer_keywords: FunctionTailcall3 function [.NET Framework profiling]
  topic_type: apiref

Parameters:
  functionOrRemappedID: [in] The identifier of the currently executing function that is about to make a tail call.

Remarks:
  - The FunctionTailcall3 callback function notifies the profiler as functions are being called. Use the ICorProfilerInfo3::SetEnterLeaveFunctionHooks3 method to register your implementation of this function.
  - The FunctionTailcall3 function is a callback; you must implement it. The implementation must use the __declspec(naked) storage-class attribute.
  - The execution engine does not save any registers before calling this function.
  - On entry, you must save all registers that you use, including those in the floating-point unit (FPU).
  - On exit, you must restore the stack by popping off all the parameters that were pushed by its caller.
  - The implementation of FunctionTailcall3 should not block, because it will delay garbage collection. The implementation should not attempt a garbage collection, because the stack may not be in a garbage collection-friendly state. If a garbage collection is attempted, the runtime will block until FunctionTailcall3 returns.
  - The FunctionTailcall3 function must not call into managed code or cause a managed memory allocation in any way.

Requirements:
  Platforms: See System Requirements.
  Header: CorProf.idl
  Library: CorGuids.lib
  .NET Framework Versions: .NET Framework 4.0+
```

----------------------------------------

TITLE: ForEach Activity API Documentation
DESCRIPTION: Detailed API documentation for the non-generic `ForEach` activity, outlining its properties, arguments, and specific error conditions.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/windows-workflow-foundation/samples/non-generic-foreach.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
ForEach Activity:
  Properties:
    Body:
      Type: System.Activities.ActivityAction<System.Object>
      Description: The ActivityAction of type System.Object, which is executed for each element in the collection. Each individual element is passed into the Body through its `Argument` property.
      Optional: true
    Values:
      Type: System.Collections.IEnumerable
      Description: The collection of elements that are iterated over. Ensuring that all elements of the collection are of compatible types is done at run-time.
      Optional: true
  Arguments:
    iterationVariable:
      Type: System.Activities.DelegateInArgument<System.Object>
      Description: An argument used within the Body to represent the current element during iteration.
  Conditions:
    Values is null:
      Message: Value for a required activity argument 'Values' was not supplied.
      Severity: Error
      Exception Type: System.InvalidOperationException
```

----------------------------------------

TITLE: Extending CSharpSyntaxRewriter in C#
DESCRIPTION: This code defines the `TypeInferenceRewriter` class, inheriting from `CSharpSyntaxRewriter`. This base class provides the foundational structure for traversing and transforming C# syntax trees, allowing custom logic to be applied to specific syntax nodes.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-transformation.md#_snippet_5

LANGUAGE: C#
CODE:
```
class TypeInferenceRewriter : CSharpSyntaxRewriter
{
}
```

----------------------------------------

TITLE: F# MiniCsvProvider Compiler Translation Example
DESCRIPTION: Illustrates the underlying code that the F# compiler generates from the user's `MiniCsvProvider` API calls. This shows how the provided types are erased at runtime to simpler types like `CsvFile` and array indexing.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/tutorials/type-providers/creating-a-type-provider.md#_snippet_39

LANGUAGE: F#
CODE:
```
let info = new CsvFile("info.csv")
for row in info.Data do
let (time:float) = row[1]
printfn "%f{float time}"
```

----------------------------------------

TITLE: ASP.NET Core 6 RC 2: No Automatic EndpointName Metadata
DESCRIPTION: Starting in ASP.NET Core 6 RC 2, `IEndpointNameMetadata` is no longer automatically set for endpoints. This example shows the same code as before, but it no longer generates any `IEndpointNameMetadata`.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/compatibility/aspnet-core/6.0/endpointname-metadata.md#_snippet_1

LANGUAGE: csharp
CODE:
```
app.MapGet("/foo", GetFoo);
```

----------------------------------------

TITLE: NextMethod Function API Documentation
DESCRIPTION: Comprehensive API documentation for the NextMethod function, detailing its parameters, return values, and usage remarks within the Unmanaged API context.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/wmi/nextmethod.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
NextMethod function:
  Retrieves the next method in an enumeration that begins with a call to BeginMethodEnumeration.

Syntax:
  HRESULT NextMethod (
     [in] int                 vFunc,
     [in] IWbemClassObject*   ptr,
     [in] LONG                lFlags,
     [out] BSTR*              pName,
     [out] IWbemClassObject** ppInSignature,
     [out] IWbemClassObject** ppOutSignature
  );

Parameters:
  vFunc:
    [in] int
    Description: This parameter is unused.
  ptr:
    [in] IWbemClassObject*
    Description: A pointer to an IWbemClassObject instance.
  lFlags:
    [in] LONG
    Description: Reserved. This parameter must be 0.
  pName:
    [out] BSTR*
    Description: A pointer that points to null prior to the call. When the function returns, the address of a new BSTR that contains the method name.
  ppSignatureIn:
    [out] IWbemClassObject**
    Description: A pointer that receives a pointer to an IWbemClassObject that contains the in parameters for the method.
  ppSignatureOut:
    [out] IWbemClassObject**
    Description: A pointer that receives a pointer to an IWbemClassObject that contains the out parameters for the method.

Return Value:
  The following values returned by this function are defined in the WbemCli.h header file, or you can define them as constants in your code:
  WBEM_E_UNEXPECTED:
    Value: 0x8004101d
    Description: There was no call to the BeginEnumeration function.
  WBEM_S_NO_ERROR:
    Value: 0
    Description: The function call was successful.
  WBEM_S_NO_MORE_DATA:
    Value: 0x40005
    Description: There are no more properties in the enumeration.

Remarks:
  This function wraps a call to the IWbemClassObject::NextMethod method.
  The caller begins the enumeration sequence by calling the BeginMethodEnumeration function, and then calls the NextMethod function until the function returns WBEM_S_NO_MORE_DATA. Optionally, the caller finishes the sequence by calling EndMethodEnumeration. The caller may terminate the enumeration early by calling EndMethodEnumeration at any time.

Requirements:
  Platforms: See System Requirements.
  Header: WMINet_Utils.idl
  .NET Framework Versions: .NET Framework 4.7.2+
```

----------------------------------------

TITLE: WCF API References for Session Management
DESCRIPTION: This section lists key .NET Framework types and members related to WCF session management, instance context control, and operation contract attributes as referenced in the documentation.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/wcf/using-sessions.md#_snippet_3

LANGUAGE: APIDOC
CODE:
```
Referenced WCF API Members:
- System.ServiceModel.SessionMode: An enumeration used in a contract to specify session requirements. Interacts with InstanceContextMode.
- System.ServiceModel.ServiceBehaviorAttribute.InstanceContextMode: A property that controls the association between channels and specific service objects.
- System.ServiceModel.InstanceContext: An object representing the runtime context for a service instance, which can be shared or managed manually.
- System.ServiceModel.OperationContractAttribute.IsInitiating: A property on an operation contract indicating if the operation initiates a session.
- System.ServiceModel.OperationContractAttribute.IsTerminating: A property on an operation contract indicating if the operation terminates a session.
```

----------------------------------------

TITLE: Available .NET Project SDKs
DESCRIPTION: A list of the available .NET project SDKs, their purpose, and their associated GitHub repositories. Each SDK is identified by its ID, a brief description of its functionality, and the link to its source code repository.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/project-sdk/overview.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
Available SDKs:
- ID: Microsoft.NET.Sdk
  Description: The .NET SDK
  Repo: https://github.com/dotnet/sdk
- ID: Microsoft.NET.Sdk.Web
  Description: The .NET Web SDK
  Repo: https://github.com/dotnet/sdk
- ID: Microsoft.NET.Sdk.Razor
  Description: The .NET Razor SDK
  Repo: https://github.com/dotnet/aspnetcore
- ID: Microsoft.NET.Sdk.BlazorWebAssembly
  Description: The .NET Blazor WebAssembly SDK
  Repo: https://github.com/dotnet/aspnetcore
- ID: Microsoft.NET.Sdk.Worker
  Description: The .NET Worker Service SDK
  Repo: https://github.com/dotnet/aspnetcore
- ID: Aspire.AppHost.Sdk
  Description: The .NET Aspire SDK
  Repo: https://github.com/dotnet/aspire
- ID: MSTest.Sdk
  Description: The MSTest SDK
  Repo: https://github.com/microsoft/testfx
```

----------------------------------------

TITLE: Declaring C# Syntax Walker Base Class
DESCRIPTION: This snippet defines the `UsingCollector` class, which inherits from `CSharpSyntaxWalker`. This inheritance is crucial for enabling the class to traverse a C# syntax tree and override specific visit methods for nodes of interest.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_11

LANGUAGE: C#
CODE:
```
class UsingCollector : CSharpSyntaxWalker
```

----------------------------------------

TITLE: IdentityServer Hybrid Flow Sign-in Process
DESCRIPTION: Details the sign-in process for the eShop multi-platform app using IdentityServer's hybrid authentication flow, outlining the sequence of API calls to the `connect/authorize` and `connect/token` endpoints and the expected responses.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/authentication-and-authorization.md#_snippet_7

LANGUAGE: APIDOC
CODE:
```
Sign-in Request Flow:
  1. Request to: <base endpoint>:5105/connect/authorize
  2. IdentityServer Response (on successful authentication): Authentication response containing an authorization code and an identity token.
  3. Authorization code sent to: <base endpoint>:5105/connect/token
  4. Token Endpoint Response: Access, identity, and refresh tokens.
```

----------------------------------------

TITLE: FunctionTailcall2 Callback API Reference
DESCRIPTION: Detailed API documentation for the `FunctionTailcall2` callback function, including its C++ signature, parameters, behavioral remarks, and system requirements for profiler implementations.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/functiontailcall2-function.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
void __stdcall FunctionTailcall2 (
    [in] FunctionID         funcId,
    [in] UINT_PTR           clientData,
    [in] COR_PRF_FRAME_INFO func  
);

Parameters:
funcId [in]: The identifier of the currently executing function that is about to make a tail call.
clientData [in]: The remapped function identifier, which the profiler previously specified via FunctionIDMapper, of the currently executing function that is about to make a tail call.
func [in]: A COR_PRF_FRAME_INFO value that points to information about the stack frame. The profiler should treat this as an opaque handle that can be passed back to the execution engine in the ICorProfilerInfo2::GetFunctionInfo2 method.

Remarks:
The target function of the tail call will use the current stack frame, and will return directly to the caller of the function that made the tail call. This means that a FunctionLeave2 callback will not be issued for a function that is the target of a tail call.
The value of the func parameter is not valid after the FunctionTailcall2 function returns because the value may change or be destroyed.
The FunctionTailcall2 function is a callback; you must implement it. The implementation must use the __declspec(naked) storage-class attribute.
The execution engine does not save any registers before calling this function.
- On entry, you must save all registers that you use, including those in the floating-point unit (FPU).
- On exit, you must restore the stack by popping off all the parameters that were pushed by its caller.
The implementation of FunctionTailcall2 should not block because it will delay garbage collection. The implementation should not attempt a garbage collection because the stack may not be in a garbage collection-friendly state. If a garbage collection is attempted, the runtime will block until FunctionTailcall2 returns.
Also, the FunctionTailcall2 function must not call into managed code or in any way cause a managed memory allocation.

Requirements:
Platforms: See System Requirements.
Header: CorProf.idl
Library: CorGuids.lib
.NET Framework Versions: .NET Framework 2.0 Plus
```

----------------------------------------

TITLE: APIDOC: ProvidedType API for On-Demand Member Creation
DESCRIPTION: Documents the 'ProvidedType' API methods 'AddMemberDelayed' and 'AddMembersDelayed', which enable the creation of types and members on-demand. These methods accept functions that return 'MemberInfo' or a list of 'MemberInfo'.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/tutorials/type-providers/creating-a-type-provider.md#_snippet_49

LANGUAGE: APIDOC
CODE:
```
  type ProvidedType =
      member AddMemberDelayed  : (unit -> MemberInfo)      -> unit
      member AddMembersDelayed : (unit -> MemberInfo list) -> unit
```

----------------------------------------

TITLE: WCF Session Control and Client Channel Management API References
DESCRIPTION: This section documents key WCF API members used for controlling service session behavior and managing client communication channels. It includes attributes for marking operations as initiating or terminating sessions, and methods for opening and closing client channels to manage session lifetimes.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/wcf/using-sessions.md#_snippet_2

LANGUAGE: APIDOC
CODE:
```
System.ServiceModel.OperationContractAttribute.IsInitiating:
  Description: Property to mark an operation as initiating a new session.
  Type: Boolean
  Usage: Set to 'true' for operations that must be called as the first operation of a new session.

System.ServiceModel.OperationContractAttribute.IsTerminating:
  Description: Property to mark an operation as terminating an existing session.
  Type: Boolean
  Usage: Set to 'true' for operations that must be called as the last message in an existing session.

System.ServiceModel.InstanceContext:
  Description: Represents the runtime instance context for a service object.
  Usage: Can be manipulated to explicitly control the lifetime of the service instance.

System.ServiceModel.ICommunicationObject.Open():
  Description: Opens the communication object.
  Usage: Used by clients to start a session by opening a session-based channel.

System.ServiceModel.ChannelFactory<TChannel>.CreateChannel():
  Description: Creates a channel of a specified type to a service endpoint.
  Usage: Returns a channel that can be opened to initiate a session.

System.ServiceModel.ClientBase<TChannel>.Open():
  Description: Opens the client communication object.
  Usage: Used by WCF client objects (generated by Svcutil.exe) to automatically open the channel and initiate a session.

System.ServiceModel.ICommunicationObject.Close():
  Description: Closes the communication object.
  Usage: Used by clients to gracefully tear down an existing session by closing a session-based channel.

System.ServiceModel.ClientBase<TChannel>.Close():
  Description: Closes the client communication object.
  Usage: Used by WCF client objects (generated by Svcutil.exe) to end a session.
```

----------------------------------------

TITLE: WCF Service Hosting API Usage Steps
DESCRIPTION: Describes the sequence of API calls and configurations required to programmatically host a WCF service, including setting up base addresses, service hosts, endpoints, and metadata exchange.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/wcf/how-to-host-and-run-a-basic-wcf-service.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
System.Uri:
  - Purpose: Holds the base address of the service.
  - Format: <transport>://<machine-name or domain><:optional port #>/<optional URI segment>
  - Example: http://localhost:8000/GettingStarted/

System.ServiceModel.ServiceHost:
  - Purpose: Hosts the service.
  - Constructor:
    - Parameters:
      - serviceType: Type of the class implementing the service contract.
      - baseAddress: Uri instance for the service's base address.
  - Methods:
    - Open(): Initiates listening for incoming messages. Recommended to use within a try/catch block for exception handling.

System.ServiceModel.Description.ServiceEndpoint:
  - Purpose: Defines a service endpoint (address, binding, service contract).
  - Constructor:
    - Parameters:
      - serviceContractInterfaceType: The service contract interface type (e.g., ICalculator).
      - binding: A built-in binding (e.g., System.ServiceModel.WSHttpBinding).
      - address: An address segment to append to the base address (e.g., CalculatorService).
  - Note: Optional for .NET Framework 4+ due to default endpoints.

System.ServiceModel.Description.ServiceMetadataBehavior:
  - Purpose: Enables metadata exchange for client proxy generation.
  - Properties:
    - HttpGetEnabled: bool (Set to `true` to enable HTTP GET for metadata).
  - Usage:
    - Add to System.ServiceModel.ServiceHost.Behaviors collection.
```

----------------------------------------

TITLE: FunctionEnter Function API Documentation
DESCRIPTION: Comprehensive API documentation for the `FunctionEnter` callback, detailing its purpose, parameters, critical implementation requirements, and deprecation status within the .NET Framework profiling API.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/functionenter-function.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
FunctionEnter Function API Documentation:
  Purpose: Notifies the profiler that control is being passed to a function.
  Note: Deprecated in .NET Framework version 2.0, and its use will incur a performance penalty. Use the FunctionEnter2 function instead.

  Parameters:
    funcID: [in] FunctionID - The identifier of the function to which control is passed.

  Remarks:
    - The FunctionEnter function is a callback; you must implement it.
    - The implementation must use the __declspec(naked) storage-class attribute.
    - The execution engine does not save any registers before calling this function.
    - On entry, you must save all registers that you use, including those in the floating-point unit (FPU).
    - On exit, you must restore the stack by popping off all the parameters that were pushed by its caller.
    - The implementation of FunctionEnter should not block because it will delay garbage collection.
    - The implementation should not attempt a garbage collection because the stack may not be in a garbage collection-friendly state. If a garbage collection is attempted, the runtime will block until FunctionEnter returns.
    - Also, the FunctionEnter function must not call into managed code or in any way cause a managed memory allocation.

  Requirements:
    Platforms: See System Requirements.
    Header: CorProf.idl
    Library: CorGuids.lib
    .NET Framework Versions: 1.1, 1.0
```

----------------------------------------

TITLE: Examining Main Method Syntax in C# Roslyn
DESCRIPTION: This snippet extracts and displays detailed syntactic information about the `Main` method, including its return type, the number of parameters, the type of the 'args' parameter, and the full text of the method's body.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_7

LANGUAGE: C#
CODE:
```
Console.WriteLine($"The return type of the Main method is {mainMethodDeclaration.ReturnType}.");
Console.WriteLine($"The method has {mainMethodDeclaration.ParameterList.Parameters.Count} parameters.");
ParameterSyntax argsParameter = mainMethodDeclaration.ParameterList.Parameters[0];
Console.WriteLine($"The type of the args parameter is {argsParameter.Type}.");
Console.WriteLine($"The body text of the Main method follows:");
Console.WriteLine(mainMethodDeclaration.Body.ToFullString());
```

----------------------------------------

TITLE: Creating C# Syntax Tree and Root Node
DESCRIPTION: This code snippet demonstrates how to parse the defined `codeText` into a `SyntaxTree` using `CSharpSyntaxTree.ParseText` and then retrieve the root node of that tree. The root node is essential for initiating the syntax walker's traversal.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_10

LANGUAGE: C#
CODE:
```
SyntaxTree tree = CSharpSyntaxTree.ParseText(codeText);
SyntaxNode root = tree.GetRoot();
```

----------------------------------------

TITLE: IHostIoCompletionManager::CreateIoCompletionPort API Documentation
DESCRIPTION: Detailed API documentation for the IHostIoCompletionManager::CreateIoCompletionPort method, including its parameters, return values, and remarks on its usage within the .NET Framework hosting environment.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/hosting/ihostiocompletionmanager-createiocompletionport-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Method: CreateIoCompletionPort
Signature: HRESULT CreateIoCompletionPort ([out] HANDLE *phPort)

Parameters:
  phPort:
    Type: HANDLE*
    Direction: out
    Description: A pointer to a handle to the newly created I/O completion port, or 0 (zero), if the port could not be created.

Return Values (HRESULT):
  S_OK: CreateIoCompletionPort returned successfully.
  HOST_E_CLRNOTAVAILABLE: The common language runtime (CLR) has not been loaded into a process, or the CLR is in a state in which it cannot run managed code or process the call successfully.
  HOST_E_TIMEOUT: The call timed out.
  HOST_E_NOT_OWNER: The caller does not own the lock.
  HOST_E_ABANDONED: An event was canceled while a blocked thread or fiber was waiting on it.
  E_FAIL: An unknown catastrophic failure occurred. When a method returns E_FAIL, the CLR is no longer usable within the process. Subsequent calls to hosting methods return HOST_E_CLRNOTAVAILABLE.
  E_OUTOFMEMORY: Not enough memory was available to allocate the requested resource.

Remarks:
  The CLR calls the CreateIoCompletionPort method to request that the host create a new I/O completion port. It binds I/O operations to this port through a call to the IHostIoCompletionManager::Bind method. The host reports status back to the CLR by calling ICLRIoCompletionManager::OnComplete.
```

----------------------------------------

TITLE: Initializing SemanticModel in C# Syntax Rewriter
DESCRIPTION: This snippet declares a private, read-only `SemanticModel` field and initializes it via the constructor of the `TypeInferenceRewriter`. The `SemanticModel` is crucial for performing semantic analysis, such as determining type information, which is necessary for the type inference refactoring.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-transformation.md#_snippet_6

LANGUAGE: C#
CODE:
```
private readonly SemanticModel SemanticModel;

public TypeInferenceRewriter(SemanticModel semanticModel) : base()
{
    this.SemanticModel = semanticModel;
}
```

----------------------------------------

TITLE: System.Delegate.CreateDelegate Method Overloads API Documentation
DESCRIPTION: Detailed documentation for the System.Delegate.CreateDelegate method overloads, explaining parameter binding, return type compatibility, and behavior with static/instance methods and null first arguments.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/runtime-libraries/system-delegate-createdelegate.md#_snippet_2

LANGUAGE: APIDOC
CODE:
```
System.Delegate.CreateDelegate Method Overloads:
  - CreateDelegate(type: System.Type, firstArgument: System.Object, method: System.Reflection.MethodInfo)
  - CreateDelegate(type: System.Type, firstArgument: System.Object, method: System.Reflection.MethodInfo, throwOnFailure: System.Boolean)

Description:
  These overloads create a delegate that represents the specified static or instance method.
  The 'throwOnFailure' parameter in the second overload controls whether an exception is thrown if binding fails.

Parameter: firstArgument (System.Object)
  Purpose: The first argument of the method the delegate represents.
  Behavior:
    - If supplied, it is passed to 'method' every time the delegate is invoked.
    - The delegate is said to be "closed over its first argument".
    - If 'method' is static: The argument list supplied when invoking the delegate includes all parameters except the first.
    - If 'method' is an instance method: 'firstArgument' is passed to the hidden instance parameter (e.g., 'this' in C#, 'Me' in Visual Basic).
  Compatibility:
    - If 'firstArgument' is supplied, the first parameter of 'method' must be a reference type, and 'firstArgument' must be compatible with that type.
    - IMPORTANT: If 'method' is static and its first parameter is of type System.Object or System.ValueType, 'firstArgument' can be a value type and is automatically boxed.

Return Type Compatibility:
  - The return type of 'method' must be assignable to the return type of 'type' (the delegate type).

Behavior with null firstArgument:

  If firstArgument is null reference and method is an instance method:
    - If the signature of 'type' explicitly includes the hidden first parameter of 'method':
      - Delegate represents an "open instance method".
      - When invoked, the first argument in the argument list is passed to the hidden instance parameter of 'method'.
    - If the signatures of 'method' and 'type' match (all parameter types compatible):
      - Delegate is "closed over a null reference".
      - Invoking is like calling an instance method on a null instance.

  If firstArgument is null reference and method is static:
    - If the signature of 'method' and 'type' match (all parameter types compatible):
      - Delegate represents an "open static method".
      - This is the most common case for static methods.
      - Performance note: For this case, System.Delegate.CreateDelegate(System.Type, System.Reflection.MethodInfo) overload can offer slightly better performance.
    - If the signature of 'type' begins with the second parameter of 'method' and the rest of the parameter types are compatible:
      - Delegate is "closed over a null reference".
      - When invoked, a null reference is passed to the first parameter of 'method'.
```

----------------------------------------

TITLE: Injecting INavigationService into a Constructor in C#
DESCRIPTION: This example shows how to resolve the `INavigationService` interface by injecting it into the constructor of a class, such as `AppShell`. This allows the class to access the navigation service's methods without direct instantiation, adhering to dependency inversion principles.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/navigation.md#_snippet_3

LANGUAGE: csharp
CODE:
```
public AppShell(INavigationService navigationService)
```

----------------------------------------

TITLE: System.Activities.WorkflowApplication API References for Custom Instance Stores
DESCRIPTION: Key API members from the System.Activities.WorkflowApplication class that are essential for implementing and configuring a custom workflow instance store. These include properties for assigning the store and methods for handling persistence.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/windows-workflow-foundation/how-to-create-a-custom-instance-store.md#_snippet_16

LANGUAGE: APIDOC
CODE:
```
System.Activities.WorkflowApplication.InstanceStore
  Type: Property
  Description: Assign an instance of your custom instance store to this property to integrate it with the workflow application.

System.Activities.WorkflowApplication.PersistableIdle
  Type: Method
  Description: Implement this method to define custom logic that executes when the workflow application becomes persistable and idle, typically used for saving workflow state.
```

----------------------------------------

TITLE: ML.NET AutoML API Reference
DESCRIPTION: Key classes, methods, and concepts used in ML.NET AutoML for defining pipelines, configuring experiments, and running training.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/machine-learning/how-to-guides/how-to-use-the-automl-api.md#_snippet_8

LANGUAGE: APIDOC
CODE:
```
SweepablePipeline:
  Description: A collection of SweepableEstimator objects, defining data processing and machine learning steps.
SweepableEstimator:
  Description: An ML.NET Estimator extended with a SearchSpace for hyperparameter tuning.
AutoCatalog.Featurizer:
  Description: Convenience API to build a sweepable pipeline of data processing estimators based on column information, automating data preprocessing.
  Output: A single column numerical feature vector.
AutoMLExperiment:
  Description: A container for AutoML trials, representing a collection of TrialResult objects.
  Methods:
    CreateExperiment(): Initializes a new AutoML experiment.
    SetPipeline(pipeline: SweepablePipeline): Configures the sweepable pipeline for the experiment.
    SetRegressionMetric(metric: RegressionMetric, labelColumn: string): Sets the metric to optimize during training (e.g., RSquared).
    SetTrainingTimeInSeconds(seconds: int): Defines the maximum training duration in seconds.
    SetDataset(data: IDataView): Provides the training and validation datasets.
    RunAsync(): Asynchronously starts the AutoML experiment and returns the best TrialResult.
MLContext.Log:
  Description: An event on MLContext that can be subscribed to track the progress and messages of AutoML experiments.
TrialResult:
  Description: Represents the outcome of an AutoML trial, typically containing the best model found during training.
AutoML Tasks (via AutoCatalog):
  BinaryClassification: For binary classification problems.
  MultiClassification: For multi-class classification problems.
  Regression: For regression problems (predicting numerical values).
```

----------------------------------------

TITLE: F# Language Keyword Reference
DESCRIPTION: Detailed documentation for each F# keyword, explaining its purpose and providing links to further information within the F# documentation.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/language-reference/keyword-reference.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
Keyword: `abstract`
  Description: Indicates a method that either has no implementation in the type in which it is declared or that is virtual and has a default implementation.
  Links:
    - Members
    - Abstract Classes

Keyword: `and`
  Description: Used in mutually recursive bindings and records, in property declarations, and with multiple constraints on generic parameters.
  Links:
    - `let` Bindings
    - Records
    - Members
    - Constraints

Keyword: `as`
  Description: Used to give the current class object an object name. Also used to give a name to a whole pattern within a pattern match.
  Links:
    - Classes
    - Pattern Matching

Keyword: `assert`
  Description: Used to verify code during debugging.
  Links:
    - Assertions

Keyword: `base`
  Description: Used as the name of the base class object.
  Links:
    - Classes
    - Inheritance

Keyword: `begin`
  Description: In verbose syntax, indicates the start of a code block.
  Links:
    - Verbose Syntax

Keyword: `class`
  Description: In verbose syntax, indicates the start of a class definition.
  Links:
    - Classes

Keyword: `default`
  Description: Indicates an implementation of an abstract method; used together with an abstract method declaration to create a virtual method.
  Links:
    - Members

Keyword: `delegate`
  Description: Used to declare a delegate.
  Links:
    - Delegates

Keyword: `do`
  Description: Used in looping constructs or to execute imperative code.
  Links:
    - `do` Bindings
    - Loops: `for...to` Expression
    - Loops: `for...in` Expression
    - Loops: `while...do` Expression

Keyword: `done`
  Description: In verbose syntax, indicates the end of a block of code in a looping expression.
  Links:
    - Verbose Syntax

Keyword: `downcast`
  Description: Used to convert to a type that is lower in the inheritance chain.
  Links:
    - Casting and Conversions

Keyword: `downto`
  Description: In a `for` expression, used when counting in reverse.
  Links:
    - Loops: `for...to` Expression

Keyword: `elif`
  Description: Used in conditional branching. A short form of `else if`.
  Links:
    - Conditional Expressions: `if...then...else`

Keyword: `else`
  Description: Used in conditional branching.
  Links:
    - Conditional Expressions: `if...then...else`

Keyword: `end`
  Description: In type definitions and type extensions, indicates the end of a section of member definitions.\n\nIn verbose syntax, used to specify the end of a code block that starts with the `begin` keyword.
  Links:
    - Structs
    - Discriminated Unions
    - Records
    - Type Extensions
    - Verbose Syntax

Keyword: `exception`
  Description: Used to declare an exception type.
  Links:
    - Exception Handling
    - Exception Types

Keyword: `extern`
  Description: Indicates that a declared program element is defined in another binary or assembly.
  Links:
    - External Functions

Keyword: `false`
  Description: Used as a Boolean literal.
  Links:
    - Primitive Types

Keyword: `finally`
  Description: Used together with `try` to introduce a block of code that executes regardless of whether an exception occurs.
  Links:
    - Exceptions: The `try...finally` Expression

Keyword: `fixed`
  Description: Used to \"pin\" a pointer on the stack to prevent it from being garbage collected.
  Links:
    - Fixed

Keyword: `for`
  Description: Used in looping constructs.
  Links:
    - Loops: `for...to` Expression
    - Loops: for...in Expression

Keyword: `fun`
  Description: Used in lambda expressions, also known as anonymous functions.
  Links:
    - Lambda Expressions: The `fun` Keyword

Keyword: `function`
  Description: Used as a shorter alternative to the `fun` keyword and a `match` expression in a lambda expression that has pattern matching on a single argument.
  Links:
    - Match Expressions
    - Lambda Expressions: The fun Keyword

Keyword: `global`
  Description: Used to reference the top-level .NET namespace.
  Links:
    - Namespaces

Keyword: `if`
  Description: Used in conditional branching constructs.
  Links:
    - Conditional Expressions: `if...then...else`

Keyword: `in`
  Description: Used for sequence expressions and, in verbose syntax, to separate expressions from bindings.
  Links:
    - Loops: for...in Expression
    - Verbose Syntax

Keyword: `inherit`
  Description: Used to specify a base class or base interface.
  Links:
    - Inheritance
```

----------------------------------------

TITLE: Iterator Get Accessor Example in Visual Basic
DESCRIPTION: This example illustrates how to implement an iterator as a `Get` accessor within a property declaration. The `Iterator` modifier is applied to the property, and the `Get` accessor uses `Yield` to return a sequence of integer values (0, 1, 2).
SOURCE: https://github.com/dotnet/docs/blob/main/docs/visual-basic/language-reference/modifiers/iterator.md#_snippet_1

LANGUAGE: Visual Basic
CODE:
```
Public Class Class1
    Private Property My  As Integer()
        Iterator Get
            Yield 0
            Yield 1
            Yield 2
        End Get
    End Property
End Class
```

----------------------------------------

TITLE: C# Example: Registering and Running a Testing Framework
DESCRIPTION: This C# snippet shows the entry point for an application, demonstrating how to create a `TestApplicationBuilder`, register the custom testing framework using the `AddTestingFramework` extension method, build the test application, and finally run it.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/testing/microsoft-testing-platform-architecture-extensions.md#_snippet_6

LANGUAGE: csharp
CODE:
```
var testApplicationBuilder = await TestApplication.CreateBuilderAsync(args);
// Register the testing framework
testApplicationBuilder.AddTestingFramework();
using var testApplication = await testApplicationBuilder.BuildAsync();
return await testApplication.RunAsync();
```

----------------------------------------

TITLE: Get function API Reference
DESCRIPTION: Detailed API documentation for the unmanaged Get function, which retrieves a specified property value from an IWbemClassObject instance. Includes syntax, parameter descriptions, return values, and remarks.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/wmi/get.md#_snippet_0

LANGUAGE: cpp
CODE:
```
HRESULT Get (
   [in] int               vFunc,
   [in] IWbemClassObject* ptr,
   [in] LPCWSTR           wszName,
   [in] LONG              lFlags,
   [out] VARIANT*         pVal,
   [out] CIMTYPE*         pvtType,
   [out] LONG*            plFlavor
);
```

LANGUAGE: APIDOC
CODE:
```
Function: Get
Description: Retrieves the specified property value if it exists.

Parameters:
  vFunc: [in] int - This parameter is unused.
  ptr: [in] IWbemClassObject* - A pointer to an IWbemClassObject instance.
  wszName: [in] LPCWSTR - The name of the property.
  lFlags: [in] LONG - Reserved. This parameter must be 0.
  pVal: [out] VARIANT* - If the function returns successfully, contains the value of the wszName property. The pval argument is assigned the correct type and value for the qualifier.
  pvtType: [out] CIMTYPE* - If the function returns successfully, contains a CIM-type constant that indicates the property type. Its value can also be null.
  plFlavor: [out] LONG* - If the function returns successfully, receives information about the origin of the property. Its value can be null, or one of the following WBEM_FLAVOR_TYPE constants defined in the WbemCli.h header file:
    WBEM_FLAVOR_ORIGIN_SYSTEM: 0x40 - The property is a standard system property.
    WBEM_FLAVOR_ORIGIN_PROPAGATED: 0x20 - For a class: The property is inherited from the parent class. For an instance: The property, while inherited from the parent class, has not been modified by the instance.
    WBEM_FLAVOR_ORIGIN_LOCAL: 0 - For a class: The property belongs to the derived class. For an instance: The property is modified by the instance; that is, a value was supplied, or a qualifier was added or modified.

Return Value (HRESULT):
  WBEM_E_FAILED: 0x80041001 - There has been a general failure.
  WBEM_E_INVALID_PARAMETER: 0x80041008 - One or more parameters are not valid.
  WBEM_E_NOT_FOUND: 0x80041002 - The specified property was not found.
  WBEM_E_OUT_OF_MEMORY: 0x80041006 - Not enough memory is available to complete the operation.
  WBEM_S_NO_ERROR: 0 - The function call was successful.

Remarks:
  This function wraps a call to the IWbemClassObject::Get method.
  The Get function can also return system properties.
  The pVal argument is assigned the correct type and value for the qualifier and the COM VariantInit function.
```

----------------------------------------

TITLE: Additional Resources for Owned Entity Types
DESCRIPTION: This section provides a list of external resources, including articles, books, and GitHub discussions, for further reading on value objects, domain-driven design, and owned entity types in Entity Framework Core.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects.md#_snippet_14

LANGUAGE: APIDOC
CODE:
```
Additional Resources:
  - Martin Fowler. ValueObject pattern: https://martinfowler.com/bliki/ValueObject.html
  - Eric Evans. Domain-Driven Design: Tackling Complexity in the Heart of Software. (Book)
  - Vaughn Vernon. Implementing Domain-Driven Design. (Book)
  - Owned Entity Types: https://learn.microsoft.com/ef/core/modeling/owned-entities
  - Shadow Properties: https://learn.microsoft.com/ef/core/modeling/shadow-properties
  - Complex types and/or value objects. Discussion in the EF Core GitHub repo (Issues tab): https://github.com/dotnet/efcore/issues/246
  - ValueObject.cs. Base value object class in eShopOnContainers: https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Services/Ordering/Ordering.Domain/SeedWork/ValueObject.cs
  - ValueObject.cs. Base value object class in CSharpFunctionalExtensions: https://github.com/vkhorikov/CSharpFunctionalExtensions/blob/master/CSharpFunctionalExtensions/ValueObject/ValueObject.cs
  - Address class. Sample value object class in eShopOnContainers: https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Services/Ordering/Ordering.Domain/AggregatesModel/OrderAggregate/Address.cs
```

----------------------------------------

TITLE: Finding Main Method Declaration in C# Roslyn Class
DESCRIPTION: This code casts the class declaration to `ClassDeclarationSyntax` and then accesses its members to locate the `Main` method. It prints the number of members in the class and the kind of the first member, finally casting it to `MethodDeclarationSyntax` for further analysis.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_6

LANGUAGE: C#
CODE:
```
ClassDeclarationSyntax programClass = (ClassDeclarationSyntax)classDeclaration;
Console.WriteLine($"There are {programClass.Members.Count} members declared in the Program class.");
MemberDeclarationSyntax mainMethod = programClass.Members[0];
Console.WriteLine($"The first member is a {mainMethod.Kind()}.");
MethodDeclarationSyntax mainMethodDeclaration = (MethodDeclarationSyntax)mainMethod;
```

----------------------------------------

TITLE: F# Option Type Properties and Methods
DESCRIPTION: Documentation for the properties and methods available on the F# option type, including their purpose and return types.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/language-reference/options.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
Option Type:
  Properties and Methods:
    None:
      Type: 'T option
      Description: A static member that creates an option value that has the None value.
    IsNone:
      Type: bool
      Description: Returns true if the option has the None value.
    IsSome:
      Type: bool
      Description: Returns true if the option has a value that is not None.
    Some:
      Type: 'T option
      Description: A static member that creates an option that has a value that is not None.
    Value:
      Type: 'T
      Description: Returns the underlying value, or throws a System.NullReferenceException if the value is None.
```

----------------------------------------

TITLE: C# Read/Write Indexer Example
DESCRIPTION: Demonstrates a basic C# indexer with both `get` and `set` accessors, allowing instances of `SampleCollection<T>` to be accessed like an array. Includes a `Main` method for usage example.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/programming-guide/indexers/index.md#_snippet_0

LANGUAGE: csharp
CODE:
```
using System;
using System.Collections.Generic;

public class SampleCollection<T>
{
    // Declare an array to store the data elements.
    private T[] arr = new T[100];

    // Define the indexer to allow client code to use [] notation.
    public T this[int i]
    {
        get
        {
            // This is the get accessor.
            return arr[i];
        }
        set
        {
            // This is the set accessor.
            arr[i] = value;
        }
    }
}

public class Program
{
    public static void Main()
    {
        SampleCollection<string> stringCollection = new SampleCollection<string>();
        stringCollection[0] = "Hello, World";
        stringCollection[1] = "C# Indexers";

        Console.WriteLine(stringCollection[0]);
        Console.WriteLine(stringCollection[1]);
    }
}
```

----------------------------------------

TITLE: Observer Pattern Exception Handling Best Practices (APIDOC)
DESCRIPTION: Recommendations for providers on handling exceptions and using the `OnError` method, clarifying its informational nature and the implications for further notifications and unsubscription.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/standard/events/observer-design-pattern-best-practices.md#_snippet_2

LANGUAGE: APIDOC
CODE:
```
Provider Responsibilities for Exceptions:
  Handle its own exceptions if specific requirements exist.
  Do not expect or require observers to handle exceptions in a particular way.
  Call IObserver<T>.OnError(error: Exception) when an exception compromises the provider's ability to provide updates.
    Information about the exception can be passed to the observer.
    No need to notify observers for all exceptions.

Post-Error/Completion Behavior:
  Once OnError or OnCompleted is called, there should be no further notifications.
  Provider can unsubscribe its observers after calling OnError or OnCompleted.
  Observers can unsubscribe themselves at any time (before or after OnError/OnCompleted).
  In single-threaded applications, Dispose implementation should validate object reference and collection membership before removal.
  In multi-threaded applications, use thread-safe collection objects for subscriber management.
```

----------------------------------------

TITLE: Examining Root Node Properties in C# Roslyn
DESCRIPTION: This snippet displays various properties of the root node of the syntax tree, including its kind, the count of its members, and the number and names of its using directives. It helps in understanding the top-level structure of the parsed code.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_3

LANGUAGE: C#
CODE:
```
Console.WriteLine($"The tree is a {root.Kind()} node.");
Console.WriteLine($"The tree has {root.Members.Count} elements in it.");
Console.WriteLine($"The tree has {root.Usings.Count} using directives.");
foreach (UsingDirectiveSyntax element in root.Usings)
{
    Console.WriteLine($"\t{element.Name}");
}
```

----------------------------------------

TITLE: IHostMemoryManager::VirtualQuery Method API Documentation
DESCRIPTION: Detailed API documentation for the IHostMemoryManager::VirtualQuery method, including its parameters, return values, and usage remarks.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/hosting/ihostmemorymanager-virtualquery-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Method: IHostMemoryManager::VirtualQuery
Signature: HRESULT VirtualQuery (
    [in]  void*    lpAddress,
    [out] void*    lpBuffer,
    [in]  SIZE_T   dwLength,
    [out] SIZE_T*  pResult
)
Parameters:
  lpAddress: [in] A pointer to the address in virtual memory to be queried.
  lpBuffer: [out] A pointer to a structure that contains information about the specified memory region.
  dwLength: [in] The size, in bytes, of the buffer that lpBuffer points to.
  pResult: [out] A pointer to the number of bytes returned by the information buffer.
Return Values:
  S_OK: VirtualQuery returned successfully.
  HOST_E_CLRNOTAVAILABLE: The common language runtime (CLR) has not been loaded into a process, or the CLR is in a state in which it cannot run managed code or process the call successfully.
  HOST_E_TIMEOUT: The call timed out.
  HOST_E_NOT_OWNER: The caller does not own the lock.
  HOST_E_ABANDONED: An event was canceled while a blocked thread or fiber was waiting on it.
  E_FAIL: An unknown catastrophic failure occurred. When a method returns E_FAIL, the CLR is no longer usable within the process. Subsequent calls to hosting methods return HOST_E_CLRNOTAVAILABLE.
Remarks:
  VirtualQuery provides information about a range of pages in the virtual address space of the calling process. This implementation sets the value of the pResult parameter to the number of bytes returned in the information buffer, and returns an HRESULT value. In the Win32 VirtualQuery function, the return value is the buffer size. For more information, see the Windows Platform documentation.
  The operating system's implementation of VirtualQuery does not incur deadlock and can run to completion with random threads suspended in user code. Use great caution when implementing a hosted version of this method.
```

----------------------------------------

TITLE: Finding Class Declaration in C# Roslyn Namespace
DESCRIPTION: This snippet casts the first member (expected to be a `NamespaceDeclarationSyntax`) and then examines its members to find the class declaration within that namespace. It prints the count of members in the namespace and the kind of the first member, which should be the class.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_5

LANGUAGE: C#
CODE:
```
NamespaceDeclarationSyntax namespaceDeclaration = (NamespaceDeclarationSyntax)firstMember;
Console.WriteLine($"There are {namespaceDeclaration.Members.Count} members declared in this namespace.");
MemberDeclarationSyntax classDeclaration = namespaceDeclaration.Members[0];
Console.WriteLine($"The first member is a {classDeclaration.Kind()}.");
```

----------------------------------------

TITLE: C# Complete Program.cs File for Web API Client
DESCRIPTION: Presents the full `Program.cs` file for the .NET web API client, demonstrating asynchronous HTTP requests, JSON deserialization, and formatted console output of repository details including custom date conversions.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/tutorials/console-webapiclient.md#_snippet_22

LANGUAGE: C#
CODE:
```
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPIClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var repositories = await ProcessRepositoriesAsync(client);

            foreach (var repo in repositories)
            {
                Console.WriteLine($"Name: {repo.Name}");
                Console.WriteLine($"Homepage: {repo.Homepage}");
                Console.WriteLine($"GitHub: {repo.GitHubHomeUrl}");
                Console.WriteLine($"Description: {repo.Description}");
                Console.WriteLine($"Watchers: {repo.Watchers:#,0}");
                Console.WriteLine($"Last push: {repo.LastPush}");
                Console.WriteLine();
            }
        }

        static async Task<List<Repository>> ProcessRepositoriesAsync(HttpClient client)
        {
            await using Stream stream =
                await client.GetStreamAsync("https://api.github.com/orgs/dotnet/repos");
            var repositories =
                await JsonSerializer.DeserializeAsync<List<Repository>>(stream);
            return repositories ?? new();
        }
    }
}
```

----------------------------------------

TITLE: API Documentation for get / nth Function
DESCRIPTION: Documents the 'get' or 'nth' function, which returns an element from a collection given its index.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/language-reference/fsharp-collection-types.md#_snippet_6

LANGUAGE: APIDOC
CODE:
```
Function: get / nth
Description: Returns an element from the collection given its index.
Complexity:
  Array: O(1)
  List: O(N)
  Seq: O(N)
  Set: -
  Map: -
```

----------------------------------------

TITLE: API Documentation for System.Text.Json Namespace
DESCRIPTION: Lists serialization-related constructors and methods for types within the System.Text.Json namespace.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/compatibility/core-libraries/8.0/obsolete-apis-with-custom-diagnostics.md#_snippet_33

LANGUAGE: APIDOC
CODE:
```
System.Text.Json.JsonException
  #ctor(System.Runtime.Serialization.SerializationInfo, System.Runtime.Serialization.StreamingContext)
  GetObjectData(System.Runtime.Serialization.SerializationInfo, System.Runtime.Serialization.StreamingContext)
```

----------------------------------------

TITLE: Creating a Test C# Compilation Programmatically
DESCRIPTION: This snippet defines the `CreateTestCompilation` method, which constructs a `CSharpCompilation` object programmatically. It embeds a C# source code string, adds necessary assembly references, and parses the source into a `SyntaxTree`, providing a self-contained environment for testing the `TypeInferenceRewriter`.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-transformation.md#_snippet_15

LANGUAGE: C#
CODE:
```
private static Compilation CreateTestCompilation()
{
    string source = "\n    using System;\n    using System.Collections.Generic;\n    using System.Linq;\n    using System.Text;\n    using System.Threading.Tasks;\n    using Microsoft.CodeAnalysis;\n    using Microsoft.CodeAnalysis.CSharp;\n    using Microsoft.CodeAnalysis.CSharp.Syntax;\n\n    namespace ConsoleApplication1\n    {\n        class Program\n        {\n            static void Main(string[] args)\n            {\n                const int i = 5;\n                const string s = \"hello\";\n                int j = i;\n                int k = i + j;\n                string str = s;\n                string str2 = s + str;\n                List<int> list = new List<int>();\n                var z = list;\n            }\n        }\n    }";
            return CSharpCompilation.Create("TransformationCS")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(source));
}
```

----------------------------------------

TITLE: IMonitor Interface Lifecycle Events
DESCRIPTION: Documentation for the `Microsoft.ML.AutoML.IMonitor` interface, detailing its four lifecycle methods used for monitoring AutoML experiment progress.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/machine-learning/how-to-guides/how-to-use-the-automl-api.md#_snippet_29

LANGUAGE: APIDOC
CODE:
```
Microsoft.ML.AutoML.IMonitor Interface:
  ReportBestTrial(): Reports the best trial result.
  ReportCompletedTrial(result: TrialResult): Reports a completed trial result.
  ReportFailTrial(settings: TrialSettings, exception: Exception): Reports a failed trial.
  ReportRunningTrial(setting: TrialSettings): Reports a trial that is currently running.
```

----------------------------------------

TITLE: XML Example: Subscribe to Workflow Instance Started State
DESCRIPTION: Demonstrates how to configure a `workflowInstanceQuery` to subscribe to the `Started` state of a workflow instance, enabling tracking of workflow start events.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/configure-apps/file-schema/windows-workflow-foundation/workflowinstancequery.md#_snippet_1

LANGUAGE: XML
CODE:
```
<workflowInstanceQueries>  
    <workflowInstanceQuery>  
      <states>  
        <state name="Started"/>  
      </states>  
    </workflowInstanceQuery>  
</workflowInstanceQueries>
```

----------------------------------------

TITLE: Finding First Member of Root Node in C# Roslyn
DESCRIPTION: This code retrieves the first member declared directly within the root node of the syntax tree and prints its kind. In the context of the 'Hello World!' program, this typically identifies the namespace declaration.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_4

LANGUAGE: C#
CODE:
```
MemberDeclarationSyntax firstMember = root.Members[0];
Console.WriteLine($"The first member is a {firstMember.Kind()}.");
```

----------------------------------------

TITLE: Next Function API Documentation
DESCRIPTION: Detailed API documentation for the Next function, including its signature, parameters, return values, and usage remarks for property origin flavors.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/wmi/next.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Next function:
  Signature: HRESULT Next (
    [in] int               vFunc,
    [in] IWbemClassObject* ptr,
    [in] LONG              lFlags,
    [out] BSTR*            pstrName,
    [out] VARIANT*         pVal,
    [out] CIMTYPE*         pvtType,
    [out] LONG*            plFlavor
  )

  Parameters:
    vFunc: [in] int
      Description: This parameter is unused.
    ptr: [in] IWbemClassObject*
      Description: A pointer to an IWbemClassObject instance.
    lFlags: [in] LONG
      Description: Reserved. This parameter must be 0.
    pstrName: [out] BSTR*
      Description: A new BSTR that contains the property name. You can set this parameter to null if the name is not required.
    pVal: [out] VARIANT*
      Description: A VARIANT filled with the value of the property. You can set this parameter to null if the value is not required. If the function returns an error code, the VARIANT passed to pVal is left unmodified.
    pvtType: [out] CIMTYPE*
      Description: A pointer to a CIMTYPE variable (a LONG into which the type of the property is placed). The value of this property can be a VT_NULL_VARIANT, in which case it is necessary to determine the actual type of the property. This parameter can also be null.
    plFlavor: [out] LONG*
      Description: null, or a value that receives information on the origin of the property. See the Remarks section for possible values.

  Return Values (WbemCli.h):
    WBEM_E_FAILED: 0x80041001
      Description: There has been a general failure.
    WBEM_E_INVALID_PARAMETER: 0x80041008
      Description: A parameter is invalid.
    WBEM_E_UNEXPECTED: 0x8004101d
      Description: There was no call to the BeginEnumeration function.
    WBEM_E_OUT_OF_MEMORY: 0x80041006
      Description: Not enough memory is available to begin a new enumeration.
    WBEM_E_TRANSPORT_FAILURE: 0x80041015
      Description: The remote procedure call between the current process and Windows Management failed.
    WBEM_S_NO_ERROR: 0
      Description: The function call was successful.
    WBEM_S_NO_MORE_DATA: 0x40005
      Description: There are no more properties in the enumeration.

  Remarks (plFlavor values):
    WBEM_FLAVOR_ORIGIN_SYSTEM: 0x40
      Description: The property is a standard system property.
    WBEM_FLAVOR_ORIGIN_PROPAGATED: 0x20
      Description: For a class: The property is inherited from the parent class. \n For an instance: The property, while inherited from the parent class, has not been modified by the instance.
    WBEM_FLAVOR_ORIGIN_LOCAL: 0
      Description: For a class: The property belongs to the derived class. \n For an instance: The property is modified by the instance; that is, a value was supplied, or a qualifier was added or modified.
```

----------------------------------------

TITLE: Extracting Type Information for C# Declarations
DESCRIPTION: This snippet extracts the `VariableDeclaratorSyntax` from the local declaration and uses the `SemanticModel` to obtain `TypeInfo` for both the initializer's value and the declared type. This information is crucial for comparing types and determining if type inference (`var`) can be safely applied.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-transformation.md#_snippet_9

LANGUAGE: C#
CODE:
```
VariableDeclaratorSyntax variable = node.Declaration.Variables[0];
TypeInfo varType = SemanticModel.GetTypeInfo(variable.Initializer.Value);
TypeInfo declarationType = SemanticModel.GetTypeInfo(node.Declaration.Type);
```

----------------------------------------

TITLE: Host a Workflow and Resume a Bookmark with User Input (C#)
DESCRIPTION: This C# example demonstrates how to create and host a workflow that utilizes the custom `ReadLine` activity. The host application initializes the workflow, starts it, and then collects user input from the console. It then uses `WorkflowApplication.ResumeBookmark` to pass the collected data back to the workflow, resuming its execution. The example also illustrates the implicit synchronization behavior of `ResumeBookmark`.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/windows-workflow-foundation/bookmarks.md#_snippet_1

LANGUAGE: csharp
CODE:
```
Variable<string> name = new Variable<string>
{
    Name = "name"
};
  
Activity wf = new Sequence
{
    Variables =
    {
        name
    },
    Activities =
    {
        new WriteLine()
        {
            Text = "What is your name?"
        },
        new ReadLine()
        {
            BookmarkName = "UserName",
            Result = name
        },
        new WriteLine()
        {
            Text = new InArgument<string>((env) => "Hello, " + name.Get(env))
        }
    }
};
  
AutoResetEvent syncEvent = new AutoResetEvent(false);
  
// Create the WorkflowApplication using the desired
// workflow definition.
WorkflowApplication wfApp = new WorkflowApplication(wf);
  
// Handle the desired lifecycle events.
wfApp.Completed = delegate(WorkflowApplicationCompletedEventArgs e)
{
    // Signal the host that the workflow is complete.
    syncEvent.Set();
};
  
// Start the workflow.
wfApp.Run();
  
// Collect the user's name and resume the bookmark.
// Bookmark resumption only occurs when the workflow
// is idle. If a call to ResumeBookmark is made and the workflow
// is not idle, ResumeBookmark blocks until the workflow becomes
// idle before resuming the bookmark.
wfApp.ResumeBookmark("UserName", Console.ReadLine());
  
// Wait for Completed to arrive and signal that
// the workflow is complete.
syncEvent.WaitOne();
```

----------------------------------------

TITLE: Core Hosted Service Interfaces and Methods
DESCRIPTION: API documentation for key interfaces and classes related to hosted services in .NET, including IHostedService, BackgroundService, and IHostApplicationLifetime, outlining their primary methods and important usage notes derived from the text.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/extensions/workers.md#_snippet_3

LANGUAGE: APIDOC
CODE:
```
Microsoft.Extensions.Hosting.IHostedService Interface:
  StartAsync(cancellationToken: System.Threading.CancellationToken): System.Threading.Tasks.Task
    Description: Called when the host starts, allowing the service to perform startup logic.
  StopAsync(cancellationToken: System.Threading.CancellationToken): System.Threading.Tasks.Task
    Description: Called when the host stops, allowing the service to perform shutdown logic.

Microsoft.Extensions.Hosting.BackgroundService Class:
  Inherits: IHostedService
  StartAsync(cancellationToken: System.Threading.CancellationToken): System.Threading.Tasks.Task
    Note: When overriding, you must call and await the base class method to ensure proper startup.
  StopAsync(cancellationToken: System.Threading.CancellationToken): System.Threading.Tasks.Task
    Note: When overriding, you must call and await the base class method to ensure proper shutdown.
  ExecuteAsync(cancellationToken: System.Threading.CancellationToken): System.Threading.Tasks.Task
    Description: Abstract method to be implemented by subclasses for the main background execution logic.

Microsoft.Extensions.Hosting.IHostApplicationLifetime Interface:
  StopApplication(): void
    Description: Signals to the host that it should stop the application. Essential for short-lived services that need to terminate the host.
```

----------------------------------------

TITLE: Define a custom message type with ValueChangedMessage
DESCRIPTION: This code example demonstrates how to define a custom message, `AddProductMessage`, by inheriting from `ValueChangedMessage<T>`. This approach ensures compile-time type safety and refactoring support, allowing both publishers and subscribers to expect messages of a specific, consistent type.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/communicating-between-components.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class AddProductMessage : ValueChangedMessage<int>
{
    public AddProductMessage(int count) : base(count)
    {
    }
}
```

----------------------------------------

TITLE: CreateVersionStringFromModule API Reference
DESCRIPTION: Detailed API documentation for the CreateVersionStringFromModule function, including parameter descriptions, return values, and usage remarks.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/debugging/createversionstringfrommodule-function-for-silverlight.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
CreateVersionStringFromModule Function:
  Purpose: Creates a version string from a common language runtime (CLR) path in a target process.

  Parameters:
    pidDebuggee:
      Direction: [in]
      Type: DWORD
      Description: Identifier of the process in which the target CLR is loaded.
    szModuleName:
      Direction: [in]
      Type: LPCWSTR
      Description: Full or relative path to the target CLR that is loaded in the process.
    pBuffer:
      Direction: [out]
      Type: LPWSTR (size_is(cchBuffer), length_is(*pdwLength))
      Description: Return buffer for storing the version string for the target CLR.
    cchBuffer:
      Direction: [in]
      Type: DWORD
      Description: Size of pBuffer.
    pdwLength:
      Direction: [out]
      Type: DWORD*
      Description: Length of the version string returned by pBuffer.

  Return Values:
    S_OK: The version string for the target CLR was successfully returned in pBuffer.
    E_INVALIDARG: szModuleName is null, or either pBuffer or cchBuffer is null. pBuffer and cchBuffer must both be null or non-null.
    HRESULT_FROM_WIN32(ERROR_INSUFFICIENT_BUFFER): pdwLength is greater than cchBuffer. This may be an expected result if you have passed null for both pBuffer and cchBuffer, and queried the necessary buffer size by using pdwLength.
    HRESULT_FROM_WIN32(ERROR_MOD_NOT_FOUND): szModuleName does not contain a path to a valid CLR in the target process.
    E_FAIL (or other E_ return codes): pidDebuggee does not refer to a valid process, or other failure.

  Remarks:
    This function accepts a CLR process that is identified by pidDebuggee and a string path that is specified by szModuleName. The version string is returned in the buffer that pBuffer points to. This string is opaque to the function user; that is, there is no intrinsic meaning in the version string itself. It is used solely in the context of this function and the CreateDebuggingInterfaceFromVersion function.
    This function should be called twice. When you call it the first time, pass null for both pBuffer and cchBuffer. When you do this, the size of the buffer necessary for pBuffer will be returned in pdwLength. You can then call the function a second time, and pass the buffer in pBuffer and its size in cchBuffer.
```

----------------------------------------

TITLE: New .NET 6.0.5+ Console Logger Output (Multiline)
DESCRIPTION: Example of the console logger output in `aspnet` container images starting from .NET 6.0.5, showing simple, multiline, human-readable formatting due to the `Logging__Console__FormatterName` environment variable being unset by default.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/compatibility/containers/6.0/console-formatter-default.md#_snippet_1

LANGUAGE: txt
CODE:
```
warn: Microsoft.AspNetCore.Server.HttpSys.MessagePump[37]
      Overriding address(es) ''. Binding to endpoints added to UrlPrefixes instead.
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:7000/
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:7001/
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:7002/
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\source\temp\web50
```

----------------------------------------

TITLE: .NET SDKs for OpenAI Model Integration
DESCRIPTION: This table lists recommended .NET SDKs for integrating with OpenAI and Azure OpenAI models, detailing their NuGet packages, supported models, maintainers, and documentation links.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/ai/dotnet-ai-ecosystem.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
NuGet Package | Supported Models | Maintainer/Vendor | Documentation
-------------------------------------------------------------------
Microsoft.SemanticKernel | OpenAI models, Azure OpenAI supported models | Semantic Kernel (Microsoft) | Semantic Kernel documentation
Azure OpenAI SDK | Azure OpenAI supported models | Azure SDK for .NET (Microsoft) | Azure OpenAI services documentation
OpenAI SDK | OpenAI supported models | OpenAI SDK for .NET (OpenAI) | OpenAI services documentation
```

----------------------------------------

TITLE: Removed APIs in System.Runtime.Intrinsics.X86.Avx10v2
DESCRIPTION: List of APIs in the System.Runtime.Intrinsics.X86.Avx10v2 type that were removed starting from .NET 10 Preview 5 due to changes in AVX10.2 requirements. These APIs are no longer available.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/compatibility/core-libraries/10.0/ymm-embedded-rounding.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.Runtime.Intrinsics.X86.Avx10v2.ConvertToSByteWithSaturationAndZeroExtendToInt32
System.Runtime.Intrinsics.X86.Avx10v2.ConvertToByteWithSaturationAndZeroExtendToInt32
System.Runtime.Intrinsics.X86.Avx10v2.Add
System.Runtime.Intrinsics.X86.Avx10v2.Divide
System.Runtime.Intrinsics.X86.Avx10v2.Multiply
System.Runtime.Intrinsics.X86.Avx10v2.Scale
System.Runtime.Intrinsics.X86.Avx10v2.Sqrt
System.Runtime.Intrinsics.X86.Avx10v2.Subtract
System.Runtime.Intrinsics.X86.Avx10v2.ConvertToVector128Int32
System.Runtime.Intrinsics.X86.Avx10v2.ConvertToVector128Single
System.Runtime.Intrinsics.X86.Avx10v2.ConvertToVector128UInt32
System.Runtime.Intrinsics.X86.Avx10v2.ConvertToVector256Double
System.Runtime.Intrinsics.X86.Avx10v2.ConvertToVector256Int32
System.Runtime.Intrinsics.X86.Avx10v2.ConvertToVector256Int64
System.Runtime.Intrinsics.X86.Avx10v2.ConvertToVector256Single
System.Runtime.Intrinsics.X86.Avx10v2.ConvertToVector256UInt32
System.Runtime.Intrinsics.X86.Avx10v2.ConvertToVector256UInt64
```

----------------------------------------

TITLE: Binding Initializer Expression Type in C#
DESCRIPTION: This snippet uses the `SemanticModel` to bind the initializer expression's value and obtain its `TypeInfo`. This step is essential for comparing the inferred type with the explicitly declared type to determine if `var` can be used without changing semantics.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-transformation.md#_snippet_10

LANGUAGE: C#
CODE:
```
TypeInfo varType = SemanticModel.GetTypeInfo(variable.Initializer.Value);
```

----------------------------------------

TITLE: Executing C# Syntax Walker and Displaying Results
DESCRIPTION: This final snippet in `Program.cs` instantiates the `UsingCollector`, initiates the syntax tree traversal by calling `Visit` on the root node, and then iterates through the collected `Usings` to print the names of the non-System namespaces found. This demonstrates how to run the walker and access its results.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_14

LANGUAGE: C#
CODE:
```
UsingCollector collector = new UsingCollector();
collector.Visit(root);

foreach (var directive in collector.Usings)
{
    Console.WriteLine(directive.Name);
}
```

----------------------------------------

TITLE: dotnet vstest Command API Reference
DESCRIPTION: Detailed API documentation for the `dotnet vstest` command, including its arguments and various options with their descriptions and usage examples.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/tools/dotnet-vstest.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Command: dotnet vstest
  Description: Runs tests from the specified assemblies.

  Arguments:
    TEST_FILE_NAMES:
      Description: Run tests from the specified assemblies. Separate multiple test assembly names with spaces. Wildcards are supported.

  Options:
    --Blame:
      Description: Runs the tests in blame mode. This option is helpful in isolating the problematic tests causing test host to crash. It creates an output file in the current directory as *Sequence.xml* that captures the order of tests execution before the crash.
    --Diag <PATH_TO_LOG_FILE>:
      Description: Enables verbose logs for the test platform. Logs are written to the provided file.
    --Framework <FRAMEWORK>:
      Description: Target .NET Framework version used for test execution. Examples of valid values are `.NETFramework,Version=v4.6` or `.NETCoreApp,Version=v1.0`. Other supported values are `Framework40`, `Framework45`, `FrameworkCore10`, and `FrameworkUap10`.
    --InIsolation:
      Description: Runs the tests in an isolated process. This makes *vstest.console.exe* process less likely to be stopped on an error in the tests, but tests may run slower.
    -lt|--ListTests <FILE_NAME>:
      Description: Lists all discovered tests from the given test container.
    --logger <LOGGER_URI/FRIENDLY_NAME>:
      Description: Specify a logger for test results.
      Examples:
        TfsPublisher:
          Code: /logger:TfsPublisher;\n        Collection=<team project collection url>;\n        BuildName=<build name>;\n        TeamProject=<team project name>\n        [;Platform=<Defaults to \"Any CPU\">]\n        [;Flavor=<Defaults to \"Debug\">]\n        [;RunTitle=<title>]
        trx:
          Code: /logger:trx [;LogFileName=<Defaults to unique file name>]
    --Parallel:
      Description: Run tests in parallel. By default, all available cores on the machine are available for use. Specify an explicit number of cores by setting the `MaxCpuCount` property under the `RunConfiguration` node in the *runsettings* file.
    --ParentProcessId <PROCESS_ID>:
      Description: Process ID of the parent process responsible for launching the current process.
    --Platform <PLATFORM_TYPE>:
      Description: Target platform architecture used for test execution. Valid values are `x86`, `x64`, and `ARM`.
    --Port <PORT>:
      Description: Specifies the port for the socket connection and receiving the event messages.
    --ResultsDirectory:<PATH>:
      Description: Test results directory will be created in specified path if not exists.
    --Settings <SETTINGS_FILE>:
      Description: Settings to use when running tests.
    --TestAdapterPath <PATH>:
      Description: Use custom test adapters from a given path (if any) in the test run.
    --TestCaseFilter <EXPRESSION>:
      Description: Run tests that match the given expression. `<EXPRESSION>` is of the format `<property>Operator<value>[|&<EXPRESSION>]`, where Operator is one of `=`, `!=`, or `~`. Operator `~` has 'contains' semantics and is applicable for string properties like `DisplayName`. Parentheses `()` are used to group subexpressions. For more information, see [TestCase filter](https://github.com/Microsoft/vstest-docs/blob/main/docs/filter.md).
    --Tests <TEST_NAMES>:
      Description: Run tests with names that match the provided values. Separate multiple values with commas.
    -?|--Help:
      Description: Prints out a short help for the command.
    @<file>:
      Description: Reads response file for more options.
    args:
      Description: Specifies extra arguments to pass to the adapter. Arguments are specified as name-value pairs of the form `<n>=<v>`, where `<n>` is the argument name and `<v>` is the argument value. Use a space to separate multiple arguments.
```

----------------------------------------

TITLE: FunctionEnter3 .NET Profiling API Reference
DESCRIPTION: Comprehensive API documentation for the `FunctionEnter3` callback function, detailing its purpose, parameters, specific implementation requirements, and related profiling functions within the .NET Framework.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/functionenter3-function.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Function: FunctionEnter3
  Purpose: Notifies the profiler that control is being passed to a function.
  Syntax: void __stdcall FunctionEnter3(FunctionOrRemappedID functionOrRemappedID);
  Parameters:
    functionOrRemappedID:
      Direction: [in]
      Description: The identifier of the function to which control is passed.
  Remarks:
    - The FunctionEnter3 callback function notifies the profiler as functions are being called, but does not support argument inspection.
    - Use the ICorProfilerInfo3::SetEnterLeaveFunctionHooks3 method to register your implementation of this function.
    - The FunctionEnter3 function is a callback; you must implement it.
    - The implementation must use the __declspec(naked) storage-class attribute.
    - The execution engine does not save any registers before calling this function.
    - On entry, you must save all registers that you use, including those in the floating-point unit (FPU).
    - On exit, you must restore the stack by popping off all the parameters that were pushed by its caller.
  Requirements:
    Platforms: See System Requirements.
    Header: CorProf.idl
    Library: CorGuids.lib
    .NET Framework Versions: 4.0+
  See also:
    - FunctionLeave3
    - FunctionTailcall3
    - FunctionEnter3WithInfo
    - FunctionLeave3WithInfo
    - FunctionTailcall3WithInfo
    - SetEnterLeaveFunctionHooks3
    - SetEnterLeaveFunctionHooks3WithInfo
    - SetFunctionIDMapper
    - SetFunctionIDMapper2
    - Profiling Global Static Functions
```

----------------------------------------

TITLE: Injecting ViewModel into View Constructor in .NET MAUI
DESCRIPTION: Shows an example of a `View` (FiltersView) that receives a `ViewModel` (CatalogViewModel) as a dependency through its constructor. This demonstrates how the dependency injection container, particularly when used with Shell navigation, can automatically create view instances and inject their required dependencies.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/dependency-injection.md#_snippet_6

LANGUAGE: csharp
CODE:
```
namespace eShop.Views;

public partial class FiltersView : ContentPage
{
    public FiltersView(CatalogViewModel viewModel)
    {
        BindingContext = viewModel;

        InitializeComponent();
    }
}
```

----------------------------------------

TITLE: Average Method for Product List Price by Style (LINQ to DataSet)
DESCRIPTION: This example demonstrates using the `Average` method to compute the average list price for products, grouped by their respective styles.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/data/adonet/method-based-query-syntax-examples-aggregate-operators.md#_snippet_15

LANGUAGE: csharp
CODE:
```
[!code-csharp[DP LINQ to DataSet Examples#Average2_MQ](../../../../samples/snippets/csharp/VS_Snippets_ADO.NET/DP LINQ to DataSet Examples/CS/Program.cs#average2_mq)]
```

LANGUAGE: vb
CODE:
```
[!code-vb[DP LINQ to DataSet Examples#Average2_MQ](../../../../samples/snippets/visualbasic/VS_Snippets_ADO.NET/DP LINQ to DataSet Examples/VB/Module1.vb#average2_mq)]
```

----------------------------------------

TITLE: Windows Workflow Foundation (WF) Core API Reference
DESCRIPTION: This section provides an overview of key classes and methods within Windows Workflow Foundation (WF) for hosting, executing, and interacting with workflow instances and activities. It covers the purpose and primary usage of each component as described in the documentation.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/windows-workflow-foundation/overview.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.Activities.WorkflowInvoker:
  - Purpose: Invokes workflows like a method.
  - Usage: For lightweight workflows that do not need management from the host.
  - Method: Invoke()
    - Purpose: Used to invoke several workflow instances.

System.Activities.WorkflowApplication:
  - Purpose: Provides explicit control over the execution of a single workflow instance.
  - Usage: For workflows that need management from the host (e.g., Bookmark resumption).
  - Method: Run()
    - Purpose: Executes workflows needing host management.

System.ServiceModel.WorkflowServiceHost:
  - Purpose: For message-based interactions in multi-instance scenarios.

System.Activities.ActivityInstance:
  - Purpose: Represents the core activity runtime, responsible for activity execution.
  - Context: Several objects can run concurrently within an application domain.

System.Activities.Statements.Sequence:
  - Type: An activity.
  - Purpose: Contains child activities (e.g., WriteLine).

System.Activities.Statements.WriteLine:
  - Type: A child activity.
  - Usage: A Variable of the parent activity can be bound to its InArgument.

System.Activities.Variable:
  - Purpose: Can be bound to an InArgument of a child activity.

System.Activities.InArgument:
  - Type: Argument.
  - Purpose: Used for input to activities.

System.Activities.OutArgument:
  - Type: Argument.
  - Purpose: Returned from an activity to the calling method.

System.Activities.CodeActivity:
  - Type: Abstract class.
  - Purpose: Custom activities can derive from this class.
  - Method: Execute(CodeActivityContext context)
    - Purpose: Can access run-time features (tracking, properties) via CodeActivityContext.

System.Activities.CodeActivityContext:
  - Purpose: Available as a parameter to CodeActivity.Execute().
  - Provides access to: run-time features like tracking and properties.
```

----------------------------------------

TITLE: Example XML Structure for Package Validation Suppression File
DESCRIPTION: This XML snippet illustrates the typical structure of a CompatibilitySuppressions.xml file. Each <Suppression> entry specifies a DiagnosticId, the Target code element, and the Left and Right operands for API comparison, allowing fine-grained control over error suppression.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/apicompat/diagnostic-ids.md#_snippet_3

LANGUAGE: XML
CODE:
```
<?xml version="1.0" encoding="utf-8"?>
<Suppressions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Suppression>
    <DiagnosticId>CP0002</DiagnosticId>
    <Target>M:A.B.DoStringManipulation(System.String)</Target>
    <Left>lib/netstandard2.0/A.dll</Left>
    <Right>lib/net6.0/A.dll</Right>
    <IsBaselineSuppression>false</IsBaselineSuppression>
  </Suppression>
</Suppressions>
```

----------------------------------------

TITLE: Define API endpoint with MapGet in C#
DESCRIPTION: This C# code defines a simple GET endpoint for the web API using `app.MapGet`. It maps the root path ('/') to a handler, setting up the basic API structure.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/diagnostics/observability-prgrja-example.md#_snippet_2

LANGUAGE: csharp
CODE:
```
// In Program.cs, after app = WebApplication.CreateBuilder(args).Build();
app.MapGet("/", () =>
{
    // The logic for handling the request, including observability,
    // would be placed here. See Snippet_SendGreeting for details.
    return "Hello World!";
});
```

----------------------------------------

TITLE: Update IdentityEndpointBase in ISettingsService (C#)
DESCRIPTION: This C# method, `UpdateIdentityEndpoint`, is called when the `IdentityEndpoint` property in `SettingsViewModel` is set with a valid value. It updates the `IdentityEndpointBase` property of the injected `ISettingService` implementation, ensuring the user-provided base endpoint URL is persisted, for example, to platform-specific storage if `SettingsService` is used.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/app-settings-management.md#_snippet_4

LANGUAGE: csharp
CODE:
```
private void UpdateIdentityEndpoint()
{
    _settingsService.IdentityEndpointBase = _identityEndpoint;
}
```

----------------------------------------

TITLE: CreateICeeFileGen Function API Documentation
DESCRIPTION: Detailed API documentation for the CreateICeeFileGen function, including parameters, return values, remarks, and requirements.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/hosting/createiceefilegen-function.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
CreateICeeFileGen Function:
  Description: Creates an ICeeFileGen object. This function has been deprecated in the .NET Framework 4.
  Parameters:
    ceeFileGen:
      Type: [out] ICeeFileGen **
      Description: A pointer to the address of a new ICeeFileGen object.
  Return Value:
    Type: HRESULT
    Description: This method returns standard COM error codes.
  Remarks:
    The ICeeFileGen object is used to create common language runtime (CLR) portable executable (PE) files.
    Call the DestroyICeeFileGen function to destroy the ICeeFileGen object when finished.
  Requirements:
    Platforms: See System Requirements.
    Header: ICeeFileGen.h
    Library: MSCorPE.dll
    .NET Framework Versions: [!INCLUDE[net_current_v10plus](../../../../includes/net-current-v10plus-md.md)]
```

----------------------------------------

TITLE: Classes
DESCRIPTION: Learn about classes, which are types that represent objects that can have properties, methods, and events.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/language-reference/index.md#_snippet_29

LANGUAGE: APIDOC
CODE:
```
Classes:
  Description: Types representing objects with properties, methods, and events.
```

----------------------------------------

TITLE: C# Preserve Single-Line Blocks Formatting Example
DESCRIPTION: This C# code snippet demonstrates the effect of the `csharp_preserve_single_line_blocks` formatting option. When set to `true`, properties like `Foo` are kept on a single line. When set to `false`, properties like `MyProperty` are expanded to multiple lines, with `get; set;` on separate lines.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/code-analysis/style-rules/csharp-formatting-options.md#_snippet_42

LANGUAGE: csharp
CODE:
```
//csharp_preserve_single_line_blocks = true
public int Foo { get; set; }

//csharp_preserve_single_line_blocks = false
public int MyProperty
{
    get; set;
}
```

----------------------------------------

TITLE: ICorProfilerObjectEnum::Next Method API Reference
DESCRIPTION: Detailed API documentation for the `ICorProfilerObjectEnum::Next` method, including its parameters, their types, directions, and descriptions.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/icorprofilerobjectenum-next-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Method Signature:
HRESULT Next (
    [in] ULONG                    celt,
    [out, size_is(celt), length_is(*pceltFetched)]
        ObjectID                  objects[],
    [out] ULONG                   *pceltFetched
);

Parameters:
  celt:
    Direction: [in]
    Description: The number of objects to be retrieved.
  objects:
    Direction: [out, size_is(celt), length_is(*pceltFetched)]
    Description: An array of ObjectID values, each of which represents a retrieved object.
  pceltFetched:
    Direction: [out]
    Description: A pointer to the number of elements actually returned in the objects array.
```

----------------------------------------

TITLE: System.Net.HttpListener Class and Related API Documentation
DESCRIPTION: Detailed API documentation for the System.Net.HttpListener class, its constructor, properties, and methods, along with related classes like HttpListenerContext, HttpListenerRequest, and HttpListenerResponse. It also includes information on URI prefix formatting and wildcard usage.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/runtime-libraries/system-net-httplistener.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.Net.HttpListener class:
  Description: Creates a simple HTTP protocol listener that responds to HTTP requests.
  Constructor:
    HttpListener()
  Properties:
    Prefixes: System.Net.HttpListenerPrefixCollection
      Description: Collection that holds URI prefix strings the HttpListener should process.
  Methods:
    Start(): void
      Description: Begins listening for requests from clients.
    GetContext(): System.Net.HttpListenerContext
      Description: Synchronously waits for a client to send a request and returns the HttpListenerContext object for processing.
    BeginGetContext(callback: AsyncCallback, state: object): IAsyncResult
      Description: Asynchronously specifies an application-defined method to be called for each incoming request.
      Parameters:
        callback: An AsyncCallback delegate that references the method to invoke when a pending asynchronous operation completes.
        state: An object that contains state information for this request.
    EndGetContext(asyncResult: IAsyncResult): System.Net.HttpListenerContext
      Description: Obtains the HttpListenerContext object from the asynchronous operation.
      Parameters:
        asyncResult: The IAsyncResult that stores state information and any user-defined data for this asynchronous operation.

System.Net.HttpListenerContext class:
  Description: Represents the context of a client request.
  Properties:
    Request: System.Net.HttpListenerRequest
      Description: Represents the incoming client request.
    Response: System.Net.HttpListenerResponse
      Description: Represents the response to the client.

System.Net.HttpListenerRequest class:
  Description: Represents an incoming HTTP request received by an HttpListener.
  Accessed via: HttpListenerContext.Request

System.Net.HttpListenerResponse class:
  Description: Represents a response to a client from an HttpListener.
  Accessed via: HttpListenerContext.Response

URI Prefix String Format:
  Scheme: http or https
  Host: hostname, "*", or "+"
  Port: optional
  Path: optional
  Example: http://www.contoso.com:8080/customerData/
  Requirement: Must end in a forward slash ("/").

URI Prefix Wildcards:
  Host Wildcard ("*"):
    Example: http://*:8080/
    Description: HttpListener accepts requests sent to the port if the requested URI does not match any other prefix.
  Host Wildcard ("+"):
    Example: https://+:8080
    Description: HttpListener accepts all requests sent to a port.
  Subdomain Wildcard ("*"):
    Example: http://*.foo.com/
    Method: HttpListenerPrefixCollection.Add()
    Description: Specifies a wildcard subdomain.
  Security Warning:
    Top-level wildcard bindings (http://*:8080/ and http://+:8080) should NOT be used due to security vulnerabilities. Use explicit host names.
```

----------------------------------------

TITLE: WCF Tracking Profile: Example for Subscribing to Started Workflow State
DESCRIPTION: An example XML configuration snippet demonstrating how to use the <states> element within a workflowInstanceQuery to subscribe specifically to workflow instance-level tracking records when the instance enters the Started state.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/configure-apps/file-schema/wcf/states-of-wcf-workflowinstancequery.md#_snippet_1

LANGUAGE: xml
CODE:
```
<workflowInstanceQueries>
  <workflowInstanceQuery>
    <states>
      <state name="Started" />
    </states>
  </workflowInstanceQuery>
</workflowInstanceQueries>
```

----------------------------------------

TITLE: Defining Source Code for C# Analysis
DESCRIPTION: This snippet defines a multi-line C# string constant named `codeText` which serves as the input source code for syntax analysis. It includes various `using` directives, some of which import `System` namespaces and others that do not, demonstrating a typical C# file structure for analysis.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_9

LANGUAGE: C#
CODE:
```
const string codeText =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SyntaxWalker
{
    class Program
    {
        static void Main(string[] args)
        {
            // You'll add code here.
        }
    }
}

namespace MyUtilities
{
    using System.ComponentModel;

    class MyClass2
    {
    }
}

namespace MyCompany.MyProduct
{
    using System.Runtime.InteropServices;
    using System.CodeDom;

    class MyClass
    {
    }
}
";
```

----------------------------------------

TITLE: Related ICorProfiler Interfaces
DESCRIPTION: Lists other relevant ICorProfiler interfaces for further reference.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/icorprofilerinfo2-dostacksnapshot-method.md#_snippet_4

LANGUAGE: APIDOC
CODE:
```
See also:
  - ICorProfilerInfo Interface
  - ICorProfilerInfo2 Interface
```

----------------------------------------

TITLE: WCF Transport Default Values API Reference
DESCRIPTION: This section details the new default values for various transport properties in WCF, providing an overview of changes to settings like channel initialization timeout, listen backlog, and pending connections for improved performance and scalability.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/wcf/whats-new.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
Property: channelInitializationTimeout
  Applies To: System.ServiceModel.NetTcpBinding
  New Default: 30 seconds
  More Info: System.ServiceModel.Channels.ConnectionOrientedTransportBindingElement.ChannelInitializationTimeout

Property: listenBacklog
  Applies To: System.ServiceModel.NetTcpBinding
  New Default: 12 * number of processors
  More Info: System.ServiceModel.NetTcpBinding.ListenBacklog

Property: maxPendingAccepts
  Applies To: ConnectionOrientedTransportBindingElement, SMSvcHost.exe
  New Default: 2 * number of processors for transport, 4 * number of processors for SMSvcHost.exe
  More Info: System.ServiceModel.Channels.ConnectionOrientedTransportBindingElement.MaxPendingAccepts, Configuring the Net.TCP Port Sharing Service

Property: maxPendingConnections
  Applies To: ConnectionOrientedTransportBindingElement
  New Default: 12 * number of processors
  More Info: System.ServiceModel.Channels.ConnectionOrientedTransportBindingElement.MaxPendingConnections

Property: receiveTimeout
  Applies To: SMSvcHost.exe
  New Default: 30 seconds
  More Info: Configuring the Net.TCP Port Sharing Service
```

----------------------------------------

TITLE: Interfaces
DESCRIPTION: Learn about interfaces, which specify sets of related members that other classes implement.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/language-reference/index.md#_snippet_30

LANGUAGE: APIDOC
CODE:
```
Interfaces:
  Description: Specify sets of related members for other classes to implement.
```

----------------------------------------

TITLE: Applying Transformation and Saving Modified C# Trees
DESCRIPTION: This snippet applies the `TypeInferenceRewriter` to the root of a `SyntaxTree` using `rewriter.Visit()`. It then compares the transformed tree with the original; if changes were made, the modified tree's full string representation is written back to its original file path, effectively saving the refactoring.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-transformation.md#_snippet_14

LANGUAGE: C#
CODE:
```
SyntaxNode newSource = rewriter.Visit(tree.GetRoot());

if (!tree.GetRoot().Equals(newSource))
{
    File.WriteAllText(tree.FilePath, newSource.ToFullString());
}
```

----------------------------------------

TITLE: APIDOC: .NET Basic I/O Classes for Files, Drives, and Directories
DESCRIPTION: Documentation for core .NET Framework classes used for file, drive, and directory operations. This includes classes providing static and instance methods for creating, copying, deleting, moving, and opening files and directories, as well as classes for managing file attributes and permissions.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/visual-basic/developing-apps/programming/drives-directories-files/classes-used-in-net-framework-file-io-and-the-file-system.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.IO.Directory: Provides static methods for creating, moving, and enumerating through directories and subdirectories.
System.IO.DirectoryInfo: Provides instance methods for creating, moving, and enumerating through directories and subdirectories.
System.IO.DriveInfo: Provides instance methods for creating, moving, and enumerating through drives.
System.IO.File: Provides static methods for creating, copying, deleting, moving, and opening files, and aids in the creation of a FileStream.
System.IO.FileAccess: Defines constants for read, write, or read/write access to a file.
System.IO.FileAttributes: Provides attributes for files and directories such as Archive, Hidden, and ReadOnly.
System.IO.FileInfo: Provides static methods for creating, copying, deleting, moving, and opening files, and aids in the creation of a FileStream.
System.IO.FileMode: Controls how a file is opened. This parameter is specified in many of the constructors for FileStream and IsolatedStorageFileStream, and for the Open methods of System.IO.File and System.IO.FileInfo.
System.IO.FileShare: Defines constants for controlling the type of access other file streams can have to the same file.
System.IO.Path: Provides methods and properties for processing directory strings.
System.Security.Permissions.FileIOPermission: Controls the access of files and folders by defining System.Security.Permissions.FileIOPermissionAttribute.Read, System.Security.Permissions.FileIOPermissionAttribute.Write, System.Security.Permissions.FileIOPermissionAttribute.Append and System.Security.Permissions.FileIOPermissionAttribute.PathDiscovery permissions.
```

----------------------------------------

TITLE: Web API Prediction Output
DESCRIPTION: Example output from the Web API after a successful prediction request, showing the predicted fare score.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/machine-learning/tutorials/predict-prices-with-model-builder.md#_snippet_2

LANGUAGE: powershell
CODE:
```
score
-----
15.020833
```

----------------------------------------

TITLE: C# Examples of SupportedOSPlatform and UnsupportedOSPlatform Attributes with Diagnostics
DESCRIPTION: This C# code demonstrates the application of `SupportedOSPlatform` and `UnsupportedOSPlatform` attributes to control API availability based on the operating system and its version. It includes examples for APIs supported only on specific platforms, supported across multiple, or with version-specific support/unsupport. The `Caller` methods illustrate the compile-time diagnostics generated when these platform-constrained APIs are invoked from potentially incompatible environments.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/standard/analyzers/platform-compat-analyzer.md#_snippet_6

LANGUAGE: csharp
CODE:
```
// An API supported only on Windows all versions.
[SupportedOSPlatform("Windows")]
public void WindowsOnlyApi() { }

// an API supported on Windows and Linux.
[SupportedOSPlatform("Windows")]
[SupportedOSPlatform("Linux")]
public void SupportedOnWindowsAndLinuxOnly() { }

// an API only supported on Windows 6.2 and later, not supported for all other.
// an API is removed/unsupported from version 10.0.19041.0.
[SupportedOSPlatform("windows6.2")]
[UnsupportedOSPlatform("windows10.0.19041.0")]
public void ApiSupportedFromWindows8UnsupportedFromWindows10() { }

// an Assembly supported on Windows, the API added from version 10.0.19041.0.
[assembly: SupportedOSPlatform("Windows")]
[SupportedOSPlatform("windows10.0.19041.0")]
public void AssemblySupportedOnWindowsApiSupportedFromWindows10() { }

public void Caller()
{
    WindowsOnlyApi(); // warns: This call site is reachable on all platforms. 'WindowsOnlyApi()' is only supported on: 'windows'

    // This call site is reachable on all platforms. 'SupportedOnWindowsAndLinuxOnly()' is only supported on: 'Windows', 'Linux'
    SupportedOnWindowsAndLinuxOnly();

    // This call site is reachable on all platforms. 'ApiSupportedFromWindows8UnsupportedFromWindows10()' is only supported on: 'windows' from version 6.2 to 10.0.19041.0
    ApiSupportedFromWindows8UnsupportedFromWindows10();

    // for same platform analyzer only warn for the latest version.
    // This call site is reachable on all platforms. 'AssemblySupportedOnWindowsApiSupportedFromWindows10()' is only supported on: 'windows' 10.0.19041.0 and later
    AssemblySupportedOnWindowsApiSupportedFromWindows10();
}

// an API not supported on android but supported on all other.
[UnsupportedOSPlatform("android")]
public void DoesNotWorkOnAndroid() { }

// an API was unsupported on Windows until version 6.2.
// The API is considered supported everywhere else without constraints.
[UnsupportedOSPlatform("windows")]
[SupportedOSPlatform("windows6.2")]
public void StartedWindowsSupportFromVersion8() { }

// an API was unsupported on Windows until version 6.2.
// Then the API is removed (unsupported) from version 10.0.19041.0.
// The API is considered supported everywhere else without constraints.
[UnsupportedOSPlatform("windows")]
[SupportedOSPlatform("windows6.2")]
[UnsupportedOSPlatform("windows10.0.19041.0")]
public void StartedWindowsSupportFrom8UnsupportedFrom10() { }

public void Caller2()
{
    DoesNotWorkOnAndroid(); // This call site is reachable on all platforms.'DoesNotWorkOnAndroid()' is unsupported on: 'android'

    // This call site is reachable on all platforms. 'StartedWindowsSupportFromVersion8()' is unsupported on: 'windows' 6.2 and before.
    StartedWindowsSupportFromVersion8();

    // This call site is reachable on all platforms. 'StartedWindowsSupportFrom8UnsupportedFrom10()' is supported on: 'windows' from version 6.2 to 10.0.19041.0
    StartedWindowsSupportFrom8UnsupportedFrom10();
}
```

----------------------------------------

TITLE: API Documentation for CreateDebuggingInterfaceFromVersionEx Function
DESCRIPTION: Detailed API documentation for the CreateDebuggingInterfaceFromVersionEx function, including its purpose, parameters, return values, and general remarks.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/unmanaged-api/debugging/createdebugginginterfacefromversionex-function.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Function: CreateDebuggingInterfaceFromVersionEx
Purpose: Accepts a common language runtime (CLR) version string that is returned from the CreateVersionStringFromModule function, and returns a corresponding debugger interface (typically, ICorDebug).

Parameters:
  iDebuggerVersion: [in] int - The version of interface the debugger expects.
  szDebuggeeVersion: [in] LPCWSTR - Version string of the CLR in the target debuggee, which is returned by the CreateVersionStringFromModule function.
  ppCordb: [out] IUnknown** - Pointer to a pointer to a COM object (IUnknown). This object will be cast to an ICorDebug object before it is returned.

Return Value (HRESULT):
  S_OK: ppCordb references a valid object that implements the ICorDebug interface.
  E_INVALIDARG: Either szDebuggeeVersion or ppCordb is null.
  CORDBG_E_DEBUG_COMPONENT_MISSING: A component that is necessary for CLR debugging cannot be located. Either _mscordbi.dll or _mscordaccore.dll was not found in the same directory as the target CoreCLR.dll.
  CORDBG_E_INCOMPATIBLE_PROTOCOL: Either mscordbi.dll or mscordaccore.dll is not the same version as the target CoreCLR.dll.
  E_FAIL (or other E_ return codes): Unable to return an ICorDebug interface.

Remarks:
  The interface that is returned provides the facilities for attaching to a CLR in a target process and debugging the managed code that the CLR is running.

Requirements:
  Platforms: See .NET supported operating systems.
  Header: dbgshim.h
  Library: dbgshim.dll, libdbgshim.so, libdbgshim.dylib
  .NET Versions: Available since .NET Core 2.1
```

----------------------------------------

TITLE: ITypeNameBuilder::AddByRef Method Documentation
DESCRIPTION: Provides comprehensive documentation for the ITypeNameBuilder::AddByRef method, detailing its purpose, syntax, API specifics, and system requirements within the .NET Framework hosting context.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/hosting/itypenamebuilder-addbyref-method.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
ITypeNameBuilder::AddByRef Method:
  Description: This method supports the .NET Framework infrastructure and is not intended to be used directly from your code.
  API Details:
    api_name: ITypeNameBuilder.AddByRef
    api_location: mscoree.dll
    api_type: COM
    f1_keywords: AddByRef
    helpviewer_keywords:
      - ITypeNameBuilder::AddByRef method [.NET Framework hosting]
      - AddByRef method [.NET Framework hosting]
  Requirements:
    Platforms: See System Requirements
    Header: MSCorEE.h
    Library: Included as a resource in MSCorEE.dll
    .NET Framework Versions: .NET Framework 2.0+
```

LANGUAGE: cpp
CODE:
```
HRESULT AddByRef ( );
```

----------------------------------------

TITLE: Asynchronous Azure Queue Storage Operations in F#
DESCRIPTION: This example illustrates how to integrate asynchronous workflows with common Azure Queue storage APIs in F#. Using async workflows improves application responsiveness and resource utilization by allowing operations to run concurrently without blocking the main thread.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/using-fsharp-on-azure/queue-storage.md#_snippet_11

LANGUAGE: F#
CODE:
```
open Azure.Storage.Queues
open System

let useAsyncQueueOperations (queueClient: QueueClient) =
    async {
        // Send a message asynchronously
        do! queueClient.SendMessage("Hello, Async from F#!") |> Async.AwaitTask
        printfn "Message sent asynchronously."

        // Receive a message asynchronously
        let! message = queueClient.ReceiveMessage() |> Async.AwaitTask
        match message with
        | null -> printfn "No message received."
        | msg ->
            printfn "Received async message: %s" msg.Body.ToString()
            // Delete the message asynchronously
            do! queueClient.DeleteMessage(msg.MessageId, msg.PopReceipt) |> Async.AwaitTask
            printfn "Async message deleted."
    }
```

----------------------------------------

TITLE: System.IO.File Span Helper Methods API Documentation
DESCRIPTION: New helper methods added to the System.IO.File class to directly write ReadOnlySpan<char>/byte and ReadOnlyMemory<char>/byte to files, improving I/O efficiency.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/whats-new/dotnet-9/libraries.md#_snippet_26

LANGUAGE: APIDOC
CODE:
```
System.IO.File.WriteAllText(string path, ReadOnlySpan<char> contents)
  - Writes the specified ReadOnlySpan<char> to a file.
System.IO.File.WriteAllText(string path, ReadOnlySpan<byte> contents)
  - Writes the specified ReadOnlySpan<byte> to a file.
System.IO.File.WriteAllText(string path, ReadOnlyMemory<char> contents)
  - Writes the specified ReadOnlyMemory<char> to a file.
System.IO.File.WriteAllText(string path, ReadOnlyMemory<byte> contents)
  - Writes the specified ReadOnlyMemory<byte> to a file.
```

----------------------------------------

TITLE: Simple GET Request using HttpClient (C#)
DESCRIPTION: Illustrates how to perform a basic HTTP GET request using the modern HttpClient class, showcasing its simpler API for asynchronous operations.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/networking/http/httpclient-migrate-from-httpwebrequest.md#_snippet_1

LANGUAGE: C#
CODE:
```
HttpClient client = new();
using HttpResponseMessage message = await client.GetAsync(uri);
```

----------------------------------------

TITLE: Get Text Container from Table Cell Content using UI Automation
DESCRIPTION: Demonstrates a sequence of UI Automation API calls to retrieve the AutomationElement representing the text container, including embedded images, from a specific table cell. This involves navigating from a grid item to its enclosing text elements.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/ui-automation/textpattern-and-embedded-objects-overview.md#_snippet_5

LANGUAGE: APIDOC
CODE:
```
Method Called: System.Windows.Automation.GridPattern.GetItem with parameters (0,0)
Result: Returns the System.Windows.Automation.AutomationElement representing the content of the table cell; in this case, the element is a text control.

Method Called: System.Windows.Automation.TextPattern.RangeFromChild where System.Windows.Automation.AutomationElement is the object returned by the previous GetItem method.
Result: Returns the range that spans the image.

Method Called: System.Windows.Automation.Text.TextPatternRange.GetEnclosingElement for the object returned by the previous RangeFromChild method.
Result: Returns the System.Windows.Automation.AutomationElement representing the table cell; in this case, the element is a text control that supports TableItemPattern.

Method Called: System.Windows.Automation.Text.TextPatternRange.GetEnclosingElement for the object returned by the previous GetEnclosingElement method.
Result: Returns the System.Windows.Automation.AutomationElement representing the table.

Method Called: System.Windows.Automation.Text.TextPatternRange.GetEnclosingElement for the object returned by the previous GetEnclosingElement method.
Result: Returns the System.Windows.Automation.AutomationElement that represents the text provider itself.
```

----------------------------------------

TITLE: API Details for IHostSyncManager::CreateAutoEvent Method
DESCRIPTION: Comprehensive API documentation for the IHostSyncManager::CreateAutoEvent method, including parameter descriptions, return values, and behavioral remarks.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/hosting/ihostsyncmanager-createautoevent-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Parameters:
  ppEvent:
    [out] A pointer to the address of an IHostAutoEvent instance implemented by the host, or null if the event object could not be created.
```

LANGUAGE: APIDOC
CODE:
```
Return Values:
  S_OK: CreateAutoEvent returned successfully.
  HOST_E_CLRNOTAVAILABLE: The common language runtime (CLR) has not been loaded into a process, or the CLR is in a state in which it cannot run managed code or process the call successfully.
  HOST_E_TIMEOUT: The call timed out.
  HOST_E_NOT_OWNER: The caller does not own the lock.
  HOST_E_ABANDONED: An event was canceled while a blocked thread or fiber was waiting on it.
  E_FAIL: An unknown catastrophic failure occurred. When a method returns E_FAIL, the CLR is no longer usable within the process. Subsequent calls to hosting methods return HOST_E_CLRNOTAVAILABLE.
  E_OUTOFMEMORY: Not enough memory was available to create the requested event object.
```

LANGUAGE: APIDOC
CODE:
```
Remarks:
  CreateAutoEvent creates an auto-event object whose state is automatically changed to non-signaled after the waiting thread has been released. This method mirrors the Win32 CreateEvent function with a value of false specified for the bManualReset parameter.
```

LANGUAGE: APIDOC
CODE:
```
Requirements:
  Platforms: See System Requirements.
  Header: MSCorEE.h
  Library: Included as a resource in MSCorEE.dll
  .NET Framework Versions: .NET Framework 2.0 plus
```

----------------------------------------

TITLE: Example Ocelot API Gateway ReRoute Configuration
DESCRIPTION: A simplified example of a comprehensive `configuration.json` file for Ocelot, illustrating how to define multiple API ReRoutes. It showcases the mapping of `Upstream` paths to `Downstream` services, specifying host/port, HTTP methods, and basic authentication options for different microservices.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/microservices/multi-container-microservice-net-applications/implement-api-gateways-with-ocelot.md#_snippet_7

LANGUAGE: json
CODE:
```
{
  "ReRoutes": [
    {
      "DownstreamPathTemplate": "/api/{version}/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "catalog-api",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/api/{version}/c/{everything}",
      "UpstreamHttpMethod": [ "POST", "PUT", "GET" ]
    },
    {
      "DownstreamPathTemplate": "/api/{version}/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "basket-api",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/api/{version}/b/{everything}",
      "UpstreamHttpMethod": [ "POST", "PUT", "GET" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "IdentityApiKey",
        "AllowedScopes": []
      }
    }

  ],
    "GlobalConfiguration": {
      "RequestIdKey": "OcRequestId",
      "AdministrationPath": "/administration"
    }
  }
```

----------------------------------------

TITLE: System.IO.Directory and System.IO.File Class API Reference
DESCRIPTION: Detailed API documentation for the static methods available in the `System.IO.Directory` and `System.IO.File` classes, along with `System.IO.DirectoryInfo` constructor and methods, used for managing directories and files within the file system.
SOURCE: https://github.com/dotnet/docs/blob/main/includes/core-changes/corefx/2.1/path-apis-dont-throw-exception-for-invalid-paths.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.IO.Directory Class:
  - CreateDirectory (multiple overloads)
  - Delete (multiple overloads)
  - EnumerateDirectories (multiple overloads)
  - EnumerateFiles (multiple overloads)
  - EnumerateFileSystemEntries (multiple overloads)
  - GetCreationTime(System.String path)
  - GetCreationTimeUtc(System.String path)
  - GetDirectories (multiple overloads)
  - GetDirectoryRoot(System.String path)
  - GetFiles (multiple overloads)
  - GetFileSystemEntries (multiple overloads)
  - GetLastAccessTime(System.String path)
  - GetLastAccessTimeUtc(System.String path)
  - GetLastWriteTime(System.String path)
  - GetLastWriteTimeUtc(System.String path)
  - GetParent(System.String path)
  - Move(System.String sourceDirName, System.String destDirName)
  - SetCreationTime(System.String path, System.DateTime creationTime)
  - SetCreationTimeUtc(System.String path, System.DateTime creationTimeUtc)
  - SetCurrentDirectory(System.String path)
  - SetLastAccessTime(System.String path, System.DateTime lastAccessTime)
  - SetLastAccessTimeUtc(System.String path, System.DateTime lastAccessTimeUtc)
  - SetLastWriteTime(System.String path, System.DateTime lastWriteTime)
  - SetLastWriteTimeUtc(System.String path, System.DateTime lastWriteTimeUtc)

System.IO.DirectoryInfo Class:
  - .ctor(System.String path) - Constructor
  - GetFileSystemInfos (multiple overloads)

System.IO.File Class:
  - AppendAllText (multiple overloads)
  - AppendAllTextAsync (multiple overloads)
  - Copy (multiple overloads)
  - Create (multiple overloads)
  - CreateText (multiple overloads)
  - Decrypt (multiple overloads)
  - Delete (multiple overloads)
  - Encrypt (multiple overloads)
  - GetAttributes(System.String path)
  - GetCreationTime(System.String path)
  - GetCreationTimeUtc(System.String path)
  - GetLastAccessTime(System.String path)
  - GetLastAccessTimeUtc(System.String path)
  - GetLastWriteTime(System.String path)
  - GetLastWriteTimeUtc(System.String path)
  - Move (multiple overloads)
  - Open (multiple overloads)
  - OpenRead(System.String path)
  - OpenText(System.String path)
  - OpenWrite(System.String path)
  - ReadAllBytes(System.String path)
  - ReadAllBytesAsync(System.String path, System.Threading.CancellationToken cancellationToken)
  - ReadAllLines(System.String path)
  - ReadAllLinesAsync(System.String path, System.Threading.CancellationToken cancellationToken)
  - ReadAllText(System.String path)
  - ReadAllTextAsync(System.String path, System.Threading.CancellationToken cancellationToken)
  - SetAttributes(System.String path, System.IO.FileAttributes fileAttributes)
  - SetCreationTime(System.String path, System.DateTime creationTime)
  - SetCreationTimeUtc(System.String path, System.DateTime creationTimeUtc)
  - SetLastAccessTime(System.String path, System.DateTime lastAccessTime)
  - SetLastAccessTimeUtc(System.String path, System.DateTime lastAccessTimeUtc)
  - SetLastWriteTime(System.String path, System.DateTime lastWriteTime)
  - SetLastWriteTimeUtc(System.String path, System.DateTime lastWriteTimeUtc)
```

----------------------------------------

TITLE: IDE0005 Example: Unnecessary F# Open Declaration
DESCRIPTION: Provides an example of an unnecessary `open` declaration in F# that would be flagged by the IDE0005 rule.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/code-analysis/style-rules/ide0005.md#_snippet_4

LANGUAGE: F#
CODE:
```
open System.Collections

printfn "Hello from F#"
```

----------------------------------------

TITLE: APIDOC: ICorProfilerCallback4::ReJITCompilationFinished Method Details
DESCRIPTION: Detailed API documentation for the `ReJITCompilationFinished` method, including its purpose, parameters, and their descriptions.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/icorprofilercallback4-rejitcompilationfinished-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
ICorProfilerCallback4::ReJITCompilationFinished Method
  Description: Notifies the profiler that the just-in-time (JIT) compiler has finished recompiling a function.

  Parameters:
    functionId:
      Direction: [in]
      Type: FunctionID
      Description: The ID of the function that was recompiled.
    rejitId:
      Direction: [in]
      Type: ReJITID
      Description: The identity of the JIT-recompiled function.
    hrStatus:
      Direction: [in]
      Type: HRESULT
      Description: A value that indicates whether the JIT recompilation was successful.
    fIsSafeToBlock:
      Direction: [in]
      Type: BOOL
      Description: true to indicate that blocking may cause the runtime to wait for the calling thread to return from this callback; false to indicate that blocking will not affect the operation of the runtime. A value of true does not harm the runtime, but can affect the profiling results.
```

----------------------------------------

TITLE: F# Interactive: Enhanced #help Directive for List.map
DESCRIPTION: The #help directive in F# Interactive now provides detailed documentation for objects and functions, such as List.map, without requiring quotes. It outlines parameters, return types, and provides examples for better understanding.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/whats-new/fsharp-9.md#_snippet_7

LANGUAGE: APIDOC
CODE:
```
List.map:
  Description: Builds a new collection whose elements are the results of applying the given function to each of the elements of the collection.
  Parameters:
    - name: mapping
      type: function
      description: The function to transform elements from the input list.
    - name: list
      type: collection
      description: The input list.
  Returns: The list of transformed elements.
  Examples:
    let inputs = [ "a"; "bbb"; "cc" ]
    inputs |> List.map (fun x -> x.Length)
    // Evaluates to [ 1; 3; 2 ]
  Full name: Microsoft.FSharp.Collections.ListModule.map
  Assembly: FSharp.Core.dll
```

----------------------------------------

TITLE: Obsolete Formatter-based Serialization APIs in .NET 8
DESCRIPTION: List of APIs related to formatter-based serialization that are obsolete starting in .NET 8, generating a SYSLIB0050 warning upon use. These APIs should be avoided in new development.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/syslib-diagnostics/syslib0050.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.Runtime.Serialization.FormatterConverter
System.Runtime.Serialization.FormatterServices
System.Runtime.Serialization.IFormatterConverter
System.Runtime.Serialization.IObjectReference
System.Runtime.Serialization.ISafeSerializationData
System.Runtime.Serialization.ISerializationSurrogate
System.Runtime.Serialization.ISurrogateSelector
System.Runtime.Serialization.ObjectIDGenerator
System.Runtime.Serialization.ObjectManager
System.Runtime.Serialization.SafeSerializationEventArgs
System.Runtime.Serialization.SerializationObjectManager
System.Runtime.Serialization.StreamingContextStates
System.Runtime.Serialization.SurrogateSelector
System.Runtime.Serialization.Formatters.FormatterAssemblyStyle
System.Runtime.Serialization.Formatters.FormatterTypeStyle
System.Runtime.Serialization.Formatters.IFieldInfo
System.Runtime.Serialization.Formatters.TypeFilterLevel
System.Type.IsSerializable
System.Reflection.FieldAttributes.NotSerialized
System.Reflection.FieldInfo.IsNotSerialized
System.Reflection.TypeAttributes.Serializable
System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)
System.Runtime.Serialization.SerializationInfo.#ctor(System.Type,System.Runtime.Serialization.IFormatterConverter,System.Boolean)
System.Runtime.Serialization.SerializationInfo.#ctor(System.Type,System.Runtime.Serialization.IFormatterConverter)
System.Runtime.Serialization.StreamingContext.#ctor(System.Runtime.Serialization.StreamingContextStates,System.Object)
System.Runtime.Serialization.StreamingContext.#ctor(System.Runtime.Serialization.StreamingContextStates)
```

----------------------------------------

TITLE: dotnet new apicontroller Template Options
DESCRIPTION: Documents the command-line options for the `dotnet new apicontroller` template, which generates an API controller with optional read/write actions.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/tools/dotnet-new-sdk-templates.md#_snippet_28

LANGUAGE: APIDOC
CODE:
```
Template: apicontroller
Description: API Controller with or without read/write actions.
Options:
  -p:n|--name <NAME>:
    Description: The namespace for the generated code.
    Type: string
    Default: MyApp.Namespace
  -ac|--actions:
    Description: Create a controller with read/write actions.
    Type: boolean
    Default: false
```

----------------------------------------

TITLE: CreateCordbObject Function API Reference
DESCRIPTION: Detailed API documentation for the `CreateCordbObject` function, including its signature, parameters, return values, and remarks for establishing a remote debugging session.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/debugging/createcordbobject-function.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
CreateCordbObject Function:
  Description: Creates a debugger interface (ICorDebug) that provides functionality for instantiating a managed debugging session on a remote process.
  Syntax:
    HRESULT CordbCreateObject (
           [in]  int         iDebuggerVersion,
           [out] IUnknown**  ppCordb
    );
  Parameters:
    iDebuggerVersion: [in] int
      Description: Debugger version of the target process. This parameter must be CorDebugVersion_2_0 for remote debugging.
    ppCordb: [out] IUnknown**
      Description: Pointer to a pointer to an object that will be cast to an ICorDebug interface and returned.
  Return Value:
    S_OK: The number of CLRs in the process was successfully determined, and the corresponding handle and path arrays were properly filled.
    E_INVALIDARG: ppCordb is null, or iDebuggerVersion is not CorDebugVersion_2_0.
    E_OUTOFMEMORY: Unable to allocate enough memory for ppCordb
    E_FAIL (or other E_ return codes): Other failures.
  Remarks:
    The ICorDebug interface that is returned in ppCordb is the top-level debugging interface for all managed debugging services.
  Requirements:
    Platforms: See System Requirements.
    Header: CoreClrRemoteDebuggingInterfaces.h
    Library: mscordbi_macx86.dll
    .NET Framework Versions: 3.5 SP1
```

----------------------------------------

TITLE: Example .NET EditorConfig File with Default Options
DESCRIPTION: This example .editorconfig file demonstrates the default code style options for .NET projects. It includes core EditorConfig options for indentation, spacing, and new line preferences, as well as .NET specific coding conventions for organizing usings, 'this.'/'Me.' preferences, language keywords vs. BCL types, parentheses, and accessibility modifiers. This file can be used as a starting point for defining consistent code style.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/code-analysis/code-style-rule-options.md#_snippet_0

LANGUAGE: INI
CODE:
```
# Remove the line below if you want to inherit .editorconfig settings from higher directories
root = true

# C# files
[*.cs]

#### Core EditorConfig Options ####

# Indentation and spacing
indent_size = 4
indent_style = space
tab_width = 4

# New line preferences
end_of_line = crlf
insert_final_newline = false

#### .NET Coding Conventions ####

# Organize usings
dotnet_separate_import_directive_groups = false
dotnet_sort_system_directives_first = false
file_header_template = unset

# this. and Me. preferences
dotnet_style_qualification_for_event = false
dotnet_style_qualification_for_field = false
dotnet_style_qualification_for_method = false
dotnet_style_qualification_for_property = false

# Language keywords vs BCL types preferences
dotnet_style_predefined_type_for_locals_parameters_members = true
dotnet_style_predefined_type_for_member_access = true

# Parentheses preferences
dotnet_style_parentheses_in_arithmetic_binary_operators = always_for_clarity
dotnet_style_parentheses_in_other_binary_operators = always_for_clarity
dotnet_style_parentheses_in_other_operators = never_if_unnecessary
dotnet_style_parentheses_in_relational_binary_operators = always_for_clarity

# Modifier preferences
dotnet_style_require_accessibility_modifiers = for_non_interface_members
```

----------------------------------------

TITLE: XPathNavigator Class and Selection Methods API
DESCRIPTION: Documentation for key classes and methods used in XML data selection with XPathNavigator, including their purpose, parameters, and return types, as described in the .NET documentation.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/standard/data/xml/select-xml-data-using-xpathnavigator.md#_snippet_2

LANGUAGE: APIDOC
CODE:
```
Class: System.Xml.XPath.XPathNavigator
  Description: Provides a set of methods used to select a set of nodes in an XPathDocument or XmlDocument object using an XPath expression.
  Methods:
    Select(xpathExpression: string): System.Xml.XPath.XPathNodeIterator
      Description: Selects a set of nodes using an XPath expression.
      Return: An XPathNodeIterator object to navigate the selected nodes.
    SelectSingleNode(xpathExpression: string): System.Xml.XPath.XPathNavigator
      Description: Selects a single node using an XPath expression.
      Return: An XPathNavigator object positioned on the single selected node.
    SelectChildren(nodeType: System.Xml.XPath.XPathNodeType): System.Xml.XPath.XPathNodeIterator
      Description: Selects child nodes based on an XPathNodeType value. Optimized for performance.
      Return: An XPathNodeIterator object.
    SelectChildren(localName: string, namespaceURI: string): System.Xml.XPath.XPathNodeIterator
      Description: Selects child nodes based on local name and namespace URI. Optimized for performance.
      Return: An XPathNodeIterator object.
    SelectAncestors(nodeType: System.Xml.XPath.XPathNodeType): System.Xml.XPath.XPathNodeIterator
      Description: Selects ancestor nodes based on an XPathNodeType value. Optimized for performance.
      Return: An XPathNodeIterator object.
    SelectAncestors(localName: string, namespaceURI: string): System.Xml.XPath.XPathNodeIterator
      Description: Selects ancestor nodes based on local name and namespace URI. Optimized for performance.
      Return: An XPathNodeIterator object.
    SelectDescendants(nodeType: System.Xml.XPath.XPathNodeType): System.Xml.XPath.XPathNodeIterator
      Description: Selects descendant nodes based on an XPathNodeType value. Optimized for performance.
      Return: An XPathNodeIterator object.
    SelectDescendants(localName: string, namespaceURI: string): System.Xml.XPath.XPathNodeIterator
      Description: Selects descendant nodes based on local name and namespace URI. Optimized for performance.
      Return: An XPathNodeIterator object.

Class: System.Xml.XPath.XPathDocument
  Description: Represents an in-memory store for XML documents, optimized for XPath queries. Used to create an XPathNavigator.

Class: System.Xml.XmlDocument
  Description: Represents an XML document. Used to create an XPathNavigator.

Class: System.Xml.XPath.XPathNodeIterator
  Description: Provides the ability to iterate over a selected set of nodes.
  Methods:
    MoveNext(): bool
      Description: Advances the iterator to the next node in the selected set.
    Current: System.Xml.XPath.XPathNavigator
      Description: Gets the XPathNavigator for the current node in the selected set.
```

----------------------------------------

TITLE: Deprecated Global Mono and Emscripten APIs
DESCRIPTION: Lists the legacy Mono and Emscripten APIs that are no longer exported to the global `window` object in Blazor WebAssembly apps starting with .NET 9 GA. These APIs are either removed or now accessible through `Blazor.runtime`.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/compatibility/aspnet-core/9.0/legacy-apis.md#_snippet_2

LANGUAGE: APIDOC
CODE:
```
window.MONO.*
window.BINDING.*
window.Module.*
```

----------------------------------------

TITLE: Querying Tree for Main Method Arguments in C# Roslyn
DESCRIPTION: This code demonstrates using LINQ with Roslyn's query methods, specifically `DescendantNodes().OfType<ParameterSyntax>().Single()`, to find the 'args' parameter of the `Main` method. This provides an alternative, more concise way to locate specific syntax nodes compared to manual traversal.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-analysis.md#_snippet_8

LANGUAGE: C#
CODE:
```
ParameterSyntax argsParameter2 = root.DescendantNodes()
                                            .OfType<ParameterSyntax>()
                                            .Single();
Console.WriteLine($"The args parameter is {argsParameter2.Identifier.Text}");
```

----------------------------------------

TITLE: System.IO Namespace API Reference
DESCRIPTION: Comprehensive API documentation for key classes and methods within the System.IO namespace, including file operations, path manipulation, and stream management. This reference details method signatures and constructor parameters for common file system interactions.
SOURCE: https://github.com/dotnet/docs/blob/main/includes/core-changes/corefx/2.1/path-apis-dont-throw-exception-for-invalid-paths.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
System.IO.File:
  SetLastWriteTimeUtc(path: System.String, writeTime: System.DateTime)
  WriteAllBytes(path: System.String, bytes: System.Byte[])
  WriteAllBytesAsync(path: System.String, bytes: System.Byte[], cancellationToken: System.Threading.CancellationToken)
  WriteAllLines(path: System.String, contents: System.Collections.Generic.IEnumerable<System.String>)
  WriteAllLinesAsync(path: System.String, contents: System.Collections.Generic.IEnumerable<System.String>, cancellationToken: System.Threading.CancellationToken)
  WriteAllText(path: System.String, contents: System.String)

System.IO.FileInfo:
  __ctor__(fileName: System.String)
  CopyTo(destFileName: System.String)
  MoveTo(destFileName: System.String)

System.IO.FileStream:
  __ctor__(path: System.String, mode: System.IO.FileMode)

System.IO.Path:
  GetFullPath(path: System.String)
  IsPathRooted(path: System.String)
  GetPathRoot(path: System.String)
  ChangeExtension(path: System.String, extension: System.String)
  GetDirectoryName(path: System.String)
  GetExtension(path: System.String)
  HasExtension(path: System.String)
  Combine(paths: params System.String[])
```

----------------------------------------

TITLE: System.Net.PeerToPeer.Collaboration Namespace API Reference
DESCRIPTION: Detailed API documentation for the System.Net.PeerToPeer.Collaboration namespace, outlining its primary classes, methods, and the four-step process for establishing peer collaboration sessions.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/network-programming/about-the-system-net-peertopeer-collaboration-namespace.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.Net.PeerToPeer.Collaboration Namespace:
  Description: Provides classes and APIs for implementing peer collaboration activities using the Peer-to-Peer Collaboration Infrastructure.

  Classes:
    ContactManager:
      Description: Manages and stores peer contacts.
      Methods:
        CreateContact(): Adds a remote peer and one of its local peers to the Contact Manager.

    PeerApplication:
      Description: Represents an application for collaboration, such as a game, chat client, or conferencing solution.

    PeerContact:
      Description: Represents a peer participating in a collaboration activity.
      Methods:
        Invite(): Used to invite peers to a collaboration session.

    PeerNearMe:
      Description: Represents a peer collaborating in an activity, typically on the local subnet.

    PeerEndPoint:
      Description: Represents a peer collaborating in an activity.

    PeerCollaboration:
      Description: A static class that specifies which applications are available and which peers are participating in them.

    PeerScope:
      Description: Specifies the level of participation allowed for a peer.
      Values:
        Internet: Global participation.
        NearMe: Subnet participation.
        None: No participation.

  Collaboration Session Steps:
    1. Discovery: Discover or publish applications, peers, and presence information (e.g., finding others with the same games).
    2. Invitation: Send and accept secure invitations for remote peers to start or join PeerCollaboration sessions.
    3. Contact Management: Add discovered peers as a contact to a ContactManager.
    4. Communication: Utilize System.Net APIs, System.Net.PeerToPeer API, or Windows Communication Foundation Peer Channel classes for multiparty communications.
```

----------------------------------------

TITLE: CreateDebuggingInterfaceFromVersion3 Function API Reference
DESCRIPTION: Detailed API documentation for the CreateDebuggingInterfaceFromVersion3 function, including parameter descriptions, return values, and general remarks on its usage and behavior.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/unmanaged-api/debugging/createdebugginginterfacefromversion3-function.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
CreateDebuggingInterfaceFromVersion3 Function:
  Description: Accepts a common language runtime (CLR) version string that is returned from the CreateVersionStringFromModule function, and returns a corresponding debugger interface (typically, ICorDebug).
  Parameters:
    iDebuggerVersion:
      Type: int
      Direction: in
      Description: The version of interface the debugger expects.
    szDebuggeeVersion:
      Type: LPCWSTR
      Direction: in
      Description: Version string of the CLR in the target debuggee, which is returned by the CreateVersionStringFromModule function.
    szApplicationGroupId:
      Type: LPCWSTR
      Direction: in
      Description: A string representing the application group ID of a sandboxed process running in macOS. Pass NULL if the process is not running in a sandbox on macOS or on other platforms.
    pLibraryProvider:
      Type: ICLRDebuggingLibraryProvider3*
      Direction: in
      Description: A callback interface instance for locating DBI and DAC. See ICLRDebuggingLibraryProvider3 interface.
    ppCordb:
      Type: IUnknown**
      Direction: out
      Description: Pointer to a pointer to a COM object (IUnknown). This object will be cast to an ICorDebug object before it is returned.
  Return Value:
    Type: HRESULT
    Possible Values:
      S_OK: ppCordb references a valid object that implements the ICorDebug interface.
      E_INVALIDARG: Either szDebuggeeVersion or ppCordb is null.
      CORDBG_E_DEBUG_COMPONENT_MISSING: A component that is necessary for CLR debugging cannot be located. Either _mscordbi.dll_ or _mscordaccore.dll_ was not found in the same directory as the target CoreCLR.dll.
      CORDBG_E_INCOMPATIBLE_PROTOCOL: Either mscordbi.dll or mscordaccore.dll is not the same version as the target CoreCLR.dll.
      E_FAIL (or other E_ return codes): Unable to return an ICorDebug interface.
  Remarks:
    The interface that is returned provides the facilities for attaching to a CLR in a target process and debugging the managed code that the CLR is running.
  Requirements:
    Platforms: See .NET supported operating systems.
    Header: dbgshim.h
    Library: dbgshim.dll, libdbgshim.so, libdbgshim.dylib
    .NET Versions: Available since .NET 6.0
```

----------------------------------------

TITLE: Obsolete .NET Strong-Name Signing APIs
DESCRIPTION: These .NET APIs related to strong-name signing are obsolete starting in .NET 6, generating SYSLIB0017 warning at compile time and throwing PlatformNotSupportedException at runtime. Usage of these APIs is not supported.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/syslib-diagnostics/syslib0017.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.Reflection.AssemblyName.KeyPair
System.Reflection.StrongNameKeyPair
```

----------------------------------------

TITLE: UriTemplate Query String Parameter Type Formats and Examples
DESCRIPTION: This API documentation details the supported .NET data types and their required string formats when used as query string parameters in URLs with UriTemplate. It covers primitive types, complex types like DateTime and TimeSpan, and provides an example of how enumerations are handled.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/wcf/feature-details/wcf-web-http-programming-model-overview.md#_snippet_2

LANGUAGE: APIDOC
CODE:
```
UriTemplate Query String Parameter Type Formats:
Type: System.Byte
  Format: 0 - 255
Type: System.SByte
  Format: -128 - 127
Type: System.Int16
  Format: -32768 - 32767
Type: System.Int32
  Format: -2,147,483,648 - 2,147,483,647
Type: System.Int64
  Format: -9,223,372,036,854,775,808 - 9,223,372,036,854,775,807
Type: System.UInt16
  Format: 0 - 65535
Type: System.UInt32
  Format: 0 - 4,294,967,295
Type: System.UInt64
  Format: 0 - 18,446,744,073,709,551,615
Type: System.Single
  Format: -3.402823e38 - 3.402823e38 (exponent notation is not required)
Type: System.Double
  Format: -1.79769313486232e308 - 1.79769313486232e308 (exponent notation is not required)
Type: System.Char
  Format: Any single character
Type: System.Decimal
  Format: Any decimal in standard notation (no exponent)
Type: System.Boolean
  Format: True or False (case insensitive)
Type: System.String
  Format: Any string (null string is not supported and no escaping is done)
Type: System.DateTime
  Format: MM/DD/YYYY
          MM/DD/YYYY HH:MM:SS [AM|PM]
          Month Day Year
          Month Day Year HH:MM:SS [AM|PM]
Type: System.TimeSpan
  Format: DD.HH:MM:SS (Where DD = Days, HH = Hours, MM = minutes, SS = Seconds)
Type: System.Guid
  Format: A GUID, for example: 936DA01F-9ABD-4d9d-80C7-02AF85C822A8
Type: Enumerations
  Format: The enumeration value (or its corresponding integer value).
    Example Enum Definition:
    public enum Days{ Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };
Type: Types with TypeConverterAttribute
  Format: Depends on the Type Converter.
```

----------------------------------------

TITLE: IALink.AddFile Method API Reference
DESCRIPTION: Detailed API documentation for the IALink.AddFile method, including parameters, their types and descriptions, and the return value.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/alink/addfile-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
IALink.AddFile Method:
  Purpose: Adds files to the assembly. Can also be used to create unbound modules.
  Syntax: HRESULT AddFile(mdAssembly AssemblyID, LPCWSTR pszFilename, DWORD dwFlags, IMetaDataEmit* pEmitter, mdFile* pFileToken) PURE;
  Parameters:
    AssemblyID: mdAssembly - Unique ID of the assembly to be augmented.
    pszFilename: LPCWSTR - Fully qualified name of file to be added.
    dwFlags: DWORD - COM+ FileDef flags such as ffContainsNoMetaData and ffWriteable. dwFlags is passed to DefineFile Method.
    pEmitter: IMetaDataEmit* - IMetaDataEmit Interface to be used to emit metadata, if necessary.
    pFileToken: mdFile* - Pointer to where the unique ID of the added file will be stored.
  Return Value:
    HRESULT: Returns S_OK if the method succeeds.
  Requirements: alink.h
```

----------------------------------------

TITLE: Invoke Property Change Notification in View Model Property
DESCRIPTION: This C# example demonstrates how a view model property, such as 'IsLogin', can use the RaisePropertyChanged method (from a base class like ExtendedBindableObject) with a lambda expression. This pattern ensures that data-bound controls are notified when the property's value changes, maintaining UI synchronization.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/mvvm.md#_snippet_3

LANGUAGE: csharp
CODE:
```
public bool IsLogin
{
    get => _isLogin;
    set
    {
        _isLogin = value;
        RaisePropertyChanged(() => IsLogin);
    }
}
```

----------------------------------------

TITLE: Get Document Version for ISymUnmanagedReader Interface
DESCRIPTION: This API method retrieves the version of a specified document from the symbol store. It takes a pointer to an ISymUnmanagedDocument object, and outputs the document's version number and a boolean indicating if it's the current version. The version starts at 1 and increments with each update via UpdateSymbolStore.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/diagnostics/isymunmanagedreader-getdocumentversion-method.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
Method: ISymUnmanagedReader::GetDocumentVersion

Signature:
HRESULT GetDocumentVersion (
    [in]  ISymUnmanagedDocument *pDoc,
    [out] int* version,
    [out] BOOL* pbCurrent
);

Parameters:
pDoc [in]: The specified document.
version [out]: A pointer to a variable that receives the version of the specified document.
pbCurrent [out]: A pointer to a variable that receives true if this is the latest version of the document, or false if it isn't the latest version.

Return Value:
S_OK if the method succeeds; otherwise, E_FAIL or some other error code.

Requirements:
Header: CorSym.idl, CorSym.h
```

----------------------------------------

TITLE: Adding Access Token to HttpClient Authorization Header in C#
DESCRIPTION: This C# snippet demonstrates how to add an access token to the `HttpClient`'s default request headers. The token is prefixed with 'Bearer' and assigned to the `Authorization` header, ensuring all subsequent requests made by this `HttpClient` instance include the necessary authentication for protected APIs.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/authentication-and-authorization.md#_snippet_18

LANGUAGE: csharp
CODE:
```
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
```

----------------------------------------

TITLE: Workflow Runtime Interaction and Context Classes
DESCRIPTION: Details the classes used by hosts to interact with the workflow runtime and by activities to access the runtime environment.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/windows-workflow-foundation/fundamental-concepts.md#_snippet_2

LANGUAGE: APIDOC
CODE:
```
Host Interaction with Workflow Runtime:
  System.Activities.WorkflowInvoker:
    Capabilities:
      - Synchronously invoke a workflow.
      - Provide input to, or retrieve output from a workflow.
      - Add extensions to be used by activities.

  System.Activities.ActivityInstance:
    Purpose: Thread-safe proxy for hosts to interact with the runtime.
    Capabilities:
      - Acquire an instance by creating it or loading it from an instance store.
      - Be notified of instance life-cycle events.
      - Control workflow execution.
      - Provide input to, or retrieve output from a workflow.
      - Signal a workflow continuation and pass values into the workflow.
      - Persist workflow data.
      - Add extensions to be used by activities.

Activity Access to Workflow Runtime:
  System.Activities.ActivityContext (derived classes like NativeActivityContext, CodeActivityContext):
    Purpose: Provides activities access to the workflow runtime environment.
    Usage: For resolving arguments and variables, scheduling child activities, and other runtime interactions.
```

----------------------------------------

TITLE: Example .NET Package Store Manifest with Newtonsoft.Json and Moq
DESCRIPTION: This example demonstrates a `packages.csproj` file, serving as a package store manifest, configured to include specific versions of the `Newtonsoft.Json` and `Moq` NuGet packages for a runtime package store.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/deploying/runtime-store.md#_snippet_1

LANGUAGE: XML
CODE:
```
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="10.0.3" />
    <PackageReference Include="Moq" Version="4.7.63" />
  </ItemGroup>
</Project>
```

----------------------------------------

TITLE: Error Codes for .NET Package and API Compatibility Validation
DESCRIPTION: Details diagnostic IDs, their descriptions, and recommended actions for resolving issues related to .NET package and API compatibility validation.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/apicompat/diagnostic-ids.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
Diagnostic ID: PKV001
Description: A compile-time asset for a compatible framework is missing.
Recommended action: Add the appropriate target framework to the project.

Diagnostic ID: PKV002
Description: A run-time asset for a compatible framework and runtime is missing.
Recommended action: Add the appropriate asset for corresponding runtime to the package.

Diagnostic ID: PKV003
Description: A run-time independent asset for a compatible framework is missing.
Recommended action: Add the appropriate run-time independent target framework to the project.

Diagnostic ID: PKV004
Description: A compatible run-time asset for a compile-time asset is missing.
Recommended action: Add the appropriate run-time asset to the package.

Diagnostic ID: PKV005
Description: A compatible run-time asset for a compile-time asset and a supported runtime identifier is missing.
Recommended action: Add the appropriate run-time asset to the package.

Diagnostic ID: PKV006
Description: The target framework is dropped in the latest version.
Recommended action: Add the appropriate target framework to the project.

Diagnostic ID: PKV007
Description: The target framework and runtime identifier pair is dropped in the latest version.
Recommended action: Add the appropriate target framework and RID to the project.

Diagnostic ID: CP0001
Description: A type, enum, record, or struct visible outside the assembly is missing in the compared assembly when required to be present.
Recommended action: Add the missing type to the assembly where it is missing.

Diagnostic ID: CP0002
Description: A member that is visible outside of the assembly is missing in the compared assembly when required to be present.
Recommended action: Add the missing member to the assembly where it is missing.

Diagnostic ID: CP0003
Description: Some part of the assembly identity (name, public key token, culture, retargetable attribute, or version) does not match on both sides of the comparison.
Recommended action: Update the assembly identity so that both sides match.

Diagnostic ID: CP0004
Description: A matching assembly was not found on one side of the comparison when creating the assembly mapping.
Recommended action: Make sure the missing assembly is added to the package.

Diagnostic ID: CP0005
Description: An `abstract` member was added to the right side of the comparison to an unsealed type.
Recommended action: Remove the member or don't annotate it as `abstract`.

Diagnostic ID: CP0006
Description: A member was added to an interface without a default implementation.
Recommended action: If the target framework and language version support default implementations, add one, or just remove the member from the interface.

Diagnostic ID: CP0007
Description: A base type on the class hierarchy was removed from one of the compared sides.
Recommended action: Add the base type back. (A new base type can be introduced in the hierarchy if that's intended.)

Diagnostic ID: CP0008
Description: A base interface was removed from the interface hierarchy from one of the compared sides.
Recommended action: Add the interface back to the hierarchy.

Diagnostic ID: CP0009
Description: A type that was unsealed on one side was annotated as `sealed` on the other compared side.
Recommended action: Remove the `sealed` annotation from the type.

Diagnostic ID: CP0010
Description: The underlying type of an enum changed from one side to the other.
Recommended action: Change the underlying type back to what it was previously.

Diagnostic ID: CP0011
Description: The value of a member in an enum changed from one side to the other.
Recommended action: Change the value of the member back to what it was previously.

Diagnostic ID: CP0012
Description: Either the `virtual` or `abstract` keyword was removed from a member that was previously virtual or abstract.
Recommended action: If the member was previously virtual, add the `virtual` keyword back. If the member was previously abstract, add either the `virtual` or `abstract` keyword to the member.

Diagnostic ID: CP0013
Description: The `virtual` keyword was added to a member that was previously not virtual.
Recommended action: Remove the `virtual` keyword from the member.

Diagnostic ID: CP0014
Description: An attribute was removed from a member that previously had it.
Recommended action: Add the attribute back to the member.

Diagnostic ID: CP0015
Description: The arguments passed to an attribute changed from one side to the other.
Recommended action: Change the arguments to the attribute back to what they were previously.

Diagnostic ID: CP0016
Description: An attribute was added to a member that previously did not have it.
Recommended action: Remove the attribute from the member.

Diagnostic ID: CP0017
Description: The name of a method's parameter changed from one side to the other.
Recommended action: Change the parameter's name back to what it was previously.

Diagnostic ID: CP0018
Description: The `sealed` keyword was added to an interface member that was previously not sealed.
Recommended action: Remove the `sealed` keyword from the interface member.

Diagnostic ID: CP0019
Description: The visibility of a member was reduced from one side to the other.
Recommended action: Change the member's visibility back to what it was previously.

Diagnostic ID: CP0020
Description: The visibility of a member was expanded from one side to the other.
Recommended action: Change the member's visibility back to what it was previously.

Diagnostic ID: CP1001
Description: A matching assembly could not be found in the search directories. (Not applicable for package validation, only when using API Compat directly.)
Recommended action: Provide the search directory when loading matching assemblies using `AssemblySymbolLoader`.
```

----------------------------------------

TITLE: ICorProfilerModuleEnum::Next Method API Details
DESCRIPTION: Comprehensive documentation for the `ICorProfilerModuleEnum::Next` method, detailing its parameters, their types and descriptions, and possible HRESULT return values.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/icorprofilermoduleenum-next-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Method: ICorProfilerModuleEnum::Next
  Signature: HRESULT Next([in] ULONG celt, [out, size_is(celt), length_is(*pceltFetched)] ModuleID ids[], [out] ULONG * pceltFetched)
  Parameters:
    celt:
      Direction: [in]
      Type: ULONG
      Description: The number of modules to retrieve.
    ids:
      Direction: [out, size_is(celt), length_is(*pceltFetched)]
      Type: ModuleID[]
      Description: An array of ModuleID values, each of which represents a retrieved module.
    pceltFetched:
      Direction: [out]
      Type: ULONG *
      Description: A pointer to the number of elements actually returned in the ids array.
  Return Value:
    Description: This method returns the following specific HRESULTs as well as HRESULT errors that indicate method failure.
    HRESULTs:
      S_OK: celt elements were returned.
      S_FALSE: Fewer than celt elements were returned, which indicates that the enumeration is complete.
```

----------------------------------------

TITLE: Configuring IdentityServer Authorization Middleware in C#
DESCRIPTION: This C# extension method, `AddDefaultAuthentication`, configures IdentityServer's authorization middleware. It prevents 'sub' claim mapping, adds JWT Bearer authentication with specified authority and audience, and enables authorization services, ensuring API access requires a valid access token.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/authentication-and-authorization.md#_snippet_15

LANGUAGE: csharp
CODE:
```
public static IServiceCollection AddDefaultAuthentication(this IHostApplicationBuilder builder)
{
    var services = builder.Services;
    var configuration = builder.Configuration;

    var identitySection = configuration.GetSection("Identity");

    if (!identitySection.Exists())
    {
        // No identity section, so no authentication
        return services;
    }

    // prevent from mapping "sub" claim to nameidentifier.
    JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

    services.AddAuthentication().AddJwtBearer(options =>
    {
        var identityUrl = identitySection.GetRequiredValue("Url");
        var audience = identitySection.GetRequiredValue("Audience");

        options.Authority = identityUrl;
        options.RequireHttpsMetadata = false;
        options.Audience = audience;
        options.TokenValidationParameters.ValidIssuers = [identityUrl];
        options.TokenValidationParameters.ValidateAudience = false;
    });

    services.AddAuthorization();

    return services;
}
```

----------------------------------------

TITLE: Example Custom Framework Structure
DESCRIPTION: Illustrates the final directory structure of the custom iOS framework after all packaging steps are completed, showing the binary and metadata file.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/deploying/native-aot/ios-like-platforms/creating-and-consuming-custom-frameworks.md#_snippet_9

LANGUAGE: text
CODE:
```
MyNativeAOTLibrary.framework
    |_ MyNativeAOTLibrary
    |_ Info.plist
```

----------------------------------------

TITLE: Illustrating TransactionScopeOption values in C#
DESCRIPTION: This C# example showcases the different behaviors of nested TransactionScope objects when instantiated with various TransactionScopeOption values. It demonstrates how Required joins an existing transaction, RequiresNew always starts a new transaction, and Suppress ensures no transaction participation, even when an ambient transaction is present.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/data/transactions/implementing-an-implicit-transaction-using-transaction-scope.md#_snippet_2

LANGUAGE: csharp
CODE:
```
using(TransactionScope scope1 = new TransactionScope())
//Default is Required
{
    using(TransactionScope scope2 = new TransactionScope(TransactionScopeOption.Required))
    {
        //...
    }

    using(TransactionScope scope3 = new TransactionScope(TransactionScopeOption.RequiresNew))
    {
        //...  
    }
  
    using(TransactionScope scope4 = new TransactionScope(TransactionScopeOption.Suppress))
    {
        //...  
    }
}
```

----------------------------------------

TITLE: API Documentation for CreateVersionStringFromModule Function
DESCRIPTION: Detailed API documentation for the `CreateVersionStringFromModule` function, including parameters, return values, and remarks.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/unmanaged-api/debugging/createversionstringfrommodule-function.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
CreateVersionStringFromModule Function:
  Description: Creates a version string from a common language runtime (CLR) path in a target process.

  Syntax:
    HRESULT CreateVersionStringFromModule (
        [in]  DWORD      pidDebuggee,
        [in]  LPCWSTR    szModuleName,
        [out, size_is(cchBuffer),
        length_is(*pdwLength)] LPWSTR Buffer,
        [in]  DWORD      cchBuffer,
        [out] DWORD*     pdwLength
    );

  Parameters:
    pidDebuggee [in]: Identifier of the process in which the target CLR is loaded.
    szModuleName [in]: Full or relative path to the target CLR that is loaded in the process.
    pBuffer [out]: Return buffer for storing the version string for the target CLR.
    cchBuffer [in]: Size of pBuffer.
    pdwLength [out]: Length of the version string returned by pBuffer.

  Return Values:
    S_OK: The version string for the target CLR was successfully returned in pBuffer.
    E_INVALIDARG: szModuleName is null, or either pBuffer or cchBuffer is null. pBuffer and cchBuffer must both be null or non-null.
    HRESULT_FROM_WIN32(ERROR_INSUFFICIENT_BUFFER): pdwLength is greater than cchBuffer. This may be an expected result if you have passed null for both pBuffer and cchBuffer, and queried the necessary buffer size by using pdwLength.
    HRESULT_FROM_WIN32(ERROR_MOD_NOT_FOUND): szModuleName does not contain a path to a valid CLR in the target process.
    E_FAIL (or other E_ return codes): pidDebuggee does not refer to a valid process, or other failure.

  Remarks:
    This function accepts a CLR process that is identified by pidDebuggee and a string path that is specified by szModuleName. The version string is returned in the buffer that pBuffer points to. This string is opaque to the function user; that is, there is no intrinsic meaning in the version string itself. It is used solely in the context of this function and the CreateDebuggingInterfaceFromVersion function.
    This function should be called twice. When you call it the first time, pass null for both pBuffer and cchBuffer. When you do this, the size of the buffer necessary for pBuffer will be returned in pdwLength. You can then call the function a second time, and pass the buffer in pBuffer and its size in cchBuffer.

  Requirements:
    Platforms: See .NET supported operating systems.
    Header: dbgshim.h
    Library: dbgshim.dll, libdbgshim.so, libdbgshim.dylib
    .NET Versions: Available since .NET Core 2.1
```

----------------------------------------

TITLE: Affected System.Text.Json APIs by Reflection Fallback Setting
DESCRIPTION: This section lists the System.Text.Json APIs that are affected by the 'System.Text.Json.Serialization.EnableSourceGenReflectionFallback' AppContext switch. These APIs include methods for getting converters and various overloads of the JsonSerializer.Serialize method.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/compatibility/serialization/7.0/reflection-fallback.md#_snippet_8

LANGUAGE: APIDOC
CODE:
```
System.Text.Json.JsonSerializerOptions.GetConverter(System.Type)
System.Text.Json.JsonSerializer.Serialize(System.IO.Stream, System.Object, System.Type, System.Text.Json.JsonSerializerOptions)
System.Text.Json.JsonSerializer.Serialize(System.Object, System.Type, System.Text.Json.JsonSerializerOptions)
System.Text.Json.JsonSerializer.Serialize(System.Text.Json.Utf8JsonWriter, System.Object, System.Type, System.Text.Json.JsonSerializerOptions)
System.Text.Json.JsonSerializer.Serialize<T>(T, System.Text.Json.JsonSerializerOptions)
System.Text.Json.JsonSerializer.Serialize<T>(System.IO.Stream, T, System.Text.Json.JsonSerializerOptions)
System.Text.Json.JsonSerializer.Serialize<T>(System.Text.Json.Utf8JsonWriter, T, System.Text.Json.JsonSerializerOptions)
System.Text.Json.JsonSerializer.SerializeAsync(System.IO.Stream, System.Object, System.Type, System.Text.Json.JsonSerializerOptions, System.Threading.CancellationToken)
System.Text.Json.JsonSerializer.SerializeAsync<T>(System.IO.Stream, T, System.Text.Json.JsonSerializerOptions, System.Threading.CancellationToken)
```

----------------------------------------

TITLE: IHostSemaphore::ReleaseSemaphore Method API Details
DESCRIPTION: Detailed API documentation for the `IHostSemaphore::ReleaseSemaphore` method, including parameter descriptions, return values, and remarks on its usage by the Common Language Runtime (CLR).
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/hosting/ihostsemaphore-releasesemaphore-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Method: IHostSemaphore::ReleaseSemaphore
Description: Increases the count of the current IHostSemaphore instance by the specified amount.

Parameters:
  lReleaseCount:
    Type: LONG
    Direction: [in]
    Description: The amount by which to increase the count of the current IHostSemaphore instance. This amount must be greater than zero.
  lpPreviousCount:
    Type: LONG*
    Direction: [out]
    Description: A pointer to the previous count, or null if the caller does not require the previous count.

Return Value (HRESULT):
  S_OK: ReleaseSemaphore returned successfully.
  HOST_E_CLRNOTAVAILABLE: The common language runtime (CLR) has not been loaded into a process, or the CLR is in a state in which it cannot run managed code or process the call successfully.
  HOST_E_TIMEOUT: The call timed out.
  HOST_E_NOT_OWNER: The caller does not own the lock.
  HOST_E_ABANDONED: An event was canceled while a blocked thread or fiber was waiting on it.
  E_FAIL: An unknown catastrophic failure occurred. When a method returns E_FAIL, the CLR is no longer usable within the process. Subsequent calls to hosting methods return HOST_E_CLRNOTAVAILABLE.

Remarks:
  The CLR typically calls ReleaseSemaphore to notify the host that it has finished using a resource, passing a value of 1 for the lReleaseCount parameter.
```

----------------------------------------

TITLE: Obsolete .NET Networking APIs (SYSLIB0014)
DESCRIPTION: List of System.Net APIs marked as obsolete starting in .NET 6 (or .NET 9 for ServicePointManager), which generate the SYSLIB0014 compile-time warning. These APIs should be replaced with HttpClient for modern network operations.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/syslib-diagnostics/syslib0014.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.Net.WebRequest:
  - Constructor
  - Create() method
  - CreateHttp() method
  - CreateDefault(System.Uri) method
System.Net.HttpWebRequest:
  - Constructor(System.Runtime.Serialization.SerializationInfo, System.Runtime.Serialization.StreamingContext)
System.Net.ServicePointManager (Obsolete from .NET 9):
  - Class itself
  - FindServicePoint() method
System.Net.WebClient:
  - Constructor

Note: The System.Net.ServicePoint class itself is not marked obsolete, but all methods to obtain its instances are.
```

----------------------------------------

TITLE: IHostAssemblyStore::ProvideAssembly Method API Reference
DESCRIPTION: Comprehensive API documentation for the `IHostAssemblyStore::ProvideAssembly` method, detailing its purpose, input and output parameters, return values, and important remarks regarding its usage.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/hosting/ihostassemblystore-provideassembly-method.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
IHostAssemblyStore::ProvideAssembly Method:
  Purpose: Gets a reference to an assembly not referenced by ICLRAssemblyReferenceList. The CLR calls this for each assembly not in the list.

  Syntax:
    HRESULT ProvideAssembly (
      [in]  AssemblyBindInfo *pBindInfo,
      [out] UINT64           *pAssemblyId,
      [out] UINT64           *pHostContext,
      [out] IStream          **ppStmAssemblyImage,
      [out] IStream          **ppStmPDB
    );

  Parameters:
    pBindInfo [in]: A pointer to an AssemblyBindInfo instance used by the host to determine bind characteristics (e.g., versioning policy, assembly to bind to).
    pAssemblyId [out]: A pointer to a unique identifier for the requested assembly for this IStream.
    pHostContext [out]: A pointer to host-specific data used to determine the evidence of the requested assembly without a platform invoke call. Corresponds to System.Reflection.Assembly.HostContext property.
    ppStmAssemblyImage [out]: A pointer to the address of an IStream that contains the portable executable (PE) image to be loaded, or null if not found.
    ppStmPDB [out]: A pointer to the address of an IStream that contains the program debug (PDB) information, or null if the .pdb file could not be found.

  Return Value (HRESULT):
    S_OK: ProvideAssembly returned successfully.
    HOST_E_CLRNOTAVAILABLE: The CLR has not been loaded into a process, or is in a state where it cannot run managed code or process the call.
    HOST_E_TIMEOUT: The call timed out.
    HOST_E_NOT_OWNER: The caller does not own the lock.
    HOST_E_ABANDONED: An event was canceled while a blocked thread or fiber was waiting on it.
    E_FAIL: An unknown catastrophic failure occurred. CLR is no longer usable within the process. Subsequent calls return HOST_E_CLRNOTAVAILABLE.
    COR_E_FILENOTFOUND (0x80070002): The requested assembly could not be located.
    E_NOT_SUFFICIENT_BUFFER: The buffer size specified by pAssemblyId is not large enough to hold the identifier.

  Remarks:
    The identity value for pAssemblyId is host-specified and must be unique within the process lifetime. The CLR uses this as a unique stream identifier. If the host returns the same pAssemblyId for another IStream, the CLR checks if the stream contents are already mapped. If so, it loads the existing copy instead of mapping a new one.
```

----------------------------------------

TITLE: CreateInstanceEnumWmi Function API Documentation
DESCRIPTION: Detailed API documentation for the `CreateInstanceEnumWmi` function, including its purpose, required parameters with types and descriptions, and possible return values with their corresponding error codes and explanations.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/wmi/createinstanceenumwmi.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Function: CreateInstanceEnumWmi
  Return Type: HRESULT
  Description: Returns an enumerator that returns the instances of a specified class that meet specified selection criteria.

  Parameters:
    strFilter:
      Direction: [in]
      Type: BSTR
      Description: The name of the class for which instances are desired. This parameter cannot be null.
    lFlags:
      Direction: [in]
      Type: long
      Description: A combination of flags that affect the behavior of this function.
      Constants:
        WBEM_FLAG_USE_AMENDED_QUALIFIERS (0x20000): If set, the function retrieves the amended qualifiers stored in the localized namespace of the current connection's locale. If not set, the function retrieves only the qualifiers stored in the immediate namespace.
        WBEM_FLAG_DEEP (0): The enumeration includes this and all subclasses in the hierarchy.
        WBEM_FLAG_SHALLOW (1): The enumeration includes only pure instances of this class and excludes all instances of subclasses that supply properties not found in this class.
        WBEM_FLAG_RETURN_IMMEDIATELY (0x10): The flag causes a semisynchronous call.
        WBEM_FLAG_FORWARD_ONLY (0x20): The function returns a forward-only enumerator. Typically, forward-only enumerators are faster and use less memory than conventional enumerators, but they do not allow calls to Clone.
        WBEM_FLAG_BIDIRECTIONAL (0): WMI retains pointers to objects in the enumeration until they are released.
      Recommended Flags: WBEM_FLAG_RETURN_IMMEDIATELY, WBEM_FLAG_FORWARD_ONLY for best performance.
    pCtx:
      Direction: [in]
      Type: IWbemContext*
      Description: Typically, this value is null. Otherwise, it is a pointer to an IWbemContext instance that may be used by the provider that is providing the requested instances.
    ppEnum:
      Direction: [out]
      Type: IEnumWbemClassObject**
      Description: Receives the pointer to the enumerator.
    authLevel:
      Direction: [in]
      Type: DWORD
      Description: The authorization level.
    impLevel:
      Direction: [in]
      Type: DWORD
      Description: The impersonation level.
    pCurrentNamespace:
      Direction: [in]
      Type: IWbemServices*
      Description: A pointer to an IWbemServices object that represents the current namespace.
    strUser:
      Direction: [in]
      Type: BSTR
      Description: The user name. See the ConnectServerWmi function for more information.
    strPassword:
      Direction: [in]
      Type: BSTR
      Description: The password. See the ConnectServerWmi function for more information.
    strAuthority:
      Direction: [in]
      Type: BSTR
      Description: The domain name of the user. See the ConnectServerWmi function for more information.

  Return Values:
    WBEM_E_ACCESS_DENIED (0x80041003): The user does not have permission to view instances of the specified class.
    WBEM_E_FAILED (0x80041001): An unspecified error has occurred.
    WBEM_E_INVALID_CLASS (0x80041010): strFilter does not exist.
    WBEM_E_INVALID_PARAMETER (0x80041008): A parameter is not valid.
    WBEM_E_OUT_OF_MEMORY (0x80041006): Not enough memory is available to complete the operation.
    WBEM_E_SHUTTING_DOWN (0x80041033): WMI was probably stopped and restarting. Call ConnectServerWmi again.
    WBEM_E_TRANSPORT_FAILURE (0x80041015): The remote procedure call (RPC) link between the current process and WMI has failed.
    WBEM_S_NO_ERROR (0): The function call was successful.
```

----------------------------------------

TITLE: C#: Example of obsolete Remoting API usage
DESCRIPTION: Demonstrates the usage of an obsolete remoting API, InitializeLifetimeService, which throws a PlatformNotSupportedException at runtime and generates a SYSLIB0010 compile-time warning in .NET 5 and later versions.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/compatibility/core-libraries/5.0/remoting-apis-obsolete.md#_snippet_0

LANGUAGE: csharp
CODE:
```
// MemoryStream, like all Stream instances, subclasses MarshalByRefObject.
MemoryStream stream = new MemoryStream();
// Throws PlatformNotSupportedException; also produces warning SYSLIB0010.
obj.InitializeLifetimeService();
```

----------------------------------------

TITLE: Workflow Services and Messaging Activities
DESCRIPTION: Explains how workflows implement and access loosely-coupled services using messaging activities built on WCF, and how workflow services are hosted.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/windows-workflow-foundation/fundamental-concepts.md#_snippet_3

LANGUAGE: APIDOC
CODE:
```
Workflow Services:
  Messaging Activities:
    Purpose: Primary mechanism to get data into and out of a workflow.
    Foundation: Built on WCF.
    Capability: Compose to model any message exchange pattern.
  System.ServiceModel.Activities.WorkflowServiceHost:
    Purpose: Class used for hosting workflow services.
```

----------------------------------------

TITLE: Guidelines for ArrayList and List<T> in Public APIs
DESCRIPTION: Recommendations against using `ArrayList` and `List<T>` in public APIs due to their nature as internal data structures and potential for API inflexibility.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/standard/design-guidelines/guidelines-for-collections.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
System.Collections.ArrayList:
  - DO NOT use in public APIs.
  - Reason: Internal implementation data structure.

System.Collections.Generic.List<T>:
  - DO NOT use in public APIs.
  - Reason: Internal implementation data structure, optimized for performance/power at cost of API cleanness/flexibility.
  - Example: Cannot receive notifications when client code modifies the collection if returned.
  - Exposes many members (e.g., BinarySearch) not useful in public API scenarios.
```

----------------------------------------

TITLE: Start and List Running Docker Containers
DESCRIPTION: This example demonstrates how to start a named Docker container using `docker start` and then verify its running status with `docker ps`. The `docker ps` command only shows currently running containers.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/core/docker/build-container.md#_snippet_17

LANGUAGE: console
CODE:
```
docker start core-counter
core-counter

docker ps
CONTAINER ID   IMAGE           COMMAND                  CREATED          STATUS          PORTS     NAMES
cf01364df453   counter-image   "dotnet DotNet.Docke…"   53 seconds ago   Up 10 seconds             core-counter
```

----------------------------------------

TITLE: PutInstanceWmi Function API Reference
DESCRIPTION: Detailed API documentation for the PutInstanceWmi function, including its purpose, required parameters, and possible return values with their descriptions.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/wmi/putinstancewmi.md#_snippet_1

LANGUAGE: APIDOC
CODE:
```
Function: PutInstanceWmi
  Description: Creates or updates an instance of an existing class. The instance is written to the WMI repository.
  Signature: HRESULT PutInstanceWmi (
    [in] IWbemClassObject* pInst,
    [in] long lFlags,
    [in] IWbemContext* pCtx,
    [out] IWbemCallResult** ppCallResult
  )
  Parameters:
    pInst:
      Type: [in] IWbemClassObject*
      Description: A pointer to the instance to be written.
    lFlags:
      Type: [in] long
      Description: A combination of flags that affect the behavior of this function.
      Constants:
        WBEM_FLAG_USE_AMENDED_QUALIFIERS (0x20000): If set, WMI does not store any qualifiers with the Amended flavor. If not set, it is assumed that this object is not localized, and all qualifiers are stored with this instance.
        WBEM_FLAG_CREATE_OR_UPDATE (0): Create the instance if it does not exist, or overwrite it if it exists already.
        WBEM_FLAG_UPDATE_ONLY (1): Update the instance. The instance must exist for the call to be successful.
        WBEM_FLAG_CREATE_ONLY (2): Create the instance. The call fails if the instance already exists.
        WBEM_FLAG_RETURN_IMMEDIATELY (0x10): The flag causes a semisynchronous call.
    pCtx:
      Type: [in] IWbemContext*
      Description: Typically, this value is null. Otherwise, it is a pointer to an IWbemContext instance that can be used by the provider that is providing the requested classes.
    ppCallResult:
      Type: [out] IWbemCallResult**
      Description: If null, this parameter is unused. If lFlags contains WBEM_FLAG_RETURN_IMMEDIATELY, the function returns immediately with WBEM_S_NO_ERROR. The ppCallResult parameter receives a pointer to a new IWbemCallResult object.
  Return Value: HRESULT
    Possible Values:
      WBEM_E_ACCESS_DENIED (0x80041003): The user does not have permission to update an instance of the specified class.
      WBEM_E_FAILED (0x80041001): An unspecified error has occurred.
      WBEM_E_INVALID_CLASS (0x80041010): The class supporting this instance is not valid.
      WBEM_E_ILLEGAL_NULL (0x80041028): a null was specified for a property that cannot be null, such as one that is marked by an Indexed or Not_Null qualifier.
      WBEM_E_INVALID_OBJECT (0x8004100f): The specified instance is not valid. (For example, calling PutInstanceWmi with a class returns this value.)
      WBEM_E_INVALID_PARAMETER (0x80041008): A parameter is not valid.
      WBEM_E_ALREADY_EXISTS (0x80041019): The WBEM_FLAG_CREATE_ONLY flag was specified, but the instance already exists.
      WBEM_E_NOT_FOUND (0x80041002): WBEM_FLAG_UPDATE_ONLY was specified in lFlags, but the instance does not exist.
      WBEM_E_OUT_OF_MEMORY (0x80041006): Not enough memory is available to complete the operation.
      WBEM_E_SHUTTING_DOWN (0x80041033): WMI was probably stopped and restarting. Call ConnectServerWmi again.
      WBEM_E_TRANSPORT_FAILURE (0x80041015): The remote procedure call (RPC) link between the current process and WMI has failed.
      WBEM_S_NO_ERROR (0): The function call was successful.
```

----------------------------------------

TITLE: XML Example: Subscribing to Workflow Instance Started State
DESCRIPTION: Demonstrates how to configure a workflow instance query to subscribe to the 'Started' state, capturing workflow instance-level tracking records.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/configure-apps/file-schema/wcf/workflowinstancequeries-of-wcf.md#_snippet_1

LANGUAGE: XML
CODE:
```
<workflowInstanceQueries>
  <workflowInstanceQuery>
    <states>
      <state name="Started" />
    </states>
  </workflowInstanceQuery>
</workflowInstanceQueries>
```

----------------------------------------

TITLE: Calling String Extension Method Example (Visual Basic)
DESCRIPTION: This snippet demonstrates how to consume the previously defined extension method. It imports the `StringExtensions` module and then calls `PrintAndPunctuate` on a `String` variable `example`, showcasing how extension methods are invoked as if they were regular instance methods.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/visual-basic/programming-guide/language-features/procedures/how-to-write-an-extension-method.md#_snippet_4

LANGUAGE: Visual Basic
CODE:
```
' Import the module that holds the extension method you want to use,
' and call it.

Imports ConsoleApplication2.StringExtensions

Module Module1

    Sub Main()
        Dim example = "Hello"
        example.PrintAndPunctuate("?")
        example.PrintAndPunctuate("!!!!")
    End Sub

End Module
```

----------------------------------------

TITLE: New APIs for ASP.NET Applications
DESCRIPTION: Documentation for new APIs introduced in .NET Framework 4.5.2 to enhance ASP.NET application development. These APIs provide more efficient ways to manage HTTP responses and schedule background tasks.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/whats-new/index.md#_snippet_82

LANGUAGE: APIDOC
CODE:
```
System.Web.HttpResponse.AddOnSendingHeaders()
System.Web.HttpResponseBase.AddOnSendingHeaders()
System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem()
System.Web.HttpResponse.HeadersWritten
System.Web.HttpResponseBase.HeadersWritten
System.Web.HttpResponse.StatusCode (example of API that throws exceptions if headers written)
```

----------------------------------------

TITLE: Obsolete Code Access Security (CAS) APIs in .NET
DESCRIPTION: A comprehensive list of Code Access Security (CAS) related APIs that are obsolete starting in .NET 5 and generate the SYSLIB0003 warning when used. These APIs are part of a deprecated legacy technology.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/syslib-diagnostics/syslib0003.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
System.AppDomain.ExecuteAssembly(System.String,System.String[],System.Byte[],System.Configuration.Assemblies.AssemblyHashAlgorithm)
System.AppDomain.PermissionSet
System.Configuration.ConfigurationPermission
System.Configuration.ConfigurationPermissionAttribute
System.Data.Common.DBDataPermission
System.Data.Common.DBDataPermissionAttribute
System.Data.Odbc.OdbcPermission
System.Data.Odbc.OdbcPermissionAttribute
System.Data.OleDb.OleDbPermission
System.Data.OleDb.OleDbPermissionAttribute
System.Data.OracleClient.OraclePermission
System.Data.OracleClient.OraclePermissionAttribute
System.Data.SqlClient.SqlClientPermission
System.Data.SqlClient.SqlClientPermissionAttribute
System.Diagnostics.EventLogPermission
System.Diagnostics.EventLogPermissionAttribute
System.Diagnostics.PerformanceCounterPermission
System.Diagnostics.PerformanceCounterPermissionAttribute
System.DirectoryServices.DirectoryServicesPermission
System.DirectoryServices.DirectoryServicesPermissionAttribute
System.Drawing.Printing.PrintingPermission
System.Drawing.Printing.PrintingPermissionAttribute
System.Net.DnsPermission
System.Net.DnsPermissionAttribute
System.Net.Mail.SmtpPermission
System.Net.Mail.SmtpPermissionAttribute
System.Net.NetworkInformation.NetworkInformationPermission
System.Net.NetworkInformation.NetworkInformationPermissionAttribute
System.Net.PeerToPeer.Collaboration.PeerCollaborationPermission
System.Net.PeerToPeer.Collaboration.PeerCollaborationPermissionAttribute
System.Net.PeerToPeer.PnrpPermission
System.Net.PeerToPeer.PnrpPermissionAttribute
System.Net.SocketPermission
System.Net.SocketPermissionAttribute
System.Net.WebPermission
System.Net.WebPermissionAttribute
System.Runtime.InteropServices.AllowReversePInvokeCallsAttribute
System.Security.CodeAccessPermission
System.Security.HostProtectionException
System.Security.IPermission
System.Security.IStackWalk
System.Security.NamedPermissionSet
System.Security.PermissionSet
System.Security.Permissions.CodeAccessSecurityAttribute
System.Security.Permissions.DataProtectionPermission
System.Security.Permissions.DataProtectionPermissionAttribute
System.Security.Permissions.DataProtectionPermissionFlags
System.Security.Permissions.EnvironmentPermission
System.Security.Permissions.EnvironmentPermissionAccess
System.Security.Permissions.EnvironmentPermissionAttribute
System.Security.Permissions.FileDialogPermission
```

----------------------------------------

TITLE: Replacing Explicit Type with 'var' in C#
DESCRIPTION: This snippet conditionally replaces the explicit type name in a local variable declaration with the `var` keyword. The replacement occurs only if the inferred type of the initializer matches the declared type, preserving leading and trailing trivia to maintain formatting. If types don't match, the original node is returned.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-transformation.md#_snippet_11

LANGUAGE: C#
CODE:
```
if (varType.Type.Equals(declarationType.Type))
{
    TypeSyntax newTypeName = SyntaxFactory.IdentifierName("var")
        .WithLeadingTrivia(node.Declaration.Type.GetLeadingTrivia())
        .WithTrailingTrivia(node.Declaration.Type.GetTrailingTrivia());

    return node.ReplaceNode(node.Declaration.Type, newTypeName);
}
else
{
    return node;
}
```

----------------------------------------

TITLE: Take First Three Addresses in Seattle
DESCRIPTION: This example illustrates a nested use of the `Take` method, applied after an initial filtering operation. It retrieves only the first three addresses located in 'Seattle' from the `Address` table, demonstrating how to limit results from a specific subset of data.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/data/adonet/method-based-query-syntax-examples-partitioning-linq.md#_snippet_5

LANGUAGE: csharp
CODE:
```
var addresses = dataSet.Tables["Address"].AsEnumerable()
    .Where(a => a.Field<string>("City") == "Seattle")
    .Take(3);
```

LANGUAGE: vb
CODE:
```
Dim addresses = dataSet.Tables("Address").AsEnumerable() _
    .Where(Function(a) a.Field(Of String)("City") = "Seattle") _
    .Take(3)
```

----------------------------------------

TITLE: Excluding Specific C# Local Declarations from Rewriting
DESCRIPTION: This code adds conditional checks within `VisitLocalDeclarationStatement` to skip rewriting local variable declarations that involve multiple variables or lack an initializer. The method returns the original node unmodified if these conditions are met, ensuring only simple `Type variable = expression;` forms are processed.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/csharp/roslyn-sdk/get-started/syntax-transformation.md#_snippet_8

LANGUAGE: C#
CODE:
```
// Only deal with single variable declarations
if (node.Declaration.Variables.Count > 1)
    return node;

// Only deal with declarations that have an initializer
if (node.Declaration.Variables[0].Initializer == null)
    return node;
```

----------------------------------------

TITLE: Sequence Expressions
DESCRIPTION: Learn about sequence expressions, which let you generate sequences of data on-demand.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fsharp/language-reference/index.md#_snippet_22

LANGUAGE: APIDOC
CODE:
```
Sequence Expressions:
  Description: Generate sequences of data on-demand.
```

----------------------------------------

TITLE: Subscribe to a message with IMessenger.Register
DESCRIPTION: This example illustrates how to subscribe to the `AddProductMessage` using `WeakReferenceMessenger.Default.Register<T>`. It registers a callback delegate that executes UI updates upon receiving the message. It's important to use the `recipient` parameter instead of `this` within the callback to improve performance and avoid capturing the object, and payload data should be immutable to prevent concurrency errors.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/communicating-between-components.md#_snippet_2

LANGUAGE: csharp
CODE:
```
WeakReferenceMessenger.Default
    .Register<CatalogView, Messages.AddProductMessage>(
        this,
        async (recipient, message) =>
        {
            await recipient.Dispatcher.DispatchAsync(
                async () =>
                {
                    await recipient.badge.ScaleTo(1.2);
                    await recipient.badge.ScaleTo(1.0);
                });
        });
```

----------------------------------------

TITLE: ICorProfilerInfo7 Interface API Reference
DESCRIPTION: Detailed API documentation for the ICorProfilerInfo7 interface, including its purpose, supported versions, and methods. This interface extends ICorProfilerInfo6.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/unmanaged-api/profiling/icorprofilerinfo7-interface.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
ICorProfilerInfo7 Interface:
  Supported in the .NET Framework 4.6.1 and later versions
  Description: A subclass of ICorProfilerInfo6 that provides a method to apply newly defined metadata to a module and that provides access to an in-memory symbol stream.

Methods:
  ApplyMetaData Method:
    Description: Applies the metadata newly defined by the IMetadataEmit::Define* methods to a specified module.
  GetInMemorySymbolsLength Method:
    Description: Returns the length of an in-memory symbol stream.
  ReadInMemorySymbols:
    Description: Reads bytes from an in-memory symbol stream.

Requirements:
  Platforms: See System Requirements.
  Header: CorProf.idl, CorProf.h
  .NET Framework Versions: .NET Framework 4.6.1 and later
```

----------------------------------------

TITLE: HttpClient and HttpRequestException API Reference
DESCRIPTION: Reference for key HttpClient methods and HttpRequestException constructor relevant to HTTP error handling and proxy configuration.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/fundamentals/networking/http/httpclient.md#_snippet_18

LANGUAGE: APIDOC
CODE:
```
System.Net.Http.HttpClient:
  GetByteArrayAsync(requestUri: string, cancellationToken: CancellationToken): Task<byte[]>
    Description: Sends a GET request to the specified URI and returns the response body as a byte array. Implicitly calls EnsureSuccessStatusCode.
  GetStreamAsync(requestUri: string, cancellationToken: CancellationToken): Task<Stream>
    Description: Sends a GET request to the specified URI and returns the response body as a stream. Implicitly calls EnsureSuccessStatusCode.
  GetStringAsync(requestUri: string, cancellationToken: CancellationToken): Task<string>
    Description: Sends a GET request to the specified URI and returns the response body as a string. Implicitly calls EnsureSuccessStatusCode.
  DefaultProxy: IWebProxy
    Description: Gets or sets the default proxy used by all HttpClient instances.

System.Net.Http.HttpClientHandler:
  Proxy: IWebProxy
    Description: Gets or sets the proxy used by this HttpClientHandler.

System.Net.Http.HttpRequestException:
  __ctor__(message: string, innerException: Exception = null, statusCode: HttpStatusCode = null)
    Description: Initializes a new instance of the HttpRequestException class with a specified error message, a reference to the inner exception that is the cause of this exception, and an optional HTTP status code.
  StatusCode: HttpStatusCode?
    Description: Gets the HTTP status code of the response if the exception was thrown as a result of an unsuccessful HTTP response.

System.Threading.Tasks.TaskCanceledException:
  Description: The exception that is thrown when a Task is canceled.
  InnerException: Exception
    Description: Gets the Exception instance that caused the current exception. Can be TimeoutException for HTTP timeouts.

System.TimeoutException:
  Description: The exception that is thrown when the time allotted for an operation has expired.
```

----------------------------------------

TITLE: Creating Commands with RelayCommand Attribute in C#
DESCRIPTION: The "RelayCommand" attribute simplifies command creation by automatically generating "RelayCommand" or "AsyncRelayCommand" instances from methods. This example demonstrates applying the attribute to both synchronous ("Validate") and asynchronous ("SettingsAsync") methods within a ViewModel.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/architecture/maui/mvvm-community-toolkit-features.md#_snippet_5

LANGUAGE: csharp
CODE:
```
public partial class SampleViewModel : ObservableObject
{
    public INavigationService NavigationService { get; set; }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    bool _isValid;

    [RelayCommand]
    private Task SettingsAsync()
    {
        return NavigationService.NavigateToAsync("Settings");
    }

    [RelayCommand]
    private void Validate()
    {
        IsValid = !string.IsNullOrEmpty(Name);
    }
}
```

----------------------------------------

TITLE: Understanding UI Automation Properties and Their Usage
DESCRIPTION: This API documentation outlines the structure and usage of UI Automation properties for client applications, including how properties are exposed, their read-only nature, and how property IDs are used for element identification and searching.
SOURCE: https://github.com/dotnet/docs/blob/main/docs/framework/ui-automation/ui-automation-properties-for-clients.md#_snippet_0

LANGUAGE: APIDOC
CODE:
```
UI Automation Properties Overview:

1.  AutomationElement Properties:
    -   Generic properties of UI elements.
    -   Exposed via: System.Windows.Automation.AutomationElement.AutomationElementInformation structure.
    -   Usage: Read-only. To modify, use methods of appropriate control patterns.

2.  Control Pattern Properties (e.g., ScrollPattern):
    -   Specific to the control pattern (e.g., ScrollPattern properties for scrollability, view sizes, positions).
    -   Exposed via: ControlPattern.ControlPatternInformation structure (e.g., System.Windows.Automation.ScrollPattern.ScrollPatternInformation).
    -   Methods for setting properties:
        -   System.Windows.Automation.ScrollPattern.Scroll(double horizontalAmount, double verticalAmount)

3.  Property Identifiers (IDs):
    -   Encapsulated in: System.Windows.Automation.AutomationProperty objects.
    -   Client applications obtain IDs from:
        -   System.Windows.Automation.AutomationElement class
        -   Specific control pattern classes (e.g., System.Windows.Automation.ScrollPattern)
    -   UI Automation providers obtain IDs from:
        -   System.Windows.Automation.AutomationElementIdentifiers
        -   Control pattern identifiers classes (e.Windows.Automation.ScrollPatternIdentifiers)
    -   Key properties of AutomationIdentifier:
        -   Id: Numeric ID, used by providers in System.Windows.Automation.Provider.IRawElementProviderSimple.GetPropertyValue()
        -   ProgrammaticName: Used for debugging/diagnostics.

4.  Property Conditions for Finding Elements:
    -   Used in: System.Windows.Automation.PropertyCondition objects.
    -   Purpose: To find System.Windows.Automation.AutomationElement objects based on property values.
    -   Each PropertyCondition specifies an AutomationProperty identifier and a matching value.
    -   Methods using PropertyCondition:
        -   System.Windows.Automation.AutomationElement.FindFirst(TreeScope scope, Condition condition)
        -   System.Windows.Automation.AutomationElement.FindAll(TreeScope scope, Condition condition)
        -   System.Windows.Automation.TreeWalker.Condition property
```