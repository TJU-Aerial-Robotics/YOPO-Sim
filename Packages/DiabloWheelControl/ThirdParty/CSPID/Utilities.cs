using System;

namespace CSPID
{
    internal static class Utilities
    {
        internal static double Scale(this double value, Range<double> range, Range<double> targetRange)
        {
            if (value < range.Minimum || value > range.Maximum)
                throw new ArgumentOutOfRangeException($"Expected {nameof(value)} to fall within {nameof(range)}");

            return targetRange.Minimum + (value - range.Minimum) / (range.Maximum - range.Minimum) * (targetRange.Maximum - targetRange.Minimum);
        }

        internal static double Clamp(this double value, Range<double> range)
        {
            return value < range.Minimum ? range.Minimum : value > range.Maximum ? range.Maximum : value;
        }

        internal static double ClampToMaximumStep(this double current, double previous, double maximumStep)
        {
            if (maximumStep <= 0)
                throw new ArgumentOutOfRangeException($"Expected {nameof(maximumStep)} to be greater than 0");

            if (current - previous > maximumStep) return previous + maximumStep;
            if (previous - current > maximumStep) return previous - maximumStep;
            return current;
        }
    }
}
