using Shouldly;
using Xunit;

namespace ConfigLimitFixer.Tests;

public class ThreadUtilTests
{
    [Theory]
    [InlineData("ValidThreadName", true)]
    [InlineData(null, false)]
    [InlineData("", true)]
    [InlineData("A", true)]
    [InlineData("Invalid\nName", false)]
    public void ValidateThreadName(string threadName, bool expectedIsValid)
    {
        // Act
        var isValid = ThreadUtil.ValidateThreadName(threadName);

        // Assert
        isValid.ShouldBe(expectedIsValid);
    }
}
