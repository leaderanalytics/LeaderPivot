﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderAnalytics.LeaderPivot
{
    public class Validator<T>
    {
        public void Validate(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures)
        {

            if (!(data?.Any() ?? false))
                throw new ArgumentNullException(nameof(data) + " cannot be null and must contain at least one element.");

            if (!(dimensions?.Any() ?? false))
                throw new ArgumentNullException(nameof(dimensions) + " cannot be null and must contain at least one element.");

            if (!(measures?.Any() ?? false))
                throw new ArgumentNullException(nameof(measures) + " cannot be null and must contain at least one element.");

            if (!dimensions.Any(x => !x.IsRow))
                throw new Exception($"At least one column dimension is required.  One or more dimensions with {nameof(Dimension<T>.IsRow)} property set to false are required.");

            if (!dimensions.Any(x => x.IsRow))
                throw new Exception($"At least one row dimension is required.  One or more dimensions with {nameof(Dimension<T>.IsRow)} property set to true are required.");

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

        public List<Dimension<T>> SortDimensions(IEnumerable<Dimension<T>> dimensions) => dimensions.OrderBy(x => !x.IsRow).ThenBy(x => x.Sequence).ToList();
            

        public List<Measure<T>> SortAndFilterMeasures(IEnumerable<Measure<T>> measures) => measures.OrderBy(x => x.Sequence).Where(x => x.IsEnabled).ToList();
    }
}
