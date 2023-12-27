namespace ConfigLimitFixer;

public static class ThreadUtil
{
    /// <summary>
    /// Validates the thread name.
    /// </summary>
    /// <param name="threadName">
    /// The thread name.
    /// </param>
    /// <returns>
    /// <c>true</c> if the thread name is valid; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Validation rule for Thread.Name is as follows:
    /// - The name can be set only once, and cannot be set to null.
    /// - The name can be a zero-length string.
    /// - The name can contain a maximum of 255 characters.
    /// - The name cannot contain control characters (\0, \n, \r).
    /// </remarks>
    public static bool ValidateThreadName(string threadName)
    {
        // Validation rule for Thread.Name
        // https://docs.microsoft.com/en-us/dotnet/api/system.threading.thread.name?view=netframework-4.8#remarks

        return threadName == null || threadName.Length > 255
            ? false
            : threadName.IndexOfAny(new char[] { '\0', '\n', '\r' }) == -1;
    }
}
