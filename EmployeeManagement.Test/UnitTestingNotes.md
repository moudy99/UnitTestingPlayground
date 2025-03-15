# UnitTestingNotes

Welcome to my unit testing notes for .NET projects! This is a comprehensive guide where I’ve compiled everything I’ve learned—naming conventions, best practices, tools, and more.
---

## Naming Conventions

This section covers how to name things in unit tests so they’re consistent, readable, and make sense to anyone reviewing them.

### Test Project Naming
- **Rule**: Name the test project after the project it tests, appending `.Test`.  
  - Example: `Farah.Domain` → `Farah.Domain.Test`  
- **Why**: Links tests directly to their production code, keeping the solution organized.

**Best Practice**:  
- Group test projects in a `Tests` folder (e.g., `Tests/Farah.Domain.Test`) for a cleaner structure in larger solutions.

### Test Class Naming
- **Rule**: Name the test class after the class under test, appending `Tests`.  
  - Example: `EmployeeFactory` → `EmployeeFactoryTests`  
- **Why**: Clearly indicates what’s being tested.

**Best Practice**:  
- For complex classes, split tests into multiple focused test classes (e.g., `EmployeeFactoryCreationTests` and `EmployeeFactoryValidationTests`).

### Test Method Naming
- **Rule**: Use a three-part naming structure:  
  1. **Method Name**: The method being tested (e.g., `CreateEmployee`).  
  2. **Scenario**: The condition or input being tested (e.g., `WithOnlyFirstNameAndLastName`).  
  3. **Expected Outcome**: The expected result (e.g., `CreatesEmployeeWith2500Salary`).  
  - Example: `CreateEmployee_WithOnlyFirstNameAndLastName_CreatesEmployeeWith2500Salary`  

**Best Practice**:  
- For tests with multiple scenarios, use parameterized tests with attributes like `[TestCase]` in NUnit:  
  ```csharp
  [TestCase("John", "Doe", 2500)]
  [TestCase("Jane", "Smith", 3000)]
  public void CreateEmployee_WithValidInput_SetsExpectedSalary(string firstName, string lastName, int expectedSalary)
  {
      // Test logic here
  }
  ```
  This reduces repetition and keeps your test suite DRY (Don’t Repeat Yourself) while covering multiple cases efficiently.

## Subject Under Test (SUT)
- **Rule**: Name the instance of the class being tested `sut` (Subject Under Test).
- **Example:**
  ```csharp
  private readonly EmployeeFactory sut = new EmployeeFactory();
  ```
- **Why**: This is a widely recognized convention that instantly identifies the object under test.

**Best Practice**:
- Declare `sut` as `readonly` to avoid accidental changes during tests.
- For complex setups (e.g., with dependencies), initialize `sut` in a setup method:
  ```csharp
  private EmployeeFactory sut;

  [SetUp]
  public void Setup()
  {
      sut = new EmployeeFactory(new Mock<IDependency>().Object);
  }
  ```

## Best Practices

### Arrange-Act-Assert (AAA)
- **Structure**: Divide each test into three clear sections:
  1. **Arrange**: Prepare the test data and the SUT.
  2. **Act**: Call the method being tested.
  3. **Assert**: Check the result.

- **Example:**
  ```csharp
  [Test]
  public void CreateEmployee_WithOnlyFirstNameAndLastName_CreatesEmployeeWith2500Salary()
  {
      // Arrange
      var sut = new EmployeeFactory();
      var firstName = "John";
      var lastName = "Doe";

      // Act
      var employee = sut.CreateEmployee(firstName, lastName);

      // Assert
      Assert.That(employee.Salary, Is.EqualTo(2500));
      Assert.That(employee.Type, Is.EqualTo("Internal"));
  }
  ```

### Single Responsibility
- **Rule**: Each test should verify one logical behavior or outcome.
- **Example**: Test `employee.Salary` in one test and `employee.Type` in another.
- **Why**: Focused tests are easier to understand and pinpoint failures quickly.

