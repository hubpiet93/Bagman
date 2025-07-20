TITLE: Basic Exception vs ErrorOr Comparison in C#
DESCRIPTION: Demonstrates the difference between traditional exception handling and using ErrorOr<T> for error handling in a division operation.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_0

LANGUAGE: csharp
CODE:
```
public float Divide(int a, int b)
{
    if (b == 0)
    {
        throw new Exception("Cannot divide by zero");
    }

    return a / b;
}

try
{
    var result = Divide(4, 2);
    Console.WriteLine(result * 2); // 4
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    return;
}
```

LANGUAGE: csharp
CODE:
```
public ErrorOr<float> Divide(int a, int b)
{
    if (b == 0)
    {
        return Error.Unexpected(description: "Cannot divide by zero");
    }

    return a / b;
}

var result = Divide(4, 2);

if (result.IsError)
{
    Console.WriteLine(result.FirstError.Description);
    return;
}

Console.WriteLine(result.Value * 2); // 4
```

----------------------------------------

TITLE: Functional Error Handling with Then/Switch in C#
DESCRIPTION: Shows how to use functional methods Then and Switch for more elegant error handling flow.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_1

LANGUAGE: csharp
CODE:
```
Divide(4, 2)
    .Then(val => val * 2)
    .SwitchFirst(
        onValue: Console.WriteLine, // 4
        onFirstError: error => Console.WriteLine(error.Description));
```

----------------------------------------

TITLE: Handling Errors with ErrorOr in C#
DESCRIPTION: Demonstrates how to use the ErrorOr<T> type to handle errors and access the list of errors that occurred.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_3

LANGUAGE: csharp
CODE:
```
ErrorOr<int> result = User.Create();

if (result.IsError)
{
    result.Errors // contains the list of errors that occurred
        .ForEach(error => Console.WriteLine(error.Description));
}
```

----------------------------------------

TITLE: Multiple Error Handling with User Creation in C#
DESCRIPTION: Demonstrates handling multiple validation errors in a user creation scenario using ErrorOr.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_2

LANGUAGE: csharp
CODE:
```
public class User(string _name)
{
    public static ErrorOr<User> Create(string name)
    {
        List<Error> errors = [];

        if (name.Length < 2)
        {
            errors.Add(Error.Validation(description: "Name is too short"));
        }

        if (name.Length > 100)
        {
            errors.Add(Error.Validation(description: "Name is too long"));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(Error.Validation(description: "Name cannot be empty or whitespace only"));
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return new User(name);
    }
}
```

----------------------------------------

TITLE: Implementing ValidationBehavior with ErrorOr in C#
DESCRIPTION: This code snippet demonstrates how to create a ValidationBehavior that integrates Mediator, FluentValidation, and ErrorOr. It validates requests before they reach the handler and returns errors using ErrorOr instead of throwing exceptions.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_29

LANGUAGE: csharp
CODE:
```
public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IErrorOr
{
    private readonly IValidator<TRequest>? _validator = validator;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator is null)
        {
            return await next();
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }

        var errors = validationResult.Errors
            .ConvertAll(error => Error.Validation(
                code: error.PropertyName,
                description: error.ErrorMessage));

        return (dynamic)errors;
    }
}
```

----------------------------------------

TITLE: Organizing Errors in ErrorOr Library in C#
DESCRIPTION: Demonstrates a recommended approach for organizing and using custom errors with the ErrorOr library.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_28

LANGUAGE: csharp
CODE:
```
public static partial class DivisionErrors
{
    public static Error CannotDivideByZero = Error.Unexpected(
        code: "Division.CannotDivideByZero",
        description: "Cannot divide by zero.");
}

public ErrorOr<float> Divide(int a, int b)
{
    if (b == 0)
    {
        return DivisionErrors.CannotDivideByZero;
    }

    return a / b;
}
```

----------------------------------------

TITLE: Mixing Features in ErrorOr Chains in C#
DESCRIPTION: Shows how to combine various ErrorOr methods like Then, FailIf, Else, and MatchFirst in a single chain.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_24

LANGUAGE: csharp
CODE:
```
ErrorOr<string> foo = await result
    .ThenDoAsync(val => Task.Delay(val))
    .FailIf(val => val > 2, Error.Validation(description: $"{val} is too big"))
    .ThenDo(val => Console.WriteLine($"Finished waiting {val} seconds."))
    .ThenAsync(val => Task.FromResult(val * 2))
    .Then(val => $"The result is {val}")
    .Else(errors => Error.Unexpected())
    .MatchFirst(
        value => value,
        firstError => $"An error occurred: {firstError.Description}");
```

----------------------------------------

