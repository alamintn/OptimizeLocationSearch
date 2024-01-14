namespace OptimizeLocationSearch
{
    internal class Quadtree<T> : IQuadtree<T> where T : class
    {
        private const int Capacity = 4; // Maximum number of points in a leaf node
        private QuadtreeNode<T> root;

        public Quadtree(Boundary<T> bounds)
        {
            root = new QuadtreeNode<T>(bounds);
        }

        // Insert a point into the Quadtree
        public void Insert(Point<T> point)
        {
            InsertRecursive(root, point);
        }

        private void InsertRecursive(QuadtreeNode<T> node, Point<T> point)
        {
            if (node.Children != null)
            {
                // If the node has children, insert into the appropriate child
                foreach (var child in node.Children)
                {
                    if (child.Bounds.Contains(point))
                    {
                        InsertRecursive(child, point);
                        return;
                    }
                }
            }

            // If the node is a leaf or a null node, insert the point
            node.Points.Add(point);

            // Split the node if it exceeds capacity
            if (node.Points.Count > Capacity)
            {
                SplitNode(node);
            }
        }

        // Split a node into four child nodes
        private void SplitNode(QuadtreeNode<T> node)
        {
            double latMid = (node.Bounds.LatitudeMin + node.Bounds.LatitudeMax) / 2;
            double lonMid = (node.Bounds.LongitudeMin + node.Bounds.LongitudeMax) / 2;

            node.Children = new QuadtreeNode<T>[4];
            node.Children[0] = new QuadtreeNode<T>(new Boundary<T>(latMid, lonMid, node.Bounds.LatitudeMax, node.Bounds.LongitudeMax)); // NE
            node.Children[1] = new QuadtreeNode<T>(new Boundary<T>(node.Bounds.LatitudeMin, lonMid, latMid, node.Bounds.LongitudeMax)); // NW
            node.Children[2] = new QuadtreeNode<T>(new Boundary<T>(node.Bounds.LatitudeMin, node.Bounds.LongitudeMin, latMid, lonMid)); // SW
            node.Children[3] = new QuadtreeNode<T>(new Boundary<T>(latMid, node.Bounds.LongitudeMin, node.Bounds.LatitudeMax, lonMid)); // SE

            // Reinsert points into children
            foreach (var point in node.Points)
            {
                foreach (var child in node.Children)
                {
                    if (child.Bounds.Contains(point))
                    {
                        InsertRecursive(child, point);
                        break;
                    }
                }
            }

            node.Points.Clear();
        }

        // Search for points within a specified range
        public List<Point<T>> QueryRange(Boundary<T> range)
        {
            List<Point<T>> result = new List<Point<T>>();
            QueryRangeRecursive(root, range, result);
            return result;
        }

        private void QueryRangeRecursive(QuadtreeNode<T> node, Boundary<T> range, List<Point<T>> result)
        {
            if (node == null)
            {
                return;
            }

            foreach (var point in node.Points)
            {
                if (range.Contains(point))
                {
                    result.Add(point);
                }
            }

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    if (child.Bounds.intersects(range))
                    {
                        QueryRangeRecursive(child, range, result);
                    }
                }
            }
        }


        public void Delete(Point<T> point)
        {
            DeleteRecursive(root, point);
        }

        private bool DeleteRecursive(QuadtreeNode<T> node, Point<T> point)
        {
            if (node == null)
            {
                return false;
            }

            if (node.Children != null)
            {
                // If the node has children, find the appropriate child and attempt deletion
                foreach (var child in node.Children)
                {
                    if (child.Bounds.Contains(point))
                    {
                        if (DeleteRecursive(child, point))
                        {
                            // Check if the child node is empty after the deletion
                            if (child.Points.Count == 0 && AllChildrenAreLeaves(child))
                            {
                                // If the child is empty and all its children are leaves, prune it
                                PruneChild(node, child);
                            }
                            return true;
                        }
                        break;
                    }
                }
            }

            // If the node is a leaf and contains the point, remove it
            if (node.Points.Remove(point))
            {
                return true;
            }

            return false;
        }


        // Check if all children of a node are leaves
        private bool AllChildrenAreLeaves(QuadtreeNode<T> node)
        {
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    if (child.Children != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Prune a child node from the parent
        private void PruneChild(QuadtreeNode<T> parent, QuadtreeNode<T> child)
        {
            parent.Children = null;
            parent.Points.AddRange(child.Points);
        }

        public List<Point<T>> QueryRadius(double queryLatitude, double queryLongitude, double radius)
        {
            List<Point<T>> result = new List<Point<T>>();
            QueryRadiusRecursive(root, queryLatitude, queryLongitude, radius, result);
            return result;
        }

        private void QueryRadiusRecursive(QuadtreeNode<T> node, double queryLatitude, double queryLongitude, double radius, List<Point<T>> result)
        {
            if (node == null)
            {
                return;
            }

            foreach (var point in node.Points)
            {
                if (IsWithinRadius(point, queryLatitude, queryLongitude, radius))
                {
                    result.Add(point);
                }
            }

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    if (IsNodeWithinRadius(child, queryLatitude, queryLongitude, radius))
                    {
                        QueryRadiusRecursive(child, queryLatitude, queryLongitude, radius, result);
                    }
                }
            }
        }

        // Check if a point is within a specified radius from a given location
        private bool IsWithinRadius(Point<T> point, double queryLatitude, double queryLongitude, double radius)
        {
            double distance = HaversineDistance(point.Latitude, point.Longitude, queryLatitude, queryLongitude);
            return distance <= radius;
        }

        private double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Radius of the Earth in kilometers
            double earthRadius = 6371;

            // Convert latitude and longitude from degrees to radians
            double lat1Rad = ToRadians(lat1);
            double lon1Rad = ToRadians(lon1);
            double lat2Rad = ToRadians(lat2);
            double lon2Rad = ToRadians(lon2);

            // Calculate differences in coordinates
            double deltaLat = lat2Rad - lat1Rad;
            double deltaLon = lon2Rad - lon1Rad;

            // Haversine formula
            double a = Math.Pow(Math.Sin(deltaLat / 2), 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Pow(Math.Sin(deltaLon / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Calculate the distance
            double distance = earthRadius * c;

            return distance;
        }

        // Helper method to convert degrees to radians
        private double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        private double PointToRectangleDistance(Boundary<T> bounds, double queryLatitude, double queryLongitude)
        {
            // Find the closest point on the rectangle to the query point
            double closestLatitude = Math.Max(bounds.LatitudeMin, Math.Min(queryLatitude, bounds.LatitudeMax));
            double closestLongitude = Math.Max(bounds.LongitudeMin, Math.Min(queryLongitude, bounds.LongitudeMax));

            // Calculate the distance between the query point and the closest point on the rectangle
            double distance = HaversineDistance(queryLatitude, queryLongitude, closestLatitude, closestLongitude);

            return distance;
        }

        // Check if a QuadtreeNode's bounds intersect with a circle of a specified radius from a given location
        private bool IsNodeWithinRadius(QuadtreeNode<T> node, double queryLatitude, double queryLongitude, double radius)
        {
            double distance = PointToRectangleDistance(node.Bounds, queryLatitude, queryLongitude);
            return distance <= radius;
        }

        #region ClearAll
        public void ClearAll()
        {
            ClearAllRecursive(root);
        }

        private void ClearAllRecursive(QuadtreeNode<T> node)
        {
            if (node == null)
            {
                return;
            }

            // Clear points in the current node
            node.Points.Clear();

            // Recursively clear points in children
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    ClearAllRecursive(child);
                }
                // Reset children to null since all points are cleared
                node.Children = null;
            }
        }

        #endregion ClearAll
    }

    internal interface IQuadtree<T> where T : class
    {
        void Insert(Point<T> point);
        void Delete(Point<T> point);
        List<Point<T>> QueryRadius(double queryLatitude, double queryLongitude, double radius);
        List<Point<T>> QueryRange(Boundary<T> range);
        void ClearAll();
    }
}
