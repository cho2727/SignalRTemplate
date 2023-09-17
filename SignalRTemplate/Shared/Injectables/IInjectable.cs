namespace SignalRTemplate.Shared.Injectables;

public interface IInjectable { }
public interface ITransient : IInjectable { }
public interface IScoped : IInjectable { }
public interface ISingleton : IInjectable { }
