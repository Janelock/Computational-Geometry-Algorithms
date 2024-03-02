using CGUtilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            //Getting unique points:

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

            List<bool> IrrelevantPoints = new List<bool>();

            for (int i = 0; i < points.Count(); i++)
            {
                IrrelevantPoints.Add(false);
            }

            for (int i = 0; i < points.Count(); i++)
            {
                for (int j = 0; j < points.Count(); j++)
                {
                    for (int k = 0; k < points.Count(); k++)
                    {
                        for (int l = 0; l < points.Count(); l++)
                        {
                            if (i != j && i != k && i != l && j != k && j != l && k != l)
                            {
                                CGUtilities.Enums.PointInPolygon calc = HelperMethods.PointInTriangle(points[i], points[j], points[k], points[l]);
                                if (calc == Enums.PointInPolygon.Inside)
                                {
                                    IrrelevantPoints[i] = true;
                                }
                            }
                        }
                    }
                }
            }

            List<Point> Validpoints = new List<Point>();

            for (int i = 0; i < IrrelevantPoints.Count(); i++)
            {
                if (!IrrelevantPoints[i])
                {
                    Validpoints.Add(points[i]);
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
            return "Convex Hull - Extreme Points";
        }
    }
}
//Extreme points