using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ValkyrEngine.MessageQueue.Exceptions;

namespace ValkyrEngine.MessageQueue
{
  /// <summary>
  /// Provides a base class for handling messages andreceiver. 
  /// </summary>
  public class MessageSystem
  {
    private readonly List<IMessageHandler> receiver = new List<IMessageHandler>();
    private readonly List<Task> runningReceiverTasks = new List<Task>();
    private readonly ConcurrentQueue<IMessage> messages = new ConcurrentQueue<IMessage>();
    private readonly MessageHandlerFactory receiverFactory = new MessageHandlerFactory();

    /// <summary>
    /// Returns the registered receiver.
    /// </summary>
    public IReadOnlyList<IMessageHandler> Receiver => receiver;
    /// <summary>
    /// Retuns the pending messages.
    /// </summary>
    /// <remarks>
    /// Pending messages will be processed by the next <seealso cref="ProcessMessagesAsync"/> call.
    /// </remarks>
    public IReadOnlyCollection<IMessage> ActiveMessages => messages;

    /// <summary>
    /// Registeres a new receiver callback. 
    /// </summary>
    /// <typeparam name="T">Type of message, that the receiver recognizes.</typeparam>
    /// <param name="callback">Callback, that should be called for the message.</param>
    public void RegisterReceiver<T>(Func<T, Task> callback)
      where T : IMessage
    {
      if (callback == null)
        throw new ArgumentNullException(nameof(callback));

      if (FindMessageHandlerWithCallback(callback) != null)
        throw new RegistrationException($"Handler with callback '{callback.Method.Name}' is already registered");

      receiver.Add(receiverFactory.CreateMessageHandler(callback));
    }
    /// <summary>
    /// Removes an existing receier from the messaging system.
    /// </summary>
    /// <typeparam name="T">Type of message, that the receiver recognizes.</typeparam>
    /// <param name="callback">Callback, with whon the receiver was registered.</param>
    public void UnregisterReceiver<T>(Func<T, Task> callback)
      where T : IMessage
    {
      if (callback == null)
        throw new ArgumentNullException(nameof(callback));

      IMessageHandler handler = FindMessageHandlerWithCallback(callback);

      if (handler == null)
        throw new RegistrationException($"There is no handler with callback '{callback.Method.Name}' registered");

      receiver.Remove(handler);
    }
    /// <summary>
    /// Adds a new message to the messaging system. 
    /// </summary>
    /// <typeparam name="T">Type of the message.</typeparam>
    /// <param name="message">Message, that should be added.</param>
    public void SendMessage<T>(T message)
      where T : IMessage
    {
      if (message == null)
        throw new ArgumentNullException(nameof(message));

      messages.Enqueue(message);
    }
    /// <summary>
    /// Processes all pending messages with an asynchronous operation.
    /// </summary>
    /// <returns></returns>
    public async Task ProcessMessagesAsync()
    {
      runningReceiverTasks.Clear();

      while (messages.TryDequeue(out IMessage message))
      {
        foreach (IMessageHandler handler in receiver)
        {
          if (handler.CanHandle(message))
          {
            runningReceiverTasks.Add(handler.HandleMessageAsync(message));
          }
        }
      }

      await Task.WhenAll(runningReceiverTasks);
    }

    private IMessageHandler FindMessageHandlerWithCallback<T>(Func<T, Task> callback)
      where T : IMessage
    {
      return receiver.Find((receiver) =>
                            receiver is MessageHandler<T>
                            && ((MessageHandler<T>)receiver).HasAction(callback));

    }
  }
}
