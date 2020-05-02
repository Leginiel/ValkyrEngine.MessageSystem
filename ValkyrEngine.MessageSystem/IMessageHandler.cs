using System.Threading.Tasks;

namespace ValkyrEngine.MessageSystem
{
  /// <summary>
  /// Represents an object, that is capable of handling a specified message.
  /// </summary>
  public interface IMessageHandler
  {
    /// <summary>
    /// Handles the specified message as an asynchronous operation. 
    /// </summary>
    /// <param name="message">Specified message, that should be handled.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task HandleMessageAsync(IMessage message);
    /// <summary>
    /// Determines, if this handler can handle the given message.
    /// </summary>
    /// <param name="message">Specified message, that should be handled.</param>
    /// <returns><code>True</code> if the handle is able to handle the message, otherwise <code>False</code>.</returns>
    bool CanHandle(IMessage message);
  }
}
