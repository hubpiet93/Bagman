TITLE: Define Customer and Address Data Models in C#
DESCRIPTION: Defines the `Customer` and `Address` classes in C#, illustrating how a `Customer` object can contain a complex property of type `Address`. These models serve as the basis for applying FluentValidation rules.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_11

LANGUAGE: csharp
CODE:
```
public class Customer 
{
  public string Name { get; set; }
  public Address Address { get; set; }
}

public class Address 
{
  public string Line1 { get; set; }
  public string Line2 { get; set; }
  public string Town { get; set; }
  public string Country { get; set; }
  public string Postcode { get; set; }
}
```

----------------------------------------

TITLE: Reuse Address Validator in Customer Validator in FluentValidation C#
DESCRIPTION: Demonstrates how to integrate and reuse the `AddressValidator` within a `CustomerValidator`. By using `SetValidator`, FluentValidation automatically executes the child validator for the `Address` property when the `CustomerValidator` is invoked, combining all validation results.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_13

LANGUAGE: csharp
CODE:
```
public class CustomerValidator : AbstractValidator<Customer> 
{
  public CustomerValidator()
  {
    RuleFor(customer => customer.Name).NotNull();
    RuleFor(customer => customer.Address).SetValidator(new AddressValidator());
  }
}
```

----------------------------------------

TITLE: Define Inline Validation for Nested Properties in FluentValidation C#
DESCRIPTION: Illustrates an alternative approach to validating nested properties by defining rules inline, directly referencing the child property (e.g., `customer.Address.Postcode`). This method does not automatically perform null checks on intermediate properties like `Address`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_14

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Address.Postcode).NotNull()
```

----------------------------------------

TITLE: Define IContact Interface and Concrete Implementations
DESCRIPTION: This C# code defines an `IContact` interface with `Name` and `Email` properties, along with two concrete implementations: `Person` (adding `DateOfBirth`) and `Organisation` (adding `Headquarters`). It also includes a `ContactRequest` class that holds an `IContact` instance, demonstrating a common scenario for inheritance validation.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/inheritance.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public interface IContact 
{
  string Name { get; set; }
  string Email { get; set; }
}

public class Person : IContact 
{
  public string Name { get; set; }
  public string Email { get; set; }

  public DateTime DateOfBirth { get; set; }
}

public class Organisation : IContact 
{
  public string Name { get; set; }
  public string Email { get; set; }

  public Address Headquarters { get; set; }
}

public class ContactRequest 
{
  public IContact Contact { get; set; }

  public string MessageToSend { get; set; }
}
```

----------------------------------------

TITLE: Create Specific Validators for Person and Organisation
DESCRIPTION: These C# classes, `PersonValidator` and `OrganisationValidator`, extend `AbstractValidator` to define validation rules specific to the `Person` and `Organisation` types, respectively. They ensure properties like `Name`, `Email`, `DateOfBirth`, and `HeadQuarters` meet specified criteria.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/inheritance.md#_snippet_1

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator() 
  {
    RuleFor(x => x.Name).NotNull();
    RuleFor(x => x.Email).NotNull();
    RuleFor(x => x.DateOfBirth).GreaterThan(DateTime.MinValue);
  }
}

public class OrganisationValidator : AbstractValidator<Organisation> 
{
  public OrganisationValidator() 
  {
    RuleFor(x => x.Name).NotNull();
    RuleFor(x => x.Email).NotNull();
    RuleFor(x => x.HeadQuarters).SetValidator(new AddressValidator());
  }
}
```

----------------------------------------

TITLE: Implement Inheritance Validation for Single Contact Property
DESCRIPTION: This C# code demonstrates how to use `SetInheritanceValidator` within `ContactRequestValidator` to dynamically apply the correct child validator (`OrganisationValidator` or `PersonValidator`) based on the runtime type of the `Contact` property, which is of type `IContact`. This ensures polymorphic validation for a single object.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/inheritance.md#_snippet_2

LANGUAGE: csharp
CODE:
```
public class ContactRequestValidator : AbstractValidator<ContactRequest>
{
  public ContactRequestValidator()
  {

    RuleFor(x => x.Contact).SetInheritanceValidator(v => 
    {
      v.Add<Organisation>(new OrganisationValidator());
      v.Add<Person>(new PersonValidator());
    });

  }
}
```

----------------------------------------

TITLE: Apply Inheritance Validation to a Collection of Contacts
DESCRIPTION: This C# code shows how to apply inheritance validation to a collection of `IContact` objects using `RuleForEach` combined with `SetInheritanceValidator`. It ensures that each item in the `Contacts` list is validated by its appropriate `OrganisationValidator` or `PersonValidator` based on its runtime type.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/inheritance.md#_snippet_4

LANGUAGE: csharp
CODE:
```
public class ContactRequestValidator : AbstractValidator<ContactRequest>
{
  public ContactRequestValidator()
  {

    RuleForEach(x => x.Contacts).SetInheritanceValidator(v => 
    {
      v.Add<Organisation>(new OrganisationValidator());
      v.Add<Person>(new PersonValidator());
    });
  }
}
```

----------------------------------------

TITLE: Implement Basic List Count Validation in FluentValidation
DESCRIPTION: Shows how to create a `FluentValidation` validator for the `Person` class. It uses the `RuleFor` method combined with `Must` to ensure the `Pets` list contains fewer than 10 items, providing a custom error message.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_1

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    RuleFor(x => x.Pets).Must(list => list.Count < 10)
      .WithMessage("The list must contain fewer than 10 items");
  }
}
```

----------------------------------------

TITLE: Create Reusable FluentValidation Extension for List Count
DESCRIPTION: Demonstrates how to encapsulate list count validation logic into a reusable C# extension method. This `ListMustContainFewerThan` method extends `IRuleBuilder` to apply a `Must` rule, making the validation chainable and generic for any `IList<TElement>`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_2

LANGUAGE: csharp
CODE:
```
public static class MyCustomValidators {
  public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {
	return ruleBuilder.Must(list => list.Count < num).WithMessage("The list contains too many items");
  }
}
```

----------------------------------------

TITLE: Custom FluentValidation Extension with Message Placeholders
DESCRIPTION: Enhances the `ListMustContainFewerThan` extension method to support custom message placeholders. It uses an overload of `Must` that provides `ValidationContext`, allowing `MaxElements` to be appended to the message formatter for dynamic error messages.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_4

LANGUAGE: csharp
CODE:
```
public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {

  return ruleBuilder.Must((rootObject, list, context) => {
    context.MessageFormatter.AppendArgument("MaxElements", num);
    return list.Count < num;
  })
  .WithMessage("{PropertyName} must contain fewer than {MaxElements} items.");
}
```

----------------------------------------

TITLE: FluentValidation Extension with Multiple Dynamic Message Placeholders
DESCRIPTION: Further refines the custom `ListMustContainFewerThan` extension. This version adds a second dynamic message placeholder, `TotalElements`, to the error message, providing more detailed feedback about the actual number of items in the list during validation failure.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_5

LANGUAGE: csharp
CODE:
```
public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {

  return ruleBuilder.Must((rootObject, list, context) => {
    context.MessageFormatter
      .AppendArgument("MaxElements", num)
      .AppendArgument("TotalElements", list.Count);

    return list.Count < num;
  })
  .WithMessage("{PropertyName} must contain fewer than {MaxElements} items. The list contains {TotalElements} element");
}
```

----------------------------------------

TITLE: Implement a Custom Property Validator for List Count
DESCRIPTION: This C# class demonstrates how to create a custom `PropertyValidator` named `ListCountValidator`. It checks if a list property contains fewer than a specified maximum number of elements. It overrides `IsValid` to perform the validation logic and `GetDefaultMessageTemplate` for custom error messages.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_9

LANGUAGE: csharp
CODE:
```
using System.Collections.Generic;
using FluentValidation.Validators;

public class ListCountValidator<T, TCollectionElement> : PropertyValidator<T, IList<TCollectionElement>> {
	private int _max;

	public ListCountValidator(int max) {
		_max = max;
	}

	public override bool IsValid(ValidationContext<T> context, IList<TCollectionElement> list) {
		if(list != null && list.Count >= _max) {
			context.MessageFormatter.AppendArgument("MaxElements", _max);
			return false;
		}

		return true;
	}

  public override string Name => "ListCountValidator";

	protected override string GetDefaultMessageTemplate(string errorCode)
		=> "{PropertyName} must contain fewer than {MaxElements} items.";
}
```

----------------------------------------

TITLE: Apply Condition to Single Rule in FluentValidation
DESCRIPTION: Demonstrates how to apply a conditional execution using `When` directly to a single validation rule. The `CustomerDiscount` rule will only execute if `IsPreferredCustomer` is true.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/conditions.md#_snippet_0

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.CustomerDiscount).GreaterThan(0).When(customer => customer.IsPreferredCustomer);
```

----------------------------------------

TITLE: Apply Conditional Rules with Otherwise Block in FluentValidation
DESCRIPTION: Illustrates the use of `When` with an `Otherwise` block to define alternative validation rules. If the `IsPreferred` condition is met, one set of rules applies; otherwise, a different set of rules is executed.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/conditions.md#_snippet_2

LANGUAGE: csharp
CODE:
```
When(customer => customer.IsPreferred, () => {
   RuleFor(customer => customer.CustomerDiscount).GreaterThan(0);
   RuleFor(customer => customer.CreditCardNumber).NotNull();
}).Otherwise(() => {
  RuleFor(customer => customer.CustomerDiscount).Equal(0);
});
```

----------------------------------------

TITLE: Define FluentValidation RuleSet for Grouping Rules
DESCRIPTION: Demonstrates how to define a RuleSet named 'Names' within a FluentValidation AbstractValidator. This groups specific validation rules (Surname, Forename) together, while other rules (Id) remain outside the RuleSet, allowing for targeted validation.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/rulesets.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator() 
  {
     RuleSet("Names", () => 
     {
        RuleFor(x => x.Surname).NotNull();
        RuleFor(x => x.Forename).NotNull();
     });

     RuleFor(x => x.Id).NotEqual(0);
  }
}
```

----------------------------------------

TITLE: Execute Multiple FluentValidation RuleSets
DESCRIPTION: Illustrates how to execute multiple named RuleSets simultaneously. By providing a comma-separated list of RuleSet names to the IncludeRuleSets option during validation, all rules belonging to any of the specified RuleSets will be processed.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/rulesets.md#_snippet_2

LANGUAGE: csharp
CODE:
```
var result = validator.Validate(person, options => 
{
  options.IncludeRuleSets("Names", "MyRuleSet", "SomeOtherRuleSet");
});
```

----------------------------------------

TITLE: Include Default Rules in FluentValidation Validation
DESCRIPTION: Demonstrates two equivalent ways to include rules that are not part of any explicit RuleSet (often referred to as 'default' rules) along with named RuleSets: using IncludeRulesNotInRuleSet() or by specifying the special 'default' RuleSet name.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/rulesets.md#_snippet_3

LANGUAGE: csharp
CODE:
```
validator.Validate(person, options => 
{
  // Option 1: IncludeRulesNotInRuleSet is the equivalent of using the special ruleset name "default"
  options.IncludeRuleSets("Names").IncludeRulesNotInRuleSet();
  // Option 2: This does the same thing.
  option.IncludeRuleSets("Names", "default");
});
```

----------------------------------------

TITLE: Execute All FluentValidation RuleSets
DESCRIPTION: Explains how to force the execution of all validation rules defined in a validator, regardless of whether they are part of a specific RuleSet or not. This is achieved by using IncludeAllRuleSets() during the validation process, which is equivalent to including all RuleSets.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/rulesets.md#_snippet_4

LANGUAGE: csharp
CODE:
```
validator.Validate(person, options => 
{
  options.IncludeAllRuleSets();
});
```

----------------------------------------

TITLE: Apply NotNull Validation in C#
DESCRIPTION: Demonstrates how to use the `NotNull` validator in FluentValidation to ensure a property is not null. This validator is applicable to any property type and is a fundamental check for data integrity.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_0

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Surname).NotNull();
```

----------------------------------------

TITLE: Apply NotEmpty Validation in C#
DESCRIPTION: Illustrates the usage of the `NotEmpty` validator in FluentValidation. This validator ensures a property is not null, empty, or whitespace for strings, and not the default value for value types. It also checks if an `IEnumerable` is not empty, making it versatile for various data types.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_1

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Surname).NotEmpty();
```

----------------------------------------

TITLE: Apply NotEqual Validation in C#
DESCRIPTION: Shows how to use the `NotEqual` validator in FluentValidation to ensure a property's value is not equal to a specific value or another property's value. An optional `StringComparer` can be provided for culture-specific or case-insensitive comparisons, offering flexibility in validation logic.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_2

LANGUAGE: csharp
CODE:
```
//Not equal to a particular value
RuleFor(customer => customer.Surname).NotEqual("Foo");

//Not equal to another property
RuleFor(customer => customer.Surname).NotEqual(customer => customer.Forename);
```

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Surname).NotEqual("Foo", StringComparer.OrdinalIgnoreCase);
```

----------------------------------------

TITLE: Apply Equal Validation in C#
DESCRIPTION: Demonstrates the `Equal` validator in FluentValidation, used to ensure a property's value is equal to a particular value or another property's value. Similar to `NotEqual`, it supports an optional `StringComparer` for different comparison types, allowing for precise control over equality checks.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_3

LANGUAGE: csharp
CODE:
```
//Equal to a particular value
RuleFor(customer => customer.Surname).Equal("Foo");

//Equal to another property
RuleFor(customer => customer.Password).Equal(customer => customer.PasswordConfirmation);
```

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Surname).Equal("Foo", StringComparer.OrdinalIgnoreCase);
```

----------------------------------------

TITLE: Apply Length Range Validation in C#
DESCRIPTION: Explains how to use the `Length` validator in FluentValidation to enforce a minimum and maximum length for a string property. It's important to note that this validator only checks length and does not ensure the string property itself isn't null, requiring a separate `NotNull` or `NotEmpty` check if needed.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_4

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Surname).Length(1, 250); //must be between 1 and 250 chars (inclusive)
```

----------------------------------------

TITLE: Apply MaxLength Validation in C#
DESCRIPTION: Illustrates the `MaximumLength` validator in FluentValidation, which ensures a string property's length does not exceed a specified maximum. This validator is only applicable to string properties and is useful for enforcing character limits in text fields.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_5

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Surname).MaximumLength(250); //must be 250 chars or fewer
```

