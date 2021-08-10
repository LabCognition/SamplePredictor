using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;
using LC.Predictor;
using LC.Runtime;

namespace SamplePredictor
{
    /// <summary>
    /// This class contains the SamplePredictor client implementation 
    /// which may be used in this or similar fashion by any 3rd party C# application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// This class contains the commandline options
        /// </summary>
        public class Options
        {
            [Value(0, Required = false, HelpText = "Space separated file paths (*.txt) containing comma separated x,y data to predict", Default = null)]
            public IEnumerable<string>? Filenames { get; set; }

            [Option('m', Required = false, HelpText = "File path of the calibration model (*.calibration)", Default = null)]
            public string? ModelPath { get; set; }

            [Option('f', Required = true, HelpText = "Prediction engine executable file implementing the LC.Predictor.IPredictorFactory interface")]
            public string? PredictionEngineFactoryPath { get; set; }
        }

        static void Main(string[] args)
        {
            try {
                Parser.Default
                    .ParseArguments<Options>(args)
                    .WithParsed((Options options) => {
                        // Create an instance of the prediction engine factory and load a calibration model
                        IPredictor predictor = CreatePredictor(options.PredictionEngineFactoryPath, options.ModelPath);
                        if (options.Filenames == null ||
                            options.Filenames.Count() == 0)
                        {
                            // retrieve all possible results being returned when predicting data with the calibration model
                            IPredictionResult[]? preview = predictor.GetResultPreview();

                            // Show a preview of all prediction results for a given calibration model.
                            Console.WriteLine(ResultsToString(preview, options.ModelPath));
                            return;
                        }

                        // load the x,y data files
                        var data = LoadData(options.Filenames);

                        // Predict one or more files containing tab separated x,y data with the given calibration model.
                        Console.WriteLine(GetPredictionReport(predictor, data));
                    });
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public static IEnumerable<(string name, double[] x, double[] y)> LoadData(IEnumerable<string>? dataPaths)
        {
            if (dataPaths != null)
            {
                foreach (var dataPath in dataPaths)
                {
                    if (!File.Exists(dataPath))
                    {
                        Console.WriteLine($"The file '{dataPath}' does not exist!");
                        continue;
                    }

                    // load the data for the prediction
                    (string name, double[] x, double[] y) data = ("", new double[0], new double[0]);
                    try
                    {
                        data = LoadPredictionData(dataPath);
                    }
                    catch
                    {
                        Console.WriteLine($"The file '{dataPath}' failed to load!");
                        continue;
                    }

                    yield return data;
                }
            }
        }

        private static string GetPredictionReport(IPredictor predictor, IEnumerable<(string name, double[] x, double[] y)> data)
        {
            if (data == null ||
                data.Count() == 0)
            {
                return "No x,y data files loaded!";
            }

            var text = "";

            // do a prediction for the passed in data files
            foreach (var (name, x, y) in data)
            {
                // predict the data
                IPredictionResult[]? results = predictor.Predict(x, y);

                // create a plain text result output
                text += ResultsToString(results, name) + "\r\n";
            }

            return text;
        }

        public static string ResultsToString(IPredictionResult[]? results, string? name)
        {
            if (results == null)
            {
                return "No results!\r\n";
            }

            var text = "";
            if (name != null)
            {
                text += $"Results for: {name}\r\n";
            }

            text += $"Property\tValue [Unit]\tConstituent\r\n";
            for (int r = 0; r < results.Length; r++)
            {
                text += ResultToString(results[r]) + "\r\n";
            }

            return text;
        }

        public static string ResultToString(IPredictionResult result)
        {
            string value = "None";
            if (result == null)
            {
                return value;
            }

            // Format the result value according to the value type
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

            // append unit if any
            if (!string.IsNullOrEmpty(result.Unit))
            {
                value += $" [{result.Unit}]";
            }

            // create formatted line output
            return string.IsNullOrWhiteSpace(result.Constituent) ? 
                $"{result.Property}\t{value}" :
                $"{result.Property}\t{value}\t{result.Constituent}";
        }


        private static IPredictor CreatePredictor(string? factoryPath, string? modelPath)
        {
            if (factoryPath == null ||
                string.IsNullOrWhiteSpace(factoryPath))
            {
                throw new ArgumentNullException($"The '{nameof(factoryPath)}' must not be null or empty!");
            }

            if (modelPath == null ||
                string.IsNullOrWhiteSpace(modelPath))
            {
                throw new ArgumentNullException($"The '{nameof(modelPath)}' must not be null or empty!");
            }

            // create the prediction engine factory
            var factory = InstanceFactory<IPredictorFactory>.Create(factoryPath);

            // load the prediction model via the factory
            return factory.ReadModelFromFile(modelPath);
        }

        public static (string name, double[] x, double[] y) LoadPredictionData(string filename)
        {
            var data = LoadPredictionData();

            return (Path.GetFileNameWithoutExtension(filename),
                data.Select(x => x.x).ToArray(), data.Select(x => x.y).ToArray());

            IEnumerable<(double x, double y)> LoadPredictionData()
            {
                var lines = 1;
                using (StreamReader sr = File.OpenText(filename)) {
                    string? s = sr.ReadLine();
                    while (s != null) {
                        string[] tok = s.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (tok.Length == 2) {
                            if (double.TryParse(tok[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) &&
                                double.TryParse(tok[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y)) {
                                yield return (x, y);
                            }
                            else
                            {
                                throw new InvalidDataException($"Invalid data format at position '{lines}'");
                            }
                        }
                        else
                        {
                            throw new InvalidDataException($"Invalid data format at position '{lines}'");
                        }

                        s = sr.ReadLine();
                        lines++;
                    }
                }
            }
        }
    }
}
