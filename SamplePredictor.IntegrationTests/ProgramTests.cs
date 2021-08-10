using LC.Predictor;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace SamplePredictor.IntegrationTests
{
    class ProgramTests
    {
        // Imported from SweetenerAspartame.txt
        internal static readonly double[] AspartameX = new double[] { 1751.09057617188, 1757.69799804688, 1764.29846191406, 1770.89208984375, 1777.47888183594, 1784.05871582031, 1790.63159179688, 1797.19775390625, 1803.75695800781, 1810.30920410156, 1816.85461425781, 1823.39318847656, 1829.92492675781, 1836.44982910156, 1842.9677734375, 1849.47888183594, 1855.98315429688, 1862.48046875, 1868.97106933594, 1875.45483398438, 1881.931640625, 1888.40173339844, 1894.86486816406, 1901.3212890625, };
        internal static readonly double[] AspartameY = new double[] { -0.034399289637804, -0.0367538370192051, -0.0383448004722595, -0.0394912138581276, -0.040363684296608, -0.0406774245202541, -0.0406552739441395, -0.0404726304113865, -0.0401344336569309, -0.039515171200037, -0.038575816899538, -0.0378936901688576, -0.0370917208492756, -0.0362767390906811, -0.0356259755790234, -0.0348698049783707, -0.0347286388278008, -0.0347655937075615, -0.0346791446208954, -0.0361259467899799, -0.0389033928513527, -0.0424059890210629, -0.0474696792662144, -0.052199799567461, };
        // Imported from Milk.txt
        internal static readonly double[] MilkDataX = new double[] { 1708.62145618313, 1712.47838942271, 1716.33532266229, 1720.19225590188, 1724.04918914146, 1727.90612238104, 1731.76305562062, 1735.6199888602, 1739.47692209978, 1743.33385533936, 1747.19078857894, 1751.04772181852, 1754.9046550581, 1758.76158829768, 1762.61852153726, 1766.47545477685, 1770.33238801643, 1774.18932125601, 1778.04625449559, 1781.90318773517, };
        internal static readonly double[] MilkDataY = new double[] { -0.0694859996438026, -0.0641269981861115, -0.0570180006325245, -0.0484939999878407, -0.03846700116992, -0.025203000754118, -0.00550100021064281, 0.0223040003329515, 0.0534729994833469, 0.0788810029625893, 0.0858410000801086, 0.0689370036125183, 0.0392739996314049, 0.0118979997932911, -0.00642400002107024, -0.0159629993140697, -0.0202820003032684, -0.0221009999513626, -0.0227830000221729, -0.0230509992688894, };

        class ConsoleOutput : IDisposable
        {
            private StringWriter stringWriter = new StringWriter();
            private TextWriter originalOutput = Console.Out;

            public ConsoleOutput() => 
                Console.SetOut(stringWriter);

            public string GetOuput() => 
                stringWriter.ToString();

            public void Dispose()
            {
                Console.SetOut(originalOutput);
                stringWriter.Dispose();
            }
        }

        [Test]
        public void LoadPredictionData_Milk_Equals_Expected_Test()
        {
            var actual = Program.LoadPredictionData(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Data\Milk.txt"));
            Assert.That(actual.x.Length, Is.EqualTo(MilkDataX.Length));
            Assert.That(actual.y.Length, Is.EqualTo(MilkDataY.Length));
            for (int i = 0; i < MilkDataX.Length; i++)
            {
                Assert.That(actual.x[i], Is.EqualTo(MilkDataX[i]));
                Assert.That(actual.y[i], Is.EqualTo(MilkDataY[i]));
            }
        }

        [Test]
        public void LoadPredictionData_Aspartame_Equals_Expected_Test()
        {
            var actual = Program.LoadPredictionData(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Data\SweetenerAspartame.txt"));
            Assert.That(actual.x.Length, Is.EqualTo(AspartameX.Length));
            Assert.That(actual.y.Length, Is.EqualTo(AspartameY.Length));
            for (int i = 0; i < AspartameX.Length; i++)
            {
                Assert.That(actual.x[i], Is.EqualTo(AspartameX[i]));
                Assert.That(actual.y[i], Is.EqualTo(AspartameY[i]));
            }
        }

        [Test]
        public void ResultToString_StringValueWithUnit_Equals_Expected_Test()
        {
            var source = new PredictionResult<string>
            {
                Constituent = "constituent",
                Property = "property",
                Value = "value",
                Unit = "%",
            };

            var actual = Program.ResultToString(source);
            Assert.That(actual, Is.EqualTo("property\tvalue [%]\tconstituent"));
        }

        [Test]
        public void ResultToString_ValueWithUnit_Equals_Expected_Test()
        {
            var source = new PredictionResult<double>
            {
                Constituent = "constituent",
                Property = "property",
                Value = 3.1,
                Unit = "%",
            };

            var actual = Program.ResultToString(source);
            Assert.That(actual, Is.EqualTo("property\t3.1 [%]\tconstituent"));
        }

        [Test]
        public void ResultToString_ConstituentValueWithoutUnit_Equals_Expected_Test()
        {
            var source = new PredictionResult<double>
            {
                Constituent = "constituent",
                Property = "property",
                Value = 3.1,
                Unit = null,
            };

            var actual = Program.ResultToString(source);
            Assert.That(actual, Is.EqualTo("property\t3.1\tconstituent"));
        }

        [Test]
        public void ResultToString_SpectralValueWithoutUnit_Equals_Expected_Test()
        {
            var source = new PredictionResult<double>
            {
                Constituent = null,
                Property = "property",
                Value = 3.1,
                Unit = null,
            };

            var actual = Program.ResultToString(source);
            Assert.That(actual, Is.EqualTo("property\t3.1"));
        }

        [Test]
        public void ResultToString_Null_Equals_None_Test() =>
            Assert.That(Program.ResultToString(null), Is.EqualTo("None"));

        [Test]
        public void ResultsToString_Null_Equals_None_Test() =>
            Assert.That(Program.ResultsToString(null, null), Is.EqualTo("No results!\r\n"));

        [Test]
        public void ResultsToString_SpectralValueWithoutUnit_Equals_Expected_Test()
        {
            var source = new IPredictionResult[] {
                new PredictionResult<double>
                {
                    Constituent = null,
                    Property = "property",
                    Value = 3.1,
                    Unit = null,
                }
            };

            var actual = Program.ResultsToString(source, "name");
            Assert.That(actual, Is.EqualTo(
                "Results for: name\r\n" +
                "Property\tValue [Unit]\tConstituent\r\n" +
                "property\t3.1\r\n"));
        }

        [Test]
        public void ResultsToString_Null_Equals_NoResult_Test()
        {
            var actual = Program.ResultsToString(null, "name");
            Assert.That(actual, Is.EqualTo("No results!\r\n"));
        }

        [Test]
        public void LoadData_NoFiles_IsNull_Test() => 
            Assert.That(Program.LoadData(null).ToArray(), Is.EqualTo(new (string name, double[] x, double[] y)[0]));

        [Test]
        public void LoadData_LoadMissingFile_Equals_ExpectedName_Test()
        {
            var unknownFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", Guid.NewGuid().ToString() + ".txt");
            var filePaths = new string[] {
                unknownFile,
            };

            string expectedName = $"The file '{unknownFile}' does not exist!\r\n";
            // grab console output
            using (var console = new ConsoleOutput())
            {
                var actual = Program.LoadData(filePaths).ToArray();
                Assert.That(actual.Length, Is.EqualTo(0));
                Assert.That(console.GetOuput(), Is.EqualTo(expectedName));
            }
        }

        [Test]
        public void LoadData_LoadInvalidFile_Equals_ExpectedName_Test()
        {
            var invalidFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");
            // create temp file
            File.WriteAllText(invalidFile, "1.23\t");

            var filePaths = new string[] {
                invalidFile,
            };

            string expectedName = $"The file '{invalidFile}' failed to load!\r\n";

            // grab console output
            using (var console = new ConsoleOutput())
            {
                var actual = Program.LoadData(filePaths).ToArray();
                Assert.That(actual.Length, Is.EqualTo(0));
                Assert.That(console.GetOuput(), Is.EqualTo(expectedName));
            }

            try
            {
                // cleanup temp file
                File.Delete(invalidFile);
            }
            catch { }
        }
    }
}