----------------------------------------

TITLE: Apply MinLength Validation to String Properties in C#
DESCRIPTION: Ensures a string property's length meets a minimum requirement. This validator is applicable only to string properties and checks if the length is greater than or equal to the specified value. It provides specific format arguments for error messages like PropertyName, MinLength, TotalLength, PropertyValue, and PropertyPath.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_6

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Surname).MinimumLength(10); //must be 10 chars or more
```

----------------------------------------

TITLE: Apply Greater Than Validation for Numeric or Comparable Properties in C#
DESCRIPTION: Validates that a property's value is strictly greater than a specified constant or another property's value. This validator is suitable for types implementing `IComparable<T>` and is commonly used for numerical comparisons. Error messages can utilize PropertyName, ComparisonValue, ComparisonProperty, PropertyValue, and PropertyPath.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_9

LANGUAGE: csharp
CODE:
```
//Greater than a particular value
RuleFor(customer => customer.CreditLimit).GreaterThan(0);

//Greater than another property
RuleFor(customer => customer.CreditLimit).GreaterThan(customer => customer.MinimumCreditLimit);
```

----------------------------------------

TITLE: Apply Regular Expression Validation Rule (C#)
DESCRIPTION: Applies a regular expression validation rule to a property, ensuring its value matches the specified regex pattern. The example shows how to use `Matches` with a placeholder regex. Error messages can be formatted using `{PropertyName}`, `{PropertyValue}`, `{RegularExpression}`, and `{PropertyPath}`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_12

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Surname).Matches("some regex here");
```

----------------------------------------

TITLE: Apply Credit Card Validation Rule (C#)
DESCRIPTION: Checks if a string property could be a valid credit card number. This validator performs a basic check for credit card number format. Error messages can be formatted using `{PropertyName}`, `{PropertyValue}`, and `{PropertyPath}`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_14

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.CreditCard).CreditCard();
```

----------------------------------------

TITLE: Apply Enum Value Validation Rule (C#)
DESCRIPTION: Validates if a numeric value is a valid member of an enum type, preventing invalid casts to non-existent enum values. The example demonstrates defining an enum and a model, then applying the `IsInEnum` validator. Error messages can be formatted using `{PropertyName}`, `{PropertyValue}`, and `{PropertyPath}`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_15

LANGUAGE: csharp
CODE:
```
public enum ErrorLevel 
{
  Error = 1,
  Warning = 2,
  Notice = 3
}

public class Model
{
  public ErrorLevel ErrorLevel { get; set; }
}

var model = new Model();
model.ErrorLevel = (ErrorLevel)4;
```

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.ErrorLevel).IsInEnum();
```

----------------------------------------

TITLE: Apply Empty Validation Rule (C#)
DESCRIPTION: Checks if a property value is null, its default value, or if an IEnumerable (like arrays or collections) is empty. This validator is the opposite of the `NotEmpty` validator. Error messages can be formatted using `{PropertyName}`, `{PropertyValue}`, and `{PropertyPath}`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_17

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Surname).Empty();
```

----------------------------------------

TITLE: Null Validator Usage
DESCRIPTION: Checks if a property value is null. This validator is the opposite of the `NotNull` validator. The example error message is: 'Surname' must be empty. String format arguments available for customization include: {PropertyName} (Name of the property being validated), {PropertyValue} (Current value of the property), and {PropertyPath} (The full path of the property).
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_18

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Surname).Null();
```

----------------------------------------

TITLE: ExclusiveBetween Validator Usage
DESCRIPTION: Checks whether the property value falls within a specified numerical range, exclusively. This means the property value must be greater than the lower bound and less than the upper bound. The example error message is: 'Id' must be between 1 and 10 (exclusive). You entered 1. String format arguments available for customization include: {PropertyName} (Name of the property being validated), {PropertyValue} (Current value of the property), {From} (Lower bound of the range), {To} (Upper bound of the range), and {PropertyPath} (The full path of the property).
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_19

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Id).ExclusiveBetween(1,10);
```

----------------------------------------

TITLE: PrecisionScale Validator Usage
DESCRIPTION: Checks whether a decimal value adheres to a specified total precision (total number of digits) and scale (number of digits after the decimal point). The example error message is: 'Amount' must not be more than 4 digits in total, with allowance for 2 decimals. 5 digits and 3 decimals were found. String format arguments available for customization include: {PropertyName} (Name of the property being validated), {PropertyValue} (Current value of the property), {ExpectedPrecision} (Expected precision), {ExpectedScale} (Expected scale), {Digits} (Total number of digits in the property value), {ActualScale} (Actual scale of the property value), and {PropertyPath} (The full path of the property). The third parameter, `ignoreTrailingZeros`, affects how precision and scale are calculated; if `true`, trailing zeros after the decimal point do not count towards the expected number of decimal places. For example, with `ignoreTrailingZeros` as `false`, `123.4500` has precision 7 and scale 4, but with `true`, it has precision 5 and scale 2. This method also implies a certain range of acceptable values. Note that prior to FluentValidation 11.4, this method was called `ScalePrecision` and had its parameters reversed.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_21

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Amount).PrecisionScale(4, 2, false);
```

----------------------------------------

TITLE: Define FluentValidation Rule with Multiple Validators
DESCRIPTION: Illustrates a basic FluentValidation rule for a property with multiple chained validators (NotNull and NotEqual) where all validators are executed by default, even if an earlier one fails.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/cascade.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    RuleFor(x => x.Surname).NotNull().NotEqual("foo");
  }
}
```

----------------------------------------

TITLE: Set Rule-Level Cascade Mode to Stop
DESCRIPTION: Demonstrates how to apply `CascadeMode.Stop` to a specific FluentValidation rule. This prevents subsequent validators in the chain for that rule from executing if an earlier validator fails.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/cascade.md#_snippet_1

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Surname).Cascade(CascadeMode.Stop).NotNull().NotEqual("foo");
```

----------------------------------------

TITLE: Configure Global Validator Cascade Mode in FluentValidation
DESCRIPTION: Demonstrates how to set the default cascade mode for all validators globally. When set to 'StopOnFirstFailure', validation will cease for a rule once its first failure is encountered.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/Changelog.txt#_snippet_0

LANGUAGE: C#
CODE:
```
ValidationOptions.CascadeMode = ValidatorCascadeMode.StopOnFirstFailure
```

----------------------------------------

TITLE: Set Default Rule-Level Cascade Mode for Validator Class
DESCRIPTION: Illustrates setting the `RuleLevelCascadeMode` property within an `AbstractValidator` to apply a default cascade mode (e.g., `Stop`) to all rules defined in that validator, reducing repetition.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/cascade.md#_snippet_3

LANGUAGE: csharp
CODE:
```
RuleLevelCascadeMode = CascadeMode.Stop;

RuleFor(x => x.Forename).NotNull().NotEqual("foo");
RuleFor(x => x.MiddleNames).NotNull().NotEqual("foo");
RuleFor(x => x.Surname).NotNull().NotEqual("foo");
```

----------------------------------------

TITLE: Testing Validator Extensions
DESCRIPTION: Provides examples of extension methods used for testing FluentValidation rules, enabling developers to assert the presence or absence of validation errors for specific properties in their unit tests.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/Changelog.txt#_snippet_7

LANGUAGE: C#
CODE:
```
validator.ShouldNotHaveValidationErrorFor(x => x.PropertyName);
```

LANGUAGE: C#
CODE:
```
validator.ShouldHaveValidationErrorFor(x => x.PropertyName);
```

----------------------------------------

TITLE: FluentValidation Cascade Mode API References
DESCRIPTION: Documentation for key properties controlling cascade behavior in FluentValidation at rule, class, and global levels, including their purpose and types.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/cascade.md#_snippet_4

LANGUAGE: APIDOC
CODE:
```
AbstractValidator:
  RuleLevelCascadeMode: CascadeMode
    Description: Controls the default cascade behavior for validators within a single rule.
  ClassLevelCascadeMode: CascadeMode
    Description: Controls the cascade behavior between different rules within the validator class.

ValidatorOptions.Global:
  DefaultRuleLevelCascadeMode: CascadeMode
    Description: Global default setting for rule-level cascade mode.
  DefaultClassLevelCascadeMode: CascadeMode
    Description: Global default setting for class-level cascade mode.
```

----------------------------------------

TITLE: Refactored Property Transformation with FluentValidation
DESCRIPTION: This C# example refactors the transformation logic into a separate helper method, `StringToNullableInt`, for improved readability. The `Transform` method then references this helper, applying the same string-to-nullable-integer conversion before a `GreaterThan(10)` validation. This syntax is available in FluentValidation 9.5 and newer.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/transform.md#_snippet_1

LANGUAGE: csharp
CODE:
```
Transform(x => x.SomeStringProperty, StringToNullableInt)
    .GreaterThan(10);

int? StringToNullableInt(string value)
  => int.TryParse(value, out int val) ? (int?) val : null;
```

----------------------------------------

TITLE: Define Asynchronous Validation Rule for Customer ID in FluentValidation
DESCRIPTION: This C# code defines a `CustomerValidator` using FluentValidation, demonstrating how to implement an asynchronous validation rule with `MustAsync`. It checks if a customer's ID already exists by calling an external web API, ensuring the ID is unique. The rule is defined within the constructor of the validator.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/async.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class CustomerValidator : AbstractValidator<Customer> 
{
  SomeExternalWebApiClient _client;

  public CustomerValidator(SomeExternalWebApiClient client) 
  {
    _client = client;

    RuleFor(x => x.Id).MustAsync(async (id, cancellation) => 
    {
      bool exists = await _client.IdExists(id);
      return !exists;
    }).WithMessage("ID Must be unique");
  }
}
```

----------------------------------------

TITLE: Invoke Asynchronous FluentValidation Validator
DESCRIPTION: This C# snippet shows how to execute a FluentValidation validator that contains asynchronous rules. It demonstrates calling `ValidateAsync` on an instance of `CustomerValidator` to trigger the validation process, ensuring both synchronous and asynchronous rules are correctly evaluated.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/async.md#_snippet_1

LANGUAGE: csharp
CODE:
```
var validator = new CustomerValidator(new SomeExternalWebApiClient());
var result = await validator.ValidateAsync(customer);
```

----------------------------------------

TITLE: Apply NotNull rule to each item in a simple type collection
DESCRIPTION: Demonstrates how to use `RuleForEach` with `NotNull()` to ensure each string item within the `AddressLines` collection of a `Person` object is not null. This applies the validation rule individually to every element.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/collections.md#_snippet_1

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator() 
  {
    RuleForEach(x => x.AddressLines).NotNull();
  }
}
```

----------------------------------------

TITLE: Define classes with a complex type collection
DESCRIPTION: Defines a `Customer` class with a `List<Order>` and an `Order` class with a `Total` property. This setup illustrates a common scenario where a collection consists of other complex objects that require their own validation.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/collections.md#_snippet_3

LANGUAGE: csharp
CODE:
```
public class Customer 
{
  public List<Order> Orders { get; set; } = new List<Order>();
}

public class Order 
{
  public double Total { get; set; }
}
```

----------------------------------------

TITLE: Conditionally validate collection items using Where
DESCRIPTION: Shows how to use the `Where` method with `RuleForEach` to apply validation rules only to specific items in a collection that meet a defined condition. This allows for flexible and targeted validation based on item properties.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/collections.md#_snippet_6

LANGUAGE: csharp
CODE:
```
RuleForEach(x => x.Orders)
  .Where(x => x.Cost != null)
  .SetValidator(new OrderValidator());
```

----------------------------------------

TITLE: Define Customer Validator with FluentValidation Rules
DESCRIPTION: This C# code defines a `CustomerValidator` class inheriting from `AbstractValidator<Customer>`. It sets up validation rules for the `Surname` and `Forename` properties to be not null, and applies an `OrderValidator` to each item in the `Orders` collection.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/specific-properties.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class CustomerValidator : AbstractValidator<Customer>
{
  public CustomerValidator()
  {
    RuleFor(x => x.Surname).NotNull();
    RuleFor(x => x.Forename).NotNull();
    RuleForEach(x => x.Orders).SetValidator(new OrderValidator());
  }
}
```

----------------------------------------

TITLE: Set Validation Rule Severity to Warning
DESCRIPTION: This C# snippet demonstrates how to explicitly set the severity of a validation rule to `Warning` using the `WithSeverity` method. If the `Surname` rule fails, its associated `ValidationFailure` will have a `Severity` of `Warning` instead of the default `Error`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/severity.md#_snippet_1

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Warning);
```

----------------------------------------

TITLE: Set Validation Rule Severity with Callback (FluentValidation 9.0+)
DESCRIPTION: For FluentValidation versions 9.0 and above, this C# example shows how to use a callback function with `WithSeverity`. This allows the severity to be determined dynamically based on the validated object itself, providing more flexibility.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/severity.md#_snippet_2

LANGUAGE: csharp
CODE:
```
RuleFor(person => person.Surname).NotNull().WithSeverity(person => Severity.Warning);
```

----------------------------------------

TITLE: Validate Object and Print Failure Severities
DESCRIPTION: This C# code demonstrates how to instantiate the `PersonValidator`, validate a `Person` object, and then iterate through the `ValidationResult.Errors` collection. For each validation failure, it prints the `PropertyName` and its assigned `Severity` level to the console.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/severity.md#_snippet_3

LANGUAGE: csharp
CODE:
```
var validator = new PersonValidator();
var result = validator.Validate(new Person());
foreach (var failure in result.Errors) 
{
  Console.WriteLine($"Property: {failure.PropertyName} Severity: {failure.Severity}");
}
```

----------------------------------------

TITLE: Example Console Output of Validation Severities
DESCRIPTION: This text snippet shows the expected console output from the previous C# validation example. It illustrates that the 'Surname' property's failure is reported with 'Warning' severity, while 'Forename' retains the default 'Error' severity.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/severity.md#_snippet_4

LANGUAGE: text
CODE:
```
Property: Surname Severity: Warning
Property: Forename Severity: Error
```

----------------------------------------

