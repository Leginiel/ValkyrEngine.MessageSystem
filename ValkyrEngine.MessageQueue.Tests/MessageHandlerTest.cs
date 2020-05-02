using System;
using System.Threading.Tasks;
using ValkyrEngine.MessageQueue.Tests.Helper;
using Xunit;

namespace ValkyrEngine.MessageQueue.Tests
{
  public class MessageHandlerTest
  {
    private bool genericCallbackCalled = false;
    private bool callbackCalled = false;


    [Fact]
    [Trait("Category", "Unit")]
    public void CanHandle_ValidMessage_True()
    {
      // Arrange
      MessageHandler<UnitTestMessage> messageHandler = new MessageHandler<UnitTestMessage>(HandleMessage);
      UnitTestMessage message = new UnitTestMessage();
      bool result;
      // Act
      result = messageHandler.CanHandle(message);

      // Assert
      Assert.True(result);
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void CanHandle_NotValidMessage_False()
    {
      // Arrange
      MessageHandler<IMessage> messageHandler = new MessageHandler<IMessage>(HandleMessage);
      UnitTestMessage message = new UnitTestMessage();
      bool result;
      // Act
      result = messageHandler.CanHandle(message);

      // Assert
      Assert.False(result);
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void CanHandle_Null_False()
    {
      // Arrange
      MessageHandler<UnitTestMessage> messageHandler = new MessageHandler<UnitTestMessage>(HandleMessage);
      bool result;
      // Act
      result = messageHandler.CanHandle(null);

      // Assert
      Assert.False(result);
    }
    [Fact]
    [Trait("Category", "Unit")]
    public async void HandleMessageAsync_ValidMessage_Successful()
    {
      // Arrange
      MessageHandler<UnitTestMessage> messageHandler = new MessageHandler<UnitTestMessage>(HandleMessage);
      UnitTestMessage message = new UnitTestMessage();

      // Act
      await messageHandler.HandleMessageAsync(message);

      // Assert
      Assert.True(genericCallbackCalled);
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void HandleMessageAsync_Null_ArgumentNullException()
    {
      // Arrange
      MessageHandler<UnitTestMessage> messageHandler = new MessageHandler<UnitTestMessage>(HandleMessage);
      // Act / Assert
      Assert.ThrowsAsync<ArgumentNullException>(() => messageHandler.HandleMessageAsync(null));
    }

    private async Task HandleMessage(UnitTestMessage message)
    {
      await Task.Run(() =>
      {
        genericCallbackCalled = true;
      });
    }
    private async Task HandleMessage(IMessage obj)
    {
      await Task.Run(() =>
      {
        callbackCalled = true;
      });
    }
  }
}
