# The graph under nested `Func<T1, T>` delegates is still duplicated when inner delegates pass the same argument again

**Pure.DI master (da6574d)** and **2.5.7**, .NET SDK 10.0.403, macOS. Repro project attached (`dotnet build`, about 3 s, 23,572 generated lines).

Follow-up to #175. Master no longer duplicates the graph for delegate arguments that nothing consumes. When the argument *is* consumed, but every delegate on the way passes a new value of it, the graph is still generated once per chain of delegates. All the copies are the same code, because each view model uses the argument of the delegate that creates it.

## Minimal code

Every view model consumes `Nav` and opens the next one through two delegates that both pass a new `Nav`.

```csharp
using Pure.DI;

var composition = new Composition();
Console.WriteLine(composition.Root.GetType().Name);

public sealed record Nav(string Value);
public sealed record ArgB(string Value);

public sealed class S0;
public sealed class S1(S0 previous);
// ... same pattern ...
public sealed class S9(S8 previous);

public sealed class Vm0(Nav nav, S9 services, Func<Nav, Vm1> open, Func<Nav, ArgB, Vm1> openWithB);
// ... same pattern ...
public sealed class Vm9(Nav nav, S9 services, Func<Nav, Vm10> open, Func<Nav, ArgB, Vm10> openWithB);
public sealed class Vm10(Nav nav, S9 services);
public sealed class Top(Func<Nav, Vm0> open);

partial class Composition
{
    private static void Setup() => DI.Setup(nameof(Composition))
        .Bind<S0>().As(Lifetime.Singleton).To<S0>()
        // ... every Sk as Singleton ...
        .Bind<S9>().As(Lifetime.Singleton).To<S9>()
        .Root<Top>("Root");
}
```

## Expected

Each view model's graph generated once per delegate type, about 800 lines: the master output for the same graph when the arguments are not consumed (#175). The `Nav` that `VmN` receives always comes from the delegate that creates it. The outer `Nav` overrides are replaced before anything below consumes them.

## Actual

| view models with two delegates | generated lines (master) | generated lines (2.5.7) | constructions of the last view model |
|---|---|---|---|
| 6 | 1,772 | | 64 |
| 8 | 6,128 | | 256 |
| 10 (attached) | 23,572 | 21,820 | 1,024 |
| 12 | 93,353 | | 4,096 |

Generation time is acceptable at these sizes on master (10 s for 12 levels); the size of the output is the problem. Every lambda contains the whole graph of the next view model again:

```csharp
System.Func<global::Nav, global::Vm10> localFactory10 = new System.Func<global::Nav, global::Vm10>((global::Nav localArg1) =>
{
    global::Nav overriddenNav = localArg1;
    EnsureS9Exists307d();
    return new global::Vm10(overriddenNav, _singletonS9307d);
});
```

The same thing happens when each view model consumes two arguments and each delegate passes only one of them (`Vm(Nav nav, ArgB b, S9 services, Func<Nav, VmN> open, Func<ArgB, VmN> openWithB)`): 22,838 lines for 10 levels. There the value of the argument that a delegate doesn't pass does depend on the path. But it is read from a variable named after its contract, so the code is still the same.
