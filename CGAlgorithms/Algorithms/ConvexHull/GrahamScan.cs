using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            Stack<Point> endPoint = new Stack<Point>();
            if (points.Count() <= 3)
            {
                outPoints = points;
            }
            else
            {
                Point extreme_y_point = findExtremePoint(points);

                endPoint.Push(extreme_y_point);

                Line line_1 = new Line(new Point(extreme_y_point.X - 2, extreme_y_point.Y), extreme_y_point);
                Point vector1 = HelperMethods.GetVector(line_1);
                List<KeyValuePair<double, Point>> sortedPoints = new List<KeyValuePair<double, Point>>();

                for (int i = 0; i < points.Count(); i++)
                {
                    if (points[i].X != extreme_y_point.X || points[i].Y != extreme_y_point.Y)
                    {
                        Line line_2 = new Line(extreme_y_point, points[i]);
                        Point vector2 = HelperMethods.GetVector(line_2);
                        double angle = CalculateAngle(vector1, vector2);
                        sortedPoints.Add(new KeyValuePair<double, Point>(angle, points[i]));
                    }
                }

                sortedPoints.Sort(sortByAngleValue);
                KeyValuePair<double, Point> basePoint = sortedPoints[0];
                List<Point> finalpoints = new List<Point>();

                for (int i = 1; i < sortedPoints.Count; i++)
                {
                    if (basePoint.Key == sortedPoints[i].Key)
                    {
                        double d1 = calculateDistance(extreme_y_point, basePoint.Value);
                        double d2 = calculateDistance(extreme_y_point, sortedPoints[i].Value);
                        if (d1 != 0 && d2 != 0)
                        {
                            if (d1 < d2)
                            {
                                basePoint = sortedPoints[i];
                            }
                        }
                        if (i == sortedPoints.Count() - 1)
                        {
                            finalpoints.Add(basePoint.Value);
                        }
                    }
                    else
                    {
                        finalpoints.Add(basePoint.Value);
                        basePoint = sortedPoints[i];
                        if (i == sortedPoints.Count() - 1)
                        {
                            finalpoints.Add(sortedPoints[i].Value);
                        }
                    }
                }


                for (int i = 0; i < finalpoints.Count; i++)
                {
                    bool found = false;
                    if (finalpoints[i] == sortedPoints[0].Value)
                    {
                        endPoint.Push(finalpoints[i]);
                    }
                    else
                    {
                        while (!found && endPoint.Count() > 0)
                        {
                            Point topPoint = endPoint.Pop();
                            Point prevPoint = null;
                            if (endPoint.Count() > 0)
                            {
                                prevPoint = endPoint.Peek();
                            }
                            else
                            {
                                prevPoint = line_1.Start;
                            }
                            Line newline = new Line(prevPoint, topPoint);
                            if (HelperMethods.CheckTurn(newline, finalpoints[i]) == Enums.TurnType.Left)
                            {
                                endPoint.Push(topPoint);
                                endPoint.Push(finalpoints[i]);
                                found = true;
                            }
                            else if (HelperMethods.CheckTurn(newline, finalpoints[i]) == Enums.TurnType.Colinear)
                            {
                                found = true;
                                double d1 = calculateDistance(extreme_y_point, finalpoints[i]);
                                double d2 = calculateDistance(extreme_y_point, topPoint);
                                if (d1 != 0 && d2 != 0)
                                {
                                    if (d1 > d2)
                                    {
                                        endPoint.Push(finalpoints[i]);
                                    }
                                    else
                                    {
                                        endPoint.Push(topPoint);
                                    }
                                }
                            }

                        }
                    }
                }
                outPoints = endPoint.ToList();
            }
        }

        public Point findExtremePoint(List<Point> inputPoints)
        {
            double MinY = 1000000;
            Point extremePoint = null;
            foreach (Point p in inputPoints)
            {
                if (p.Y < MinY)
                {
                    MinY = p.Y;
                    extremePoint = p;
                }
                else if (p.Y == MinY)
                {
                    if (p.X < extremePoint.X)
                    {
                        extremePoint = p;
                    }
                }

            }
            return extremePoint;
        }
        public double CalculateAngle(Point vector1, Point vector2)
        {
            double dot_product = vector1.X * vector2.X + vector1.Y * vector2.Y;
            double mag1 = vector1.Magnitude();
            double mag2 = vector2.Magnitude();
            double costheta = dot_product / (mag1 * mag2);
            double thetaRad = Math.Acos(costheta);
            double thetadeg = thetaRad * (180.0 / Math.PI);

            return thetadeg;
        }
        static int sortByAngleValue(KeyValuePair<double, Point> p1, KeyValuePair<double, Point> p2)
        {
            return p1.Key.CompareTo(p2.Key);
        }
        public double calculateDistance(Point A, Point B)
        {
            double deltaX = B.X - A.X;
            double deltaY = B.Y - A.Y;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            return distance;
        }
        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
//graham