# AuthHttpHandler Tests - Important Notes

## Limitation: SecureStorage Dependency

The `AuthHttpHandler` class has a direct dependency on `Microsoft.Maui.Storage.SecureStorage`, which is a platform-specific API that **cannot be unit tested** in the standard .NET test environment (net9.0).

### The Problem

When running tests, you'll encounter:
```
Microsoft.Maui.ApplicationModel.NotImplementedInReferenceAssemblyException: 
This functionality is not implemented in the portable version of this assembly.
```

This happens because:
1. `SecureStorage` is platform-specific (Android/iOS/Windows)
2. Unit tests run on portable net9.0
3. The handler directly calls `SecureStorage.GetAsync()` which throws in test environments

### Current Test Status

✅ **Tests that work:**
- Constructor tests
- Authentication endpoint tests (don't trigger SecureStorage)

❌ **Tests that fail:**
- Non-auth endpoint tests (all trigger SecureStorage access)
- Token refresh tests
- HTTP method tests

### Recommended Solutions

#### Option 1: Refactor to Use ISecureStorage Abstraction (Recommended)

Create an `ISecureStorage` interface to abstract the storage layer:

```csharp
public interface ISecureStorage
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    bool Remove(string key);
}

public class SecureStorageWrapper : ISecureStorage
{
    public Task<string?> GetAsync(string key) => SecureStorage.GetAsync(key);
    public Task SetAsync(string key, string value) => SecureStorage.SetAsync(key, value);
    public bool Remove(string key) => SecureStorage.Remove(key);
}
```

Then inject `ISecureStorage` into `AuthHttpHandler` and mock it in tests.

**Benefits:**
- Fully testable
- Better separation of concerns
- Follows SOLID principles

#### Option 2: Integration Tests Only

Accept that `AuthHttpHandler` requires integration testing on actual platforms:
- Test on Android/iOS emulators
- Use UI testing frameworks (Appium, etc.)
- Manual testing

**Benefits:**
- No refactoring needed
- Tests real behavior

**Drawbacks:**
- Slower tests
- Requires emulators/devices
- More complex setup

#### Option 3: Conditional Test Execution

Keep the current tests but mark them to skip in standard unit test runs:

```csharp
[TestMethod]
[Ignore("Requires platform-specific SecureStorage - run as integration test")]
public async Task SendAsync_NonAuthEndpoint_SuccessResponse_ReturnsResponse()
{
    // Test code...
}
```

### Current Implementation

The current `AuthHttpHandlerTests.cs` file contains comprehensive test cases that demonstrate:
- What **should** be tested
- Proper test structure
- Mock setup patterns

These tests serve as:
1. **Documentation** of expected behavior
2. **Template** for future tests after refactoring
3. **Example** of proper test structure

### Action Items

1. ✅ Tests created and demonstrate proper structure
2. ⚠️ Tests document the SecureStorage limitation
3. 🔄 **Next step:** Decide on refactoring approach (recommend Option 1)
4. 🔄 Implement `ISecureStorage` abstraction
5. 🔄 Update `AuthHttpHandler` to use `ISecureStorage`
6. 🔄 Update tests to mock `ISecureStorage`

## Related Files

- Implementation: [AuthHttpHandler.cs](../../CameraApp/Services/AuthHttpHandler.cs)
- Tests: [AuthHttpHandlerTests.cs](AuthHttpHandlerTests.cs)
- Interface: [IAuthService.cs](../../CameraApp/Services/IAuthService.cs)

## Testing Without Refactoring

If you need to test the current implementation without refactoring:

1. **Test on Android:**
   ```bash
   dotnet build -t:Run -f net9.0-android
   ```

2. **Use UI Tests with Real Device/Emulator**

3. **Manual Testing** through the app UI

## Summary

The tests in `AuthHttpHandlerTests.cs` are:
- ✅ **Syntactically correct**
- ✅ **Well-structured**
- ✅ **Comprehensive**
- ❌ **Cannot run** due to platform dependency
- 📝 **Serve as documentation** and template

**Recommendation:** Implement Option 1 (ISecureStorage abstraction) to make the code fully testable.