TITLE: Creating Custom Error Types in ErrorOr Library in C#
DESCRIPTION: Demonstrates how to create and use custom error types with the ErrorOr library.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_26

LANGUAGE: csharp
CODE:
```
public static class MyErrorTypes
{
    const int ShouldNeverHappen = 12;
}

var error = Error.Custom(
    type: MyErrorTypes.ShouldNeverHappen,
    code: "User.ShouldNeverHappen",
    description: "A user error that should never happen");

var errorMessage = Error.NumericType switch
{
    MyErrorType.ShouldNeverHappen => "Consider replacing dev team",
    _ => "An unknown error occurred.",
};
```

----------------------------------------

TITLE: Built-in Error Types in ErrorOr Library in C#
DESCRIPTION: Lists the built-in error types provided by the ErrorOr library and shows how to create errors of specific types.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_25

LANGUAGE: csharp
CODE:
```
public enum ErrorType
{
    Failure,
    Unexpected,
    Validation,
    Conflict,
    NotFound,
    Unauthorized,
    Forbidden,
}

var error = Error.NotFound();

var customError = Error.Unexpected(
    code: "User.ShouldNeverHappen",
    description: "A user error that should never happen",
    metadata: new Dictionary<string, object>
    {
        { "user", user },
    });
```

----------------------------------------

TITLE: Using Built-in Result Types in ErrorOr Library in C#
DESCRIPTION: Shows the usage of built-in result types like Success, Created, Updated, and Deleted in the ErrorOr library.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_27

LANGUAGE: csharp
CODE:
```
ErrorOr<Success> result = Result.Success;
ErrorOr<Created> result = Result.Created;
ErrorOr<Updated> result = Result.Updated;
ErrorOr<Deleted> result = Result.Deleted;

ErrorOr<Deleted> DeleteUser(Guid id)
{
    var user = await _userRepository.GetByIdAsync(id);
    if (user is null)
    {
        return Error.NotFound(description: "User not found.");
    }

    await _userRepository.DeleteAsync(user);
    return Result.Deleted;
}
```

----------------------------------------

TITLE: Using Match Method with ErrorOr in C#
DESCRIPTION: Shows how to use the Match method to handle both success and error cases in ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_6

LANGUAGE: csharp
CODE:
```
string foo = result.Match(
    value => value,
    errors => $"{errors.Count} errors occurred.");
```

----------------------------------------

TITLE: Using MatchFirst Method with ErrorOr in C#
DESCRIPTION: Shows how to use the MatchFirst method to handle success and the first error in ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_8

LANGUAGE: csharp
CODE:
```
string foo = result.MatchFirst(
    value => value,
    firstError => firstError.Description);
```

----------------------------------------

TITLE: Using Switch Method with ErrorOr in C#
DESCRIPTION: Shows how to use the Switch method to execute different actions based on success or error in ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_10

LANGUAGE: csharp
CODE:
```
result.Switch(
    value => Console.WriteLine(value),
    errors => Console.WriteLine($"{errors.Count} errors occurred."));
```

----------------------------------------

TITLE: Using SwitchFirst Method with ErrorOr in C#
DESCRIPTION: Shows how to use the SwitchFirst method to handle success and the first error in ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_12

LANGUAGE: csharp
CODE:
```
result.SwitchFirst(
    value => Console.WriteLine(value),
    firstError => Console.WriteLine(firstError.Description));
```

----------------------------------------

TITLE: Using Then Method with ErrorOr in C#
DESCRIPTION: Shows how to use the Then method to chain operations on successful ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_14

LANGUAGE: csharp
CODE:
```
ErrorOr<int> foo = result
    .Then(val => val * 2);
```

----------------------------------------

TITLE: Chaining Multiple Then Methods with ErrorOr in C#
DESCRIPTION: Demonstrates how to chain multiple Then methods for sequential operations on ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_15

LANGUAGE: csharp
CODE:
```
ErrorOr<string> foo = result
    .Then(val => val * 2)
    .Then(val => $"The result is {val}");
```

----------------------------------------

TITLE: Error Handling in Then Method Chains with ErrorOr in C#
DESCRIPTION: Shows how error handling works when chaining Then methods with ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_16

LANGUAGE: csharp
CODE:
```
ErrorOr<int> Foo() => Error.Unexpected();

ErrorOr<string> foo = result
    .Then(val => val * 2)
    .Then(_ => GetAnError())
    .Then(val => $"The result is {val}") // this function will not be invoked
    .Then(val => $"The result is {val}"); // this function will not be invoked
```

----------------------------------------

