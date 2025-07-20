TITLE: Install Supabase C# Client via NuGet
DESCRIPTION: Use this command in your project directory to add the Supabase C# client library as a NuGet package, making its functionalities available for use.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/installing

LANGUAGE: C#
CODE:
```
dotnet add package supabase
```

----------------------------------------

TITLE: Initialize Supabase C# Client with Dependency Injection (Maui-like)
DESCRIPTION: This snippet illustrates how to integrate the Supabase C# client using dependency injection, specifically in a Maui-like application context. It registers the client as a singleton service, configuring options such as `AutoRefreshToken` and `AutoConnectRealtime`.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/initializing

LANGUAGE: C#
CODE:
```
public static MauiApp CreateMauiApp()
{
      // ...
      var builder = MauiApp.CreateBuilder();

      var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
      var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");
      var options = new SupabaseOptions
      {
        AutoRefreshToken = true,
        AutoConnectRealtime = true,
        // SessionHandler = new SupabaseSessionHandler() <-- This must be implemented by the developer
      };

      // Note the creation as a singleton.
      builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));
}
```

----------------------------------------

TITLE: Listen to Supabase authentication state changes in C#
DESCRIPTION: This C# code snippet demonstrates how to add a listener for Supabase authentication state changes, allowing the application to react to various events such as user sign-in, sign-out, updates, password recovery, and token refreshes.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-onauthstatechange

LANGUAGE: C#
CODE:
```
supabase.Auth.AddStateChangedListener((sender, changed) =>
{
    switch (changed)
    {
        case AuthState.SignedIn:
            break;
        case AuthState.SignedOut:
            break;
        case AuthState.UserUpdated:
            break;
        case AuthState.PasswordRecovery:
            break;
        case AuthState.TokenRefreshed:
            break;
    }
});
```

----------------------------------------

TITLE: Initialize Supabase C# Client Standardly
DESCRIPTION: This example demonstrates the standard way to initialize the Supabase C# client by providing the project URL and API key. It sets up basic options like `AutoConnectRealtime` and initializes the client asynchronously.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/initializing

LANGUAGE: C#
CODE:
```
var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");

var options = new Supabase.SupabaseOptions
{
    AutoConnectRealtime = true
};

var supabase = new Supabase.Client(url, key, options);
await supabase.InitializeAsync();
```

----------------------------------------

TITLE: Interact with Supabase C# Client using Models
DESCRIPTION: This example demonstrates how to define and use a C# model (`Message`) that maps to a Supabase database table. It shows common CRUD operations (Get, Insert, Update, Delete) using the strongly-typed `BaseModel` and `Table` attributes for mapping.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/initializing

LANGUAGE: C#
CODE:
```
// Given the following Model representing the Supabase Database (Message.cs)
[Table("messages")]
public class Message : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("username")]
    public string UserName { get; set; }

    [Column("channel_id")]
    public int ChannelId { get; set; }

    public override bool Equals(object obj)
    {
        return obj is Message message &&
                Id == message.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}

void Initialize()
{
    // Get All Messages
    var response = await client.Table<Message>().Get();
    List<Message> models = response.Models;

    // Insert
    var newMessage = new Message { UserName = "acupofjose", ChannelId = 1 };
    await client.Table<Message>().Insert();

    // Update
    var model = response.Models.First();
    model.UserName = "elrhomariyounes";
    await model.Update();

    // Delete
    await response.Models.Last().Delete();

    // etc.
}
```

----------------------------------------

TITLE: C#: Getting your data from Supabase
DESCRIPTION: Demonstrates how to fetch all data from a Supabase table using a C# model and the `Get()` method.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

LANGUAGE: csharp
CODE:
```
// Given the following Model (City.cs)
[Table("cities")]
class City : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("country_id")]
    public int CountryId { get; set; }

    //... etc.
}

// A result can be fetched like so.
var result = await supabase.From<City>().Get();
var cities = result.Models
```

----------------------------------------

TITLE: Filter rows where column equals value with Select() in C#
DESCRIPTION: Demonstrates how to filter rows in a Supabase table where a specific column's value exactly matches a given string, using the 'Where' clause and 'Get' method with 'Select()' in C#.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/eq

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Where(x => x.Name == "Bali")
  .Get();
```

----------------------------------------

TITLE: C#: Filtering with inner joins
DESCRIPTION: Explains how to filter data using inner joins with related tables and specific conditions.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

LANGUAGE: csharp
CODE:
```
var result = await supabase
  .From<Movie>()
  .Select("*, users!inner(*)")
  .Filter("user.username", Operator.Equals, "Jane")
  .Get();
```

----------------------------------------

TITLE: Applying Basic Filters in C# Supabase
DESCRIPTION: This snippet demonstrates how to apply a basic equality filter to a column using a LINQ-like `Where` clause on a `Select()` query. It retrieves a single record where the 'Name' property matches 'The Shire'.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/using-filters

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
      .Select(x => new object[] { x.Name, x.CountryId })
      .Where(x => x.Name == "The Shire")
      .Single();
```

----------------------------------------

TITLE: Upload a file to Supabase Storage in C#
DESCRIPTION: Demonstrates how to upload a local file to a specified bucket in Supabase Storage using C#. Examples include basic upload with file options and tracking upload progress with a callback. Requires `insert` permissions on objects.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-upload

LANGUAGE: C#
CODE:
```
var imagePath = Path.Combine("Assets", "fancy-avatar.png");

await supabase.Storage
  .From("avatars")
  .Upload(imagePath, "fancy-avatar.png", new FileOptions { CacheControl = "3600", Upsert = false });
```

