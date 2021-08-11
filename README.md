# SamplePredictor
This .NET C# command line application example demonstrates how to implement the
chemometrics prediction API into your software.
Demo data and integration tests are included to show real outputs.

## Breaking Changes
### Version 2.0.0
Complete review of the functionality with major changes in interface
communication:

#### Split Functionalities into two new Packages
*LabCognition.Interface (1.1.1)* has been replaced by *LC.Predictor (0.1.0)* and
*LC.Runtime (0.1.0)* offering slightly enhanced functionality.
Interfaces and features have been split into two new nuget packages.

|LabCognition.Interface (1.1.1)|LC.Runtime (0.1.0)|Action|
|----|----|----|
IPredictionEngineFactory|IPredictorFactory|interface renamed and enhanced
InstanceFactory class|InstanceFactory class|moved
MsiApi class|MsiApi class|moved
MsiConfig class|MsiConfig class|moved and enhanced to support external configuration file
Plugin class|Plugin class|moved
KnownMsi list|KnownMsi list|Change handling by reading list from json MsiConfig.config file

LabCognition.Interface (1.1.1)|LC.Predictor (0.1.0)|Action|
|----------------------|---------------------|-------|
IPredicitonEngine|IPredictor| renamed and methods changed
IPredictionEngineReport|no implemention!|deprecated; use IPredictor methods
object[,] Predict(double[] dataX, double[] dataY, string[] constituents, string[] properties);|IPredictionResult[]? Predict(double[] dataX, double[] dataY);|use type specific prediction results and changed the result output using *IPredicitonResult*
string[] GetProperties(); and string[] GetConstituents();|IPredictionResult[]? GetResultPreview();|changed concept by returning an empty preview containing flat *PredictionResult\<T\>* objects of the expected results rather than lists of properties and constituents

#### New Integration Tests
Integration tests clearly illustrate how to use the sample predictor application
with all kinds of calibration models.

#### Simplified Result Preview
With the new implementation it is much easier to gather possible prediction
results returned by a calibration model upfront using the 'GetResultPreview()'
method.
It returns a full prediction result with all properties you might expect in a
regular prediction when using the 'Predict()' method.
Only the values are empty.

### Version 1.0.0
This sample predictor application uses LabCognition.Interface to accomodate
communication between the SamplePredictor.exe and the underlying prediction
engine software, e.g. panorama.exe.

## Requirements/Downloads

