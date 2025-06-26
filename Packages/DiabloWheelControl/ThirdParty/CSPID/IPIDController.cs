namespace CSPID
{
    /// <summary>
    /// A PID (proportional-integral-derivative) controller.
    /// </summary>
    public interface IPIDController
    {
        /// <summary>
        /// Gets or sets the maximum control variable change per cycle.
        /// </summary>
        /// <value>The maximum control variable change per cycle.</value>
        double MaximumStep { get; set; }

        /// <summary>
        /// Gets or sets the proportional gain.
        /// </summary>
        /// <value>The proportional gain.</value>
        double ProportionalGain { get; set; }

        /// <summary>
        /// Gets or sets the integral gain.
        /// </summary>
        /// <value>The integral gain.</value>
        double IntegralGain { get; set; }

        /// <summary>
        /// Gets or sets the derivative gain.
        /// </summary>
        /// <value>The derivative gain.</value>
        double DerivativeGain { get; set; }

        /// <summary>
        /// Calculates the next control variable value.
        /// </summary>
        /// <returns>The control variable value.</returns>
        /// <param name="error">The process variable error.</param>
        /// <param name="elapsed">The time elapsed since the last control variable value was calculated.</param>
        double Next(double error, double elapsed = 1);

        /// <summary>
        /// Resets the controller.
        /// </summary>
        void Reset();
    }
}