LANGUAGE: C#
CODE:
```
var imagePath = Path.Combine("Assets", "fancy-avatar.png");

await supabase.Storage
  .From("avatars")
  .Upload(imagePath, "fancy-avatar.png", onProgress: (sender, progress) => Debug.WriteLine($"{progress}%"));
```

----------------------------------------

TITLE: C#: Filter data where column is not equal to a value using Select()
DESCRIPTION: This C# example demonstrates how to query data from a Supabase table (`City`), select specific columns (`Name`, `CountryId`), and apply a filter to retrieve only rows where the `Name` column's value is not equal to 'Bali'. It uses the `Select()` and `Where()` methods of the Supabase client.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/neq

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Where(x => x.Name != "Bali")
  .Get();
```

----------------------------------------

TITLE: Insert a single record into a Supabase table (C#)
DESCRIPTION: This example demonstrates how to insert a single new record into a Supabase table. It defines a `City` model with `Id`, `Name`, and `CountryId` properties and then inserts an instance of this model using `supabase.From<City>().Insert(model)`.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/insert

LANGUAGE: C#
CODE:
```
[Table("cities")]
class City : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("country_id")]
    public int CountryId { get; set; }
}

var model = new City
{
  Name = "The Shire",
  CountryId = 554
};

await supabase.From<City>().Insert(model);

```

----------------------------------------

TITLE: Supabase C# Auth: Sign In User
DESCRIPTION: Demonstrates how to authenticate an existing user with Supabase using either an email and password or a phone number and password. Both methods return a session object upon successful authentication. This functionality requires either an email and password or a phone number and password.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signinwithpassword

LANGUAGE: C#
CODE:
```
var session = await supabase.Auth.SignIn(email, password);
```

LANGUAGE: C#
CODE:
```
var session = await supabase.Auth.SignIn(SignInType.Phone, phoneNumber, password);
```

----------------------------------------

TITLE: Filter Foreign Tables in C# Supabase
DESCRIPTION: This snippet illustrates how to filter records from a primary table based on conditions in a related foreign table. It uses a `Select` statement with an inner join syntax and then applies a `Filter` on the foreign table's column ('cities.name') to retrieve countries associated with cities named 'Bali'.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/using-filters

LANGUAGE: C#
CODE:
```
var results = await supabase.From<Country>()
  .Select("name, cities!inner(name)")
  .Filter("cities.name", Operator.Equals, "Bali")
  .Get();
```

----------------------------------------

TITLE: Update Data with Filter and Set in C#
DESCRIPTION: Demonstrates how to update data in a Supabase table by filtering records using a 'Where' clause and directly setting new values with the 'Set' method. This approach performs an update operation on matching records without first retrieving them.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/update

LANGUAGE: C#
CODE:
```
var update = await supabase
  .From<City>()
  .Where(x => x.Name == "Auckland")
  .Set(x => x.Name, "Middle Earth")
  .Update();
```

----------------------------------------

TITLE: Limit query results with Range() and Select() in C#
DESCRIPTION: This example demonstrates how to limit the number of rows returned by a Supabase query using the `Range()` method in C#. It selects specific columns ('name', 'country_id') from the 'City' table and retrieves rows from index 0 to 3 (inclusive).
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/range

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Select("name, country_id")
  .Range(0, 3)
  .Get();
```

----------------------------------------

TITLE: Filter rows where column is less than or equal to a value using Select() in C#
DESCRIPTION: This C# code snippet demonstrates how to query a Supabase table to retrieve rows where a specific column's value is less than or equal to a given integer. It uses the `Where()` method with a lambda expression for filtering and `Get()` to execute the query and fetch results.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/lte

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Where(x => x.CountryId <= 250)
  .Get();
```

----------------------------------------

TITLE: Delete specific records from a Supabase table in C#
DESCRIPTION: Demonstrates how to delete specific records from a Supabase table in C#. The example uses a `Where` clause to filter records by `Id` before calling the `Delete()` method, ensuring only the targeted data is removed.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/delete

LANGUAGE: C#
CODE:
```
await supabase
  .From<City>()
  .Where(x => x.Id == 342)
  .Delete();
```

----------------------------------------

TITLE: Filter rows where column is greater than a value using Select() in C#
DESCRIPTION: Demonstrates how to query data from a Supabase table in C# using the `Select()` method to specify returned columns and the `Where()` method with a greater than (>) condition to filter rows based on a column's value. It retrieves cities where `CountryId` is greater than 250.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/gt

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Where(x => x.CountryId > 250)
  .Get();
```

----------------------------------------

TITLE: Sign out a user in C#
DESCRIPTION: This snippet demonstrates how to sign out the current user. In order to use the `SignOut()` method, the user needs to be signed in first.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signout

LANGUAGE: C#
CODE:
```
await supabase.Auth.SignOut();
```

----------------------------------------

TITLE: Filter rows with less than condition using Select() in C#
DESCRIPTION: This example demonstrates how to retrieve data from a Supabase table and filter rows where a specific column's value is less than a given number. It uses the `Select()` method to specify which columns to return, and the `Where()` clause with a lambda expression for the less than comparison. This requires the Supabase C# client library.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/lt

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Select("name, country_id")
  .Where(x => x.CountryId < 250)
  .Get();