TITLE: Define FluentValidation Rule with Custom State in C#
DESCRIPTION: This C# code defines a `PersonValidator` using FluentValidation. It illustrates how to apply the `WithState` method to a validation rule for the `Forename` property, associating an integer value (1234) as custom state with the validation failure if the property is null.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-state.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator() 
  {
    RuleFor(person => person.Surname).NotNull();
    RuleFor(person => person.Forename).NotNull().WithState(person => 1234);  
  }
}
```

----------------------------------------

TITLE: Pass Arbitrary Data to FluentValidation RootContextData
DESCRIPTION: Illustrates how to pass arbitrary, stateless data into the FluentValidation pipeline using the `RootContextData` dictionary available on the `ValidationContext`. This data can then be accessed by custom validators or `Custom` rules later in the validation process.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/advanced.md#_snippet_1

LANGUAGE: csharp
CODE:
```
var person = new Person();
var context = new ValidationContext<Person>(person);
context.RootContextData["MyCustomData"] = "Test";
var validator = new PersonValidator();
validator.Validate(context);
```

----------------------------------------

TITLE: Access RootContextData in FluentValidation Custom Rule
DESCRIPTION: Shows how to retrieve data previously stored in `RootContextData` from within a `Custom` validation rule. This enables conditional validation decisions based on external data not directly part of the validated object.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/advanced.md#_snippet_2

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Surname).Custom((x, context) => 
{
  if(context.RootContextData.ContainsKey("MyCustomData")) 
  {
    context.AddFailure("My error message");
  }
});
```

----------------------------------------

TITLE: Override RaiseValidationException for Custom FluentValidation Exception Handling
DESCRIPTION: Explains how to override the `RaiseValidationException` method in `AbstractValidator` to customize the exception thrown when `ValidateAndThrow` is invoked. This example wraps the default `ValidationException` in an `ArgumentException`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/advanced.md#_snippet_3

LANGUAGE: csharp
CODE:
```
protected override void RaiseValidationException(ValidationContext<T> context, ValidationResult result)
{
    var ex = new ValidationException(result.Errors);
    throw new ArgumentException(ex.Message, ex);
}
```

----------------------------------------

TITLE: Create FluentValidation Extension Method for Custom Exception Handling
DESCRIPTION: Provides an alternative approach to custom exception handling by creating a static extension method for `IValidator<T>`. This method performs validation and then throws a custom exception (e.g., `ArgumentException`) if validation fails, offering more granular control over when a specific exception type is thrown.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/advanced.md#_snippet_4

LANGUAGE: csharp
CODE:
```
public static class FluentValidationExtensions
{
    public static void ValidateAndThrowArgumentException<T>(this IValidator<T> validator, T instance)
    {
        var res = validator.Validate(instance);

        if (!res.IsValid)
        {
            var ex = new ValidationException(res.Errors);
            throw new ArgumentException(ex.Message, ex);
        }
    }
}
```

----------------------------------------

TITLE: Override Default Validation Message with Static String
DESCRIPTION: Demonstrates how to use the `WithMessage` method to provide a custom, static error message for a validator. This message will be displayed if the validation fails.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/configuring.md#_snippet_0

LANGUAGE: C#
CODE:
```
RuleFor(customer => customer.Surname).NotNull().WithMessage("Please ensure that you have entered your Surname");
```

----------------------------------------

TITLE: Override Validation Message with Lambda and Other Property Values
DESCRIPTION: Demonstrates using a lambda expression with `WithMessage` to create an error message that references other properties of the validated object using string interpolation.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/configuring.md#_snippet_4

LANGUAGE: C#
CODE:
```
//Referencing other property values:
RuleFor(customer => customer.Surname)
  .NotNull()
  .WithMessage(customer => $"This message references some other properties: Forename: {customer.Forename} Discount: {customer.Discount}");
```

----------------------------------------

TITLE: Override Display Property Name with Lambda Expression
DESCRIPTION: Illustrates using a lambda expression with `WithName` to dynamically set the display name of a property in the error message, potentially based on other properties of the validated object.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/configuring.md#_snippet_6

LANGUAGE: C#
CODE:
```
RuleFor(customer => customer.Surname).NotNull().WithName(customer => "Last name for customer " + customer.Id);
```

----------------------------------------

TITLE: Configure Global Display Name Resolver
DESCRIPTION: Demonstrates how to set a global `DisplayNameResolver` on `ValidatorOptions.Global` to customize how property names are resolved for all validators. This provides a centralized way to modify display names.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/configuring.md#_snippet_7

LANGUAGE: C#
CODE:
```
ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) => 
{
  if(member != null) 
  {
     return member.Name + "Foo";
  }
  return null;
};
```

----------------------------------------

TITLE: Localizing Validation Messages with IStringLocalizer
DESCRIPTION: This example shows how to integrate ASP.NET Core's IStringLocalizer for localization within a FluentValidation validator. The IStringLocalizer is injected into the validator's constructor and then used within the WithMessage callback to retrieve localized strings.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/localization.md#_snippet_1

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator(IStringLocalizer<Person> localizer)
   {
    RuleFor(x => x.Surname).NotNull().WithMessage(x => localizer["Surname is required"]);
  }
}
```

----------------------------------------

TITLE: Configuring FluentValidation to Use a Custom Language Manager
DESCRIPTION: This snippet shows how to replace FluentValidation's default LanguageManager with a custom implementation. By setting the LanguageManager property of ValidatorOptions.Global during application startup, all subsequent validations will use the custom messages defined in the CustomLanguageManager.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/localization.md#_snippet_3

LANGUAGE: csharp
CODE:
```
ValidatorOptions.Global.LanguageManager = new CustomLanguageManager();
```

----------------------------------------

TITLE: Disable FluentValidation Localization Globally (C#)
DESCRIPTION: This snippet shows how to completely disable FluentValidation's localization support, forcing all validation messages to use the default English messages, irrespective of the current UI culture. This is achieved by setting the `Enabled` property of the `LanguageManager` to `false`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/localization.md#_snippet_4

LANGUAGE: csharp
CODE:
```
ValidatorOptions.Global.LanguageManager.Enabled = false;
```

----------------------------------------

TITLE: Define a basic FluentValidation PersonValidator
DESCRIPTION: This C# code defines a simple `PersonValidator` class that inherits from `AbstractValidator<Person>`. It includes a rule to ensure the `Name` property of the `Person` object is not null.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/testing.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person>
{
   public PersonValidator()
   {
      RuleFor(person => person.Name).NotNull();
   }
}
```

----------------------------------------

TITLE: Test FluentValidation rules with NUnit and TestValidate
DESCRIPTION: This C# NUnit test fixture demonstrates how to use `FluentValidation.TestHelper.TestValidate` to verify validator behavior. It includes setup for the validator and two test cases: one asserting an error for a null name and another asserting no error for a specified name.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/testing.md#_snippet_1

LANGUAGE: csharp
CODE:
```
using NUnit.Framework;
using FluentValidation;
using FluentValidation.TestHelper;

[TestFixture]
public class PersonValidatorTester
{
    private PersonValidator validator;

    [SetUp]
    public void Setup()
    {
       validator = new PersonValidator();
    }

    [Test]
    public void Should_have_error_when_Name_is_null()
    {
      var model = new Person { Name = null };
      var result = validator.TestValidate(model);
      result.ShouldHaveValidationErrorFor(person => person.Name);
    }

    [Test]
    public void Should_not_have_error_when_name_is_specified()
    {
      var model = new Person { Name = "Jeremy" };
      var result = validator.TestValidate(model);
      result.ShouldNotHaveValidationErrorFor(person => person.Name);
    }
}
```

----------------------------------------

TITLE: Compose FluentValidation Rules Using Include (C#)
DESCRIPTION: This C# example demonstrates how to combine the previously defined `PersonAgeValidator` and `PersonNameValidator` into a single `PersonValidator` using FluentValidation's `Include` method. This allows for modular validation logic to be composed into a comprehensive validator for the `Person` type.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/including-rules.md#_snippet_1

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator()
   {
    Include(new PersonAgeValidator());
    Include(new PersonNameValidator());
  }
}
```

----------------------------------------

TITLE: Define a FluentValidation User Validator
DESCRIPTION: This C# code defines a simple validator for a `User` object using FluentValidation's `AbstractValidator<T>`. It specifies a validation rule ensuring that the `Name` property of the `User` is not null.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/di.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class UserValidator : AbstractValidator<User>
{
  public UserValidator()
  {
    RuleFor(x => x.Name).NotNull();
  }
}
```

----------------------------------------

TITLE: Alternative Methods for Automatic Validator Registration
DESCRIPTION: This C# code illustrates alternative overloads for automatically registering validators. It shows how to use `AddValidatorsFromAssemblyContaining` with a `typeof` expression and `AddValidatorsFromAssembly` to load validators from a specific `Assembly` reference, providing flexibility in how assemblies are specified for scanning.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/di.md#_snippet_5

LANGUAGE: csharp
CODE:
```
// Load using a type reference rather than the generic.
services.AddValidatorsFromAssemblyContaining(typeof(UserValidator));

// Load an assembly reference rather than using a marker type.
services.AddValidatorsFromAssembly(Assembly.Load("SomeAssembly"));
```

----------------------------------------

TITLE: Register Single FluentValidation Validator in ASP.NET Core
DESCRIPTION: This C# code demonstrates how to register a specific `IValidator<Person>` instance, implemented by `PersonValidator`, with the ASP.NET Core service provider. This is typically done in the `ConfigureServices` method of the `Startup` class using `services.AddScoped<IValidator<Person>, PersonValidator>()`, ensuring the validator is available for dependency injection.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/aspnet.md#_snippet_1

LANGUAGE: csharp
CODE:
```
public void ConfigureServices(IServiceCollection services) 
{
    // If you're using MVC or WebApi you'll probably have
    // a call to AddMvc() or AddControllers() already.
    services.AddMvc();
    
    // ... other configuration ...
    
    services.AddScoped<IValidator<Person>, PersonValidator>();
}
```

----------------------------------------

TITLE: FluentValidation Extension to Add ValidationResult Errors to ModelState
DESCRIPTION: This C# extension method, 'AddToModelState', facilitates integrating FluentValidation's 'ValidationResult' errors with ASP.NET Core's 'ModelStateDictionary'. It iterates through validation errors and adds them to 'ModelState', allowing ASP.NET's built-in mechanisms to display these errors in views. This is crucial for manual validation workflows.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/aspnet.md#_snippet_4

LANGUAGE: csharp
CODE:
```
public static class Extensions 
{
  public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState) 
  {
    foreach (var error in result.Errors) 
    {
      modelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }
  }
}
```

----------------------------------------

TITLE: FluentValidation ValidationResult ToDictionary Extension Method
DESCRIPTION: This C# extension method provides a 'ToDictionary' functionality for FluentValidation's 'ValidationResult'. It groups validation errors by property name and converts them into a dictionary where keys are property names and values are arrays of error messages. This method is particularly useful for older versions of FluentValidation (prior to 11.1) where 'ToDictionary' was not natively available on 'ValidationResult'.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/aspnet.md#_snippet_7

LANGUAGE: csharp
CODE:
```
public static class FluentValidationExtensions
{
  public static IDictionary<string, string[]> ToDictionary(this ValidationResult validationResult)
    {
      return validationResult.Errors
        .GroupBy(x => x.PropertyName)
        .ToDictionary(
          g => g.Key,
          g => g.Select(x => x.ErrorMessage).ToArray()
        );
    }
}
```

----------------------------------------

TITLE: Migrating from SetCollectionValidator to RuleForEach
DESCRIPTION: FluentValidation 8.0 deprecates `SetCollectionValidator` in favor of `RuleForEach` for validating elements within a collection. `RuleForEach` offers more comprehensive capabilities, including defining inline rules.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-8.md#_snippet_0

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.AddressLines).SetCollectionValidator(new AddressLineValidator());
```

LANGUAGE: csharp
CODE:
```
RuleForEach(x => x.AddressLines).SetValidator(new AddressLineValidator());
```

----------------------------------------

TITLE: Validating Specific Properties by Path
DESCRIPTION: FluentValidation 8.0 introduces the ability to validate specific properties within an object graph by providing their full path, allowing for more granular validation control.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-8.md#_snippet_1

LANGUAGE: csharp
CODE:
```
validator.Validate(customer, "Address.Line1", "Address.Line2");
```

----------------------------------------

TITLE: PropertyValidator API Changes: Accessing Options
DESCRIPTION: In FluentValidation 8.0, properties like `CustomStateProvider`, `Severity`, `ErrorMessageSource`, and `ErrorCodeSource` are no longer directly exposed on `PropertyValidator`. They are now accessible via the new `Options` property on `PropertyValidator` to allow for future extensibility without breaking changes.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-8.md#_snippet_3

LANGUAGE: APIDOC
CODE:
```
// FluentValidation 7.x (Conceptual API)
interface IPropertyValidator {
  object CustomStateProvider { get; }
  Severity Severity { get; }
  IStringSource ErrorMessageSource { get; }
  IStringSource ErrorCodeSource { get; }
}

// FluentValidation 8.0 (Conceptual API)
interface IPropertyValidator {
  IPropertyValidatorOptions Options { get; }
}

interface IPropertyValidatorOptions {
  object CustomStateProvider { get; }
  Severity Severity { get; }
  IStringSource ErrorMessageSource { get; }
  IStringSource ErrorCodeSource { get; }
}
```

----------------------------------------

TITLE: ValidatorAttribute and AttributedValidatorFactory Package Relocation
DESCRIPTION: The `ValidatorAttribute` and `AttributedValidatorFactory` have been moved to a separate `FluentValidation.ValidatorAttribute` package in FluentValidation 8.0. While still supported for compatibility, especially in legacy ASP.NET projects, the recommended approach for wiring validators is now via service providers (ASP.NET Core) or IoC containers (desktop/mobile).
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-8.md#_snippet_4

LANGUAGE: APIDOC
CODE:
```
// FluentValidation 7.x (Conceptual)
// ValidatorAttribute and AttributedValidatorFactory were part of the main FluentValidation package.

