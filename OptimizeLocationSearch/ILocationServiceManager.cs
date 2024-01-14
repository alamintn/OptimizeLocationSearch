namespace OptimizeLocationSearch
{
    public interface ILocationServiceManager<T> where T : class
    {
        /// <summary>
        /// Inserts a point into the location service manager.
        /// </summary>
        /// <param name="point">The point to be inserted.</param>
        void Insert(Point<T> point);

        /// <summary>
        /// Deletes a point from the location service manager.
        /// </summary>
        /// <param name="point">The point to be deleted.</param>
        void Delete(Point<T> point);

        /// <summary>
        /// Queries points within a specified radius from a given location.
        /// </summary>
        /// <param name="queryLatitude">The latitude of the query location.</param>
        /// <param name="queryLongitude">The longitude of the query location.</param>
        /// <param name="radius">The radius within which points are queried.</param>
        /// <returns>A list of points within the specified radius.</returns>
        List<Point<T>> QueryRadius(double queryLatitude, double queryLongitude, double radius);

        /// <summary>
        /// Queries points within a specified geographical range.
        /// </summary>
        /// <param name="range">The geographical range for the query.</param>
        /// <returns>A list of points within the specified range.</returns>
        List<Point<T>> QueryRange(Boundary<T> range);

        /// <summary>
        /// Clears all points from the location service manager.
        /// </summary>
        void ClearAll();

    }
}