### Mocking
- **Tip**: Use mocking libraries (e.g., Moq) to simulate external dependencies like databases or APIs.
- **Example:**
  ```csharp
  var mockDependency = new Mock<IDependency>();
  var sut = new EmployeeFactory(mockDependency.Object);
  ```
- **Why**: Keeps tests fast, isolated, and focused on the SUT’s logic.

## Tools and Resources
A quick reference for tools and learning materials to level up your unit testing skills.

| Category     | Details                         |
|-------------|---------------------------------|
| Frameworks  | NUnit, MSTest, xUnit           |
| Mocking     | Moq, NSubstitute               |
| Reading     | *The Art of Unit Testing* by Roy Osherove |

- **Frameworks**: NUnit’s flexibility (e.g., `[TestCase]`) makes it a great choice, but MSTest and xUnit are also solid.
- **Mocking**: Moq is widely used for faking dependencies; NSubstitute is another good option.
- **Reading**: *The Art of Unit Testing* is an excellent book for deeper insights into unit testing.

---

## Caveats

This section highlights common pitfalls and tricky scenarios in unit testing, along with how to handle them effectively.

### Testing Asynchronous Methods

- **Rule**: Use `async Task` for test methods that test asynchronous code, and await the SUT’s async methods.
- **Example**:
  
  ```csharp
  [Test]
  public async Task CreateEmployeeAsync_WithValidData_CreatesEmployeeSuccessfully()
  {
      // Arrange
      var sut = new EmployeeFactory();
      var firstName = "John";
      var lastName = "Doe";

      // Act
      var employee = await sut.CreateEmployeeAsync(firstName, lastName);

      // Assert
      Assert.That(employee, Is.Not.Null);
      Assert.That(employee.Salary, Is.EqualTo(2500));
  }
  ```
  
- **Why**: Async methods return a Task, and you need to await them to get the result or catch exceptions properly.
- **Best Practice**:
  - Always return Task (not void) for async tests to ensure the test framework (e.g., NUnit) waits for completion.
  - Use `Assert.ThrowsAsync` (or equivalent) to test exceptions in async code.
  - Consider using `Task.CompletedTask` for methods that don’t return meaningful data.
  - Avoid `ConfigureAwait(false)` in unit tests—it’s mainly useful for library code.
  - Implement timeouts (`Task.Delay` or `CancellationTokenSource`) to prevent hanging tests.

### Handling Exceptions

- **Rule**: Use assertion methods like `Assert.Throws` or `Assert.ThrowsAsync` to verify that the SUT throws expected exceptions.
- **Example (Synchronous)**:
  
  ```csharp
  [Test]
  public void CreateEmployee_WithEmptyName_ThrowsArgumentException()
  {
      // Arrange
      var sut = new EmployeeFactory();

      // Act & Assert
      var exception = Assert.Throws<ArgumentException>(() => sut.CreateEmployee("", "Doe"));
      Assert.That(exception.Message, Contains.Substring("Name cannot be empty"));
  }
  ```
  
- **Example (Asynchronous)**:
  
  ```csharp
  [Test]
  public async Task CreateEmployeeAsync_WithEmptyName_ThrowsArgumentException()
  {
      // Arrange
      var sut = new EmployeeFactory();

      // Act & Assert
      var exception = await Assert.ThrowsAsync<ArgumentException>(
          async () => await sut.CreateEmployeeAsync("", "Doe"));
      Assert.That(exception.Message, Contains.Substring("Name cannot be empty"));
  }
  ```
  
- **Why**: Exceptions are a key part of behavior testing—sometimes you want your code to fail in specific ways.
- **Best Practice**:
  - Be specific with exception types (e.g., `ArgumentException` instead of just `Exception`) to avoid masking unexpected errors.
  - Test the exception message or properties if they’re part of the expected behavior to make tests more robust.
  - Use `Assert.Catch<T>()` when you expect multiple exception types but still want to verify the thrown exception.
  - For custom exceptions, ensure they include meaningful messages and properties to validate in tests.
  - Implement a global test exception handler to catch unexpected failures and log debugging details.
 
