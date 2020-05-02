using Autofac;

namespace ValkyrEngine.MessageSystem
{
  /// <summary>
  /// A class that is used to setup autofac.
  /// </summary>
  public class MessageSystemModule : Module
  {
    /// <inheritdoc/>
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<MessageSystem>()
             .As<IMessageSystem>()
             .SingleInstance();
    }
  }
}
