using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {
                outPoints = points;

            }
            else
            {
                //finding unique points:

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

                // Initialize min and max points
                Point minX = points[0], maxX = points[0], minY = points[0], maxY = points[0];

                // Iterate over points to find min and max in x and y
                foreach (var point in points)
                {
                    if (point.X < minX.X) minX = point;
                    else if (point.X > maxX.X) maxX = point;

                    if (point.Y > maxY.Y) maxY = point;
                }

                foreach (var point in points)
                {
                    if (point.Y < minY.Y && !point.Equals(minX)) minY = point;
                }


                // Initializing 4 lines

                List<Line> tmpLines = new List<Line>();
                tmpLines.Add(new Line(minX, minY));
                tmpLines.Add(new Line(minY, maxX));
                tmpLines.Add(new Line(maxX, maxY));
                tmpLines.Add(new Line(maxY, minX));

                List<Line> newtmp = tmpLines;

                for (int k = 0; k < tmpLines.Count(); k++)
                {
                    if (tmpLines[k].Start.Equals(tmpLines[k].End))
                    {
                        newtmp.Remove(tmpLines[k]);
                    }
                }

                tmpLines = newtmp;

                List<Point> validPoints = new List<Point>();

                foreach (Point point in points)
                {
                    bool equal = false;

                    for (int l = 0; l < tmpLines.Count; l++)
                    {
                        if (tmpLines[l].Start.Equals(point))
                        {
                            equal = true;
                        }
                    }

                    if (!equal && !IsPointInside(point, tmpLines))
                    {
                        validPoints.Add(point);
                    }
                }

                outLines = Qh(validPoints, tmpLines);

                List<Point> Validpoints = new List<Point>();

                foreach (Line line in outLines)
                {
                    Validpoints.Add(line.Start);
                }

                List<Point> pointsToDelete = new List<Point>();

                if (Validpoints.Count <= 50)
                {

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
                }

                outPoints = Validpoints;



            }
        }

        public List<Line> Qh(List<Point> validPoints, List<Line> polygonLines)
        {

            List<Point> newValid = new List<Point>();
            List<Line> currLines = polygonLines;


            if (validPoints.Count == 0)
            {
                return polygonLines;
            }


            for (int i = 0; i < polygonLines.Count(); i++)
            {
                Point furthestP = new Point(0, 0);
                bool flag = false;
                Line line = polygonLines[i];

                foreach (Point p in validPoints)
                {
                    double currdist = 0;
                    Point tmpP = new Point(line.End.X - line.Start.X, line.End.Y - line.Start.Y);
                    Point tmpP2 = new Point(p.X - line.End.X, p.Y - line.End.Y);

                    if (!HelperMethods.CheckTurn(line, p).Equals(Enums.TurnType.Left))
                    {
                        double dist = EuclideanDistanceCalculator.CalculateDistance(line.Start, p);
                        dist += EuclideanDistanceCalculator.CalculateDistance(line.End, p);
                        if (dist > currdist)
                        {
                            currdist = dist;
                            furthestP = p;
                            flag = true;
                        }
                    }
                }

                if (flag)
                {
                    currLines.Insert(i, new Line(line.Start, furthestP));
                    currLines.Insert(i + 1, new Line(furthestP, line.End));
                    currLines.Remove(line);
                    validPoints.Remove(furthestP);

                }

            }

            List<Point> newtmpP = new List<Point>();

            for (int i = 0; i < validPoints.Count(); i++)
            {
                for (int j = 0; j < currLines.Count(); j++)
                {
                    if (!HelperMethods.PointOnSegment(validPoints[i], currLines[j].Start, currLines[j].End))
                    {
                        newtmpP.Add(validPoints[i]);
                    }
                }
            }

            validPoints = newtmpP;

            foreach (var point in validPoints)
            {
                if (!IsPointInside(point, currLines))
                {
                    newValid.Add(point);
                }
            }


            return Qh(newValid, currLines);
        }

        public bool IsPointInside(Point point, List<Line> polygonLines)
        {
            int intersectionCount = 0;

            foreach (var line in polygonLines)
            {
                // Check for intersection between the line and a horizontal ray from the point
                if (((line.Start.Y > point.Y) != (line.End.Y > point.Y)) &&
                    (point.X < (line.End.X - line.Start.X) * (point.Y - line.Start.Y) / (line.End.Y - line.Start.Y) + line.Start.X))
                {
                    intersectionCount++;
                }
            }

            // If the number of intersections is odd, the point is inside the polygon
            return intersectionCount % 2 == 1;
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
            return "Convex Hull - Quick Hull";
        }
    }
}
//quick hull