// FluentValidation 8.0 (Conceptual)
// ValidatorAttribute and AttributedValidatorFactory moved to FluentValidation.ValidatorAttribute package.
// To use attribute-based wiring, this package must be explicitly installed.
// Recommended alternatives:
// - ASP.NET Core: Use service provider to wire models to validators.
// - Desktop/Mobile: Use an IoC container for validator wiring.
```

----------------------------------------

TITLE: FluentValidation TestHelper Chained Assertions
DESCRIPTION: Illustrates the enhanced TestHelper syntax in FluentValidation 9.0, enabling chaining of assertions like WithMessage directly onto ShouldHaveValidationErrorFor and ShouldNotHaveValidationErrorFor for more concise validation tests.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-9.md#_snippet_0

LANGUAGE: C#
CODE:
```
var validator = new InlineValidator<Person>();
validator.RuleFor(x => x.Surname).NotNull().WithMessage("required");
validator.RuleFor(x => x.Address.Line1).NotEqual("foo");

// New advanced test syntax
var result = validator.TestValidate(new Person { Address = new Address()) };
result.ShouldHaveValidationErrorFor(x => x.Surname).WithMessage("required");
result.ShouldNotHaveValidationErrorFor(x => x.Address.Line1);
```

----------------------------------------

TITLE: FluentValidation Rule Severity (Pre-9.0)
DESCRIPTION: Shows the previous method of assigning a fixed severity to a FluentValidation rule using the WithSeverity method with a direct Severity enum value, common in versions prior to 9.0.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-9.md#_snippet_2

LANGUAGE: C#
CODE:
```
RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Warning);
```

----------------------------------------

TITLE: FluentValidation 9.0 API Removals and Changes
DESCRIPTION: This API documentation details various methods, properties, and classes that were deprecated in FluentValidation 8 and have been completely removed or had their visibility changed in version 9.0. It serves as a reference for migrating existing codebases.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-9.md#_snippet_7

LANGUAGE: APIDOC
CODE:
```
FluentValidation 9.0 API Removals:
  Classes/Interfaces with removed members:
    MesageFormatter:
      - ReplacePlaceholderWithValue (method)
      - GetPlaceholder (method)
    IStringSource:
      - ResourceName (property)
      - ResourceType (property)
    ValidationFailure:
      - ResourceName (property)
    PropertyValidatorContext:
      - Instance (property) - Use InstanceToValidate instead.
  Removed Classes:
    - DelegatingValidator
    - FluentValidation.Internal.Comparer
  Visibility Changes:
    - FluentValidation.Internal.TrackingCollection (now internal)
```

----------------------------------------

TITLE: Migrate Custom FluentValidation Property Validators to Generics
DESCRIPTION: This example illustrates the migration of a custom property validator from the old non-generic `PropertyValidator` class to the new generic `PropertyValidator<T,TProperty>` in FluentValidation 10.0. It shows changes to inheritance, method signatures (`IsValid`, `GetDefaultMessageTemplate`), and the addition of the `Name` property for better type safety and performance.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-10.md#_snippet_1

LANGUAGE: csharp
CODE:
```
// Before:
public class NotNullValidator : PropertyValidator
{
  protected override bool IsValid(PropertyValidatorContext context)
  {
    return context.PropertyValue != null;
  }

  protected override string GetDefaultMessageTemplate()
    => "A value for {PropertyName} is required";
}

// After:
public class NotNullValidator<T,TProperty> : PropertyValidator<T, TProperty>
{
  public override string Name => "NotNullValidator";

  public override bool IsValid(ValidationContext<T> context, TProperty value)
  {
    return value != null;
  }

  protected override string GetDefaultMessageTemplate(string errorCode)
    => "A value for {PropertyName} is required";
}
```

----------------------------------------

TITLE: Update ASP.NET Client Validator Adaptors (FluentValidation 10)
DESCRIPTION: Shows the revised signature for creating custom client-side validation adaptors and configuring them in ASP.NET. The constructor now accepts an `IRuleComponent` instead of `IPropertyValidator`, and the `Add` method for client-side configuration uses a non-generic interface for lookup.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-10.md#_snippet_4

LANGUAGE: csharp
CODE:
```
// Before:
public class MyCustomClientsideAdaptor : ClientValidatorBase
{
  public MyCustomClientsideAdaptor(PropertyRule rule, IPropertyValidator validator)
  : base(rule, validator)
  {

  }

  public override void AddValidation(ClientModelValidationContext context)
  {
    // ...
  }
}

services.AddMvc().AddFluentValidation(fv =>
{
  fv.ConfigureClientsideValidation(clientSide =>
  {
    clientSide.Add(typeof(MyCustomPropertyValidator), (context, rule, validator) => new MyCustomClientsideAdaptor(rule, validator));
  })
})


// after:
public class MyCustomClientsideAdaptor : ClientValidatorBase
{
  public MyCustomClientsideAdaptor(IValidationRule rule, IRuleComponent component)
  : base(rule, component)
  {

  }

  public override void AddValidation(ClientModelValidationContext context)
  {
    // ...
  }
}

services.AddMvc().AddFluentValidation(fv =>
{
  fv.ConfigureClientsideValidation(clientSide =>
  {
    clientSide.Add(typeof(IMyCustomPropertyValidator), (context, rule, component) => new MyCustomClientsideAdaptor(rule, component));
  })
})
```

----------------------------------------

TITLE: FluentValidation Validate Method Overload Replacements
DESCRIPTION: This snippet demonstrates the updated syntax for validating specific properties and applying multiple rulesets using the `Validate` method in FluentValidation. It replaces previously deprecated overloads, showing how to use `IncludeProperties` for property-specific validation and `IncludeRuleSets` for applying multiple rulesets.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-10.md#_snippet_5

LANGUAGE: csharp
CODE:
```
// Validating only specific properties.
// Before:
validator.Validate(instance, x => x.SomeProperty, x => x.SomeOtherProperty);
validator.Validate(instance, "SomeProperty", "SomeOtherProperty");

// After:
validator.Validate(instance, v =>
{
  v.IncludeProperties(x => x.SomeProperty, x => x.SomeOtherProperty);
});

validator.Validate(instance, v =>
{
  v.IncludeProperties("SomeProperty", "SomeOtherProperty");
});

// Validating by ruleset:
// Before (comma-delmited string to separate multiple rulesets):
validator.Validate(instance, ruleSet: "SomeRuleSet,AnotherRuleSet");

// After:
// Separate parameters for each ruleset.
validator.Validate(instance, v => 
{
  v.IncludeRuleSets("SomeRuleSet", "AnotherRuleSet");
});
```

----------------------------------------

TITLE: Update Global CascadeMode to Rule/Class Level (Continue/Stop)
DESCRIPTION: Demonstrates how to update the deprecated `ValidatorOptions.Global.CascadeMode` when set to `Continue` or `Stop` by using the new `DefaultClassLevelCascadeMode` and `DefaultRuleLevelCascadeMode` properties.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-11.md#_snippet_0

LANGUAGE: csharp
CODE:
```
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.<YourCurrentValue>;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.<YourCurrentValue>;
```

----------------------------------------

TITLE: Update AbstractValidator CascadeMode to Rule/Class Level (StopOnFirstFailure)
DESCRIPTION: Explains how to replace the deprecated `AbstractValidator.CascadeMode` when set to `StopOnFirstFailure` with the new `ClassLevelCascadeMode` and `RuleLevelCascadeMode` properties. `StopOnFirstFailure` is replaced by `Stop` at the rule level.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-11.md#_snippet_3

LANGUAGE: csharp
CODE:
```
ClassLevelCascadeMode = CascadeMode.Continue;
RuleLevelCascadeMode = CascadeMode.Stop;
```

----------------------------------------

TITLE: Optimize Rule-Level Cascade Mode Configuration
DESCRIPTION: Demonstrates how to simplify rule-level cascade mode configuration by setting `ClassLevelCascadeMode` and `RuleLevelCascadeMode` once, potentially removing redundant `.Cascade` calls on individual rules.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-11.md#_snippet_4

LANGUAGE: csharp
CODE:
```
ClassLevelCascadeMode = CascadeMode.Continue;
RuleLevelCascadeMode = CascadeMode.Stop;
```

----------------------------------------

TITLE: New MessageBuilder Single Assignment Pattern
DESCRIPTION: Illustrates the updated `MessageBuilder` usage where it is a set-only property, meaning only a single `MessageBuilder` can be assigned to a rule chain. Chaining is no longer possible, requiring custom logic to be self-contained.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-11.md#_snippet_6

LANGUAGE: csharp
CODE:
```
return ruleBuilder.Configure(rule => {
  rule.MessageBuilder = context => {
    // ... some custom logic in here.
    return context.GetDefaultMessage();
  };
});
```

----------------------------------------

TITLE: Old ASP.NET Core FluentValidation Configuration
DESCRIPTION: Shows the deprecated `RunDefaultMvcValidationAfterFluentValidationExecutes` property used in ASP.NET Core configuration for FluentValidation, which has been removed in version 11.0.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-11.md#_snippet_7

LANGUAGE: csharp
CODE:
```
services.AddFluentValidation(fv => {
  fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
});
```

----------------------------------------

TITLE: Replace Global CascadeMode for StopOnFirstFailure in FluentValidation 12.0
DESCRIPTION: This C# snippet illustrates how to migrate from the deprecated `ValidatorOptions.Global.CascadeMode = CascadeMode.StopOnFirstFailure` in FluentValidation 12.0. The `StopOnFirstFailure` option is now achieved by setting `DefaultRuleLevelCascadeMode` to `CascadeMode.Stop`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-12.md#_snippet_1

LANGUAGE: csharp
CODE:
```
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
```

----------------------------------------

TITLE: Replace AbstractValidator CascadeMode for StopOnFirstFailure in FluentValidation 12.0
DESCRIPTION: This C# snippet provides the migration path for `AbstractValidator.CascadeMode = CascadeMode.StopOnFirstFailure` in FluentValidation 12.0. This specific cascade behavior is now achieved by setting `ClassLevelCascadeMode` to `Continue` and `RuleLevelCascadeMode` to `Stop`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-12.md#_snippet_3

LANGUAGE: csharp
CODE:
```
ClassLevelCascadeMode = CascadeMode.Continue;
RuleLevelCascadeMode = CascadeMode.Stop;
```

----------------------------------------

TITLE: Migrate from InjectValidator to Constructor Injection in FluentValidation 12.0
DESCRIPTION: This C# snippet provides the recommended alternative to the removed `InjectValidator` method in FluentValidation 12.0. It shows how to use traditional constructor injection with `SetValidator` to explicitly provide child validators, offering broader applicability beyond ASP.NET MVC.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-12.md#_snippet_5

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator(IValidator<Address> addressValidator) 
  {
    RuleFor(x => x.Address).SetValidator(addressValidator);
  }
}
```

----------------------------------------

TITLE: Associate Custom Error Code with FluentValidation Rule
DESCRIPTION: This C# code demonstrates how to assign a custom error code to a validation rule within a FluentValidation `AbstractValidator`. The `WithErrorCode` method is used to attach 'ERR1234' to the `NotNull()` rule for the `Surname` property, allowing for specific error identification.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/error-codes.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator() 
  {
    RuleFor(person => person.Surname).NotNull().WithErrorCode("ERR1234");        
    RuleFor(person => person.Forename).NotNull();
  }
}
```

----------------------------------------

TITLE: Install FluentValidation using NuGet Package Manager Console
DESCRIPTION: This command installs the FluentValidation library into your project. It is intended to be run within the NuGet Package Manager Console in Visual Studio.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/installation.md#_snippet_0

LANGUAGE: PowerShell
CODE:
```
Install-Package FluentValidation
```

----------------------------------------

TITLE: Install FluentValidation using .NET CLI
DESCRIPTION: This command adds the FluentValidation NuGet package reference to your project. It is executed from a terminal window using the .NET command-line interface.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/installation.md#_snippet_1

LANGUAGE: Bash
CODE:
```
dotnet add package FluentValidation
```

----------------------------------------

TITLE: Python Project Requirements
DESCRIPTION: This snippet lists the Python packages and their version constraints required for the project's development or build environment. These are commonly found in a `requirements.txt` file and are used by tools like pip to install dependencies.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/requirements.txt#_snippet_0

LANGUAGE: Python
CODE:
```
-r requirements_rtd.txt
sphinx-autobuild==0.7.1
jinja2<3.1.0
```

----------------------------------------

TITLE: Throw Exception on FluentValidation Failure
DESCRIPTION: Introduces `ValidateAndThrow`, an extension method that, instead of returning a `ValidationResult`, throws a `ValidationException` if any validation rules fail, simplifying error handling in some scenarios.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_7

LANGUAGE: C#
CODE:
```
Customer customer = new Customer();
CustomerValidator validator = new CustomerValidator();

validator.ValidateAndThrow(customer);

```

----------------------------------------

TITLE: Import FluentValidation Namespace for Extension Methods
DESCRIPTION: Emphasizes the necessity of including the `using FluentValidation;` directive to make extension methods like `ValidateAndThrow` accessible within your C# code.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_8

LANGUAGE: C#
CODE:
```
using FluentValidation;

```

----------------------------------------

TITLE: Combine Multiple FluentValidation Options for Advanced Control
DESCRIPTION: Illustrates how to combine various validation options within a single `Validate` method call's options lambda, such as `ThrowOnFailures()`, `IncludeRuleSets()`, and `IncludeProperties()`, for fine-grained control over validation behavior.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_10

LANGUAGE: C#
CODE:
```
validator.Validate(customer, options => 
{
  options.ThrowOnFailures();
  options.IncludeRuleSets("MyRuleSets");
  options.IncludeProperties(x => x.Name);
});

```

----------------------------------------

TITLE: Create Address Validator in FluentValidation C#
DESCRIPTION: Implements an `AddressValidator` by inheriting from `AbstractValidator<Address>`. This validator defines specific rules for the `Address` class, such as ensuring the `Postcode` property is not null. This modular approach allows for reusability.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_12

LANGUAGE: csharp
CODE:
```
public class AddressValidator : AbstractValidator<Address> 
{
  public AddressValidator()
  {
    RuleFor(address => address.Postcode).NotNull();
    //etc
  }
}
```

----------------------------------------

TITLE: Define Inline Validation with Explicit Null Check for Nested Properties in FluentValidation C#
DESCRIPTION: Expands on inline validation by adding an explicit null check using the `When` condition. This ensures that the validation rule for a nested property, such as `Postcode`, is only applied if the parent complex property (`Address`) is not null, preventing potential `NullReferenceException`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_15

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Address.Postcode).NotNull().When(customer => customer.Address != null)
```

