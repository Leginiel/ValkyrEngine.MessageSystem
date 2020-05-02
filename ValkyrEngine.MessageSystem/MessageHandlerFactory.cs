using System;
using System.Threading.Tasks;

namespace ValkyrEngine.MessageSystem
{
  internal sealed class MessageHandlerFactory
  {
    public IMessageHandler CreateMessageHandler<T>(Func<T, Task> callback)
      where T : IMessage
    {
      if (callback == null)
        throw new ArgumentNullException(nameof(callback));

      return new MessageHandler<T>(callback);
    }
  }
}