```

----------------------------------------

TITLE: Verify Sms One-Time Password (OTP)
DESCRIPTION: The `VerifyOtp` method takes in different verification types. If a phone number is used, the type can either be `sms` or `phone_change`. If an email address is used, the type can be one of the following: `signup`, `magiclink`, `recovery`, `invite` or `email_change`. The verification type used should be determined based on the corresponding auth method called before `VerifyOtp` to sign up / sign-in a user.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-verifyotp

LANGUAGE: C#
CODE:
```
var session = await supabase.Auth.VerifyOTP("+13334445555", TOKEN, MobileOtpType.SMS);
```

----------------------------------------

TITLE: Sign up a new user with Supabase Auth in C#
DESCRIPTION: This snippet demonstrates how to sign up a new user using the Supabase C# client's Auth module. It awaits the `SignUp` method with the user's email and password, returning a session object.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signup

LANGUAGE: C#
CODE:
```
var session = await supabase.Auth.SignUp(email, password);
```

----------------------------------------

TITLE: C#: Querying foreign tables
DESCRIPTION: Illustrates querying data across foreign tables using string-based column selection to retrieve related data.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

LANGUAGE: csharp
CODE:
```
var data = await supabase
  .From<Transactions>()
  .Select("id, supplier:supplier_id(name), purchaser:purchaser_id(name)")
  .Get();
```

----------------------------------------

TITLE: Upsert data with conflict resolution in C#
DESCRIPTION: Illustrates how to handle conflicts during an UPSERT operation by specifying a column (e.g., 'Name') to resolve conflicts. If a record with the same 'Name' exists, it will be updated; otherwise, a new record will be inserted.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/upsert

LANGUAGE: C#
CODE:
```
var model = new City
{
  Id = 554,
  Name = "Middle Earth"
};

await supabase
  .From<City>()
  .OnConflict(x => x.Name)
  .Upsert(model);
```

----------------------------------------

TITLE: Filter rows that don't match a value using Select() in C#
DESCRIPTION: This C# code snippet demonstrates how to retrieve data from a 'Country' table, selecting specific columns (Name, CountryId), and filtering out rows where the 'Name' column is 'Paris'. It uses the Supabase client's `From`, `Select`, `Where`, and `Get` methods.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/not

LANGUAGE: C#
CODE:
```
var result = await supabase.From<Country>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Where(x => x.Name != "Paris")
  .Get();
```

----------------------------------------

TITLE: C# Sign in with OAuth provider and custom scopes
DESCRIPTION: Performs OAuth sign-in with a third-party provider while requesting specific permissions (scopes). After the user completes the authentication flow and is redirected, this snippet also demonstrates how to retrieve the user's session from the redirected URL.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signinwithoauth

LANGUAGE: C#
CODE:
```
var signInUrl = supabase.Auth.SignIn(Provider.Github, 'repo gist notifications');

// after user comes back from signin flow
var session = supabase.Auth.GetSessionFromUrl(REDIRECTED_URI);
```

----------------------------------------

TITLE: Send Magic Link using C# Supabase Auth
DESCRIPTION: Demonstrates how to send a magic link to a user's email address for passwordless sign-in, optionally specifying a redirect URL. This method is part of the Supabase Auth client.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signinwithotp

LANGUAGE: C#
CODE:
```
var options = new SignInOptions { RedirectTo = "http://myredirect.example" };
var didSendMagicLink = await supabase.Auth.SendMagicLink("joseph@supabase.io", options);
```

----------------------------------------

TITLE: Upsert data in C#
DESCRIPTION: Demonstrates a basic UPSERT operation into a Supabase table using a C# model. The primary key (Id) is included in the model for the upsert to correctly identify and update or insert the record.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/upsert

LANGUAGE: C#
CODE:
```
var model = new City
{
  Id = 554,
  Name = "Middle Earth"
};

await supabase.From<City>().Upsert(model);
```

----------------------------------------

TITLE: Create Signed URL in C#
DESCRIPTION: Creates a time-limited signed URL for a specific file in a Supabase Storage bucket. This URL allows temporary access to the file without requiring direct user permissions, valid for a specified number of seconds.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-createsignedurl

LANGUAGE: C#
CODE:
```
var url = await supabase.Storage.From("avatars").CreateSignedUrl("public/fancy-avatar.png", 60);
```

----------------------------------------

TITLE: Get the logged in user in C#
DESCRIPTION: Retrieves the currently logged-in user's data from Supabase authentication. This method returns the user object if a user is authenticated.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-getuser

LANGUAGE: C#
CODE:
```
var user = supabase.Auth.CurrentUser;
```

----------------------------------------

TITLE: Update Data by Modifying Retrieved Model in C#
DESCRIPTION: Illustrates updating data by first retrieving a single record from the Supabase table, modifying its properties, and then calling the 'Update()' method directly on the hydrated model. This method is suitable for updating specific, already loaded entities.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/update

LANGUAGE: C#
CODE:
```
var model = await supabase
  .From<City>()
  .Where(x => x.Name == "Auckland")
  .Single();

model.Name = "Middle Earth";

await model.Update<City>();
```

----------------------------------------

TITLE: C#: Selecting specific columns from a table
DESCRIPTION: Shows how to select only specific columns from a Supabase table using LINQ expressions with the `Select()` method.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

LANGUAGE: csharp
CODE:
```
// Given the following Model (Movie.cs)
[Table("movies")]
class Movie : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    //... etc.
}

// A result can be fetched like so.
var result = await supabase
  .From<Movie>()
  .Select(x => new object[] {x.Name, x.CreatedAt})
  .Get();
