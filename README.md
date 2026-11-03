# Pure.DI: the graph under nested `Func<T1, T>` delegates is still duplicated when inner delegates pass the same argument again

Minimal repro for Pure.DI master (da6574d) and 2.5.7: a chain of 11 view models that all consume `Nav`, each opening the next through two delegates that both pass a new `Nav`, generates 23,572 lines and constructs the last view model 1,024 times. Every copy is the same code: each view model uses the `Nav` of the delegate that creates it.

Follow-up to #175, which fixed this for arguments that nothing consumes. [`ISSUE.md`](ISSUE.md) is the text to paste into a Pure.DI GitHub issue.

```
dotnet build                                                                                   # 2.5.7 from nuget.org
dotnet build -p:PureDIVersion=<master build version> -p:RestoreAdditionalProjectSources=<folder with the package>
```

`grep -c "global::Vm10(" obj/Debug/net10.0/generated/Pure.DI/Pure.DI.SourceGenerator/Composition.g.cs` gives 1024.
