using System;

#pragma warning disable RECS0017 // Possible compare of value type with 'null'
namespace CSPID {
    /// <summary>
    /// An inclusive range.
    /// </summary>
    public struct Range<T> where T : IComparable<T> {
        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        public T Minimum { get; }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        public T Maximum { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSPID.Range`1"/> struct.
        /// </summary>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        public Range(T minimum, T maximum) {
            if (minimum == null)
                throw new ArgumentNullException(nameof(minimum));

            if (maximum == null)
                throw new ArgumentNullException(nameof(minimum));

            if (minimum.CompareTo(maximum) > 0)
                throw new ArgumentException($"Expected {nameof(minimum)} to be less than or equal to {nameof(maximum)}");

            Minimum = minimum;
            Maximum = maximum;
        }
    }
}