----------------------------------------

TITLE: Set Rule-Specific Validator Cascade Mode in FluentValidation
DESCRIPTION: Illustrates how to override the global cascade mode for a specific validation rule. This allows a particular rule to stop on its first failure, regardless of the global setting.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/Changelog.txt#_snippet_1

LANGUAGE: C#
CODE:
```
RuleFor(x => x.Surname).Cascade().StopOnFirstFailure()
```

----------------------------------------

TITLE: Deprecated PropertyRule Validator Replacement Methods
DESCRIPTION: Illustrates the deprecated methods for replacing validators on a property rule. 'ReplaceCurrentValidator' has been superseded by 'ReplaceValidator' for improved clarity and consistency in API usage.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/Changelog.txt#_snippet_3

LANGUAGE: C#
CODE:
```
PropertyRule.ReplaceCurrentValidator(newValidator)
```

LANGUAGE: C#
CODE:
```
PropertyRule.ReplaceValidator(originalValidator, newValidator)
```

----------------------------------------

TITLE: Chaining Validators
DESCRIPTION: Illustrates different approaches to chaining validation rules for a single property. Examples include setting a new validator and applying multiple built-in rules, noting that the explicit `And` property for chaining has been superseded by implicit chaining in later versions.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/Changelog.txt#_snippet_5

LANGUAGE: C#
CODE:
```
RuleFor(person => person.Address).SetValidator(new AddressValidator());
```

LANGUAGE: C#
CODE:
```
RuleFor(person => person.Surname).NotNull().And.NotEqual("Foo");
```

----------------------------------------

TITLE: Override PreValidate Method for Custom Pre-Validation Logic in FluentValidation
DESCRIPTION: Demonstrates how to override the `PreValidate` method in `AbstractValidator` to execute custom logic before standard validation. This allows for early aborts or modifications to the `ValidationResult`, such as handling null models before FluentValidation's default null-check behavior.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/advanced.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class MyValidator : AbstractValidator<Person> 
{
  public MyValidator() 
  {
    RuleFor(x => x.Name).NotNull();
  }

  protected override bool PreValidate(ValidationContext<Person> context, ValidationResult result) 
  {
    if (context.InstanceToValidate == null) 
    {
      result.Errors.Add(new ValidationFailure("", "Please ensure a model was supplied."));
      return false;
    }
    return true;
  }
}
```

----------------------------------------

TITLE: Chain Custom Validators using Extension Methods
DESCRIPTION: This C# example demonstrates how to use the previously defined `ListMustContainFewerThan` extension method within a FluentValidation `AbstractValidator`. It shows how custom validators can be chained seamlessly with other validation rules for cleaner syntax.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_12

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> {
    public PersonValidator() {
       RuleFor(person => person.Pets).ListMustContainFewerThan(10);
    }
}
```

----------------------------------------

TITLE: Define Person Model and FluentValidation Validator
DESCRIPTION: This C# code defines a `Person` class with properties like Id, Name, Email, and Age. It also defines a `PersonValidator` that inherits from `AbstractValidator<Person>`, setting up validation rules for each property, such as `NotNull` for Id, `Length` for Name, `EmailAddress` for Email, and `InclusiveBetween` for Age.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/aspnet.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class Person 
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string Email { get; set; }
  public int Age { get; set; }
}

public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator() 
  {
    RuleFor(x => x.Id).NotNull();
    RuleFor(x => x.Name).Length(0, 10);
    RuleFor(x => x.Email).EmailAddress();
    RuleFor(x => x.Age).InclusiveBetween(18, 60);
  }
}
```

----------------------------------------

TITLE: Register All FluentValidation Validators from Assembly
DESCRIPTION: This C# code illustrates how to automatically register all FluentValidation validators found in a specific assembly with the ASP.NET Core service provider. It uses the `AddValidatorsFromAssemblyContaining<PersonValidator>()` extension method, which requires the `FluentValidation.DependencyInjectionExtensions` package, to scan the assembly containing `PersonValidator` and register all `AbstractValidator<T>` implementations.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/aspnet.md#_snippet_2

LANGUAGE: csharp
CODE:
```
public void ConfigureServices(IServiceCollection services) 
{
    services.AddMvc();

    // ... other configuration ...

    services.AddValidatorsFromAssemblyContaining<PersonValidator>();
}
```

----------------------------------------

TITLE: Override Validation Message with Lambda and Constant Values
DESCRIPTION: Illustrates using a lambda expression with `WithMessage` to construct a dynamic error message. This example embeds constant values into the message using `string.Format`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/configuring.md#_snippet_3

LANGUAGE: C#
CODE:
```
//Using constant in a custom message:
RuleFor(customer => customer.Surname)
  .NotNull()
  .WithMessage(customer => string.Format("This message references some constant values: {0} {1}", "hello", 5))
```

----------------------------------------

TITLE: Apply Rule-Level Cascade Mode to Multiple Rules (Repetitive)
DESCRIPTION: Shows how to explicitly set `CascadeMode.Stop` for multiple individual rules, highlighting the repetitive nature of this approach before introducing a more concise method.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/cascade.md#_snippet_2

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Forename).Cascade(CascadeMode.Stop).NotNull().NotEqual("foo");
RuleFor(x => x.MiddleNames).Cascade(CascadeMode.Stop).NotNull().NotEqual("foo");
RuleFor(x => x.Surname).Cascade(CascadeMode.Stop).NotNull().NotEqual("foo");
```

----------------------------------------

TITLE: Deprecated FluentValidation Cascade Chaining Methods
DESCRIPTION: Demonstrates the deprecated chaining methods for setting cascade behavior. These methods, 'StopOnFirstFailure()' and 'Continue()', have been replaced by a more consistent 'Cascade(cascadeMode)' overload for setting validation flow.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/Changelog.txt#_snippet_4

LANGUAGE: C#
CODE:
```
Cascade().StopOnFirstFailure()
```

LANGUAGE: C#
CODE:
```
Cascade().Continue()
```

----------------------------------------

TITLE: FluentValidation Built-in Message Placeholders
DESCRIPTION: Reference for special placeholders that can be used within custom validation error messages in FluentValidation. These placeholders are replaced with dynamic values at runtime.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/configuring.md#_snippet_2

LANGUAGE: APIDOC
CODE:
```
Placeholders:
  {PropertyName}: Name of the property being validated
  {PropertyValue}: Value of the property being validated

Comparison Validators (Equal, NotEqual, GreaterThan, etc.):
  {ComparisonValue}: Value that the property should be compared to
  {ComparisonProperty}: Name of the property being compared against (if any)

Length Validator:
  {MinLength}: Minimum length
  {MaxLength}: Maximum length
  {TotalLength}: Number of characters entered
```

----------------------------------------

TITLE: InclusiveBetween Validator Usage
DESCRIPTION: Checks whether the property value falls within a specified numerical range, inclusively. This means the property value must be greater than or equal to the lower bound and less than or equal to the upper bound. The example error message is: 'Id' must be between 1 and 10. You entered 0. String format arguments available for customization include: {PropertyName} (Name of the property being validated), {PropertyValue} (Current value of the property), {From} (Lower bound of the range), {To} (Upper bound of the range), and {PropertyPath} (The full path of the property).
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_20

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Id).InclusiveBetween(1,10);
```

----------------------------------------

TITLE: ASP.NET Core Razor View for Displaying FluentValidation Errors
DESCRIPTION: This HTML Razor view ('Create.cshtml') demonstrates how to display validation errors populated in 'ModelState' by FluentValidation. It uses ASP.NET Core Tag Helpers ('asp-validation-summary', 'asp-validation-for') to show model-level and property-specific errors, providing user feedback for invalid form submissions.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/aspnet.md#_snippet_5

LANGUAGE: html
CODE:
```
@model Person

<div asp-validation-summary="ModelOnly"></div>

<form asp-action="Create">
  Id: <input asp-for="Id" /> <span asp-validation-for="Id"></span>
  <br />
  Name: <input asp-for="Name" /> <span asp-validation-for="Name"></span>
  <br />
  Email: <input asp-for="Email" /> <span asp-validation-for="Email"></span>
  <br />
  Age: <input asp-for="Age" /> <span asp-validation-for="Age"></span>

  <br /><br />
  <input type="submit" value="submit" />
</form>
```

----------------------------------------

TITLE: Define ContactRequest with a Collection of IContact
DESCRIPTION: This C# class modifies the `ContactRequest` to include a `List<IContact>` property, `Contacts`, instead of a single `IContact`. This setup is used to demonstrate inheritance validation for collections where each item might be a different subclass, requiring individual validation.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/inheritance.md#_snippet_3

LANGUAGE: csharp
CODE:
```
public class ContactRequest 
{
  public List<IContact> Contacts { get; } = new();
}
```

----------------------------------------

TITLE: Localizing Validation Messages with WithMessage and .resx
DESCRIPTION: This snippet demonstrates how to use the WithMessage method with strongly-typed wrappers for .resx files to provide localized error messages for a validation rule. It's suitable for obtaining localized strings from resource files or other sources via a lambda expression.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/localization.md#_snippet_0

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Surname).NotNull().WithMessage(x => MyLocalizedMessages.SurnameRequired);
```

----------------------------------------

TITLE: Access Current Property Validator (FluentValidation 10)
DESCRIPTION: Illustrates the change in accessing the "current" property validator on a rule. In FluentValidation 10, the `Current` property of the rule returns a `RuleComponent`, from which the `CurrentValidator` can then be retrieved.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-10.md#_snippet_3

LANGUAGE: csharp
CODE:
```
// before:
PropertyRule rule = ...;
IPropertyValidator currentValidator = rule.CurrentValidator;

// after:
IValidationRule<T,TProperty> rule = ...;
RuleComponent<T, TProperty> component = rule.Current;
IPropertyValidator currentValidator = component.CurrentValidator;
```

----------------------------------------

TITLE: Access Property Validators via Rule Instance (FluentValidation 10)
DESCRIPTION: Demonstrates the updated approach for iterating through property validators associated with a validation rule. Previously, validators were directly accessible from `rule.Validators`; now, they are accessed via `rule.Components` and then `component.Validator`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-10.md#_snippet_2

LANGUAGE: csharp
CODE:
```
// Before:
IValidationRule rule = ...;
foreach (IPropertyValidator propertyValidator in rule.Validators) 
{
  // ...
}

// After:
IValidationRule rule = ...;
foreach (IRuleComponent component in rule.Componetnts) 
{
  IPropertyValiator propertyValidator = component.Validator;
}
```

----------------------------------------

TITLE: Validating Specific Properties
DESCRIPTION: Shows how to validate only a subset of properties on an object by passing property expressions to the `Validate` method, allowing for more granular control over the validation process.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/Changelog.txt#_snippet_6

LANGUAGE: C#
CODE:
```
validator.Validate(person, x => x.Surname, x => x.Forename);
```

----------------------------------------

TITLE: Manual Validation in ASP.NET Core Controller with FluentValidation
DESCRIPTION: This C# code demonstrates how to manually validate a 'Person' object within an ASP.NET Core MVC controller using FluentValidation. It shows constructor injection of 'IValidator<Person>', asynchronous validation using 'ValidateAsync', and how to copy validation errors into 'ModelState' for display in the view. It also includes basic CRUD operations like saving the person and redirecting.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/aspnet.md#_snippet_3

LANGUAGE: csharp
CODE:
```
public class PeopleController : Controller 
{
  private IValidator<Person> _validator;
  private IPersonRepository _repository;

  public PeopleController(IValidator<Person> validator, IPersonRepository repository) 
  {
    // Inject our validator and also a DB context for storing our person object.
    _validator = validator;
    _repository = repository;
  }

  public ActionResult Create() 
  {
    return View();
  }

  [HttpPost]
  public async Task<IActionResult> Create(Person person) 
  {
    ValidationResult result = await _validator.ValidateAsync(person);

    if (!result.IsValid) 
    {
      // Copy the validation results into ModelState.
      // ASP.NET uses the ModelState collection to populate 
      // error messages in the View.
      result.AddToModelState(this.ModelState);

      // re-render the view when validation failed.
      return View("Create", person);
    }

    _repository.Save(person); //Save the person to the database, or some other logic

    TempData["notice"] = "Person successfully created";
    return RedirectToAction("Index");
  }
}
```

----------------------------------------

TITLE: Chain assertions to verify specific validation error details
DESCRIPTION: This C# snippet illustrates chaining methods like `WithErrorMessage`, `WithSeverity`, and `WithErrorCode` after `ShouldHaveValidationErrorFor`. This allows for precise verification of individual components of a validation failure, such as the error message, severity level, and error code.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/testing.md#_snippet_3

LANGUAGE: csharp
CODE:
```
var result = validator.TestValidate(person);

result.ShouldHaveValidationErrorFor(person => person.Name)
  .WithErrorMessage("'Name' must not be empty.")
  .WithSeverity(Severity.Error)
  .WithErrorCode("NotNullValidator");
```

----------------------------------------

TITLE: Perform multiple validation assertions using TestValidate
DESCRIPTION: This C# example shows how to perform multiple assertions on a single validation result obtained from `TestValidate`. It demonstrates checking for errors on specific properties using both lambda expressions and string-based property paths.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/testing.md#_snippet_2

LANGUAGE: csharp
CODE:
```
var person = new Person { Name = "Jeremy" };
var result = validator.TestValidate(person);

// Assert that there should be a failure for the Name property.
result.ShouldHaveValidationErrorFor(x => x.Name);

// Assert that there are no failures for the age property.
result.ShouldNotHaveValidationErrorFor(x => x.Age);

// You can also use a string name for properties that can't be easily represented with a lambda, eg:
result.ShouldHaveValidationErrorFor("Addresses[0].Line1");
```

----------------------------------------

TITLE: Apply Less Than Validation for Numeric or Comparable Properties in C#
DESCRIPTION: Validates that a property's value is strictly less than a specified constant or another property's value. This validator is suitable for types implementing `IComparable<T>` and is commonly used for numerical comparisons like credit limits. Error messages can utilize PropertyName, ComparisonValue, ComparisonProperty, PropertyValue, and PropertyPath.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_7

LANGUAGE: csharp
CODE:
```
//Less than a particular value
RuleFor(customer => customer.CreditLimit).LessThan(100);

//Less than another property
RuleFor(customer => customer.CreditLimit).LessThan(customer => customer.MaxCreditLimit);
```

