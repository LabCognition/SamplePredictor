using NUnit.Framework;
using System.Diagnostics;
using System.IO;

namespace SamplePredictor.IntegrationTests
{
    /// <summary>
    /// This test class includes 'Predict' method and 'GetResultPreview' method tests
    /// using all supported calibration model types in LabCognition software suite
    /// </summary>
    /// <remarks>
    /// <para>
    /// The SamplePredictor.exe ist run with a suitable calibration model file (*.calibration)
    /// and corresponding spectrum data file (*.txt) located in the included 'Data' folder.
    /// The output is a text report containing prediction statistics values.
    /// The 'GetResultPreview' method is used automatically if no data files (*.txt) are provided.
    /// In this case the output contains all statistics values without any predicted values.
    /// 
    /// <b>NOTE: Some values might be NaN (Not a number) though a proper prediction is done!</b>
    /// </para>
    /// <para>
    /// <list type="bullet">
    /// <listheader>Spectrum files:</listheader>
    /// <item>Milk.txt
    /// <description>
    ///   This is an excerpt of a mid infrared spectrum of cow milk powder.
    ///   It may be used with all quantitative calibration models prefixed with 'Milk-'.
    /// </description>
    /// </item>
    /// <item>SweetenerAspartame.txt
    /// <description>
    ///   This is an excerpt of a near infrared spectrum of Aspartame powder.
    ///   It may be used with all qualitative calibration models (i.e. PCA models).
    ///   Such models are prefixed
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// <list type="bullet">
    /// <listheader>Calibration model files:</listheader>
    /// <item>Quantitative calibration models:
    /// <list type="bullet">
    /// <item>Milk-MLR-Fat.calibration
    /// <description>
    /// Multiple linear regression model calibrating 'Fat' content in milk powder
    /// </description>
    /// </item>
    /// <item>Milk-PCR-Fat.calibration
    /// <description>
    /// Principle component regression model calibrating 'Fat' content in milk powder
    /// </description>
    /// </item>
    /// <item>Milk-PLS1-Fat.calibration
    /// <description>
    /// Partial least squares model calibrating 'Fat' content in milk powder
    /// </description>
    /// </item>
    /// <item>Milk-PLS1-Fat-Lac.calibration
    /// <description>
    /// Partial least squares model calibrating two constituents, 'Fat' and 'Lac', content in milk powder
    /// </description>
    /// </item>
    /// <item>Milk-Univar-Fat.calibration
    /// <description>
    /// Polynomial fit calibration model calibrating 'Fat' content in milk powder
    /// </description>
    /// </item>
    /// </list>
    /// <item>Qualitative calibration modles:
    /// <item>SugarAndSweetener-PCA-unassigned.calibration
    /// <description>
    /// Principle component analysis model calibrating a group of sugars and sweeteners. 
    /// Aspartame is one of them. Group identification is either 'passed' or 'unknown'
    /// indicated by the 'IdentifiedAs' property.
    /// </description>
    /// </item>
    /// <item>SugarAndSweetener-PCA-assigned.calibration
    /// <description>
    /// Principle component analysis model calibrating a group of sugars and sweeteners including 
    /// material identification. The name of the identified material is indicated by the 'IdentifiedAs' property.
    /// In preview a list of all potential constituents is provided as individual 'IdentifiedAs' items.
    /// <b>NOTE: Multiple assignments are possible as comma separated values!</b>
    /// </description>
    /// </item>
    /// </item>
    /// </list>
    /// </list>
    /// </para>
    /// </remarks>
    public class PredictionTests
    {
        // SET A PROPER FILE PATH OF THE PREDICTOR SOFTWARE BEFORE RUNNING THESE TESTS
        // e.g.: @"c:\Program Files (x86)\LabCognition\Panorama\Panorama.exe"
        // Requires Version 8.0.0 or higher
        private static string PredictorExecutablePath => @"c:\Program Files (x86)\LabCognition\Panorama\Panorama.exe";

