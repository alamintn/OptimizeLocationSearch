
namespace OptimizeLocationSearch
{
    public class LocationServiceManager<T> : ILocationServiceManager<T> where T : class
    {
        private readonly object quadtreeLock = new object();
        private Quadtree<T> quadtree;

        public LocationServiceManager()
        {
            quadtree = new Quadtree<T>(new Boundary<T>(-90, -180, 90, 180));
        }

        public void ClearAll()
        {
            lock (quadtreeLock)
            {
                if (quadtree != null)
                {
                    quadtree.ClearAll();
                }
            }
        }

        public void Delete(Point<T> point)
        {
            lock (quadtreeLock)
            {
                if (quadtree != null)
                {
                    quadtree.Delete(point);
                }
            }
        }

        public void Insert(Point<T> point)
        {
            lock (quadtreeLock)
            {
                if (quadtree != null)
                {
                    quadtree.Insert(point);
                }
            }
        }

        public List<Point<T>> QueryRadius(double queryLatitude, double queryLongitude, double radius)
        {
            List<Point<T>> searchResult = new List<Point<T>>();

            lock (quadtreeLock)
            {
                if (quadtree != null)
                {
                    searchResult = quadtree.QueryRadius(queryLatitude, queryLongitude, radius);
                }
            }

            return searchResult;
        }

        public List<Point<T>> QueryRange(Boundary<T> range)
        {
            List<Point<T>> searchResult = new List<Point<T>>();

            lock (quadtreeLock)
            {
                if (quadtree != null)
                {
                    searchResult = quadtree.QueryRange(range);
                }
            }

            return searchResult;
        }
    }

}
