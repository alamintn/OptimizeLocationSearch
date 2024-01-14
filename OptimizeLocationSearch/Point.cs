namespace OptimizeLocationSearch
{
    public class Point<T> where T : class
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public T? Value { get; }

        public Point(IPointOptions<T> options)
        {
            Latitude = options.Latitude;
            Longitude = options.Longitude;
            Value = options.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Point<T> other = (Point<T>)obj;
            return Latitude == other.Latitude &&
                   Longitude == other.Longitude &&
                   EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }


    public interface IPointOptions<T> where T: class
    {
        double Latitude { get; }
        public double Longitude { get; }
        public T? Value { get; }
    }
}
