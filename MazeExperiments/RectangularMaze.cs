using System.Collections.ObjectModel;

namespace MazeExperiments;
public class RectangularMaze : IMaze<(int x, int y)> {
    readonly bool[,] rightPassages; //rightWalls[x,y] is whether there is a passage at cell x,y
    readonly bool[,] downPassages; //similar, but for down

    public int Width { get; }
    public int Height { get; }

    public RectangularMaze(int width, int height) {
        Width = width;
        Height = height;
        rightPassages = new bool[width - 1, height];
        downPassages = new bool[width, height - 1];
    }

    public struct CellPassages {
        public bool Up, Right, Down, Left;
    }

    public CellPassages GetCellPassages(int x, int y) => new CellPassages {
        Left = x > 0 && rightPassages[x - 1, y],
        Right = x < Width - 1 && rightPassages[x, y],
        Up = y > 0 && downPassages[x, y - 1],
        Down = y < Height - 1 && downPassages[x, y]
    };

    public int EstimateDistance((int x, int y) nodeA, (int x, int y) nodeB) => Math.Abs(nodeA.x - nodeB.x) + Math.Abs(nodeA.y - nodeB.y);

    public List<(int x, int y)> GetNeighbors((int x, int y) node) {
        CellPassages passages = GetCellPassages(node.x, node.y);
        var neighbors = new List<(int, int)>();
        if (passages.Left) {
            neighbors.Add((node.x - 1, node.y));
        }
        if (passages.Right) {
            neighbors.Add((node.x + 1, node.y));
        }
        if (passages.Up) {
            neighbors.Add((node.x, node.y - 1));
        }
        if (passages.Down) {
            neighbors.Add((node.x, node.y + 1));
        }
        return neighbors;
    }

    public List<(int x, int y)> GetPossibleNeighbors((int x, int y) node) {
        CellPassages passages = GetCellPassages(node.x, node.y);
        var neighbors = new List<(int, int)>();
        if (node.x > 0 && !passages.Left) {
            neighbors.Add((node.x - 1, node.y));
        }
        if (node.x < Width - 1 && !passages.Right) {
            neighbors.Add((node.x + 1, node.y));
        }
        if (node.y > 0 && !passages.Up) {
            neighbors.Add((node.x, node.y - 1));
        }
        if (node.y < Height - 1 && !passages.Down) {
            neighbors.Add((node.x, node.y + 1));
        }
        return neighbors;
    }

    public void SetConnection((int x, int y) nodeA, (int x, int y) nodeB, bool set = true) {
        if (nodeA.x == nodeB.x) {
            int yDiff = nodeB.y - nodeA.y;
            if (yDiff == 1) {
                downPassages[nodeA.x, nodeA.y] = set;
                return;
            } else if (yDiff == -1) {
                downPassages[nodeA.x, nodeB.y] = set;
                return;
            }
        } else if (nodeA.y == nodeB.y) {
            int xDiff = nodeB.x - nodeA.x;
            if (xDiff == 1) {
                rightPassages[nodeA.x, nodeA.y] = set;
                return;
            } else if (xDiff == -1) {
                rightPassages[nodeB.x, nodeA.y] = set;
                return;
            }
        }
        throw new InvalidOperationException();
    }
}
