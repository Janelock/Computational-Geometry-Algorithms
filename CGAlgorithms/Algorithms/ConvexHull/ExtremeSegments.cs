using CGUtilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            List<Point> unique = new List<Point>();

            for (int i = 0; i < points.Count(); i++)
            {
                bool found = false;
                for (int j = 0; j < unique.Count(); j++)
                {
                    if (points[i].Equals(unique[j]))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    unique.Add(points[i]);
                }
            }

            points = unique;

            List<Point> Validpoints = new List<Point>();

            foreach (var point_i in points)
            {
                if (points.Count == 1)
                    Validpoints.Add(point_i);
                foreach (var point_j in points)
                {
                    if (!(point_j.Equals(point_i)))
                    {
                        bool l_side = false;
                        bool r_side = false;
                        lines.Add(new Line(point_i, point_j));

                        foreach (var point_k in points)
                        {
                            if (!(point_k.Equals(point_i)) && !(point_k.Equals(point_j)))
                            {
                                if (HelperMethods.CheckTurn(lines[0], point_k) == Enums.TurnType.Left)
                                {
                                    l_side = true;
                                }
                                if (HelperMethods.CheckTurn(lines[0], point_k) == Enums.TurnType.Right)
                                {
                                    r_side = true;
                                }
                            }
                        }

                        if (l_side == false || r_side == false)
                        {
                            if (!Validpoints.Contains(point_j))
                            {
                                Validpoints.Add(point_j);
                            }
                            if (!Validpoints.Contains(point_i))
                            {
                                Validpoints.Add(point_i);
                            }
                            if (!(outLines.Contains(new Line(point_i, point_j)) || outLines.Contains(new Line(point_j, point_i))))
                            {
                                outLines.Add(new Line(point_i, point_j));
                            }
                        }

                        lines.Clear();
                    }
                }
            }

            List<Point> pointsToDelete = new List<Point>();

            for (int i = 0; i < Validpoints.Count(); i++)
            {
                for (int j = 0; j < Validpoints.Count(); j++)
                {
                    for (int k = 0; k < Validpoints.Count(); k++)
                    {
                        if (i != j && j != k && i != k)
                        {
                            int toRemove = -1;
                            if (HelperMethods.PointOnSegment(Validpoints[i], Validpoints[j], Validpoints[k]))
                            {
                                toRemove = i;
                            }
                            else if (HelperMethods.PointOnSegment(Validpoints[j], Validpoints[i], Validpoints[k]))
                            {
                                toRemove = j;
                            }
                            else if (HelperMethods.PointOnSegment(Validpoints[k], Validpoints[j], Validpoints[i]))
                            {
                                toRemove = k;
                            }

                            if (toRemove != -1)
                            {
                                pointsToDelete.Add(Validpoints[toRemove]);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < pointsToDelete.Count; i++)
            {
                Validpoints.Remove(pointsToDelete[i]);
            }

            outPoints = Validpoints;
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
// extreme segments