using System;

namespace ValkyrEngine.MessageQueue.Exceptions
{
  public class RegistrationException : Exception
  {
    public RegistrationException(string message)
      : base(message) { }
  }
}
