# SamplePredictor
This .NET C# command line application example demonstrates how to implement the chemometrics prediction API into your software. Demo data is included to show a real output.

## Requirements/Downloads
### Development
- [LabCognition.Interface NuGet package](https://www.nuget.org/packages/LabCognition.Interface/) 
- [CommandLineParser NuGet Package](https://github.com/commandlineparser/commandline)
- .NET Standard 2.0
### Micsellaneous
- [panorama Software](https://www.labcognition.com/en/Download.html)

   The [Quantify add-on module](https://www.labcognition.com/en/Quantify.html) in panorama provides you the chemometric data modelling environment to create univariate and multivariate calibration models from your spectroscopic data files yourself. It also provides the prediction capabilities used by the SamplePredictor application. 

# Getting Started
## Contents
- SamplePredictor.exe source code
- Demo calibration model (_Data\CowMilkRange.calibration_)
- Demo spectrum file (_Data\Spec_Abs_Ech_03.txt_)

## Demo Application
The included demo data use a PLS-1 calibration model for predicting the fat quantity in milk powder from a mid-infrared sample spectrum.

## Starting the SamplePredictor Demo Application
Follow the steps below to run the sample predictor application:
- Download and install the [panorama Software](https://www.labcognition.com/en/Download.html) on your computer.
   
   __NOTE:__ _panorama requires a valid license to work properly! Usually it includes a 30-days trial license which grants access to all features of the software. If you run into problems, please contact support@labcognition.com._
- Open the _SamplePredictor.sln_ file with Visual Studio 2017 (or higher).
- Open the _SamplePredictor_ project properties (_Alt-Enter_ keys on the tree node in Solution Explorer).
- Open the _Debug_ section of the _SamplePredictor_ project properties.
- In the _Command line arguments_ text field of the _Start options_ add the following: 
`-f c:\Program Files (x86)\LabCognition\Panorama...\Panorama.exe`

   __NOTE:__ Replace the executable path with the fully qualified path of the panorama software installation on your computer.

   __NOTE:__ _Usually, the panorama software is installed in __c:\Program Files (c86)\LabCognition\panorama__ folder on your computer._

- Open the _Program.cs_ file in Visual Studio
- Set a break point to the last line of the `static void Main(string[] args)` method.
- Now start with _F5-key_ 
This will produce a comprehensive prediction result report using the enclosed CowMilkRange.calibration. The report contains the same output as a standard prediciton report within panorama softwre.

## Command Line Usage
The sample predictor executable can also be run from a DOS prompt with the following arguments:

`SamplePredictor.exe [OPTIONS] [FILES]`

__Example:__ 
`SamplePredictor.exe -f c:\...\Panorama.exe -m c:\...\CalibrationModel.calibration c:\...\Spectrum.txt`

### Options
|Option|Description|Comment|
|---|---|---|
|-f|Fully qualified file path to the binary of the IPredictionEngineFactory implementation. (See example above)|Mandatory|
|-m|Fully qualified file path to the calibration model file (*.calibration)||

### Calibration File
The *.calibration file contains a calibration model which must have been created with the panorama software. For details on how to create your own calibration models and export them as *.calibration files, please have a look at the [panorama calibration manual](https://www.labcognition.com/onlinehelp/en/calibration_manual.htm).

### Spectrum Files
You may provide one or more spectrum files separated by space for prediciton. The spectrum files must be UTF-8 encoded text files having a particular file format described [here](https://www.labcognition.com/onlinehelp/en/ascii_text_file_format.htm).
Please also have a look at the enclosed _Spec_Abs_Ech_03.txt_ file.

# Sientific Background
Multivariate data analysis is offering powerful mathematical methods for predicting physical properties from spectroscopic data rather than gathering such information from laboratory wet chemistry sample analysis. Derived calibration models create a correlation between the physical sample properties and spectral responses at particular wavelengths. In many applications in the Chemical and Pharmaceutical industry and R&D calibration models are used for quantitative and qualitative property predictions to solve analytical problems.

Such calibration models  in Chemical and Pharmaceutical industry and R&D applications. Spectra of your samples are taken with a spectrometer (e.g. Infrared, Raman, UV/Vis, Fluorescence, ...).
