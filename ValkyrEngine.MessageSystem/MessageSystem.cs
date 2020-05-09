using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ValkyrEngine.MessageSystem.Exceptions;

namespace ValkyrEngine.MessageSystem
{
  /// <summary>
  /// Provides a base class for handling messages andreceiver. 
  /// </summary>
  public class MessageSystem : IMessageSystem, IDisposable
  {
    private readonly List<IMessageHandler> receiver = new List<IMessageHandler>();
    private readonly CancellationTokenSource cts = new CancellationTokenSource();
    private readonly ConcurrentQueue<IMessage> messages = new ConcurrentQueue<IMessage>();
    private readonly MessageHandlerFactory receiverFactory = new MessageHandlerFactory();

    /// <inheritdoc/>
    public bool Active { get; private set; }
    /// <inheritdoc/>
    public IReadOnlyList<IMessageHandler> Receiver => receiver;
    /// <inheritdoc/>
    public IReadOnlyCollection<IMessage> ActiveMessages => messages;

    /// <inheritdoc/>
    public void RegisterReceiver<T>(Func<T, Task> callback)
      where T : IMessage
    {
      if (callback == null)
        throw new ArgumentNullException(nameof(callback));

      if (FindMessageHandlerWithCallback(callback) != null)
        throw new RegistrationException($"Handler with callback '{callback.Method.Name}' is already registered");

      receiver.Add(receiverFactory.CreateMessageHandler(callback));
    }
    /// <inheritdoc/>
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
    /// <inheritdoc/>
    public void SendMessage<T>(T message)
      where T : IMessage
    {
      if (message == null)
        throw new ArgumentNullException(nameof(message));

      messages.Enqueue(message);
    }
    /// <inheritdoc/>
    public void Activate()
    {
      Active = true;
      ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessMessagesAsync), cts.Token);
    }
    /// <inheritdoc/>
    public void Deactivate()
    {
      Active = false;
      cts.Cancel();
    }
    /// <inheritdoc/>
    public void Dispose()
    {
      cts.Dispose();
    }

    private async void ProcessMessagesAsync(object obj)
    {
      CancellationTokenSource token = (CancellationTokenSource)obj;

      while (messages.TryDequeue(out IMessage message))
      {
        if (token.IsCancellationRequested)
        {
          await ProcessMessageAsync(message);
        }
      }
    }

    private async Task ProcessMessageAsync(IMessage message)
    {
      List<Task> runningReceiverTasks = new List<Task>();
      foreach (IMessageHandler handler in receiver)
      {
        if (handler.CanHandle(message))
        {
          runningReceiverTasks.Add(handler.HandleMessageAsync(message));
        }
      }
      await Task.WhenAll(runningReceiverTasks);
    }

    private IMessageHandler FindMessageHandlerWithCallback<T>(Func<T, Task> callback)
      where T : IMessage
    {
      return receiver.Find((receiver) => ((MessageHandler<T>)receiver).HasAction(callback));
    }
  }
}
