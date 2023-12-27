using System;

namespace ConfigLimitFixer;

/// <summary>
/// Represents the situation that a method is called twice or more, while the method is not reentrant.
/// </summary>
[Serializable]
public class ReentranceForbiddenException : InvalidOperationException
{
    public ReentranceForbiddenException()
    {
    }
    public ReentranceForbiddenException(string message) : base(message) { }
    public ReentranceForbiddenException(string message, Exception inner) : base(message, inner) { }
    protected ReentranceForbiddenException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
