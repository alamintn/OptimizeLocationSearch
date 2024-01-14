namespace OptimizeLocationSearch
{
    internal class QuadtreeNode<T> where T: class
    {
        public Boundary<T> Bounds { get; set; }
        public List<Point<T>> Points { get; set; }
        public QuadtreeNode<T>[] Children { get; set; }

        public QuadtreeNode(Boundary<T> bounds)
        {
            Bounds = bounds;
            Points = new List<Point<T>>();
            Children = null;
        }
    }
}
