using OptimizeLocationSearch;

namespace OptimizeLocationSearchTest
{
    public class Location
    {
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class LocationPointOptions : IPointOptions<Location>
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public Location? Value { get; }

        public LocationPointOptions(double latitude, double longitude, Location? value)
        {
            Latitude = latitude;
            Longitude = longitude;
            Value = value;
        }
    }

}
