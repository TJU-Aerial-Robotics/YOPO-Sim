# CSPID

CSPID (pronounced "speedy") is a C# PID (proportional-integral-derivative) controller targeting .NET Standard 2.0.

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
**Contents**

- [Installation](#installation)
- [Usage](#usage)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Installation

Clone the repo with `git clone https://github.com/w-bonelli/CSPID.git` or pull the package from [Nuget.org](https://www.nuget.org/packages/CSPID/) with PowerShell: `Install-Package CSPID`.

## Usage

There's one class: `PIDController`. It has one method: `double Next(double error, double elapsed = 1)`. CSPID doesn't care how you measure your error or the passage of time. To construct a controller, provide:

- `errorRange`: the range you expect your error values to take
- `controlRange`: the range of values your control variable may take

You can tune gain in real time:

- `ProportionalGain`
- `IntegralGain`
- `DerivativeGain`

You can also impose a `MaximumStep` (the largest permissible change between successive control values).

```csharp
// create a controller
var controller = new PIDController(
    errorRange: new Range<double>(-5, 5), // Range<T> models an inclusive range
    controlRange: new Range<double>(0, 10))
{
    MaximumStep = double.MaxValue,
    ProportionalGain = 1,
    IntegralGain = 1,
    DerivativeGain = 1
} as IPIDController; // interface is handy if you need to mock

// or do it the old-fashioned way
controller = new PIDController(
    minimumError: -5,
    maximumError: 5,
    minimumControl: 0,
    maximumControl: 10)
{
    MaximumStep = double.MaxValue,
    ProportionalGain = 1,
    IntegralGain = 1,
    DerivativeGain = 1
} as IPIDController;

// get a control value
var value1 = controller.Next(error: 1); // elapsed defaults to 1

// get another one
var value2 = controller.Next(error: -1, elapsed: 0.5);
```
