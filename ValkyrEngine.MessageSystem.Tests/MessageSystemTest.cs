using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValkyrEngine.MessageSystem.Exceptions;
using ValkyrEngine.MessageSystem.Tests.Helper;
using Xunit;

namespace ValkyrEngine.MessageSystem.Tests
{
  public class MessageSystemTest
  {
    private bool callbackExecuted = false;
    private ManualResetEvent resetEvent = new ManualResetEvent(false);

    [Fact]
    [Trait("Category", "Unit")]
    public async void SendMessage_ValidMessageAndActiveSystem_MessageIsSend()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();
      UnitTestMessage message = new UnitTestMessage();

      messageSystem.Activate();
      messageSystem.RegisterReceiver<UnitTestMessage>(HandleMessage);
      // Act
      messageSystem.SendMessage(message);
      resetEvent.WaitOne();
      // Assert
      Assert.True(callbackExecuted);
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void SendMessage_ValidMessageSystemNotActive_MessageIsInPipeline()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();
      UnitTestMessage message = new UnitTestMessage();

      messageSystem.RegisterReceiver<IMessage>(HandleMessage);
      // Act
      messageSystem.SendMessage(message);

      // Assert
      Assert.Single(messageSystem.ActiveMessages);
      Assert.IsType<UnitTestMessage>(messageSystem.ActiveMessages.First());
      Assert.False(callbackExecuted);
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
    [Fact]
    [Trait("Category", "Unit")]
    public void Activate_MessageSystemShouldBeActive()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();

      // Act
      messageSystem.Activate();
      // Assert
      Assert.True(messageSystem.Active);
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void Deactivate_MessageSystemShouldNotBeActive()
    {
      // Arrange
      MessageSystem messageSystem = new MessageSystem();

      messageSystem.Activate();

      // Act 
      messageSystem.Deactivate();

      // Assert
      Assert.False(messageSystem.Active);
    }

    private async Task HandleMessage(IMessage message)
    {
      await Task.Run(() =>
      {
        callbackExecuted = true;
        resetEvent.Set();
      });
    }
  }
}