TITLE: Using ThenDo and ThenDoAsync Methods with ErrorOr in C#
DESCRIPTION: Shows how to use ThenDo and ThenDoAsync for executing actions in ErrorOr result chains.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_18

LANGUAGE: csharp
CODE:
```
ErrorOr<string> foo = result
    .ThenDo(val => Console.WriteLine(val))
    .ThenDo(val => Console.WriteLine($"The result is {val}"));
```

----------------------------------------

TITLE: Mixing Then, ThenDo, ThenAsync, and ThenDoAsync Methods with ErrorOr in C#
DESCRIPTION: Demonstrates how to combine different Then methods in a single ErrorOr result chain.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_19

LANGUAGE: csharp
CODE:
```
ErrorOr<string> foo = await result
    .ThenDoAsync(val => Task.Delay(val))
    .Then(val => val * 2)
    .ThenAsync(val => DoSomethingAsync(val))
    .ThenDo(val => Console.WriteLine($"Finsihed waiting {val} seconds."))
    .ThenAsync(val => Task.FromResult(val * 2))
    .Then(val => $"The result is {val}");
```

----------------------------------------

TITLE: Using FailIf Method with ErrorOr in C#
DESCRIPTION: Shows how to use the FailIf method to conditionally return an error in ErrorOr result chains.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_20

LANGUAGE: csharp
CODE:
```
ErrorOr<int> foo = result
    .FailIf(val => val > 2, Error.Validation(description: $"{val} is too big"));
```

----------------------------------------

TITLE: Error Handling with FailIf in ErrorOr Chains in C#
DESCRIPTION: Demonstrates how FailIf affects the execution of subsequent operations in ErrorOr result chains.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_21

LANGUAGE: csharp
CODE:
```
var result = "2".ToErrorOr()
    .Then(int.Parse) // 2
    .FailIf(val => val > 1, Error.Validation(description: $"{val} is too big") // validation error
    .Then(num => num * 2) // this function will not be invoked
    .Then(num => num * 2) // this function will not be invoked
```

----------------------------------------

TITLE: Using Else Method with ErrorOr in C#
DESCRIPTION: Shows how to use the Else method to provide fallback values or functions for ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_22

LANGUAGE: csharp
CODE:
```
ErrorOr<string> foo = result
    .Else("fallback value");

ErrorOr<string> bar = result
    .Else(errors => $"{errors.Count} errors occurred.");
```

----------------------------------------

TITLE: Accessing FirstError in ErrorOr Results in C#
DESCRIPTION: Shows how to access the first error from an ErrorOr<T> result when errors occur.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_4

LANGUAGE: csharp
CODE:
```
ErrorOr<int> result = User.Create();

if (result.IsError)
{
    var firstError = result.FirstError; // only the first error that occurred
    Console.WriteLine(firstError == result.Errors[0]); // true
}
```

----------------------------------------

TITLE: Using ErrorsOrEmptyList in ErrorOr Results in C#
DESCRIPTION: Demonstrates the use of ErrorsOrEmptyList property to get a list of errors or an empty list if no errors occurred.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_5

LANGUAGE: csharp
CODE:
```
ErrorOr<int> result = User.Create();

if (result.IsError)
{
    result.ErrorsOrEmptyList // List<Error> { /* one or more errors */  }
    return;
}

result.ErrorsOrEmptyList // List<Error> { }
```

----------------------------------------

TITLE: Using MatchAsync Method with ErrorOr in C#
DESCRIPTION: Demonstrates the asynchronous version of the Match method for ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_7

LANGUAGE: csharp
CODE:
```
string foo = await result.MatchAsync(
    value => Task.FromResult(value),
    errors => Task.FromResult($"{errors.Count} errors occurred."));
```

----------------------------------------

TITLE: Using MatchFirstAsync Method with ErrorOr in C#
DESCRIPTION: Demonstrates the asynchronous version of the MatchFirst method for ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_9

LANGUAGE: csharp
CODE:
```
string foo = await result.MatchFirstAsync(
    value => Task.FromResult(value),
    firstError => Task.FromResult(firstError.Description));
```

----------------------------------------

TITLE: Using SwitchAsync Method with ErrorOr in C#
DESCRIPTION: Demonstrates the asynchronous version of the Switch method for ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_11

LANGUAGE: csharp
CODE:
```
await result.SwitchAsync(
    value => { Console.WriteLine(value); return Task.CompletedTask; },
    errors => { Console.WriteLine($"{errors.Count} errors occurred."); return Task.CompletedTask; });
```

----------------------------------------

TITLE: Using SwitchFirstAsync Method with ErrorOr in C#
DESCRIPTION: Demonstrates the asynchronous version of the SwitchFirst method for ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_13

