using System;
using System.Threading.Tasks;
using Xunit;

namespace ValkyrEngine.MessageQueue.Tests
{
  public class MessageHandlerFactoryTest
  {
    [Fact]
    [Trait("Category", "Unit")]
    public void CreateMessageHandler_ValidCallback_IMessageHandler()
    {
      // Arrange
      MessageHandlerFactory factory = new MessageHandlerFactory();
      IMessageHandler messageHandler;
      // Act
      messageHandler = factory.CreateMessageHandler<IMessage>(HandleMessage);

      // Assert
      Assert.NotNull(messageHandler);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CreateMessageHandler_Null_ArgumentNullException()
    {
      // Arrange
      MessageHandlerFactory factory = new MessageHandlerFactory();

      // Act / Assert

      Assert.Throws<ArgumentNullException>(() => factory.CreateMessageHandler<IMessage>(null));
    }

    private Task HandleMessage(IMessage obj)
    {
      return new Task(() => { });
    }
  }
}
