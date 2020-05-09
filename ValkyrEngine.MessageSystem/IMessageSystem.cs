using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ValkyrEngine.MessageSystem
{
  /// <summary>
  /// Represents an object, that is providing a messaging system.
  /// </summary>
  public interface IMessageSystem : IDisposable
  {
    /// <summary>
    /// Returns <code>True</code> if the messaging system is active, otherwise <code>False</code>.
    /// </summary>
    bool Active { get; }

    /// <summary>
    /// Retuns the pending messages.
    /// </summary>
    /// <remarks>
    /// Pending messages will be processed next.
    /// </remarks>
    IReadOnlyCollection<IMessage> ActiveMessages { get; }
    /// <summary>
    /// Returns the registered receiver.
    /// </summary>
    IReadOnlyList<IMessageHandler> Receiver { get; }
    /// <summary>
    /// Adds a new message to the messaging system. 
    /// </summary>
    /// <typeparam name="T">Type of the message.</typeparam>
    /// <param name="message">Message, that should be added.</param>
    void SendMessage<T>(T message) where T : IMessage;
    /// <summary>
    /// Registeres a new receiver callback. 
    /// </summary>
    /// <typeparam name="T">Type of message, that the receiver recognizes.</typeparam>
    /// <param name="callback">Callback, that should be called for the message.</param>
    void RegisterReceiver<T>(Func<T, Task> callback) where T : IMessage;
    /// <summary>
    /// Removes an existing receier from the messaging system.
    /// </summary>
    /// <typeparam name="T">Type of message, that the receiver recognizes.</typeparam>
    /// <param name="callback">Callback, with whon the receiver was registered.</param>
    void UnregisterReceiver<T>(Func<T, Task> callback) where T : IMessage;
    /// <summary>
    /// Activates the processing capabilities of the message system.
    /// </summary>
    void Activate();
    /// <summary>
    /// Shuts the messagesystem down and stops processing messages.
    /// </summary>
    void Deactivate();
  }
}