----------------------------------------

TITLE: Set Global Default Validator Severity
DESCRIPTION: This C# code demonstrates how to set a global default severity level for all validators in FluentValidation. By configuring `ValidatorOptions.Global.Severity` during application startup, all rules will default to the specified severity (e.g., `Info`) unless overridden by individual rules.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/severity.md#_snippet_5

LANGUAGE: csharp
CODE:
```
ValidatorOptions.Global.Severity = Severity.Info;
```

----------------------------------------

TITLE: Replace Global CascadeMode for Continue/Stop in FluentValidation 12.0
DESCRIPTION: This C# snippet demonstrates how to replace the deprecated `ValidatorOptions.Global.CascadeMode` property when its value was `Continue` or `Stop`. FluentValidation 12.0 introduces `DefaultClassLevelCascadeMode` and `DefaultRuleLevelCascadeMode` for more granular control over validation cascade behavior at the global level.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-12.md#_snippet_0

LANGUAGE: csharp
CODE:
```
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.<YourCurrentValue>;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.<YourCurrentValue>;
```

----------------------------------------

TITLE: FluentValidation Old vs. New Localization Syntax
DESCRIPTION: These C# code snippets illustrate the evolution of localization syntax in FluentValidation. The first snippet shows the deprecated `WithLocalizedMessage` method, which directly specified a resource type and name. The second snippet demonstrates the new recommended approach using a `WithMessage` callback to explicitly access localized messages from a strongly-typed wrapper.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-9.md#_snippet_6

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Surname).NotNull().WithLocalizedMessage(typeof(MyLocalizedMessages), "SurnameRequired");
```

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Surname).NotNull().WithMessage(x => MyLocalizedMessages.SurnameRequired);
```

----------------------------------------

TITLE: Specifying Rulesets for Child Validators with SetValidator
DESCRIPTION: In FluentValidation 8.0, when using `SetValidator` for child validators, you can now explicitly define which ruleset should be invoked on the child, rather than implicitly cascading the parent's ruleset.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-8.md#_snippet_2

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Address).SetValidator(new AddressValidator(), "myRuleset");
```

----------------------------------------

TITLE: Retrieve and Display FluentValidation Error Codes
DESCRIPTION: This C# snippet illustrates how to execute a FluentValidation validator and then iterate through the resulting `ValidationFailure` collection. It shows how to access the `ErrorCode` property for each validation error, demonstrating how both custom and default error codes can be retrieved and displayed.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/error-codes.md#_snippet_1

LANGUAGE: csharp
CODE:
```
var validator = new PersonValidator();
var result = validator.Validate(new Person());
foreach (var failure in result.Errors)
{
  Console.WriteLine($"Property: {failure.PropertyName} Error Code: {failure.ErrorCode}");
}
```

----------------------------------------

TITLE: FluentValidation Rule Severity with Dynamic Callback
DESCRIPTION: Illustrates the new capability in FluentValidation 9.0 to dynamically determine a rule's severity. The WithSeverity method now accepts a callback function, allowing for conditional or context-based severity assignment.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-9.md#_snippet_3

LANGUAGE: C#
CODE:
```
RuleFor(x => x.Surname).NotNull().WithSeverity(x => Severity.Warning);
```

----------------------------------------

TITLE: Apply Email Address Validation Rule (C#)
DESCRIPTION: Applies an email address validation rule to a property. By default, it performs a simple check for an '@' sign. An alternative, deprecated mode using a .NET 4.x regex is also mentioned. Error messages can be formatted using `{PropertyName}`, `{PropertyValue}`, and `{PropertyPath}`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_13

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Email).EmailAddress();
```

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Email).EmailAddress(EmailValidationMode.Net4xRegex);
```

----------------------------------------

TITLE: Create an Extension Method for Custom Validators
DESCRIPTION: This C# code defines an extension method `ListMustContainFewerThan` that wraps the `SetValidator` call for `ListCountValidator`. This improves the readability and chaining capabilities when defining validation rules in FluentValidation.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_11

LANGUAGE: csharp
CODE:
```
public static class MyValidatorExtensions {
   public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {
      return ruleBuilder.SetValidator(new ListCountValidator<T, TElement>(num));
   }
}
```

----------------------------------------

TITLE: Validate Collection Sub-Properties using Wildcard Indexer
DESCRIPTION: This C# code illustrates how to validate a specific property (`Cost`) within each item of a collection (`Orders`) using FluentValidation's `IncludeProperties` with a wildcard indexer (`[]`). This allows for targeted validation across all elements of a collection without iterating manually.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/specific-properties.md#_snippet_2

LANGUAGE: csharp
CODE:
```
var validator = new CustomerValidator();
validator.Validate(customer, options => 
{
  options.IncludeProperties("Orders[].Cost");
});
```

----------------------------------------

TITLE: Apply Less Than Or Equal Validation for Numeric or Comparable Properties in C#
DESCRIPTION: Validates that a property's value is less than or equal to a specified constant or another property's value. This validator is suitable for types implementing `IComparable<T>` and is commonly used for numerical comparisons. Error messages can utilize PropertyName, ComparisonValue, ComparisonProperty, PropertyValue, and PropertyPath.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_8

LANGUAGE: csharp
CODE:
```
//Less than a particular value
RuleFor(customer => customer.CreditLimit).LessThanOrEqualTo(100);

//Less than another property
RuleFor(customer => customer.CreditLimit).LessThanOrEqualTo(customer => customer.MaxCreditLimit);
```

----------------------------------------

TITLE: Automatically Register FluentValidation Validators by Assembly
DESCRIPTION: This C# code snippet shows how to use the `AddValidatorsFromAssemblyContaining` extension method from `FluentValidation.DependencyInjectionExtensions` to automatically discover and register all public, non-abstract validators within the assembly containing `UserValidator`. This simplifies the setup process by eliminating the need for manual registration of each validator.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/di.md#_snippet_3

LANGUAGE: csharp
CODE:
```
using FluentValidation.DependencyInjectionExtensions;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<UserValidator>();
        // ...
    }

    // ...
}
```

----------------------------------------

TITLE: Update FluentValidation Rules after PropertyValidatorContext Deprecation
DESCRIPTION: This snippet demonstrates how to update validation rules in FluentValidation 10.0 after the deprecation of `PropertyValidatorContext`. Previously, `context.ParentContext` was used to access `ValidationContext<T>`, but now `context` directly provides access to `RootContextData`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-10.md#_snippet_0

LANGUAGE: csharp
CODE:
```
// Before:
RuleFor(x => x.Foo).Must((instance, value, context) => 
{
  return context.ParentContext.RootContextData.ContainsKey("Something");
});

// After:
RuleFor(x => x.Foo).Must((instance, value, context) => 
{
  return context.RootContextData.ContainsKey("Something");
});
```

----------------------------------------

TITLE: Combine whole collection and individual element rules with ForEach
DESCRIPTION: Demonstrates an alternative approach using the `ForEach` method within a `RuleFor` to combine validation rules for the entire collection and its individual elements into a single rule chain. While concise, separate rules are often recommended for clarity.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/collections.md#_snippet_8

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Orders)
  .Must(x => x.Count <= 10).WithMessage("No more than 10 orders are allowed")
  .ForEach(orderRule => 
  {
    orderRule.Must(order => order.Total > 0).WithMessage("Orders must have a total of more than 0")
  });
```

----------------------------------------

TITLE: Execute Specific FluentValidation RuleSet
DESCRIPTION: Shows how to validate an object using only a specific RuleSet ('Names') by passing options to the Validate method. This ensures that only rules within the specified RuleSet are executed, ignoring all other rules defined in the validator.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/rulesets.md#_snippet_1

LANGUAGE: csharp
CODE:
```
var validator = new PersonValidator();
var person = new Person();
var result = validator.Validate(person, options => options.IncludeRuleSets("Names"));
```

----------------------------------------

TITLE: Stubbing FluentValidation Failures with InlineValidator
DESCRIPTION: This example demonstrates how to create a stub implementation of a FluentValidation validator using `InlineValidator<T>`. It shows an original validator that depends on an external service and then illustrates how to create a simple `InlineValidator` instance to force a validation failure for testing purposes, which can then be used anywhere an `IValidator<Customer>` is expected.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/testing.md#_snippet_5

LANGUAGE: csharp
CODE:
```
// Original validator that relies on an external service.
// External service is used to check that the customer ID is not already used in the database.
public class CustomerValidator : AbstractValidator<Customer>
{
  public CustomerValidator(ICustomerRepository customerRepository)
  {
    RuleFor(x => x.Id)
      .Must(id => customerRepository.CheckIdNotInUse(id));
  }
}

// If you needed to stub this failure in a unit/integration test,
// you could do the following:
var validator = new InlineValidator<Customer>();
validator.RuleFor(x => x.Id).Must(id => false);

// This instance could then be passed into anywhere expecting an IValidator<Customer>
```

----------------------------------------

TITLE: Apply Custom Predicate Validation (Must) in C#
DESCRIPTION: Allows custom validation logic to be applied to a property's value using a delegate. This validator, also known as `Must`, passes the property's value into a function for custom checks. An overload is available to access the parent object for cross-property comparisons. Error messages can utilize PropertyName, PropertyValue, and PropertyPath.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_11

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Surname).Must(surname => surname == "Foo");
```

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Surname).Must((customer, surname) => surname != customer.Forename)
```

----------------------------------------

TITLE: Registering and Invoking FluentValidation in ASP.NET Core Minimal APIs
DESCRIPTION: This C# code snippet demonstrates how to integrate FluentValidation with ASP.NET Core Minimal APIs. It shows how to register an IValidator with the service provider and then invoke it asynchronously within a POST endpoint. The example handles validation results by returning a ValidationProblem for invalid inputs and a Created response for valid ones. It also highlights the use of the 'ToDictionary' method on ValidationResult.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/aspnet.md#_snippet_6

LANGUAGE: csharp
CODE:
```
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Register validator with service provider (or use one of the automatic registration methods)
builder.Services.AddScoped<IValidator<Person>, PersonValidator>();

// Also registering a DB access repository for demo purposes
// replace this with whatever you're using in your application.
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

app.MapPost("/person", async (IValidator<Person> validator, IPersonRepository repository, Person person) => 
{
  ValidationResult validationResult = await validator.ValidateAsync(person);

  if (!validationResult.IsValid) 
  {
    return Results.ValidationProblem(validationResult.ToDictionary());
  }

  repository.Save(person);
  return Results.Created($"/{person.Id}", person);
});
```

----------------------------------------

TITLE: Apply Condition to Specific Validator in FluentValidation Chain
DESCRIPTION: Explains how to apply a condition to only the immediately preceding validator in a rule chain using `ApplyConditionTo.CurrentValidator`. This ensures the condition does not affect other validators in the same `RuleFor` call.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/conditions.md#_snippet_3

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.CustomerDiscount)
    .GreaterThan(0).When(customer => customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator)
    .EqualTo(0).When(customer => ! customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator);
```

----------------------------------------

TITLE: Use ShouldHaveChildValidator Test Extension in FluentValidation
DESCRIPTION: Shows the usage of a FluentValidation test extension method. This extension helps in asserting that a specific property has a designated child validator attached, aiding in unit testing validation logic.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/Changelog.txt#_snippet_2

LANGUAGE: C#
CODE:
```
ShouldHaveChildValidator(x => x.Property, typeof(SomeChildValidator))
```

----------------------------------------

TITLE: Removed Deprecated Methods in FluentValidation 8
DESCRIPTION: FluentValidation 8.0 has removed several methods that were previously marked as obsolete. This includes older custom validation approaches, the pre-7 localization mechanism, and methods for runtime rule modification.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-8.md#_snippet_5

LANGUAGE: APIDOC
CODE:
```
// Removed in FluentValidation 8.0:
// - Pre-7 way of performing custom validation (e.g., Custom() and CustomAsync() directly on RuleBuilder)
//   Replacement: Use RuleFor(x => x).Custom()
// - Old localization mechanism (e.g., several overloads of WithLocalizedName and WithLocalizedMessage)
// - RemoveRule(), ReplaceRule(), ClearRules() (runtime modification of rules is not supported)
// - Async method overloads that did not accept a CancellationToken
//   Replacement: Use async overloads that accept CancellationToken
```

----------------------------------------

TITLE: Apply Greater Than Or Equal Validation for Numeric or Comparable Properties in C#
DESCRIPTION: Validates that a property's value is greater than or equal to a specified constant or another property's value. This validator is suitable for types implementing `IComparable<T>` and is commonly used for numerical comparisons. Error messages can utilize PropertyName, ComparisonValue, ComparisonProperty, PropertyValue, and PropertyPath.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_10

LANGUAGE: csharp
CODE:
```
//Greater than a particular value
RuleFor(customer => customer.CreditLimit).GreaterThanOrEqualTo(1);

//Greater than another property
RuleFor(customer => customer.CreditLimit).GreaterThanOrEqualTo(customer => customer.MinimumCreditLimit);
```

----------------------------------------

TITLE: Configure Service Lifetime for Auto-Registered Validators
DESCRIPTION: This C# example demonstrates how to specify a custom `ServiceLifetime` (e.g., `Transient`) when automatically registering validators using `AddValidatorsFromAssemblyContaining`. By default, validators are registered as `Scoped`, but this allows for explicit control over their lifecycle within the dependency injection container.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/di.md#_snippet_4

LANGUAGE: csharp
CODE:
```
services.AddValidatorsFromAssemblyContaining<UserValidator>(ServiceLifetime.Transient);
```

----------------------------------------

TITLE: Define and Execute FluentValidation Rules in C#
DESCRIPTION: This C# example demonstrates how to define validation rules for a `Customer` class using FluentValidation's `AbstractValidator`. It shows various rule types, including `NotEmpty`, `WithMessage`, `NotEqual`, `When`, `Length`, and `Must` for custom validation logic. The snippet also illustrates how to instantiate and execute the validator, and then inspect the validation results for success or failures.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/src/FluentValidation/README.md#_snippet_0

LANGUAGE: c#
CODE:
```
using FluentValidation;

