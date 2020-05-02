using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ValkyrEngine.MessageQueue.Exceptions;

namespace ValkyrEngine.MessageQueue
{
  public class MessageSystem
  {
    private readonly List<IMessageHandler> receiver = new List<IMessageHandler>();
    private readonly List<Task> runningReceiverTasks = new List<Task>();
    private readonly ConcurrentQueue<IMessage> messages = new ConcurrentQueue<IMessage>();
    private readonly MessageHandlerFactory receiverFactory = new MessageHandlerFactory();

    public IReadOnlyList<IMessageHandler> Receiver => receiver;
    public IReadOnlyCollection<IMessage> ActiveMessages => messages;


    public void RegisterReceiver<T>(Func<T, Task> callback)
      where T : IMessage
    {
      if (callback == null)
        throw new ArgumentNullException(nameof(callback));

      if (FindMessageHandlerWithCallback(callback) != null)
        throw new RegistrationException($"Handler with callback '{callback.Method.Name}' is already registered");

      receiver.Add(receiverFactory.CreateMessageHandler(callback));
    }

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

    public void SendMessage<T>(T message)
      where T : IMessage
    {
      if (message == null)
        throw new ArgumentNullException(nameof(message));

      messages.Enqueue(message);
    }
    public async Task ProcessMessages()
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
