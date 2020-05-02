using System;

namespace ValkyrEngine.MessageSystem.Exceptions
{
  /// <summary>
  /// Represents exceptions, that occur during registration on the message system.
  /// </summary>
  public class RegistrationException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <seealso cref="RegistrationException"/> class, with a specified error message.
    /// </summary>
    /// <param name="message">The message, that describes the error.</param>
    public RegistrationException(string message)
      : base(message) { }
  }
}