public class CustomerValidator: AbstractValidator<Customer> {
  public CustomerValidator() {
    RuleFor(x => x.Surname).NotEmpty();
    RuleFor(x => x.Forename).NotEmpty().WithMessage("Please specify a first name");
    RuleFor(x => x.Discount).NotEqual(0).When(x => x.HasDiscount);
    RuleFor(x => x.Address).Length(20, 250);
    RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
  }

  private bool BeAValidPostcode(string postcode) {
    // custom postcode validating logic goes here
  }
}

var customer = new Customer();
var validator = new CustomerValidator();

// Execute the validator.
ValidationResult results = validator.Validate(customer);

// Inspect any validation failures.
bool success = results.IsValid;
List<ValidationFailure> failures = results.Errors;
```

----------------------------------------

TITLE: Create Basic FluentValidation Validator Class
DESCRIPTION: Initializes a FluentValidation validator by creating a class, `CustomerValidator`, that inherits from `AbstractValidator<Customer>`, preparing it to define validation rules for the `Customer` object.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_1

LANGUAGE: C#
CODE:
```
using FluentValidation;

public class CustomerValidator : AbstractValidator<Customer> 
{
}
```

----------------------------------------

TITLE: Define a Customer Validator in C#
DESCRIPTION: This C# example demonstrates how to define a validation class for a `Customer` object using FluentValidation. It showcases the use of `AbstractValidator<T>` to create rules for properties like `Surname`, `Forename`, `Discount`, `Address`, and `Postcode`, including built-in validators and a custom validation method.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/index.rst#_snippet_0

LANGUAGE: C#
CODE:
```
public class CustomerValidator : AbstractValidator<Customer> 
{
  public CustomerValidator()
  {
    RuleFor(x => x.Surname).NotEmpty();
    RuleFor(x => x.Forename).NotEmpty().WithMessage("Please specify a first name");
    RuleFor(x => x.Discount).NotEqual(0).When(x => x.HasDiscount);
    RuleFor(x => x.Address).Length(20, 250);
    RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
  }

  private bool BeAValidPostcode(string postcode) 
  {
    // custom postcode validating logic goes here
  }
}
```

----------------------------------------

TITLE: Define NotNull Validation Rule for Customer Surname
DESCRIPTION: Demonstrates how to add a basic validation rule within the `CustomerValidator` constructor, ensuring the `Surname` property of a `Customer` object is not null using `RuleFor().NotNull()`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_2

LANGUAGE: C#
CODE:
```
using FluentValidation;

public class CustomerValidator : AbstractValidator<Customer>
{
  public CustomerValidator()
  {
    RuleFor(customer => customer.Surname).NotNull();
  }
}
```

----------------------------------------

TITLE: Apply Condition to Multiple Rules with Top-Level When
DESCRIPTION: Shows how to apply a single condition to a group of multiple validation rules by using the top-level `When` method. Both `CustomerDiscount` and `CreditCardNumber` rules are subject to the `IsPreferred` condition.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/conditions.md#_snippet_1

LANGUAGE: csharp
CODE:
```
When(customer => customer.IsPreferred, () => {
   RuleFor(customer => customer.CustomerDiscount).GreaterThan(0);
   RuleFor(customer => customer.CreditCardNumber).NotNull();
});
```

----------------------------------------

TITLE: Apply a Custom Validator using SetValidator
DESCRIPTION: This C# example shows how to apply the `ListCountValidator` to a property within a FluentValidation `AbstractValidator`. It uses the `SetValidator` method to associate the custom validator instance with the `Pets` property of a `Person` object.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_10

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> {
    public PersonValidator() {
       RuleFor(person => person.Pets).SetValidator(new ListCountValidator<Person, Pet>(10));
    }
}
```

----------------------------------------

TITLE: Add Custom Validation Failure in FluentValidation
DESCRIPTION: This C# code snippet shows how to manually add a validation failure using `context.AddFailure`. It illustrates two ways: specifying the property name directly or instantiating a `ValidationFailure` object. This allows overriding the inferred property name for the error.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_7

LANGUAGE: csharp
CODE:
```
context.AddFailure("SomeOtherProperty", "The list must contain 10 items or fewer");
// Or you can instantiate the ValidationFailure directly:
context.AddFailure(new ValidationFailure("SomeOtherProperty", "The list must contain 10 items or fewer");
```

----------------------------------------

TITLE: Format All FluentValidation Errors into Single String
DESCRIPTION: Explains how to use the `ToString()` method on a `ValidationResult` object to concatenate all error messages into a single string, with an option to specify a custom separator character.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_5

LANGUAGE: C#
CODE:
```
ValidationResult results = validator.Validate(customer);
string allMessages = results.ToString("~");     // In this case, each message will be separated with a `~`

```

----------------------------------------

TITLE: Separate rules for whole collection and individual elements
DESCRIPTION: Presents an example of applying a rule to the entire collection using `RuleFor` (e.g., maximum count) and a separate rule to each individual element using `RuleForEach` (e.g., individual item property validation). This highlights a clear and readable validation pattern.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/collections.md#_snippet_7

LANGUAGE: csharp
CODE:
```
// This rule acts on the whole collection (using RuleFor)
RuleFor(x => x.Orders)
  .Must(x => x.Count <= 10).WithMessage("No more than 10 orders are allowed");

// This rule acts on each individual element (using RuleForEach)
RuleForEach(x => x.Orders)
  .Must(order => order.Total > 0).WithMessage("Orders must have a total of more than 0")
```

----------------------------------------

TITLE: Apply Custom FluentValidation Extension Method
DESCRIPTION: Illustrates the concise usage of the previously defined `ListMustContainFewerThan` extension method within a `FluentValidation` rule definition, simplifying the validation syntax for list properties.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_3

LANGUAGE: csharp
CODE:
```
RuleFor(x => x.Pets).ListMustContainFewerThan(10);
```

----------------------------------------

TITLE: New ASP.NET Core FluentValidation Configuration
DESCRIPTION: Demonstrates the replacement for `RunDefaultMvcValidationAfterFluentValidationExecutes`, which is `DisableDataAnnotationsValidation`. Note that this property has inverse behavior compared to the old one.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-11.md#_snippet_8

LANGUAGE: csharp
CODE:
```
services.AddFluentValidation(fv => {
  fv.DisableDataAnnotationsValidation = true;
});
```

----------------------------------------

TITLE: Create Extension Method for Custom FluentValidation Rule
DESCRIPTION: This C# code defines an extension method `ListMustContainFewerThan` that encapsulates the custom validation logic. It simplifies the consumption of the custom rule by allowing it to be chained directly onto `RuleFor` definitions, promoting reusability and cleaner code.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_8

LANGUAGE: csharp
CODE:
```
public static IRuleBuilderOptionsConditions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {

  return ruleBuilder.Custom((list, context) => {
     if(list.Count > 10) {
       context.AddFailure("The list must contain 10 items or fewer");
     }
   });
}
```

----------------------------------------

TITLE: Define Customer Data Model in C#
DESCRIPTION: Defines a simple C# class, `Customer`, with properties like `Id`, `Surname`, `Forename`, `Discount`, and `Address`, which will be subject to validation.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_0

LANGUAGE: C#
CODE:
```
public class Customer 
{
  public int Id { get; set; }
  public string Surname { get; set; }
  public string Forename { get; set; }
  public decimal Discount { get; set; }
  public string Address { get; set; }
}
```

----------------------------------------

TITLE: Validate Specific Property (Surname) using IncludeProperties
DESCRIPTION: This C# example demonstrates how to perform validation on a specific property, `Surname`, of a `customer` object using FluentValidation's `IncludeProperties` option. It instantiates the `CustomerValidator` and then calls the `Validate` method, limiting the execution to only the rule associated with the `Surname` property.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/specific-properties.md#_snippet_1

LANGUAGE: csharp
CODE:
```
var validator = new CustomerValidator();
validator.Validate(customer, options => 
{
  options.IncludeProperties(x => x.Surname);
});
```

----------------------------------------

TITLE: Assert exclusive validation failures with the Only() method
DESCRIPTION: This C# example demonstrates the use of the `Only()` method to ensure that only specified validation failures occurred. It can be used to assert that failures are limited to a particular property or that they match a specific error message exclusively.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/testing.md#_snippet_4

LANGUAGE: csharp
CODE:
```
var result = validator.TestValidate(person);

// Assert that failures only happened for Name property.
result.ShouldHaveValidationErrorFor(person => person.Name).Only();

// Assert that failures only happened for Name property and all have the specified message
result.ShouldHaveValidationErrorFor(person => person.Name)
  .WithErrorMessage("'Name' must not be empty.")
  .Only();
```

----------------------------------------

TITLE: Update Global CascadeMode to Rule/Class Level (StopOnFirstFailure)
DESCRIPTION: Shows how to replace the deprecated `ValidatorOptions.Global.CascadeMode` when set to `StopOnFirstFailure` with the new `DefaultClassLevelCascadeMode` and `DefaultRuleLevelCascadeMode` properties. `StopOnFirstFailure` is replaced by `Stop` at the rule level.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-11.md#_snippet_1

LANGUAGE: csharp
CODE:
```
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue; // Not actually needed as this is the default. Just here for completeness.
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
```

----------------------------------------

TITLE: Limitation: Invalid Inheritance Validation for Base Interface
DESCRIPTION: This C# example illustrates a limitation of FluentValidation's inheritance validation. Attempting to add a validator for the base interface (`IContact`) using `v.Add<IContact>(new ContactBaseValidator())` will not correctly validate instances of derived classes like `Person` or `Organisation`. Each subclass must be explicitly mapped for validation to occur.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/inheritance.md#_snippet_5

LANGUAGE: csharp
CODE:
```
public class ContactBaseValidator : AbstractValidator<IContact> 
{
  public ContactBaseValidatoR() 
  {
    RuleFor(x => x.Name).NotNull();
  }
}

public class ContactRequestValidator : AbstractValidator<ContactRequest>
{
  public ContactRequestValidator()
  {

    RuleFor(x => x.Contact).SetInheritanceValidator(v => 
    {
      // THIS WILL NOT WORK.
      // This will not validate instances of Person or Organisation.
      v.Add<IContact>(new ContactBaseValidator());
    });
  }
}
```

----------------------------------------

TITLE: Execute FluentValidation on a Customer Object
DESCRIPTION: Illustrates the process of instantiating a `Customer` object and its corresponding `CustomerValidator`, then executing the validation logic using the `Validate` method to obtain a `ValidationResult`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_3

LANGUAGE: C#
CODE:
```
Customer customer = new Customer();
CustomerValidator validator = new CustomerValidator();

ValidationResult result = validator.Validate(customer);

```

----------------------------------------

TITLE: Add custom message with CollectionIndex for simple type collection validation
DESCRIPTION: Shows how to include the `{CollectionIndex}` placeholder in a custom error message when validating individual items in a collection using `RuleForEach`. This provides specific context about the position of the failed item within the collection.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/collections.md#_snippet_2

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator() 
  {
    RuleForEach(x => x.AddressLines).NotNull().WithMessage("Address {CollectionIndex} is required.");
  }
}
```

----------------------------------------

TITLE: Define in-line rules for complex type collection using ChildRules
DESCRIPTION: Illustrates using the `ChildRules` method with `RuleForEach` to define validation rules for complex collection elements directly within the parent validator. This approach avoids the need for separate validator classes for child objects.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/collections.md#_snippet_5

LANGUAGE: csharp
CODE:
```
public class CustomerValidator : AbstractValidator<Customer> 
{
  public CustomerValidator() 
  {
    RuleForEach(x => x.Orders).ChildRules(order => 
    {
      order.RuleFor(x => x.Total).GreaterThan(0);
    });
  }
}
```

----------------------------------------

TITLE: Implementing a Custom Language Manager for Default Messages
DESCRIPTION: This code defines a custom class that inherits from FluentValidation.Resources.LanguageManager. It demonstrates how to override FluentValidation's default validation messages for specific validators (e.g., NotNullValidator) across different cultures (e.g., 'en', 'en-US', 'en-GB') by using the AddTranslation method.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/localization.md#_snippet_2

LANGUAGE: csharp
CODE:
```
public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
{
  public CustomLanguageManager() 
  {
    AddTranslation("en", "NotNullValidator", "'{PropertyName}' is required.");
    AddTranslation("en-US", "NotNullValidator", "'{PropertyName}' is required.");
    AddTranslation("en-GB", "NotNullValidator", "'{PropertyName}' is required.");
  }
}
```

----------------------------------------

TITLE: Force FluentValidation Messages to a Specific Culture (C#)
DESCRIPTION: This snippet demonstrates how to configure FluentValidation to always display validation messages in a specific language, regardless of the system's current UI culture. It sets the `Culture` property of the `LanguageManager` to a new `CultureInfo` instance for the desired language, such as French ('fr').
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/localization.md#_snippet_5

LANGUAGE: csharp
CODE:
```
ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("fr");
```

----------------------------------------

TITLE: Process FluentValidation Results and Display Errors
DESCRIPTION: Shows how to inspect the `ValidationResult` object after validation. It demonstrates checking `IsValid` and iterating through the `Errors` collection to print detailed messages for each validation failure to the console.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_4

LANGUAGE: C#
CODE:
```
using FluentValidation.Results; 

Customer customer = new Customer();
CustomerValidator validator = new Customer();

ValidationResult results = validator.Validate(customer);

