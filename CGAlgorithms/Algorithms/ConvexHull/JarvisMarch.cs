using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            points = points.Distinct().ToList();

            //Find smallest x point:
            Point beginning = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X < beginning.X)
                {
                    beginning = points[i];
                }
            }

            List<Point> Validpoints = new List<Point>();

            Validpoints.Add(beginning);
            Point curr = beginning;

            while (true)
            {
                Point nextPoint = points[0];


                for (int i = 1; i < points.Count(); i++)
                {
                    if (nextPoint.Equals(points[i]))
                        continue;

                    Point curr_nextPoint = new Point(nextPoint.X - curr.X, nextPoint.Y - curr.Y);
                    Point curr_point_i = new Point(points[i].X - curr.X, points[i].Y - curr.Y);


                    double crossP = HelperMethods.CrossProduct(curr_nextPoint, curr_point_i);

                    if (crossP > 0)
                    {
                        nextPoint = points[i];
                    }
                    else if (crossP == 0)
                    {
                        if (EuclideanDistanceCalculator.CalculateDistance(curr, nextPoint) < EuclideanDistanceCalculator.CalculateDistance(curr, points[i]))
                        {
                            nextPoint = points[i];
                        }
                    }

                }


                if (nextPoint.Equals(beginning))
                {
                    break;
                }

                curr = nextPoint;
                Validpoints.Add(curr);

            }



            outPoints = Validpoints;

        }

        public static class EuclideanDistanceCalculator
        {
            public static double CalculateDistance(Point point1, Point point2)
            {
                double deltaX = point1.X - point2.X;
                double deltaY = point1.Y - point2.Y;
                return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            }
        }


        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
//jarvis