### Development
- [LC.Predictor NuGet package](https://www.nuget.org/packages/LC.Predictor/) 
- [LC.Runtime NuGet package](https://www.nuget.org/packages/LC.Runtime/) 
- [CommandLineParser NuGet Package](https://github.com/commandlineparser/commandline)
- .NET Standard 2.0
### Micsellaneous
- [panorama Software](https://www.labcognition.com/en/Download.html)

   The [Quantify add-on module](https://www.labcognition.com/en/Quantify.html)
   in panorama provides you the chemometric data modelling environment to create
   univariate and multivariate calibration models from your spectroscopic data
   files yourself. 
   It also implements the *IPredictorFactory* and *IPredictor* interfaces to
   serve as the prediction engine software for the SamplePredictor application.

# Getting Started
The source code has been created with Visual Studio 2019.
## Contents
- SamplePredictor.exe source code
- SamplePredictor integration tests source code
- Demo calibration data
  
  Qualitative and quantitative calibration model files
  (*\*.calibration*) are enclosed in the ***SamplePredictor\\Data\\*** folder to this
  project.
  Naming convention for file is the following:
  *\<Sample spectrum to use\>-\<calibration model type\>-\<calibrated constituent(s)\>.calibration*
  
  **NOTE: The calibrations have been solely designed to depict their usage.
  They do not produce any reliable scientific outputs!**

- Demo spectrum files
  
  Spectrum files are provided as comma separated x,y data text files (*\*.txt*)
  - Milk.txt
    
    Use with quanitative calibration models. 
    It is an excerpt of a mid infrared spectrum taken from cow milk powder.
    The calibrated constituent(s) are the fat content ('Fat') and the lactose content ('Lac') in \%.

  - SweetenerAspartame.txt
    
    Use with qualitative calibration models.
    It is an excerpt of a near infrared spectrum file taken from pure Aspartame sweetener powder. Qualitative calibrations may be capable of distinguishing the constituents Aspartame, brown sugar, Saccharin and Sucralose.

## Demo Application
The SamplePredictor application supports two major features:
- Sample spectrum prediction
  
  Performs qualitative or quantitative prediction using a sample spectrum to get
  the comprehensibe prediction statistics.

- Prediction preview
  
  Performs a dry run of the qualitative or quantitative prediction without any
  sample spectrum to obtain a dummy prediction statistics containing all outputs
  you can get.

## Initial Preparation
You will need the panorama software version 8.0.0 or higher serving as the
prediction engine software.

- Download and install the [panorama Software](https://www.labcognition.com/en/Download.html) on your computer.
  
  **NOTE: panorama requires a valid license to work properly!
  Usually it includes a 30-days trial license which grants access to all
  features of the software.
  If you run into problems, please contact support@labcognition.com.**

## Using the SamplePredictor Integration Tests
Follow the steps below to run the sample predictor application:
- Open the *SamplePredictor.sln* file with Visual Studio 2019 (or higher).

- The best way to get started is to have a look at the
  *SamplePredictor.IntegrationTests* project.

- Open the file *PredictionTests.cs*

  It contains tests for all availbale 'Predict' and 'GetResultPreview' use
  cases.
  Each test operates the SamplePredictor.exe with the appropriate calibration
  model and sample spectrum file.
  The expected text output is provided as well and compared with the actual test
  run's output.

- Before you run any test, please make sure to to have the panorama.exe file
  path properly set!
  Adjust the following line:

  ```csharp
  private static string PredictorExecutablePath => @"c:\Program Files (x86)\LabCognition\Panorama\Panorama.exe";
  ```

## Command Line Usage
The sample predictor executable can also be run from a DOS prompt with the following arguments:

`SamplePredictor.exe [OPTIONS] [FILES]`

***Example:***

`SamplePredictor.exe -f c:\...\Panorama.exe -m c:\...\CalibrationModel.calibration c:\...\Spectrum.txt`

### Options
|Option|Description|Comment|
|---|---|---|
|-f|Fully qualified file path to the binary implementing the IPredictorFactory. (See example above)|Mandatory|
|-m|Fully qualified file path to the calibration model file (*.calibration)||

### Tips for your own Implementation
The SamplePredictor converts the prediction and prediction preview outputs into
some text report.
Most of the source code in *Program.cs* belongs to report creation rather than the prediction itself.

In your application you would only require the following:

- Create an instance of the prediction engine software (i.e. panorama.exe) using methods from *LC.Runtime* package.
  
  This is done here:
  ```csharp
   var factory = InstanceFactory<IPredictorFactory>.Create(factoryPath);
  ```

- Load a calibration model into the factory using *LC.Predictor* package.
  
  ```csharp
  IPredictor predictor = factory.ReadModelFromFile(modelPath);
  ```

- Get a preview of all results returned when using a specific calibration model:
  
  ```csharp
  IPredictionResult[]? preview = predictor.GetResultPreview();
  ```
  Doing this might help you to accomodate the user interface or prepare your own
  data structures.

- Perform a prediction with x,y spectrum data
  ```csharp
  IPredictionResult[]? results = predictor.Predict(x, y);
  ```

#### Working with IPredictionResult
Since the value type of a result is not unique for all results they are returned
as *IPredictionResult*.
The class instance behind is generic *PredictionResult\<T\>*.
*System.Double* and *System.String* are currently supported types.

Use the *GetValueType()* method in the *IPredictionResult* interface to identify
the value type in order to resolve the *PredictionResult\<T\>* instance behind.
An example implementation for such conversion is provided in the ResultToString() method. Here is the core conversion if-statement:

***Excerpt from 'public static string ResultToString(IPredictionResult result)'***
```csharp
            if (result.GetValueType() == typeof(string))
            {
                value = (result as PredictionResult<string>)!.Value;
            }
            else if (result.GetValueType() == typeof(double))
            {
                value = (result as PredictionResult<double>)!.Value.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                // other value types are not supported!
                return value;
            }

```

### Calibration File
The *\*.calibration* file contains a calibration model which must have been
created with the panorama software.

For details on how to create your own calibration models and export them as
*\*.calibration* files, please have a look at the
[panorama calibration manual](https://www.labcognition.com/onlinehelp/en/calibration_manual.htm).

### Spectrum Files
You may provide one or more spectrum files separated by space for prediciton.

The spectrum files must be UTF-8 encoded text files containing comman separated x,y data points in lines.
One data point must be given per line followed by a carriage return line feed
(\\r\\n).
Please also have a look at the enclosed *\*.txt* files in the ***SamplePredictor\\Data\\*** folder.


# Sientific Background
Multivariate data analysis is offering powerful mathematical methods for
predicting physical properties from spectroscopic data rather than gathering
such information from laboratory wet chemistry sample analysis.
Derived calibration models create a correlation between the physical sample
properties and spectral responses at particular wavelengths.
In many applications in the Chemical, Pharmaceutical industry and R&D
calibration models are used for quantitative and qualitative property
predictions to solve analytical problems.
Spectra of your samples are taken with a spectrometer (e.g. Infrared, Raman,
UV/Vis, Fluorescence, ...).
