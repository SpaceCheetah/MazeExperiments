using Microsoft.VisualStudio.TestTools.UnitTesting;

using MazeExperiments;

namespace MazeExperimentsTests;

[TestClass]
public class RectangularMazeTests {
    [TestMethod]
    public void GetCellPassagesTest() {
        var maze = new RectangularMaze(5, 5);
        var expected = new RectangularMaze.CellPassages();
        for (int y = 0; y < maze.Height; y++) {
            for (int x = 0; x < maze.Width; x++) {
                Assert.AreEqual(expected, maze.GetCellPassages(x, y));
            }
        }
        maze.SetConnection((0, 0), (0, 1));
        expected.Down = true;
        Assert.AreEqual(expected, maze.GetCellPassages(0, 0));
        expected.Down = false;
        expected.Up = true;
        Assert.AreEqual(expected, maze.GetCellPassages(0, 1));
    }

    [TestMethod]
    public void EstimateDistanceTest() {
        Assert.AreEqual(7, new RectangularMaze(1, 1)
            .EstimateDistance((3, 7), (7, 4)));
    }

    [TestMethod]
    public void GetNeighborsTest() {
        var maze = new RectangularMaze(5, 5);
        for (int y = 0; y < maze.Height; y++) {
            for (int x = 0; x < maze.Width; x++) {
                Assert.IsTrue(maze.GetNeighbors((x, y)).Count == 0);
            }
        }
        maze.SetConnection((0, 0), (0, 1));
        Assert.AreEqual((0, 1), maze.GetNeighbors((0, 0))[0]);
        Assert.AreEqual((0, 0), maze.GetNeighbors((0, 1))[0]);
    }

    [TestMethod]
    public void GetPossibleNeighborsTest() {
        var maze = new RectangularMaze(5, 5);
        Assert.IsTrue(maze.GetPossibleNeighbors((0, 0)).Contains((0, 1)));
        Assert.IsTrue(maze.GetPossibleNeighbors((0, 1)).Contains((0, 0)));
        Assert.AreEqual(2, maze.GetPossibleNeighbors((0, 0)).Count);
        Assert.AreEqual(4, maze.GetPossibleNeighbors((1, 1)).Count);
        maze.SetConnection((0, 0), (0, 1));
        Assert.IsFalse(maze.GetPossibleNeighbors((0, 0)).Contains((0, 1)));
        Assert.IsFalse(maze.GetPossibleNeighbors((0, 1)).Contains((0, 0)));
    }

    [TestMethod]
    public void SetConnectionTest() {
        var maze = new RectangularMaze(5, 5);
        maze.SetConnection((1, 0), (0, 0));
        Assert.IsTrue(maze.GetCellPassages(0, 0).Right);
        maze.SetConnection((1, 0), (0, 0), false);
        Assert.IsFalse(maze.GetCellPassages(0, 0).Right);
        Assert.ThrowsException<InvalidOperationException>(
            () => maze.SetConnection((0, 0), (1, 1)));
        Assert.ThrowsException<InvalidOperationException>(
            () => maze.SetConnection((1, 3), (1, 1)));
    }
}