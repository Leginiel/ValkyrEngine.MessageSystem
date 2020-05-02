using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ValkyrEngine.MessageSystem
{
  /// <summary>
  /// Represents an object, that is providing a messaging system.
  /// </summary>
  public interface IMessageSystem
  {
    /// <summary>
    /// Retuns the pending messages.
    /// </summary>
    /// <remarks>
    /// Pending messages will be processed by the next <seealso cref="ProcessMessagesAsync"/> call.
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
    /// Processes all pending messages with an asynchronous operation.
    /// </summary>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task ProcessMessagesAsync();
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
  }
}