        [Test]
        public void Predict_Milk_PLS1_Fat_Test()
        {
            const string expected =
                "Results for: Milk\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "MahalanobisDistance\t0.952250952565498\r\n" +
                "Predicted\t2.19128593648249 [%]\tFat\r\n" +
                "PredictedFRatio\t0.6128548077594\tFat\r\n" +
                "PredictedFTest\t0.551110999342666\tFat\r\n" +
                "Residual\t-0.000142843589233665 [%]\tFat\r\n" +
                "Score\t-0.0114658325158742\r\n" +
                "Score-1\t-0.311981669436511\r\n" +
                "Score-2\t-0.112502860259786\r\n" +
                "Score-3\t-0.0114658325158742\r\n" +
                "SpectralResidual\t3.68995904127787E-07\r\n" +
                "SpectralResidualFRatio\t0.592494272443632\r\n" +
                "SpectralResidualFTest\t0.542332318539742\r\n" +
                "SpectralScoresFRatio\t-161.64543035543\r\n" +
                "SpectralScoresFTest\t0";
            var calibrationFilePath = @"Data\Milk-PLS1-Fat.calibration";
            var dataFiles = new string[] { @"Data\Milk.txt" };

            var actual = RunSamplePredictor(calibrationFilePath, dataFiles);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Predict_Milk_PLS2_Fat_Lac_Test()
        {
            const string expected =
                "Results for: Milk\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "MahalanobisDistance\t0.916057350051469\r\n" +
                "Predicted\t2.19205933641283 [%]\tFat\r\n" +
                "Predicted\t4.79867166335943 [%]\tLac\r\n" +
                "PredictedFRatio\t0.51029244633921\tFat\r\n" +
                "PredictedFRatio\t1.11708924188225\tLac\r\n" +
                "PredictedFTest\t0.51132431205461\tFat\r\n" +
                "PredictedFTest\t0.688640168368728\tLac\r\n" +
                "Residual\t-0.000153992084490216 [%]\tFat\r\n" +
                "Residual\t-9.08903243369745E-05 [%]\tLac\r\n" +
                "Score\t-0.0132864082874823\r\n" +
                "Score-1\t-0.339428578955783\r\n" +
                "Score-2\t-0.111870026305265\r\n" +
                "Score-3\t-0.0132864082874823\r\n" +
                "SpectralResidual\t5.01196386446889E-07\r\n" +
                "SpectralResidualFRatio\t0.421751195211346\r\n" +
                "SpectralResidualFTest\t0.470608360995185\r\n" +
                "SpectralScoresFRatio\t-140.024625640508\r\n" +
                "SpectralScoresFTest\t0";
            var calibrationFilePath = @"Data\Milk-PLS2-Fat-Lac.calibration";
            var dataFiles = new string[] { @"Data\Milk.txt" };

            var actual = RunSamplePredictor(calibrationFilePath, dataFiles);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Predict_Milk_PCR_Fat_Test()
        {
            const string expected =
                "Results for: Milk\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "MahalanobisDistance\t0.895099385316123\r\n" +
                "Predicted\t2.30150563385639 [%]\tFat\r\n" +
                "PredictedFRatio\t0.643680849363906\tFat\r\n" +
                "PredictedFTest\t0.562018205666307\tFat\r\n" +
                "Residual\t0.0041405178345711 [%]\tFat\r\n" +
                "Score\t-0.0852185552073707\r\n" +
                "Score-1\t-0.379144077709296\r\n" +
                "Score-2\t-0.0852185552073707\r\n" +
                "SpectralResidual\t0.000179355633713163\r\n" +
                "SpectralResidualFRatio\t1.85615566171223\r\n" +
                "SpectralResidualFTest\t0.801907042675243\r\n" +
                "SpectralScoresFRatio\t-18.9449745611366\r\n" +
                "SpectralScoresFTest\t0";
            var calibrationFilePath = @"Data\Milk-PCR-Fat.calibration";
            var dataFiles = new string[] { @"Data\Milk.txt" };

            var actual = RunSamplePredictor(calibrationFilePath, dataFiles);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Predict_Milk_MLR_Fat_Test()
        {
            const string expected =
                "Results for: Milk\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "MahalanobisDistance\tNaN\r\n" +
                "Predicted\t2.21001056395226 [%]\tFat\r\n" +
                "PredictedFRatio\t0.61809167702292\tFat\r\n" +
                "PredictedFTest\t0.552995283468839\tFat\r\n" +
                "Residual\tNaN [%]\tFat\r\n" +
                "SpectralResidual\tNaN\r\n" +
                "SpectralResidualFRatio\tNaN\r\n" +
                "SpectralResidualFTest\tNaN";
            var calibrationFilePath = @"Data\Milk-MLR-Fat.calibration";
            var dataFiles = new string[] { @"Data\Milk.txt" };

            var actual = RunSamplePredictor(calibrationFilePath, dataFiles);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Predict_Milk_Univar_Fat_Test()
        {
            const string expected =
                "Results for: Milk\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "MahalanobisDistance\tNaN\r\n" +
                "Predicted\t2.25055029747216 [%]\tFat\r\n" +
                "PredictedFRatio\t0.629231210849625\tFat\r\n" +
                "PredictedFTest\t0.555588879761571\tFat\r\n" +
                "Residual\tNaN [%]\tFat\r\n" +
                "SpectralResidual\tNaN";
            var calibrationFilePath = @"Data\Milk-Univar-Fat.calibration";
            var dataFiles = new string[] { @"Data\Milk.txt" };

            var actual = RunSamplePredictor(calibrationFilePath, dataFiles);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Predict_SugarAndSweetener_PCA_Assigned_Test()
        {
            const string expected =
                "Results for: SweetenerAspartame\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "IdentifiedAs\tAspartame\r\n" +
                "MahalanobisDistance\t0.828586389808309\r\n" +
                "Residual\tNaN\r\n" +
                "Score\t-0.00391413558675569\r\n" +
                "Score-1\t-0.574978557870261\r\n" +
                "Score-2\t-0.123147052380249\r\n" +
                "Score-3\t-0.00391413558675569\r\n" +
                "SpectralResidual\t6.3865867077247E-06\r\n" +
                "SpectralResidualFRatio\t0.37804030431471\r\n" +
                "SpectralResidualFTest\t0.447613179267899\r\n" +
                "SpectralScoresFRatio\t-64.0377105245707\r\n" +
                "SpectralScoresFTest\t0";
            var calibrationFilePath = @"Data\SugarsAndSweeteners-PCA-assigned.calibration";
            var dataFiles = new string[] { @"Data\SweetenerAspartame.txt" };

            var actual = RunSamplePredictor(calibrationFilePath, dataFiles);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Predict_SugarAndSweetener_PCA_Unassigned_Test()
        {
            const string expected =
                "Results for: SweetenerAspartame\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "IdentifiedAs\tPassed\r\n" +
                "MahalanobisDistance\t0.828586390423897\r\n" +
                "Residual\tNaN\r\n" +
                "Score\t-0.00391413560101499\r\n" +
                "Score-1\t-0.574978557870261\r\n" +
                "Score-2\t-0.123147052380249\r\n" +
                "Score-3\t-0.00391413560101499\r\n" +
                "SpectralResidual\t6.38658659740392E-06\r\n" +
                "SpectralResidualFRatio\t0.378040297784509\r\n" +
                "SpectralResidualFTest\t0.44761317589818\r\n" +
                "SpectralScoresFRatio\t-64.0377107578618\r\n" +
                "SpectralScoresFTest\t0";
            var calibrationFilePath = @"Data\SugarsAndSweeteners-PCA-unassigned.calibration";
            var dataFiles = new string[] { @"Data\SweetenerAspartame.txt" };

            var actual = RunSamplePredictor(calibrationFilePath, dataFiles);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetResultPreview_Milk_PLS1_Fat_Test()
        {
            const string expected =
                "Results for: Data\\Milk-PLS1-Fat.calibration\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "MahalanobisDistance\tNaN\r\n" +
                "Predicted\tNaN [%]\tFat\r\n" +
                "PredictedFRatio\tNaN\tFat\r\n" +
                "PredictedFTest\tNaN\tFat\r\n" +
                "Residual\tNaN [%]\tFat\r\n" +
                "Score\tNaN\r\n" +
                "Score-1\tNaN\r\n" +
                "Score-2\tNaN\r\n" +
                "Score-3\tNaN\r\n" +
                "SpectralResidual\tNaN\r\n" +
                "SpectralResidualFRatio\tNaN\r\n" +
                "SpectralResidualFTest\tNaN\r\n" +
                "SpectralScoresFRatio\tNaN\r\n" +
                "SpectralScoresFTest\tNaN";

            var calibrationFilePath = @"Data\Milk-PLS1-Fat.calibration";

            var actual = RunSamplePredictor(calibrationFilePath, null);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetResultPreview_Milk_PLS2_Fat_Lac_Test()
        {
            const string expected =
                "Results for: Data\\Milk-PLS2-Fat-Lac.calibration\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "MahalanobisDistance\tNaN\r\n" +
                "Predicted\tNaN [%]\tFat\r\n" +
                "Predicted\tNaN [%]\tLac\r\n" +
                "PredictedFRatio\tNaN\tFat\r\n" +
                "PredictedFRatio\tNaN\tLac\r\n" +
                "PredictedFTest\tNaN\tFat\r\n" +
                "PredictedFTest\tNaN\tLac\r\n" +
                "Residual\tNaN [%]\tFat\r\n" +
                "Residual\tNaN [%]\tLac\r\n" +
                "Score\tNaN\r\n" +
                "Score-1\tNaN\r\n" +
                "Score-2\tNaN\r\n" +
                "Score-3\tNaN\r\n" +
                "SpectralResidual\tNaN\r\n" +
                "SpectralResidualFRatio\tNaN\r\n" +
                "SpectralResidualFTest\tNaN\r\n" +
                "SpectralScoresFRatio\tNaN\r\n" +
                "SpectralScoresFTest\tNaN";

            var calibrationFilePath = @"Data\Milk-PLS2-Fat-Lac.calibration";

            var actual = RunSamplePredictor(calibrationFilePath, null);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetResultPreview_Milk_PCR_Fat_Test()
        {
            const string expected =
                "Results for: Data\\Milk-PCR-Fat.calibration\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "MahalanobisDistance\tNaN\r\n" +
                "Predicted\tNaN [%]\tFat\r\n" +
                "PredictedFRatio\tNaN\tFat\r\n" +
                "PredictedFTest\tNaN\tFat\r\n" +
                "Residual\tNaN [%]\tFat\r\n" +
                "Score\tNaN\r\n" +
                "Score-1\tNaN\r\n" +
                "Score-2\tNaN\r\n" +
                "SpectralResidual\tNaN\r\n" +
                "SpectralResidualFRatio\tNaN\r\n" +
                "SpectralResidualFTest\tNaN\r\n" +
                "SpectralScoresFRatio\tNaN\r\n" +
                "SpectralScoresFTest\tNaN";

            var calibrationFilePath = @"Data\Milk-PCR-Fat.calibration";

            var actual = RunSamplePredictor(calibrationFilePath, null);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetResultPreview_Milk_MLR_Fat_Test()
        {
            const string expected =
                "Results for: Data\\Milk-MLR-Fat.calibration\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "MahalanobisDistance\tNaN\r\n" +
                "Predicted\tNaN [%]\tFat\r\n" +
                "PredictedFRatio\tNaN\tFat\r\n" +
                "PredictedFTest\tNaN\tFat\r\n" +
                "Residual\tNaN [%]\tFat\r\n" +
                "SpectralResidual\tNaN\r\n" +
                "SpectralResidualFRatio\tNaN\r\n" +
                "SpectralResidualFTest\tNaN";
                
            var calibrationFilePath = @"Data\Milk-MLR-Fat.calibration";

            var actual = RunSamplePredictor(calibrationFilePath, null);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetResultPreview_Milk_Univar_Fat_Test()
        {
            const string expected =
                "Results for: Data\\Milk-Univar-Fat.calibration\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "MahalanobisDistance\tNaN\r\n" +
                "Predicted\tNaN [%]\tFat\r\n" +
                "PredictedFRatio\tNaN\tFat\r\n" +
                "PredictedFTest\tNaN\tFat\r\n" +
                "Residual\tNaN [%]\tFat\r\n" +
                "SpectralResidual\tNaN";
                
            var calibrationFilePath = @"Data\Milk-Univar-Fat.calibration";

            var actual = RunSamplePredictor(calibrationFilePath, null);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetResultPreview_SugarAndSweetener_PCA_Assigned_Test()
        {
            const string expected =
                "Results for: Data\\SugarsAndSweeteners-PCA-assigned.calibration\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "IdentifiedAs\t\tAspartame\r\n" +
                "IdentifiedAs\t\tBrown Sugar\r\n" +
                "IdentifiedAs\t\tSaccharin\r\n" +
                "IdentifiedAs\t\tSucralose\r\n" +
                "MahalanobisDistance\tNaN\r\n" +
                "Residual\tNaN\tAspartame\r\n" +
                "Residual\tNaN\tBrown Sugar\r\n" +
                "Residual\tNaN\tSaccharin\r\n" +
                "Residual\tNaN\tSucralose\r\n" +
                "Score\tNaN\r\n" +
                "Score-1\tNaN\r\n" +
                "Score-2\tNaN\r\n" +
                "Score-3\tNaN\r\n" +
                "SpectralResidual\tNaN\r\n" +
                "SpectralResidualFRatio\tNaN\r\n" +
                "SpectralResidualFTest\tNaN\r\n" +
                "SpectralScoresFRatio\tNaN\r\n" +
                "SpectralScoresFTest\tNaN";

            var calibrationFilePath = @"Data\SugarsAndSweeteners-PCA-assigned.calibration";

            var actual = RunSamplePredictor(calibrationFilePath, null);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetResultPreview_SugarAndSweetener_PCA_Unassigned_Test()
        {
            const string expected =
                "Results for: Data\\SugarsAndSweeteners-PCA-unassigned.calibration\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "IdentifiedAs\t\tUnassigned\r\n" +
                "MahalanobisDistance\tNaN\r\n" +
                "Residual\tNaN\tUnassigned\r\n" +
                "Score\tNaN\r\n" +
                "Score-1\tNaN\r\n" +
                "Score-2\tNaN\r\n" +
                "Score-3\tNaN\r\n" +
                "SpectralResidual\tNaN\r\n" +
                "SpectralResidualFRatio\tNaN\r\n" +
                "SpectralResidualFTest\tNaN\r\n" +
                "SpectralScoresFRatio\tNaN\r\n" +
                "SpectralScoresFTest\tNaN";

            var calibrationFilePath = @"Data\SugarsAndSweeteners-PCA-unassigned.calibration";

            var actual = RunSamplePredictor(calibrationFilePath, null);
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static string RunSamplePredictor(string calibrationFilePath, string[] dataFiles)
        {
            var arguments = $"-f {PredictorExecutablePath} -m {calibrationFilePath}";
            if (dataFiles != null &&
                dataFiles.Length > 0)
            {
                arguments += " " + string.Join(" ", dataFiles);
            }

            var report = "";
            var instance = new Process();
            instance.StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(TestContext.CurrentContext.TestDirectory, @"SamplePredictor.exe"),
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = TestContext.CurrentContext.TestDirectory,
            };

            // add CrLf here, because MS removes it!
            instance.OutputDataReceived += (s, e) => report += e.Data + "\r\n";
            instance.ErrorDataReceived += (s, e) => report += e.Data + "\r\n";
            instance.Start();
            instance.BeginOutputReadLine();
            instance.BeginErrorReadLine();
            instance.WaitForExit();
            // avoid empty lines from console while exiting process
            return report.TrimEnd("\r\n".ToCharArray());
        }
    }
}