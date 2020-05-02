using System;
using System.Threading.Tasks;

namespace ValkyrEngine.MessageSystem
{
  internal class MessageHandler<T> : IMessageHandler
    where T : IMessage
  {
    private readonly Func<T, Task> callback;

    public MessageHandler(Func<T, Task> callback)
    {
      this.callback = callback;
    }

    public bool CanHandle(IMessage message)
    {
      if (message == null)
        return false;

      return typeof(T).Equals(message.GetType());
    }

    public async Task HandleMessageAsync(IMessage message)
    {
      if (message == null)
        throw new ArgumentNullException(nameof(message));

      await callback.Invoke((T)message);
    }

    internal bool HasAction(Func<T, Task> callback)
    {
      return this.callback.Equals(callback);
    }
  }
}
