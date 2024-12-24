using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gta_1
{
    internal class AStarSearch
    {
        public struct Cell
        {
            public int parent_i, parent_j;
            public double f, g, h;
        }

        public static Cell[,] cellDetails;

        public static bool AStar(bool[,] grid, Point origin, Point destination)
        {
            int ROW = grid.GetLength(0);
            int COL = grid.GetLength(1);

            if (!IsValid(origin.X, origin.Y, ROW, COL) || !IsValid(destination.X, destination.Y, ROW, COL))
                return false;

            if (!IsUnBlocked(grid, origin.X, origin.Y) || !IsUnBlocked(grid, destination.X, destination.Y))
                return false;

            if (origin.X == destination.X && origin.Y == destination.Y)
                return false;

            bool[,] closedList = new bool[ROW, COL];

            cellDetails = new Cell[ROW, COL];

            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COL; j++)
                {
                    cellDetails[i, j].f = double.MaxValue;
                    cellDetails[i, j].g = double.MaxValue;
                    cellDetails[i, j].h = double.MaxValue;
                    cellDetails[i, j].parent_i = -1;
                    cellDetails[i, j].parent_j = -1;
                }
            }

            int x = origin.X, y = origin.Y;
            cellDetails[x, y].f = 0.0;
            cellDetails[x, y].g = 0.0;
            cellDetails[x, y].h = 0.0;
            cellDetails[x, y].parent_i = x;
            cellDetails[x, y].parent_j = y;

            SortedSet<(double, Point)> openList = new SortedSet<(double, Point)>(Comparer<(double, Point)>.Create((a, b) => a.Item1.CompareTo(b.Item1)))
            {
                (0.0, new Point(x, y))
            };

            while (openList.Count > 0)
            {
                (double f, Point Point) p = openList.Min;
                openList.Remove(p);

                x = p.Point.X;
                y = p.Point.Y;
                closedList[x, y] = true;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                            continue;

                        int newX = x + i;
                        int newY = y + j;

                        if (!IsValid(newX, newY, ROW, COL))
                            continue;

                        if (IsDestination(newX, newY, destination))
                        {
                            cellDetails[newX, newY].parent_i = x;
                            cellDetails[newX, newY].parent_j = y;

                            return true;
                        }

                        if (closedList[newX, newY] || !IsUnBlocked(grid, newX, newY))
                            continue;

                        double gNew = cellDetails[x, y].g + 1.0;
                        double hNew = CalculateHValue(newX, newY, destination);
                        double fNew = gNew + hNew;

                        if (cellDetails[newX, newY].f != double.MaxValue && cellDetails[newX, newY].f <= fNew)
                            continue;

                        openList.Add((fNew, new Point(newX, newY)));

                        cellDetails[newX, newY].f = fNew;
                        cellDetails[newX, newY].g = gNew;
                        cellDetails[newX, newY].h = hNew;
                        cellDetails[newX, newY].parent_i = x;
                        cellDetails[newX, newY].parent_j = y;
                    }
                }
            }

            return false;
        }

        public static bool IsValid(int row, int col, int ROW, int COL)
        {
            return (row >= 0) && (row < ROW) && (col >= 0) && (col < COL);
        }

        public static bool IsUnBlocked(bool[,] grid, int row, int col)
        {
            return grid[row, col];
        }

        public static bool IsDestination(int row, int col, Point dest)
        {
            return (row == dest.X && col == dest.Y);
        }

        public static double CalculateHValue(int row, int col, Point dest)
        {
            return Math.Sqrt(Math.Pow(row - dest.X, 2) + Math.Pow(col - dest.Y, 2));
        }
    }
}