LANGUAGE: csharp
CODE:
```
await result.SwitchFirstAsync(
    value => { Console.WriteLine(value); return Task.CompletedTask; },
    firstError => { Console.WriteLine(firstError.Description); return Task.CompletedTask; });
```

----------------------------------------

TITLE: Using ThenAsync Method with ErrorOr in C#
DESCRIPTION: Demonstrates the use of ThenAsync for asynchronous operations in ErrorOr result chains.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_17

LANGUAGE: csharp
CODE:
```
ErrorOr<string> foo = await result
    .ThenAsync(val => DoSomethingAsync(val))
    .ThenAsync(val => DoSomethingElseAsync($"The result is {val}"));
```

----------------------------------------

TITLE: Using ElseAsync Method with ErrorOr in C#
DESCRIPTION: Demonstrates the asynchronous version of the Else method for ErrorOr results.
SOURCE: https://github.com/amantinband/error-or/blob/main/README.md#2025-04-17_snippet_23

LANGUAGE: csharp
CODE:
```
ErrorOr<string> foo = await result
    .ElseAsync(Task.FromResult("fallback value"));

ErrorOr<string> bar = await result
    .ElseAsync(errors => Task.FromResult($"{errors.Count} errors occurred."));
```

----------------------------------------

TITLE: Implementing FailIf Method in C#
DESCRIPTION: Method signature for FailIf functionality that takes a predicate and error value to conditionally fail an ErrorOr instance.
SOURCE: https://github.com/amantinband/error-or/blob/main/CHANGELOG.md#2025-04-17_snippet_0

LANGUAGE: csharp
CODE:
```
public ErrorOr<TValue> FailIf(Func<TValue, bool> onValue, Error error)
```

----------------------------------------

TITLE: Using FailIf Method Example
DESCRIPTION: Example usage of the FailIf method showing how to conditionally fail an ErrorOr instance when a value meets certain criteria.
SOURCE: https://github.com/amantinband/error-or/blob/main/CHANGELOG.md#2025-04-17_snippet_1

LANGUAGE: csharp
CODE:
```
ErrorOr<int> errorOr = 1;
errorOr.FailIf(x => x > 0, Error.Failure());
```

----------------------------------------

TITLE: Breaking Change: Then to ThenDo Rename
DESCRIPTION: Breaking change showing the renaming of the Then method to ThenDo for action-based operations.
SOURCE: https://github.com/amantinband/error-or/blob/main/CHANGELOG.md#2025-04-17_snippet_2

LANGUAGE: csharp
CODE:
```
-public ErrorOr<TValue> Then(Action<TValue> action)
+public ErrorOr<TValue> ThenDo(Action<TValue> action)
```

----------------------------------------

TITLE: Breaking Change: Async Then to ThenDo Rename
DESCRIPTION: Breaking change showing the renaming of the async Then method to ThenDo for task-based operations.
SOURCE: https://github.com/amantinband/error-or/blob/main/CHANGELOG.md#2025-04-17_snippet_3

LANGUAGE: csharp
CODE:
```
-public static async Task<ErrorOr<TValue>> Then<TValue>(this Task<ErrorOr<TValue>> errorOr, Action<TValue> action)
+public static async Task<ErrorOr<TValue>> ThenDo<TValue>(this Task<ErrorOr<TValue>> errorOr, Action<TValue> action)
```

----------------------------------------

TITLE: Breaking Change: ThenAsync to ThenDoAsync Rename
DESCRIPTION: Breaking change showing the renaming of the ThenAsync method to ThenDoAsync for async operations.
SOURCE: https://github.com/amantinband/error-or/blob/main/CHANGELOG.md#2025-04-17_snippet_4

LANGUAGE: csharp
CODE:
```
-public async Task<ErrorOr<TValue>> ThenAsync(Func<TValue, Task> action)
+public async Task<ErrorOr<TValue>> ThenDoAsync(Func<TValue, Task> action)
```

----------------------------------------

TITLE: Breaking Change: Task ThenAsync to ThenDoAsync Rename
DESCRIPTION: Breaking change showing the renaming of the task-based ThenAsync method to ThenDoAsync for async operations.
SOURCE: https://github.com/amantinband/error-or/blob/main/CHANGELOG.md#2025-04-17_snippet_5

LANGUAGE: csharp
CODE:
```
-public static async Task<ErrorOr<TValue>> ThenAsync<TValue>(this Task<ErrorOr<TValue>> errorOr, Func<TValue, Task> action)
+public static async Task<ErrorOr<TValue>> ThenDoAsync<TValue>(this Task<ErrorOr<TValue>> errorOr, Func<TValue, Task> action)
```