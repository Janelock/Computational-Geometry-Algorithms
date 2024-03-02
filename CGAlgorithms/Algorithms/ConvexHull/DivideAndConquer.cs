using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        private List<Point> Merge(List<Point> leftList, List<Point> rightList)
        {
            int rightMostIndex = 0;
            int leftMostIndex = 0;

            for (int i = 1; i < leftList.Count; i++)
            {
                if (leftList[i].X > leftList[rightMostIndex].X)
                    rightMostIndex = i;
                else if (leftList[i].X == leftList[rightMostIndex].X && leftList[i].Y > leftList[rightMostIndex].Y)
                    rightMostIndex = i;
            }

            for (int i = 1; i < rightList.Count; i++)
            {
                if (rightList[i].X < rightList[leftMostIndex].X)
                    leftMostIndex = i;
                else if (rightList[i].X == rightList[leftMostIndex].X && rightList[i].Y < rightList[leftMostIndex].Y)
                    leftMostIndex = i;
            }

            int upperA = rightMostIndex, upperB = leftMostIndex;
            bool isUpperFound = false;
            while (!isUpperFound)
            {
                isUpperFound = true;

                if (HelperMethods.CheckTurn(new Line(rightList[upperB], leftList[upperA]),
                           leftList[(upperA + 1) % leftList.Count]) == Enums.TurnType.Colinear)
                    upperA = (upperA + 1) % leftList.Count;

                int i = upperB > 0 ? upperB - 1 : rightList.Count - 1;
                if (HelperMethods.CheckTurn(new Line(leftList[upperA], rightList[upperB]), rightList[i]) == Enums.TurnType.Colinear)
                {
                    upperB = upperB > 0 ? upperB - 1 : rightList.Count - 1;
                    i = upperB > 0 ? upperB - 1 : rightList.Count - 1;
                }

                while (HelperMethods.CheckTurn(new Line(rightList[upperB], leftList[upperA]),
                    leftList[(upperA + 1) % leftList.Count]) == Enums.TurnType.Right)
                {
                    upperA = (upperA + 1) % leftList.Count;
                    isUpperFound = false;
                }

                while (HelperMethods.CheckTurn(new Line(leftList[upperA], rightList[upperB]), rightList[i]) == Enums.TurnType.Left)
                {
                    upperB = upperB > 0 ? upperB - 1 : rightList.Count - 1;
                    i = upperB > 0 ? upperB - 1 : rightList.Count - 1;
                    isUpperFound = false;
                }
            }

            int lowerA = rightMostIndex, lowerB = leftMostIndex;
            bool isLowerFound = false;
            while (!isLowerFound)
            {
                isLowerFound = true;
                int i = lowerA > 0 ? lowerA - 1 : leftList.Count - 1;

                if (HelperMethods.CheckTurn(new Line(rightList[lowerB], leftList[lowerA]), leftList[i]) == Enums.TurnType.Colinear)
                {
                    lowerA = lowerA > 0 ? lowerA - 1 : leftList.Count - 1;
                    i = lowerA > 0 ? lowerA - 1 : leftList.Count - 1;
                }

                if (HelperMethods.CheckTurn(new Line(leftList[lowerA], rightList[lowerB]),
                    rightList[(lowerB + 1) % rightList.Count]) == Enums.TurnType.Colinear)
                    lowerB = (lowerB + 1) % rightList.Count;

                while (HelperMethods.CheckTurn(new Line(rightList[lowerB], leftList[lowerA]), leftList[i]) == Enums.TurnType.Left)
                {
                    lowerA = lowerA > 0 ? lowerA - 1 : leftList.Count - 1;
                    i = lowerA > 0 ? lowerA - 1 : leftList.Count - 1;
                    isLowerFound = false;
                }

                while (HelperMethods.CheckTurn(new Line(leftList[lowerA], rightList[lowerB]),
                    rightList[(lowerB + 1) % rightList.Count]) == Enums.TurnType.Right)
                {
                    lowerB = (lowerB + 1) % rightList.Count;
                    isLowerFound = false;
                }
            }

            List<Point> result = new List<Point>();

            int j = upperA;
            while (j != lowerA)
            {
                result.Add(leftList[j]);
                j = j < leftList.Count - 1 ? j + 1 : 0;
            }
            result.Add(leftList[lowerA]);

            j = lowerB;
            while (j != upperB)
            {
                result.Add(rightList[j]);
                j = j < rightList.Count - 1 ? j + 1 : 0;
            }
            result.Add(rightList[upperB]);

            return result;
        }

        private List<Point> DivideAndConquerRecursive(List<Point> lst)
        {
            if (lst.Count == 1)
                return lst;

            int end = lst.Count % 2 == 0 ? lst.Count / 2 : lst.Count / 2 + 1;
            return Merge(DivideAndConquerRecursive(lst.GetRange(0, lst.Count / 2)),
                DivideAndConquerRecursive(lst.GetRange(lst.Count / 2, end)));
        }

        public override void Run(List<Point> inputPoints, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (inputPoints.Count == 1)
            {
                outPoints = inputPoints;
                return;
            }

            List<Point> sortedPoints = inputPoints.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            List<Point> convexHull = DivideAndConquerRecursive(sortedPoints);

            foreach (var point in convexHull)
            {
                if (!outPoints.Contains(point))
                {
                    outPoints.Add(point);
                }
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }
    }
}
//d&c