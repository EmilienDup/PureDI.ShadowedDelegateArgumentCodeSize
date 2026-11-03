using Pure.DI;

var composition = new Composition();
Console.WriteLine(composition.Root.GetType().Name);

public sealed record Nav(string Value);
public sealed record ArgB(string Value);

// Shared singleton services
public sealed class S0;
public sealed class S1(S0 previous);
public sealed class S2(S1 previous);
public sealed class S3(S2 previous);
public sealed class S4(S3 previous);
public sealed class S5(S4 previous);
public sealed class S6(S5 previous);
public sealed class S7(S6 previous);
public sealed class S8(S7 previous);
public sealed class S9(S8 previous);

// Every view model consumes Nav; each one opens the next through two delegates that both pass a new Nav
public sealed class Vm0(Nav nav, S9 services, Func<Nav, Vm1> open, Func<Nav, ArgB, Vm1> openWithB);
public sealed class Vm1(Nav nav, S9 services, Func<Nav, Vm2> open, Func<Nav, ArgB, Vm2> openWithB);
public sealed class Vm2(Nav nav, S9 services, Func<Nav, Vm3> open, Func<Nav, ArgB, Vm3> openWithB);
public sealed class Vm3(Nav nav, S9 services, Func<Nav, Vm4> open, Func<Nav, ArgB, Vm4> openWithB);
public sealed class Vm4(Nav nav, S9 services, Func<Nav, Vm5> open, Func<Nav, ArgB, Vm5> openWithB);
public sealed class Vm5(Nav nav, S9 services, Func<Nav, Vm6> open, Func<Nav, ArgB, Vm6> openWithB);
public sealed class Vm6(Nav nav, S9 services, Func<Nav, Vm7> open, Func<Nav, ArgB, Vm7> openWithB);
public sealed class Vm7(Nav nav, S9 services, Func<Nav, Vm8> open, Func<Nav, ArgB, Vm8> openWithB);
public sealed class Vm8(Nav nav, S9 services, Func<Nav, Vm9> open, Func<Nav, ArgB, Vm9> openWithB);
public sealed class Vm9(Nav nav, S9 services, Func<Nav, Vm10> open, Func<Nav, ArgB, Vm10> openWithB);
public sealed class Vm10(Nav nav, S9 services);
public sealed class Top(Func<Nav, Vm0> open);

partial class Composition
{
    private static void Setup() => DI.Setup(nameof(Composition))
        .Bind<S0>().As(Lifetime.Singleton).To<S0>()
        .Bind<S1>().As(Lifetime.Singleton).To<S1>()
        .Bind<S2>().As(Lifetime.Singleton).To<S2>()
        .Bind<S3>().As(Lifetime.Singleton).To<S3>()
        .Bind<S4>().As(Lifetime.Singleton).To<S4>()
        .Bind<S5>().As(Lifetime.Singleton).To<S5>()
        .Bind<S6>().As(Lifetime.Singleton).To<S6>()
        .Bind<S7>().As(Lifetime.Singleton).To<S7>()
        .Bind<S8>().As(Lifetime.Singleton).To<S8>()
        .Bind<S9>().As(Lifetime.Singleton).To<S9>()
        .Root<Top>("Root");
}