```

----------------------------------------

TITLE: Sign in with SMS OTP and Verify OTP using C# Supabase Auth
DESCRIPTION: Illustrates the process of initiating a sign-in via SMS OTP to a phone number and subsequently verifying the OTP to obtain a user session. This involves two distinct calls to the Supabase Auth client.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signinwithotp

LANGUAGE: C#
CODE:
```
await supabase.Auth.SignIn(SignInType.Phone, "+13334445555");

// Paired with `VerifyOTP` to get a session
var session = await supabase.Auth.VerifyOTP("+13334445555", TOKEN, MobileOtpType.SMS);
```

----------------------------------------

TITLE: Bulk insert multiple records into a Supabase table (C#)
DESCRIPTION: This example shows how to perform a bulk insert of multiple new records into a Supabase table. It uses a `List<City>` to insert several `City` instances in a single operation via `supabase.From<City>().Insert(models)`.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/insert

LANGUAGE: C#
CODE:
```
[Table("cities")]
class City : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("country_id")]
    public int CountryId { get; set; }
}

var models = new List<City>
{
  new City { Name = "The Shire", CountryId = 554 },
  new City { Name = "Rohan", CountryId = 553 },
};

await supabase.From<City>().Insert(models);

```

----------------------------------------

TITLE: Order results with Select() in C#
DESCRIPTION: Demonstrates ordering results using the `Select()` method to specify columns and the `Order()` method for sorting in descending order based on a primary key.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/order

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Order(x => x.Id, Ordering.Descending)
  .Get();
```

----------------------------------------

TITLE: C# Sign in with a third-party OAuth provider
DESCRIPTION: Initiates the OAuth sign-in process for a user using a specified third-party provider, such as GitHub. This method returns a URL to which the user should be redirected to complete the authentication flow.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-signinwithoauth

LANGUAGE: C#
CODE:
```
var signInUrl = supabase.Auth.SignIn(Provider.Github);
```

----------------------------------------

TITLE: Listen to specific table changes in C#
DESCRIPTION: Demonstrates how to listen to all types of changes (inserts, updates, deletes) for a specific table (`City`) in a Supabase database using the C# client library.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

LANGUAGE: C#
CODE:
```
await supabase.From<City>().On(ListenType.All, (sender, change) =>
{
    Debug.WriteLine(change.Payload.Data);
});

```

----------------------------------------

