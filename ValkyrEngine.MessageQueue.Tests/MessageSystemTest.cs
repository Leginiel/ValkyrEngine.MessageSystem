using System;
using System.Linq;
using System.Threading.Tasks;
using ValkyrEngine.MessageQueue.Exceptions;
using ValkyrEngine.MessageQueue.Tests.Helper;
using Xunit;

namespace ValkyrEngine.MessageQueue.Tests
{
  public class MessageSystemTest
  {
    private bool callbackExecuted = false;

    [Fact]
    [Trait("Category", "Unit")]
    public void SendMessage_ValidMessage_MessageIsInPipeline()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();
      UnitTestMessage message = new UnitTestMessage();

      // Act
      messageSystem.SendMessage(message);

      // Assert
      Assert.Single(messageSystem.ActiveMessages);
      Assert.IsType<UnitTestMessage>(messageSystem.ActiveMessages.First());
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void SendMessage_Null_ArgumentNullException()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();

      // Act / Assert
      Assert.Throws<ArgumentNullException>(() => messageSystem.SendMessage<IMessage>(null));
    }
    [Fact]
    [Trait("Category", "Unit")]
    public async void ProcessMessages_Successful()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();
      UnitTestMessage message = new UnitTestMessage();

      messageSystem.RegisterReceiver<UnitTestMessage>(HandleMessage);
      messageSystem.SendMessage(message);

      // Act
      await messageSystem.ProcessMessages();

      // Assert
      Assert.Empty(messageSystem.ActiveMessages);
      Assert.True(callbackExecuted);
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void RegisterReceiver_ValidNonExistingReceiver_Successful()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();

      // Act
      messageSystem.RegisterReceiver<IMessage>(HandleMessage);

      // Assert
      Assert.Single(messageSystem.Receiver);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RegisterReceiver_ValidExistingReceiver_RegistrationException()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();
      messageSystem.RegisterReceiver<IMessage>(HandleMessage);

      // Act / Assert
      Assert.Throws<RegistrationException>(() => messageSystem.RegisterReceiver<IMessage>(HandleMessage));
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void RegisterReceiver_Null_ArgumentNullException()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();

      // Act / Assert
      Assert.Throws<ArgumentNullException>(() => messageSystem.RegisterReceiver<IMessage>(null));
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void UnregisterReceiver_ValidNonExistingReceiver_RegistrationException()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();

      // Act / Assert
      Assert.Throws<RegistrationException>(() => messageSystem.UnregisterReceiver<IMessage>(HandleMessage));
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void UnregisterReceiver_ValidExistingReceiver_Successful()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();
      messageSystem.RegisterReceiver<IMessage>(HandleMessage);

      // Act
      messageSystem.UnregisterReceiver<IMessage>(HandleMessage);

      // Assert
      Assert.Empty(messageSystem.Receiver);
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void UnregisterReceiver_Null_ArgumentNullException()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();

      // Act / Assert
      Assert.Throws<ArgumentNullException>(() => messageSystem.UnregisterReceiver<IMessage>(null));
    }

    private async Task HandleMessage(IMessage message)
    {
      await Task.Run(() =>
      {
        callbackExecuted = true;
      });
    }
  }
}