if(! results.IsValid) 
{
  foreach(var failure in results.Errors)
  {
    Console.WriteLine("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
  }
}
```

----------------------------------------

TITLE: Chain Multiple FluentValidation Rules for a Property
DESCRIPTION: Demonstrates how to apply multiple validation rules sequentially to a single property. This example shows chaining `NotNull()` and `NotEqual("foo")` for the `Surname` property.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_6

LANGUAGE: C#
CODE:
```
using FluentValidation;

public class CustomerValidator : AbstractValidator<Customer>
{
  public CustomerValidator()
  {
    RuleFor(customer => customer.Surname).NotNull().NotEqual("foo");
  }
}
```

----------------------------------------

TITLE: Access Custom State from FluentValidation Results in C#
DESCRIPTION: This C# code snippet demonstrates how to execute a FluentValidation validator and then iterate through the resulting validation errors. It shows how to access the `CustomState` property on each `ValidationFailure` object, retrieving the custom data previously associated with the rule using `WithState`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-state.md#_snippet_1

LANGUAGE: csharp
CODE:
```
var validator = new PersonValidator();
var result = validator.Validate(new Person());
foreach (var failure in result.Errors) 
{
  Console.WriteLine($"Property: {failure.PropertyName} State: {failure.CustomState}");
}
```

----------------------------------------

TITLE: Configure Custom Display Name Resolver for FluentValidation
DESCRIPTION: This C# code snippet demonstrates how to configure a custom `DisplayNameResolver` for FluentValidation. It allows restoring the old behavior of inferring property names from `[Display]` attributes, which was removed in newer versions. This resolver retrieves the name from the `DisplayAttribute` if present.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-9.md#_snippet_5

LANGUAGE: csharp
CODE:
```
FluentValidation.ValidatorOptions.DisplayNameResolver = (type, memberInfo, expression) => {
	return memberInfo.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.GetName();
};
```

----------------------------------------

TITLE: Filter Auto-Registered FluentValidation Validators
DESCRIPTION: This C# example demonstrates how to apply a filter function during automatic validator registration to exclude specific validators. The provided lambda expression ensures that all validators in the assembly are registered with a `Scoped` lifetime, except for `CustomerValidator`, offering fine-grained control over which validators are included.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/di.md#_snippet_6

LANGUAGE: csharp
CODE:
```
services.AddValidatorsFromAssemblyContaining<MyValidator>(ServiceLifetime.Scoped, 
    filter => filter.ValidatorType != typeof(CustomerValidator));
```

----------------------------------------

TITLE: Replace AbstractValidator CascadeMode for Continue/Stop in FluentValidation 12.0
DESCRIPTION: This C# snippet shows how to update the deprecated `AbstractValidator.CascadeMode` property when its value was `Continue` or `Stop`. FluentValidation 12.0 replaces this with `ClassLevelCascadeMode` and `RuleLevelCascadeMode` for per-validator cascade configuration.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-12.md#_snippet_2

LANGUAGE: csharp
CODE:
```
ClassLevelCascadeMode = CascadeMode.<YourCurrentValue>;
RuleLevelCascadeMode = CascadeMode.<YourCurrentValue>;
```

----------------------------------------

TITLE: Update AbstractValidator CascadeMode to Rule/Class Level (Continue/Stop)
DESCRIPTION: Illustrates how to update the deprecated `AbstractValidator.CascadeMode` when set to `Continue` or `Stop` by using the new `ClassLevelCascadeMode` and `RuleLevelCascadeMode` properties within an `AbstractValidator`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-11.md#_snippet_2

LANGUAGE: csharp
CODE:
```
ClassLevelCascadeMode = CascadeMode.<YourCurrentValue>;
RuleLevelCascadeMode = CascadeMode.<YourCurrentValue>;
```

----------------------------------------

TITLE: Configure FluentValidation to Throw Exceptions via Options
DESCRIPTION: Presents an alternative, more explicit way to configure FluentValidation to throw an exception on validation failure by using the `options` lambda with `ThrowOnFailures()` during the `Validate` call.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/start.md#_snippet_9

LANGUAGE: C#
CODE:
```
validator.Validate(customer, options => options.ThrowOnFailures());

```

----------------------------------------

TITLE: FluentValidation Validate with IValidationContext
DESCRIPTION: Demonstrates the updated approach for validating models in FluentValidation 9.0 after the removal of the non-generic IValidator.Validate(object model) overload. Users should now pass an IValidationContext instance for type-safe validation.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-9.md#_snippet_1

LANGUAGE: C#
CODE:
```
var context = new ValidationContext<object>(model);
var result = validator.Validate(context);
```

----------------------------------------

TITLE: IStringSource.GetString Signature Update
DESCRIPTION: The signature of the `GetString` method within the `IStringSource` interface has been updated in FluentValidation 8.0. It now receives a `ValidationContext` instead of the model, requiring updates to custom `IStringSource` implementations.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-8.md#_snippet_6

LANGUAGE: APIDOC
CODE:
```
// FluentValidation 7.x (Conceptual API)
interface IStringSource {
  string GetString(object model);
}

// FluentValidation 8.0 (Conceptual API)
interface IStringSource {
  string GetString(ValidationContext context);
}
```

----------------------------------------

TITLE: FluentValidation Email Validation with Net4xRegex Mode
DESCRIPTION: Provides an example of how to explicitly opt-in to the legacy regular expression-based email validation in FluentValidation 9.0, using EmailValidationMode.Net4xRegex. This mode is deprecated and will issue a warning.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-9.md#_snippet_4

LANGUAGE: C#
CODE:
```
RuleFor(customer => customer.Email).EmailAddress(EmailValidationMode.Net4xRegex);
```

----------------------------------------

TITLE: Define FluentValidation Rules for Person Properties (C#)
DESCRIPTION: This C# code defines two separate FluentValidation validators, `PersonAgeValidator` and `PersonNameValidator`, both targeting the `Person` model. `PersonAgeValidator` includes a rule for `DateOfBirth`, while `PersonNameValidator` handles `Surname` and `Forename` length and null checks. This approach promotes separation of concerns in validation logic.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/including-rules.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class PersonAgeValidator : AbstractValidator<Person>  
{
  public PersonAgeValidator() 
  {
    RuleFor(x => x.DateOfBirth).Must(BeOver18);
  }

  protected bool BeOver18(DateTime date) 
  {
    //...
  }
}

public class PersonNameValidator : AbstractValidator<Person> 
{
  public PersonNameValidator() 
  {
    RuleFor(x => x.Surname).NotNull().Length(0, 255);
    RuleFor(x => x.Forename).NotNull().Length(0, 255);
  }
}
```

----------------------------------------

TITLE: Inline Property Transformation with FluentValidation
DESCRIPTION: This C# code demonstrates an inline transformation using FluentValidation's `Transform` method. It converts a string property (`SomeStringProperty`) to a nullable integer, returning `null` if conversion fails, and then applies a `GreaterThan(10)` validation rule to the transformed value.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/transform.md#_snippet_0

LANGUAGE: csharp
CODE:
```
Transform(from: x => x.SomeStringProperty, to: value => int.TryParse(value, out int val) ? (int?) val : null)
    .GreaterThan(10);
```

----------------------------------------

TITLE: Define Basic FluentValidation Person Validator
DESCRIPTION: This C# code defines a simple `PersonValidator` class that inherits from `AbstractValidator<Person>`. It includes two basic validation rules: ensuring `Surname` and `Forename` are not null. By default, failures for these rules will have an 'Error' severity.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/severity.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person>
{
  public PersonValidator()
  {
    RuleFor(person => person.Surname).NotNull();
    RuleFor(person => person.Forename).NotNull();
  }
}
```

----------------------------------------

TITLE: Chaining Conditions with CurrentValidator in FluentValidation
DESCRIPTION: Demonstrates a complex chain of validators where `ApplyConditionTo.CurrentValidator` is used for multiple conditions. Each `When` clause applies only to its immediate preceding validator, allowing fine-grained control over conditional rule application.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/conditions.md#_snippet_4

LANGUAGE: csharp
CODE:
```
RuleFor(customer => customer.Photo)
    .NotEmpty()
    .Matches("https://wwww.photos.io/\d+\.png")
    .When(customer => customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator)
    .Empty()
    .When(customer => ! customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator);
```

----------------------------------------

TITLE: Apply SetValidator to each item in a complex type collection
DESCRIPTION: Demonstrates how to use `RuleForEach` in conjunction with `SetValidator` to apply a dedicated `OrderValidator` to each `Order` object within the `Orders` collection of a `Customer`. This is the standard way to validate complex objects within a collection.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/collections.md#_snippet_4

LANGUAGE: csharp
CODE:
```
public class OrderValidator : AbstractValidator<Order> 
{
  public OrderValidator() 
  {
    RuleFor(x => x.Total).GreaterThan(0);
  }
}

public class CustomerValidator : AbstractValidator<Customer> 
{
  public CustomerValidator() 
  {
    RuleForEach(x => x.Orders).SetValidator(new OrderValidator());
  }
}
```

----------------------------------------

TITLE: Deprecated InjectValidator Usage in FluentValidation 11.x
DESCRIPTION: This C# snippet demonstrates the deprecated `InjectValidator` method used in FluentValidation 11.x. This method implicitly injected a child validator from the ASP.NET Service Provider, primarily for ASP.NET MVC's auto-validation feature. It has been removed in FluentValidation 12.0.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-12.md#_snippet_4

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator() 
  {
    RuleFor(x => x.Address).InjectValidator();
  }
}
```

----------------------------------------

TITLE: Override Display Property Name with Static String
DESCRIPTION: Shows how to use `WithName` to change the display name of a property in the validation error message. This affects only the message, not the underlying property name in the `Errors` collection.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/configuring.md#_snippet_5

LANGUAGE: C#
CODE:
```
RuleFor(customer => customer.Surname).NotNull().WithName("Last name");
```

----------------------------------------

TITLE: Define Custom Validator Class in FluentValidation
DESCRIPTION: This C# code defines a `PersonValidator` class inheriting from `AbstractValidator<Person>`. It demonstrates how to use the `Custom` method within a `RuleFor` definition to implement a custom validation logic for the `Pets` property, adding a validation failure if the list count exceeds 10.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_6

LANGUAGE: csharp
CODE:
```
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
   RuleFor(x => x.Pets).Custom((list, context) => {
     if(list.Count > 10) {
       context.AddFailure("The list must contain 10 items or fewer");
     }
   });
  }
}
```

----------------------------------------

TITLE: Define Dependent Rules in FluentValidation
DESCRIPTION: This C# example demonstrates how to use `DependentRules` in FluentValidation. The `RuleFor(x => x.Forename).NotNull()` rule will only be executed if the preceding `RuleFor(x => x.Surname).NotNull()` rule passes, ensuring conditional validation flow.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/dependentrules.md#_snippet_0

LANGUAGE: C#
CODE:
```
RuleFor(x => x.Surname).NotNull().DependentRules(() => {
  RuleFor(x => x.Forename).NotNull();
});
```

----------------------------------------

TITLE: Old MessageBuilder Chaining Pattern
DESCRIPTION: Shows the previous pattern for chaining `MessageBuilder` instances, which allowed custom logic to be applied before or after the original message builder. This pattern is no longer supported in FluentValidation 11.0.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/upgrading-to-11.md#_snippet_5

LANGUAGE: csharp
CODE:
```
return ruleBuilder.Configure(rule => {
  var originalMessageBuilder = rule.MessageBuilder;
  rule.MessageBuilder = context => {
    
    // ... some custom logic in here.
    
    return originalMessageBuilder?.Invoke(context) ?? context.GetDefaultMessage();
  };
});
```

----------------------------------------

TITLE: Define a class with a simple type collection
DESCRIPTION: Defines a `Person` class containing a `List<string>` named `AddressLines` to represent a collection of simple string types, which can then be targeted by FluentValidation rules.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/collections.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class Person 
{
  public List<string> AddressLines { get; set; } = new List<string>();
}
```

----------------------------------------

TITLE: Override Validation Message with PropertyName Placeholder
DESCRIPTION: Shows how to include the `{PropertyName}` placeholder in a custom error message. FluentValidation will automatically replace this placeholder with the name of the validated property at runtime.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/configuring.md#_snippet_1

LANGUAGE: C#
CODE:
```
RuleFor(customer => customer.Surname).NotNull().WithMessage("Please ensure you have entered your {PropertyName}");
```

----------------------------------------

TITLE: Define Person Class with Pet List Property
DESCRIPTION: Defines a simple C# class `Person` that includes an `IList<Pet>` property named `Pets`, initialized as an empty list. This class serves as the target for validation examples.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_0

LANGUAGE: csharp
CODE:
```
public class Person {
  public IList<Pet> Pets {get;set;} = new List<Pet>();
}
```

----------------------------------------

TITLE: Manually Register FluentValidation Validator in .NET Startup
DESCRIPTION: This C# code demonstrates how to manually register a `UserValidator` with the .NET dependency injection container in the `ConfigureServices` method of a `Startup` class. The validator is registered as `IValidator<User>` with a `Scoped` lifetime, making it available for injection throughout the application.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/di.md#_snippet_1

LANGUAGE: csharp
CODE:
```
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddScoped<IValidator<User>, UserValidator>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      // ...
    }
}
```

----------------------------------------

TITLE: Inject and Use FluentValidation Validator in a Service
DESCRIPTION: This C# example illustrates how to inject an `IValidator<User>` into a `UserService` class via its constructor. The injected validator is then used asynchronously to validate a `User` object, demonstrating how to trigger validation within application logic.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/di.md#_snippet_2

LANGUAGE: csharp
CODE:
```
public class UserService
{
    private readonly IValidator<User> _validator;

    public UserService(IValidator<User> validator)
    {
        _validator = validator;
    }

    public async Task DoSomething(User user)
    {
        var validationResult = await _validator.ValidateAsync(user);
    }
}
```

----------------------------------------

TITLE: Implement a Simple NotNull Property Validator
DESCRIPTION: This C# class provides a simpler example of a custom `PropertyValidator` implementation, mirroring FluentValidation's built-in `NotNull` validator. It checks if a property's value is not null and provides a default error message.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/custom-validators.md#_snippet_13

LANGUAGE: csharp
CODE:
```
public class NotNullValidator<T,TProperty> : PropertyValidator<T,TProperty> {

  public override string Name => "NotNullValidator";

  public override bool IsValid(ValidationContext<T> context, TProperty value) {
    return value != null;
  }

  protected override string GetDefaultMessageTemplate(string errorCode)
    => "'{PropertyName}' must not be empty.";

}
```

----------------------------------------

TITLE: Apply Enum Name Validation Rule (C#)
DESCRIPTION: Checks whether a string property's value is a valid enum name. It supports both case-sensitive and case-insensitive comparisons. Error messages can be formatted using `{PropertyName}`, `{PropertyValue}`, and `{PropertyPath}`.
SOURCE: https://github.com/fluentvalidation/fluentvalidation/blob/main/docs/built-in-validators.md#_snippet_16

LANGUAGE: csharp
CODE:
```
// For a case sensitive comparison
RuleFor(x => x.ErrorLevelName).IsEnumName(typeof(ErrorLevel));

// For a case-insensitive comparison
RuleFor(x => x.ErrorLevelName).IsEnumName(typeof(ErrorLevel), caseSensitive: false);
```