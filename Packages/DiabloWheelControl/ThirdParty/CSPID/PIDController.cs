using System;

namespace CSPID
{

    /// <summary>
    /// A PID (proportional-integral-derivative) controller.
    /// </summary>
    public class PIDController : IPIDController
    {
        private readonly object _lock = new object();

        private readonly Range<double> _unitRange = new Range<double>(-1, 1);
        private readonly Range<double> _errorRange;
        private readonly Range<double> _controlRange;

        private double _integrator;
        private double _previousError;
        private double _previousControl;
        private double _maximumStep = double.MaxValue;
        private double _proportionalGain;
        private double _integralGain;
        private double _derivativeGain;

        /// <summary>
        /// Gets or sets the maximum control variable change per cycle.
        /// </summary>
        /// <value>The maximum control variable change per cycle.</value>
        public double MaximumStep
        {
            get { return _maximumStep; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException($"Expected {nameof(value)} to be greater than or equal to 0");

                lock (_lock) _maximumStep = value;
            }
        }

        /// <summary>
        /// Gets or sets the proportional gain.
        /// </summary>
        /// <value>The proportional gain.</value>
        public double ProportionalGain
        {
            get { return _proportionalGain; }
            set { lock (_lock) _proportionalGain = value; }
        }

        /// <summary>
        /// Gets or sets the integral gain.
        /// </summary>
        /// <value>The integral gain.</value>
        public double IntegralGain
        {
            get { return _integralGain; }
            set { lock (_lock) _integralGain = value; }
        }

        /// <summary>
        /// Gets or sets the derivative gain.
        /// </summary>
        /// <value>The derivative gain.</value>
        public double DerivativeGain
        {
            get { return _derivativeGain; }
            set { lock (_lock) _derivativeGain = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSPID.PIDController"/> class.
        /// </summary>
        /// <param name="errorRange">The error range.</param>
        /// <param name="controlRange">The control range.</param>
        public PIDController(
            Range<double> errorRange,
            Range<double> controlRange) : this(
                errorRange.Minimum,
                errorRange.Maximum,
                controlRange.Minimum,
                controlRange.Maximum)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CSPID.Controller"/> class.
        /// </summary>
        /// <param name="minimumError">The minimum error.</param>
        /// <param name="maximumError">The maximum error.</param>
        /// <param name="minimumControl">The minimum control value.</param>
        /// <param name="maximumControl">The maximum control value.</param>
        public PIDController(
            double minimumError,
            double maximumError,
            double minimumControl,
            double maximumControl)
        {
            _errorRange = new Range<double>(minimumError, maximumError);
            _controlRange = new Range<double>(minimumControl, maximumControl);
        }

        /// <summary>
        /// Calculates the next control variable value.
        /// </summary>
        /// <returns>The control variable value.</returns>
        /// <param name="error">The process variable error.</param>
        /// <param name="elapsed">The time elapsed since the last control value was calculated.</param>
        public double Next(double error, double elapsed)
        {
            double control;

            lock (_lock)
            {
                error = error
                    .Clamp(_errorRange)
                    .Scale(_errorRange, _unitRange);

                _integrator = (_integrator + (_integralGain * error * elapsed))
                    .Clamp(_controlRange);

                control = (_proportionalGain * error + _integrator + _derivativeGain * ((error - _previousError) / elapsed))
                    .Clamp(_unitRange)
                    .Scale(_unitRange, _controlRange)
                    .ClampToMaximumStep(_previousControl, _maximumStep);

                _previousControl = control;
                _previousError = error;
            }

            return control;
        }

        // MODIFIED: Added Reset method to reset the controller state
        public void Reset()
        {
            lock (_lock)
            {
                _integrator = 0;
                _previousError = 0;
                _previousControl = 0;
            }
        }
    }
}
