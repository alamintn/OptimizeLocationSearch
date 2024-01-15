using OptimizeLocationSearch;
using System.Diagnostics;

namespace OptimizeLocationSearchTest
{
    [TestClass]
    public class OptimizeLocationSearchTest
    {
        private ILocationServiceManager<Location>? locationServiceManager;

        [TestInitialize]
        public void TestInitialize()
        {
            locationServiceManager = new LocationServiceManager<Location>();
        }

        static double GetRandomNumber(double min, double max)
        {
            Random random = new Random();
            return random.NextDouble() * (max - min) + min;
        }
        static Point<Location> GenerateRandomDhakaPoint()
        {
            // Dhaka's approximate latitude and longitude
            var dhakaLatitude = GetRandomNumber(23.7, 23.9);
            var dhakaLongitude = GetRandomNumber(90.3, 90.5);

            var dhakaLocation = new Location { City = "Dhaka" };
            return new Point<Location>(new LocationPointOptions(dhakaLatitude, dhakaLongitude, dhakaLocation));
        }


        [TestMethod]
        public void TestInsertTimeComplexityMethod()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 1000000; i++)
            {
                locationServiceManager?.Insert(GenerateRandomDhakaPoint());
            }

            stopwatch.Stop();

            Console.WriteLine($"Time taken to insert 1,000,000 points: {stopwatch.Elapsed}");

            Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(5));
        }

        [TestMethod]
        public void TestSearchTimeComplexityMethod()
        {
            // Insert 1,000,000 random points before searching
            for (int i = 0; i < 100000; i++)
            {
                var point = GenerateRandomDhakaPoint();
                locationServiceManager?.Insert(point);
                Console.WriteLine(point.ToString());
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var searchPoint = GenerateRandomDhakaPoint();

            var searchResult = locationServiceManager?.QueryRadius(searchPoint.Latitude, searchPoint.Longitude, 5);
            
            stopwatch.Stop();

            Console.WriteLine($"Time taken to search 1,000,000 points: {stopwatch.Elapsed}, Result count => {searchResult?.Count}");

            // Add appropriate assertions based on your search operation
            Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(1));
        }

    }
}