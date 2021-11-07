![Leader Analytics](logo.png)
 
# Leader Pivot

Looking for the LeaderPivot Blazor control? This repo is not it.  Please [check here for for the Blazor control](https://github.com/leaderanalytics/LeaderPivot.Blazor), and [here for the demo project](https://github.com/leaderanalytics/LeaderPivot.BlazorDemo).



LeaderPivot is a base library that creates a data structure which can be used to render a pivot table.  This library contains only the low level components that are used to create the pivot table data structure.  A library for a specific UI can use this data structure to render a pivot table.  Currently, one UI specific implementation exists for Blazor.

## Classes

* `Measure` - Describes a column or variable to be used as a datapoint on a pivot table.
* `Dimension` - Describes how data should be grouped.  Used to represent an axis on a multi-dimensional pivot table.
* `NodeBuilder` - Groups data based on the supplied `Dimensions`. Constructs a hierarchical `Node` structure which is then passed to `MatrixBuilder`.
* `MatrixBuilder` - Traverses the `Node` structure produced by `NodeBuilder` and produces a `Matrix` consisting of `MatrixCell` objects. `MatrixCell` exposes properties the UI layer will use to render the pivot table.