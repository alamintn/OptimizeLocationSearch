namespace OptimizeLocationSearch
{
    public class Boundary<T> where T : class
    {
        public double LatitudeMin { get; }
        public double LongitudeMin { get; }
        public double LatitudeMax { get; }
        public double LongitudeMax { get; }

        public Boundary(double latMin, double lonMin, double latMax, double lonMax)
        {
            LatitudeMin = latMin;
            LongitudeMin = lonMin;
            LatitudeMax = latMax;
            LongitudeMax = lonMax;
        }

        public bool Contains(Point<T> point)
        {
            return point.Latitude >= LatitudeMin && point.Latitude <= LatitudeMax &&
                   point.Longitude >= LongitudeMin && point.Longitude <= LongitudeMax;
        }

        public bool intersects(Boundary<T> other)
        {
            return !(other.LatitudeMin > LatitudeMax || other.LatitudeMax < LatitudeMin ||
                     other.LongitudeMin > LongitudeMax || other.LongitudeMax < LongitudeMin);
        }
    }
}
