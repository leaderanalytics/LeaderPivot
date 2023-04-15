/* 
 * Copyright 2021 Leader Analytics 
 * LeaderAnalytics.com
 * SamWheat.com
 *  
 * Please do not remove this header.
 */

namespace LeaderAnalytics.LeaderPivot;

public interface INodeBuilder<T>
{
    INodeT<T> Build(IEnumerable<T> data, List<IDimensionT<T>> dimensions, List<IMeasureT<T>> measures, bool displayGrandTotals);
    INodeT<T> BuildColumnHeaders(IEnumerable<T> data, IEnumerable<IDimensionT<T>> dimensions, IEnumerable<IMeasureT<T>> measures, bool displayGrandTotals);
}