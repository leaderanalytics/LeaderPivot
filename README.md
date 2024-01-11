![Leader Analytics](./logo.png)
 
# LeaderPivot

LeaderPivot is a base library that generates a data structure which can be used to render a pivot grid.  This library contains no UI implementation but is used by multiple libraries that do.  

LeaderPivot contains the logic and the low level components that are used to create the pivot grid data structure.  It also contains methods that can be used to handle UI events such as collapsing/expanding nodes or rearranging dimensions.  

Currently, three UI specific implementation exist: 

[Blazor control](https://github.com/leaderanalytics/LeaderPivot.Blazor)

[Blazor demo project](https://github.com/leaderanalytics/LeaderPivot.BlazorDemo)

[Blazor live demo](https://leaderanalytics.com/blazor/leader-pivot-demo)

[Windows Presentation Foundation (WPF) demo and control](https://github.com/leaderanalytics/LeaderPivot.XAML.WPF)

[WinUI demo and control](https://github.com/leaderanalytics/LeaderPivot.XAML.WinUI)

## Classes

* `Measure` - Describes a column or variable to be used as a datapoint on a pivot grid.
* `Dimension` - Describes how data should be grouped.  Used to represent an axis on a multi-dimensional pivot grid.
* `Node` - Intermediate data structure produced by `NodeBuilder` as a result of grouping data.
* `Matrix` - Tabular data structure generated by `MatrixBuilder`.  
* `NodeBuilder` - Groups data based on the supplied `Dimensions`. Constructs a hierarchical `Node` structure which is then passed to `MatrixBuilder`.
* `MatrixBuilder` - Traverses the `Node` structure produced by `NodeBuilder` and produces a `Matrix` consisting of `MatrixCell` objects. `MatrixCell` exposes properties the UI layer will use to render a pivot grid.


[Get the LeaderPivot NuGet package](https://www.nuget.org/packages/LeaderAnalytics.LeaderPivot/)

## Version History
2.0.0 - Update to net 8.
1.0.0 - Initial release, net 6.
