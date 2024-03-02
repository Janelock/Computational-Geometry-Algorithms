using CGUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
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




                // Sort the input points by X-coordinate to start with the leftmost point.

                List<Point> orderedPoints = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();



                //first triangle

                List<Point> tmp = new List<Point>();

                tmp.Add(orderedPoints[0]);
                tmp.Add(orderedPoints[1]);
                tmp.Add(orderedPoints[2]);

                Point firstpoint = orderedPoints[0];
                tmp = SortPointsCounterclockwise(tmp);

                bool go = false;
                List<Point> tmp2 = new List<Point>();

                for (int i = 0; i < tmp.Count(); i++)
                {
                    if (tmp[i].Equals(firstpoint))
                    {
                        go = true;
                    }
                    if (go)
                    {
                        tmp2.Add(tmp[i]);
                    }
                }

                for (int i = 0; i < tmp.Count(); i++)
                {

                    if (tmp[i].Equals(firstpoint))
                    {
                        break;
                    }

                    tmp2.Add(tmp[i]);

                }

                tmp = tmp2;

                List<Line> tmpLines = new List<Line>();

                tmpLines.Add(new Line(tmp[0], tmp[1]));
                tmpLines.Add(new Line(tmp[1], tmp[2]));
                tmpLines.Add(new Line(tmp[2], tmp[0]));



                // Iterate through the remaining points.

                for (int i = 3; i < orderedPoints.Count; i++)
                {
                    bool flag = false;
                    bool pr_found = false;
                    Point pl = null;
                    Point pr = null;
                    int prev = -1;
                    for (int j = 1; j <= tmp.Count; j++)
                    {
                        if (j == tmp.Count) { prev = j - 1; j = 0; } else { prev = j - 1; }
                        if (HelperMethods.CheckTurn(tmpLines[prev], orderedPoints[i]) == Enums.TurnType.Colinear)
                        {
                            if (flag)
                            {
                                double dis1 = calculate_distance(orderedPoints[i], tmpLines[prev].Start);
                                double dis2 = calculate_distance(orderedPoints[i], tmpLines[prev].End);
                                if (dis1 > dis2)
                                {
                                    pr = tmpLines[prev].Start;
                                }
                                else { pr = tmpLines[prev].End; }
                            }
                            else
                            {
                                pl = tmpLines[prev].Start;
                                flag = true;
                            }

                        }
                        else if (HelperMethods.CheckTurn(tmpLines[j], orderedPoints[i]) == Enums.TurnType.Colinear)
                        {
                            if (prev != tmp.Count - 1) continue;

                        }
                        else if (HelperMethods.CheckTurn(tmpLines[prev], orderedPoints[i]) == Enums.TurnType.Left && HelperMethods.CheckTurn(tmpLines[j], orderedPoints[i]) == Enums.TurnType.Right)
                        {
                            pl = tmpLines[j].Start;
                            flag = true;
                        }
                        else if (HelperMethods.CheckTurn(tmpLines[prev], orderedPoints[i]) == Enums.TurnType.Right && HelperMethods.CheckTurn(tmpLines[j], orderedPoints[i]) == Enums.TurnType.Left)
                        {
                            pr = tmpLines[j].Start;
                        }


                        if (prev == tmp.Count - 1) break;
                    }

                    //chain from pr to pl deletion:

                    List<Line> filteredLines = new List<Line>();
                    bool start = false;

                    for (int j = 0; j < tmpLines.Count(); j++)
                    {
                        if (tmpLines[j].Start.Equals(pl))
                        {
                            start = true;
                        }
                        if (start)
                        {
                            if (tmpLines[j].Start.Equals(pl))
                            {
                                filteredLines.Add(new Line(pl, orderedPoints[i]));
                            }
                            if (tmpLines[j].End.Equals(pr))
                            {
                                filteredLines.Add(new Line(orderedPoints[i], pr));
                                start = false;
                            }
                        }
                        else
                        {
                            filteredLines.Add(tmpLines[j]);
                        }
                    }


                    tmpLines = filteredLines;

                    tmp.Clear();
                    for (int l = 0; l < tmpLines.Count; l++)
                    {
                        tmp.Add(tmpLines[l].Start);
                    }

                }

                outPoints = tmp;
            }


        }

        public static List<Point> SortPointsCounterclockwise(List<Point> points)
        {
            if (points.Count < 3)
            {
                // Handle special cases.
                return points;
            }

            // Calculate the center of the points.
            double X_avg = points.Select(p => p.X).Average();
            double Y_avg = points.Select(p => p.Y).Average();

            // Calculate angles and sort the points based on angles.
            points = points.OrderBy(p =>
            {
                double angle = Math.Atan2(p.Y - Y_avg, p.X - X_avg);
                return (angle < 0) ? angle + 2 * Math.PI : angle;
            }).ToList();

            return points;
        }

        public double calculate_distance(Point A, Point B)
        {
            double deltaX = B.X - A.X;
            double deltaY = B.Y - A.Y;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            return distance;
        }



        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }

    }
}


//incremental