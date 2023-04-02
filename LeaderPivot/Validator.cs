namespace LeaderAnalytics.LeaderPivot;

public class Validator<T>
{
    public void Validate(IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures)
    {
        if (!(dimensions?.Any() ?? false))
            throw new ArgumentNullException(nameof(dimensions) + " cannot be null and must contain at least one element.");

        if (!(measures?.Any() ?? false))
            throw new ArgumentNullException(nameof(measures) + " cannot be null and must contain at least one element.");

        if (!dimensions.Any(x => x.IsEnabled && !x.IsRow))
            throw new Exception($"At least one enabled column dimension is required.  One or more dimensions with {nameof(Dimension<T>.IsEnabled)} property set to true and {nameof(Dimension<T>.IsRow)} property set to false are required.");

        if (!dimensions.Any(x => x.IsEnabled && x.IsRow))
            throw new Exception($"At least one enabled row dimension is required.  One or more dimensions with {nameof(Dimension<T>.IsEnabled)} property set to true and {nameof(Dimension<T>.IsRow)} property set to true are required.");

        if (dimensions.Any(x => string.IsNullOrEmpty(x.DisplayValue)))
            throw new Exception("DisplayValue property for each Dimension is required.");

        if (measures.Any(x => string.IsNullOrEmpty(x.DisplayValue)))
            throw new Exception("DisplayValue property for each Measure is required.");

        if (dimensions.GroupBy(x => x.DisplayValue).Any(x => x.Count() > 1))
            throw new Exception("DisplayValue property for each dimension must be unique.");

        if (measures.GroupBy(x => x.DisplayValue).Any(x => x.Count() > 1))
            throw new Exception("DisplayValue property for each measure must be unique.");

        if (!measures.Any(x => x.IsEnabled))
            throw new Exception("IsEnabed property must be true for at least one measure.");
    }


    /// <summary>
    /// Re-sequences and resets IsLeaf property on each dimension.  Call this method after dimension drag/drop operations.
    /// </summary>
    /// <param name="dimensions"></param>
    /// <returns></returns>
    public List<Dimension<T>> ValidateDimensions(IEnumerable<Dimension<T>> dimensions)
    {
        var dimList = dimensions.Where(x => x.IsEnabled).OrderBy(x => !x.IsRow).ThenBy(x => x.Sequence).ToList();
        int ordinal = 0;

        for (int i = 0; i < 2; i++)
        {
            var axis = dimList.Where(x => i == 0 ? x.IsRow : !x.IsRow).ToList();

            for (int j = 0; j < axis.Count; j++)
            {
                axis[j].Sequence = j;
                axis[j].IsLeaf = j == axis.Count - 1; // Reset IsLeaf in case user drags dimension
                axis[j].Ordinal = ordinal++;
                axis[j].CanRepositionAcrossAxis = axis.Count > 1;
            }
        }
        return dimList;
    }


    public List<Measure<T>> ValidateMeasures(IEnumerable<Measure<T>> measures)
    {
        if (measures.Where(x => x.IsEnabled).Count() == 1)
            measures.First(x => x.IsEnabled).CanDisable = false;
        else
            measures.All(x => x.CanDisable = true);

        return measures.Where(x => x.IsEnabled).OrderBy(x => x.Sequence).ToList();
    }
}