TITLE: Fetch inserted records after an insert operation (C#)
DESCRIPTION: This example demonstrates how to retrieve the inserted record(s) immediately after an insert operation. It uses `QueryOptions { Returning = ReturnType.Representation }` to get the representation of the inserted data back from the Supabase API.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/insert

LANGUAGE: C#
CODE:
```
var result = await supabase
  .From<City>()
  .Insert(models, new QueryOptions { Returning = ReturnType.Representation });

```

----------------------------------------

TITLE: Get the current session data in C#
DESCRIPTION: Retrieves the current active session data from the Supabase authentication client. This property returns the session object if a user is logged in, otherwise it will be null.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-getsession

LANGUAGE: C#
CODE:
```
var session = supabase.Auth.CurrentSession;
```

----------------------------------------

TITLE: Combine OR and AND conditions for data filtering in C#
DESCRIPTION: Illustrates how to combine OR conditions (`||`) within one `Where` clause and chain it with another `Where` clause for an implicit AND condition, retrieving rows that satisfy both sets of criteria from a Supabase table.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/or

LANGUAGE: csharp
CODE:
```
var result = await supabase.From<Country>()
  .Where(x => x.Population > 300000 || x.BirthRate < 0.6)
  .Where(x => x.Name != "Mordor")
  .Get();
```

----------------------------------------

TITLE: Retrieve a single row with Select()
DESCRIPTION: This example demonstrates how to retrieve a single row of data from a Supabase table using the `Select()` method to specify columns and then applying the `Single()` method to ensure only one row is returned. It will throw an error if more than one row matches the query.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/single

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Single();
```

----------------------------------------

TITLE: Download a file from Supabase Storage in C#
DESCRIPTION: This snippet demonstrates how to download a file from a Supabase Storage bucket using the C# client library. It includes examples for a basic download and a download with progress tracking.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-download

LANGUAGE: C#
CODE:
```
var bytes = await supabase.Storage.From("avatars").Download("public/fancy-avatar.png");
```

LANGUAGE: C#
CODE:
```
var bytes = await supabase.Storage
  .From("avatars")
  .Download("public/fancy-avatar.png", (sender, progress) => Debug.WriteLine($"{progress}%"));
```

----------------------------------------

TITLE: C#: Querying with count option
DESCRIPTION: Demonstrates how to retrieve the count of rows matching a query with different count types using the `Count()` method.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

LANGUAGE: csharp
CODE:
```
var count = await supabase
  .From<Movie>()
  .Select(x => new object[] { x.Name })
  .Count(CountType.Exact);
```

----------------------------------------

TITLE: Match records in C# using Supabase client
DESCRIPTION: Demonstrates how to find database records using the Supabase C# client's `Match` method. Examples include matching with a C# model object and with a Dictionary<string, string> for column-value pairs. This is useful for hydrating models and correlating with database entries.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/match

LANGUAGE: csharp
CODE:
```
var city = new City
{
    Id = 224,
    Name = "Atlanta"
};

var model = supabase.From<City>().Match(city).Single();
```

LANGUAGE: csharp
CODE:
```
var opts = new Dictionary<string, string>
{
    {"name","Beijing"},
    {"country_id", "156"}
};

var model = supabase.From<City>().Match(opts).Single();
```

----------------------------------------

TITLE: Filter rows by exact column value using Select() in C#
DESCRIPTION: This C# code snippet demonstrates how to use the Supabase client to filter rows from a table (`City`) where a specific column (`Name`) has an exact value of `null`. It uses the `Where` clause with a lambda expression and retrieves the results using `Get()`.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/is

LANGUAGE: csharp
CODE:
```
var result = await supabase.From<City>()
  .Where(x => x.Name == null)
  .Get();
```

----------------------------------------

TITLE: Perform websearch full-text search in C# with Supabase
DESCRIPTION: This example demonstrates how to perform a websearch-style full-text search (WFTS) in C#. It uses `Operator.WFTS` to find matches, which is suitable for web-style search queries on the `Catchphrase` column.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/textsearch

LANGUAGE: C#
CODE:
```
var result = await supabase.From<Quote>()
  .Select(x => x.Catchphrase)
  .Filter(x => x.Catchphrase, Operator.WFTS, new FullTextSearchConfig("'fat' & 'cat", "english"))
  .Get();
```

----------------------------------------

TITLE: Order parent table by referenced table in C#
DESCRIPTION: Illustrates ordering a parent table (City) based on a column from a referenced (joined) table (Country). The ordering is applied to the 'name' column of the 'country' alias in ascending order.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/order

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Select("name, country:countries(name)")
  .Order("country(name)", Ordering.Ascending)
  .Get();
```

----------------------------------------

TITLE: Filter rows where column is greater than or equal to a value using Select() in C#
DESCRIPTION: This example demonstrates how to query data from a Supabase table (`City`) to find rows where the `CountryId` column is greater than or equal to a specific value (250). It also shows how to use the `Select()` method to project only the `Name` and `CountryId` columns from the results.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/gte

LANGUAGE: csharp
CODE:
```
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Where(x => x.CountryId >= 250)
  .Get();
```

----------------------------------------

TITLE: Invoke Supabase Edge Function with a typed response (C#)
DESCRIPTION: This example illustrates how to invoke a Supabase Edge Function and automatically deserialize the response into a C# class. It defines a `HelloResponse` class with a `JsonProperty` attribute for mapping the JSON response.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/functions-invoke

LANGUAGE: C#
CODE:
```
class HelloResponse
{
    [JsonProperty("name")]
    public string Name { get; set; }
}

await supabase.Functions.Invoke<HelloResponse>("hello");
```

----------------------------------------

TITLE: Update the password for an authenticated user
DESCRIPTION: Changes the password for the currently authenticated user. This operation requires the user to be signed in.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-updateuser

LANGUAGE: C#
CODE:
```
var attrs = new UserAttributes { Password = "***********" };
var response = await supabase.Auth.Update(attrs);

```

----------------------------------------

TITLE: Update the email for an authenticated user
DESCRIPTION: Updates the email address for the currently authenticated user. This operation requires the user to be signed in. By default, a confirmation link is sent to both the current and new email addresses, unless 'Secure email change' is disabled in project settings.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-updateuser

LANGUAGE: C#
CODE:
```
var attrs = new UserAttributes { Email = "new-email@example.com" };
var response = await supabase.Auth.Update(attrs);

```

----------------------------------------

TITLE: Update the user's metadata
DESCRIPTION: Updates the custom metadata associated with the authenticated user's profile. This operation requires the user to be signed in.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/auth-updateuser

LANGUAGE: C#
CODE:
```
var attrs = new UserAttributes
{
  Data = new Dictionary<string, string> { {"example", "data" } }
};
var response = await supabase.Auth.Update(attrs);

```

----------------------------------------

TITLE: Call a Postgres stored procedure without parameters in C#
DESCRIPTION: Demonstrates how to invoke a Postgres stored procedure (RPC) from C# using the Supabase client without passing any parameters. The `Rpc` method is used with the function name and `null` for parameters.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/rpc

LANGUAGE: C#
CODE:
```
await supabase.Rpc("hello_world", null);
```

----------------------------------------

TITLE: Call a Postgres stored procedure with parameters in C#
DESCRIPTION: Illustrates how to call a Postgres stored procedure (RPC) from C# using the Supabase client, passing a dictionary of parameters. The `Rpc` method takes the function name and a `Dictionary<string, object>` containing the parameters.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/rpc

LANGUAGE: C#
CODE:
```
await supabase.Rpc("hello_world", new Dictionary<string, object> { { "foo", "bar"} });
```

----------------------------------------

TITLE: Perform full normalized text search in C# with Supabase
DESCRIPTION: This example illustrates how to perform a full normalized full-text search (PHFTS) in C#. It utilizes `Operator.PHFTS` to find matches after applying comprehensive normalization to the tsvector value of the `Catchphrase` column.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/textsearch

LANGUAGE: C#
CODE:
```
var result = await supabase.From<Quote>()
  .Select(x => x.Catchphrase)
  .Filter(x => x.Catchphrase, Operator.PHFTS, new FullTextSearchConfig("'fat' & 'cat", "english"))
  .Get();
```

----------------------------------------

TITLE: Filter data with OR condition using Select() in C#
DESCRIPTION: Demonstrates how to use the `Where` clause with an OR condition (`||`) to select rows based on multiple criteria, followed by a `Get()` call to retrieve the results from a Supabase table.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/or

LANGUAGE: csharp
CODE:
```
var result = await supabase.From<Country>()
  .Where(x => x.Id == 20 || x.Id == 30)
  .Get();
```

----------------------------------------

TITLE: Update file
DESCRIPTION: Demonstrates how to replace an existing file in a Supabase Storage bucket using the C# client. It specifies the path to the new file and the target file name within the bucket, requiring 'update' and 'select' object permissions.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-update

LANGUAGE: C#
CODE:
```
var imagePath = Path.Combine("Assets", "fancy-avatar.png");
await supabase.Storage.From("avatars").Update(imagePath, "fancy-avatar.png");

```

----------------------------------------

TITLE: Delete a file from Supabase Storage in C#
DESCRIPTION: This example demonstrates how to delete one or more files from a specified Supabase storage bucket using the C# client library. It requires `delete` and `select` permissions on objects within the bucket.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-remove

LANGUAGE: C#
CODE:
```
await supabase.Storage.From("avatars").Remove(new List<string> { "public/fancy-avatar.png" });
```

----------------------------------------

TITLE: Listen to row-level changes in a table in C#
DESCRIPTION: Explains how to subscribe to realtime changes for specific rows within a table (`countries`) based on a filter condition (`id=eq.200`) using Supabase Realtime channels in C#.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

LANGUAGE: C#
CODE:
```
var channel = supabase.Realtime.Channel("realtime", "public", "countries", "id", "id=eq.200");

channel.AddPostgresChangeHandler(ListenType.All, (sender, change) =>
{
    // The event type
    Debug.WriteLine(change.Event);
    // The changed record
    Debug.WriteLine(change.Payload);
});

await channel.Subscribe();

```

----------------------------------------

TITLE: List files in a bucket
DESCRIPTION: This example demonstrates how to list all files within a specified Supabase storage bucket using the C# client. This operation requires `select` permissions on objects.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-list

LANGUAGE: C#
CODE:
```
var objects = await supabase.Storage.From("avatars").List();

```

----------------------------------------

TITLE: Invoke Supabase Edge Function with basic options (C#)
DESCRIPTION: This example demonstrates a basic invocation of a Supabase Edge Function in C#. It shows how to pass custom headers, such as an Authorization token, and a JSON body using `InvokeFunctionOptions`.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/functions-invoke

LANGUAGE: C#
CODE:
```
var options = new InvokeFunctionOptions
{
    Headers = new Dictionary<string, string> {{ "Authorization", "Bearer 1234" }},
    Body = new Dictionary<string, object> { { "foo", "bar" } }
};

await supabase.Functions.Invoke("hello", options: options);
```

----------------------------------------

TITLE: Filter data with case-insensitive pattern in C#
DESCRIPTION: Demonstrates how to filter rows in a Supabase table where a specific column's value matches a case-insensitive pattern using `Operator.ILike`.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/ilike

LANGUAGE: C#
CODE:
```
await supabase.From<City>()
  .Filter(x => x.Name, Operator.ILike, "%la%")
  .Get();
```

----------------------------------------

TITLE: Listen to deletes on a specific table in C#
DESCRIPTION: Illustrates how to specifically listen for `DELETE` events on a table (`City`) in a Supabase database using the C# client library.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

LANGUAGE: C#
CODE:
```
await supabase.From<City>().On(ListenType.Deletes, (sender, change) =>
{
    Debug.WriteLine(change.Payload.Data);
});

```

----------------------------------------

TITLE: Listen to inserts on a specific table in C#
DESCRIPTION: Illustrates how to specifically listen for `INSERT` events on a table (`City`) in a Supabase database using the C# client library.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

LANGUAGE: C#
CODE:
```
await supabase.From<City>().On(ListenType.Inserts, (sender, change) =>
{
    Debug.WriteLine(change.Payload.Data);
});

```

----------------------------------------

TITLE: Perform basic text search in C# with Supabase
DESCRIPTION: This example demonstrates how to perform a basic full-text search (FTS) on a specified column using the `Operator.FTS` in C#. It finds rows where the `Catchphrase` column's tsvector value matches the provided `to_tsquery` string, configured for English.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/textsearch

LANGUAGE: C#
CODE:
```
var result = await supabase.From<Quote>()
  .Select(x => x.Catchphrase)
  .Filter(x => x.Catchphrase, Operator.FTS, new FullTextSearchConfig("'fat' & 'cat", "english"))
  .Get();
```

----------------------------------------

TITLE: Listen to updates on a specific table in C#
DESCRIPTION: Illustrates how to specifically listen for `UPDATE` events on a table (`City`) in a Supabase database using the C# client library.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

LANGUAGE: C#
CODE:
```
await supabase.From<City>().On(ListenType.Updates, (sender, change) =>
{
    Debug.WriteLine(change.Payload.Data);
});

```

----------------------------------------

TITLE: Listen to broadcast messages in C#
DESCRIPTION: Demonstrates how to listen to and send broadcast messages using Supabase Realtime channels in C#. It involves registering a custom class for broadcast data, adding an event handler, subscribing to the channel, and sending messages.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

LANGUAGE: C#
CODE:
```
class CursorBroadcast : BaseBroadcast
{
    [JsonProperty("cursorX")]
    public int CursorX {get; set;}

    [JsonProperty("cursorY")]
    public int CursorY {get; set;}
}

var channel = supabase.Realtime.Channel("any");
var broadcast = channel.Register<CursorBroadcast>();
broadcast.AddBroadcastEventHandler((sender, baseBroadcast) =>
{
    var response = broadcast.Current();
});

await channel.Subscribe();

// Send a broadcast
await broadcast.Send("cursor", new CursorBroadcast { CursorX = 123, CursorY = 456 });

```

----------------------------------------

TITLE: Filter rows where column value is in an array using Select() in C#
DESCRIPTION: This C# code snippet demonstrates how to use the `Filter` method with the `Operator.In` to retrieve rows from a 'City' table where the 'Name' column matches any of the values provided in a list. It fetches all columns implicitly via `Get()` after filtering.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/in

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Filter(x => x.Name, Operator.In, new List<object> { "Rio de Janiero", "San Francisco" })
  .Get();
```

----------------------------------------

TITLE: Upsert data and return exact row count in C#
DESCRIPTION: Shows how to perform an UPSERT operation while requesting an exact count of affected rows. This is achieved by passing a QueryOptions object with Count set to QueryOptions.CountType.Exact to the Upsert method.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/upsert

LANGUAGE: C#
CODE:
```
var model = new City
{
  Id = 554,
  Name = "Middle Earth"
};

await supabase
  .From<City>()
  .Upsert(model, new QueryOptions { Count = QueryOptions.CountType.Exact });
```

----------------------------------------

TITLE: Limit rows with Select() in C#
DESCRIPTION: This example demonstrates how to limit the number of rows returned when using a `Select()` projection in a Supabase query.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/limit

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Select(x => new object[] { x.Name, x.CountryId })
  .Limit(10)
  .Get();
```

----------------------------------------

TITLE: Listen to all database changes in C#
DESCRIPTION: Shows how to subscribe to all realtime changes across an entire schema (`public`) or all tables (`*`) in a Supabase database using the C# client library. It includes handling different event types and payload data.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

LANGUAGE: C#
CODE:
```
var channel = supabase.Realtime.Channel("realtime", "public", "*");

channel.AddPostgresChangeHandler(ListenType.All, (sender, change) =>
{
    // The event type
    Debug.WriteLine(change.Event);
    // The changed record
    Debug.WriteLine(change.Payload);
});

await channel.Subscribe();

```

----------------------------------------

TITLE: Remove a Realtime channel in C#
DESCRIPTION: Demonstrates how to unsubscribe and remove a Realtime channel from the Supabase Realtime client in C# using two different approaches: unsubscribing from a channel obtained via `From<City>()` or directly from a named channel.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/removechannel

LANGUAGE: C#
CODE:
```
var channel = await supabase.From<City>().On(ChannelEventType.All, (sender, change) => { });
channel.Unsubscribe();

// OR

var channel = supabase.Realtime.Channel("realtime", "public", "*");
channel.Unsubscribe()
```

----------------------------------------

TITLE: Filter by Values within a JSON Column in C# Supabase
DESCRIPTION: This example shows how to filter records based on a value nested within a JSON column. It uses the `Filter` method with a path notation ('address->postcode') and an `Operator.Equals` to match a specific postcode.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/using-filters

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Filter("address->postcode", Operator.Equals, 90210)
  .Get();
```

----------------------------------------

TITLE: Listen to presence sync in C#
DESCRIPTION: Explains how to track user presence using Supabase Realtime channels in C#. It involves registering a custom class for presence data, adding an event handler for sync events, subscribing, and tracking presence updates.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/subscribe

LANGUAGE: C#
CODE:
```
  class UserPresence : BasePresence
  {
      [JsonProperty("cursorX")]
      public bool IsTyping {get; set;}

      [JsonProperty("onlineAt")]
      public DateTime OnlineAt {get; set;}
  }

  var channel = supabase.Realtime.Channel("any");
  var presenceKey = Guid.NewGuid().ToString();
  var presence = channel.Register<UserPresence>(presenceKey);
  presence.AddPresenceEventHandler(EventType.Sync, (sender, type) =>
  {
      Debug.WriteLine($"The Event Type: {type}");
      var state = presence.CurrentState;
  });

  await channel.Subscribe();

  // Send a presence update
  await presence.Track(new UserPresence { IsTyping = false, OnlineAt = DateTime.Now });

```

----------------------------------------

TITLE: Filter C# data with Supabase using `Like` operator
DESCRIPTION: This C# code snippet demonstrates how to filter rows in a Supabase table where a specific column's value matches a supplied pattern using the `Like` operator. It performs a case-sensitive search for the pattern within the column, returning the matching results.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/like

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Filter(x => x.Name, Operator.Like, "%la%")
  .Get();
```

----------------------------------------

TITLE: C#: Querying JSON data
DESCRIPTION: Shows how to query and filter data within JSON columns using path notation.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/select

LANGUAGE: csharp
CODE:
```
 var result = await supabase
  .From<Users>()
  .Select("id, name, address->street")
  .Filter("address->postcode", Operator.Equals, 90210)
  .Get();
```

----------------------------------------

TITLE: Limit rows with embedded resources in C#
DESCRIPTION: This example demonstrates limiting rows within an embedded resource (e.g., a nested relationship) using the `Limit()` method in a Supabase query.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/limit

LANGUAGE: C#
CODE:
```
var result = await supabase.From<Country>()
  .Select("name, cities(name)")
  .Filter("name", Operator.Equals, "United States")
  .Limit(10, "cities")
  .Get();
```

----------------------------------------

TITLE: Create a Supabase Storage Bucket in C#
DESCRIPTION: This example demonstrates how to create a new storage bucket named 'avatars' using the Supabase C# client. To successfully create a bucket, the user requires 'insert' permissions on the 'buckets' table. No specific permissions are needed for 'objects'.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-createbucket

LANGUAGE: C#
CODE:
```
var bucket = await supabase.Storage.CreateBucket("avatars");
```

----------------------------------------

TITLE: Retrieve public URL for a Supabase Storage asset in C#
DESCRIPTION: This example demonstrates how to retrieve the public URL for a file stored in a Supabase Storage bucket using the C# client library. For this to work, the bucket must be explicitly set to public, either programmatically via `UpdateBucket()` or through the Supabase dashboard. No specific `buckets` or `objects` policy permissions are required for this operation on public assets.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-getpublicurl

LANGUAGE: C#
CODE:
```
var publicUrl = supabase.Storage.From("avatars").GetPublicUrl("public/fancy-avatar.png");
```

----------------------------------------

TITLE: Order results with embedded resources in C#
DESCRIPTION: Shows how to order results when selecting embedded resources. This example filters by a parent table column and then orders a nested resource (cities) in descending order.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/order

LANGUAGE: C#
CODE:
```
var result = await supabase.From<Country>()
  .Select("name, cities(name)")
  .Filter(x => x.Name == "United States")
  .Order("cities", "name", Ordering.Descending)
  .Get();
```

----------------------------------------

TITLE: Filter data using ContainedIn operator with Select() in C#
DESCRIPTION: This C# code snippet demonstrates how to filter data from a Supabase table using the `ContainedIn` operator. It retrieves `City` records where the `MainExports` property contains any of the specified values ('oil', 'fish'). The `Get()` method executes the query asynchronously.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/containedby

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Filter(x => x.MainExports, Operator.ContainedIn, new List<object> { "oil", "fish" })
  .Get();

```

----------------------------------------

TITLE: Filter data where a column contains all elements using Select() in C#
DESCRIPTION: This C# code snippet demonstrates how to filter a table (`City`) where a specific column (`MainExports`) contains all elements from a provided list (`"oil", "fish"`). It uses the `Filter` method with the `Operator.Contains` to perform this check, retrieving the results asynchronously.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/contains

LANGUAGE: C#
CODE:
```
var result = await supabase.From<City>()
  .Filter(x => x.MainExports, Operator.Contains, new List<object> { "oil", "fish" })
  .Get();
```

----------------------------------------

TITLE: Perform basic normalized text search in C# with Supabase
DESCRIPTION: This example shows how to perform a basic normalized full-text search (PLFTS) in C#. It uses `Operator.PLFTS` to find matches after applying basic normalization to the tsvector value of the `Catchphrase` column.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/textsearch

LANGUAGE: C#
CODE:
```
var result = await supabase.From<Quote>()
  .Select(x => x.Catchphrase)
  .Filter(x => x.Catchphrase, Operator.PLFTS, new FullTextSearchConfig("'fat' & 'cat", "english"))
  .Get();
```

----------------------------------------

TITLE: Move file in C# Supabase Storage
DESCRIPTION: Demonstrates how to move a file from one path to another within a Supabase Storage bucket using the C# client. This operation can also be used to rename a file. It requires 'update' and 'select' object permissions.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-from-move

LANGUAGE: C#
CODE:
```
await supabase.Storage.From("avatars")
  .Move("public/fancy-avatar.png", "private/fancy-avatar.png");

```

----------------------------------------

TITLE: List all Storage buckets in C#
DESCRIPTION: This example demonstrates how to retrieve a list of all Storage buckets using the Supabase C# client. It requires `select` permissions on `buckets` and no permissions on `objects`.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-listbuckets

LANGUAGE: C#
CODE:
```
var buckets = await supabase.Storage.ListBuckets();

```

----------------------------------------

TITLE: Empty bucket
DESCRIPTION: Demonstrates how to remove all objects from a specified Supabase Storage bucket using the C# client library. This operation requires 'select' permissions on 'buckets' and 'select' and 'delete' permissions on 'objects'.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-emptybucket

LANGUAGE: C#
CODE:
```
var bucket = await supabase.Storage.EmptyBucket("avatars");
```

----------------------------------------

TITLE: Retrieve a specific Storage bucket in C#
DESCRIPTION: This example demonstrates how to retrieve the details of an existing Storage bucket using the Supabase C# client. It requires `select` permissions on `buckets`.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-getbucket

LANGUAGE: C#
CODE:
```
var bucket = await supabase.Storage.GetBucket("avatars");

```

----------------------------------------

TITLE: Update Supabase Storage Bucket in C#
DESCRIPTION: This C# code demonstrates updating a Supabase Storage bucket named 'avatars' to be private (`Public = false`). The operation requires `buckets` update permissions.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-updatebucket

LANGUAGE: C#
CODE:
```
var bucket = await supabase.Storage.UpdateBucket("avatars", new BucketUpsertOptions { Public = false });
```

----------------------------------------

TITLE: Delete Supabase Storage bucket in C#
DESCRIPTION: This code snippet demonstrates how to delete an existing Supabase storage bucket using the C# client library. It calls the `DeleteBucket` method on the `supabase.Storage` object, passing the bucket's name.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/storage-deletebucket

LANGUAGE: C#
CODE:
```
var result = await supabase.Storage.DeleteBucket("avatars");
```

----------------------------------------

TITLE: Retrieve all Realtime channels in C#
DESCRIPTION: This code snippet demonstrates how to access all currently active Realtime channel subscriptions from the Supabase client instance. The `Subscriptions` property returns a collection of all joined channels.
SOURCE: https://supabase.com/docs/reference/csharp/introduction/docs/reference/csharp/getchannels

LANGUAGE: C#
CODE:
```
var channels = supabase.Realtime.Subscriptions;
```