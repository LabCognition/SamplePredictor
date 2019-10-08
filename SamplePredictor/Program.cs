using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LabCognition.Interface;
using LabCognition.Interface.Calibration;
using CommandLine;

namespace ConsoleApp1
{
    class Program
    {
        public class Options
        {
            [Value(0, Required = false, HelpText = "data files to predict", Default = new string[] { @"Data\Spec_Abs_Ech 03.txt" })]
            public IEnumerable<string> Filenames { get; set; }

            [Option('m', Required = false, HelpText = "Filepath of the calibration model", Default = "Data\\CowMilkRange.calibration")]
            public string ModelPath { get; set; }

            [Option('f', Required = true, HelpText = "Binary of the IPredictionEngineFactory implementation")]
            public string PredictionEngineFactoryPath { get; set; }
        }

        static void Main(string[] args)
        {
            try {
                Parser.Default
                    .ParseArguments<Options>(args)
                    .WithParsed((Options options) => { Predict(options.PredictionEngineFactoryPath, options.ModelPath, options.Filenames); });
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void Predict(string factoryPath, string modelPath, IEnumerable<string> dataPaths)
        {
            // create the prediction engine factory
            IPredictionEngineFactory factory = InstanceFactory<IPredictionEngineFactory>.Create(factoryPath);

            // load the prediction model via the factory
            IPredictionEngine model = factory.FromFile(modelPath);

            //do a prediction for the passed in data files
            foreach (var dataPath in dataPaths) {
                // load the data for the prediction
                (string name, double[] x, double[] y) = LoadPredictionData(dataPath);

                // check if engine supports the IPredictionEngineReport interface
                if (model is IPredictionEngineReport predictionEngineReport) {
                    // do the prediction
                    (string Name, object[,] Data)[] reports = predictionEngineReport.Predict(name, x, y);

                    foreach ((string Name, object[,] Data) in reports) {
                        Console.WriteLine();
                        Console.WriteLine(Name);
                        WriteDataToConsole(Data);
                    }
                } else {
                    // predict the data for all constituents and properties
                    PredictAllConstituentsAndProperties(factory, model, x, y);
                }
            }
        }

        private static void PredictAllConstituentsAndProperties(IPredictionEngineFactory factory, IPredictionEngine model, double[] x, double[] y)
        {
            // get the constituents and properties of the model
            string[] constituents = model.GetConstituents();
            string[] properties = model.GetProperties();

            // do the prediction for all constituents and all properties
            object[,] res = model.Predict(x, y, constituents, properties);

            // write the results to the console
            for (int c = 0; c < res.GetLength(0); c++) {
                Console.WriteLine($"Results for {constituents[c]}");
                for (int p = 0; p < res.GetLength(1); p++) {
                    Console.WriteLine($"\t{properties[p]}={res[c, p]}");
                }
            }
        }

        private static void WriteDataToConsole(object[,] data)
        {
            for (int r = 0; r < data.GetLength(0); r++) {
                string line = "";
                for (int c = 0; c < data.GetLength(1); c++) {
                    line += $"\t{data[r, c]}";
                }

                Console.WriteLine(line);
            }
        }


        private static (string name, double[] x, double[] y) LoadPredictionData(string filename)
        {
            var data = LoadPredictionData();

            return (Path.GetFileNameWithoutExtension(filename),
                data.Select(x => x.x).ToArray(), data.Select(x => x.y).ToArray());

            IEnumerable<(double x, double y)> LoadPredictionData()
            {
                using (StreamReader sr = File.OpenText(filename)) {
                    string s = sr.ReadLine();
                    while (s != null) {
                        string[] tok = s.Split("\t ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (tok.Length > 1) {
                            if (double.TryParse(tok[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) &&
                                double.TryParse(tok[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y)) {
                                yield return (x, y);
                            }
                        }

                        s = sr.ReadLine();
                    }
                }
            }
        }
    }
}
