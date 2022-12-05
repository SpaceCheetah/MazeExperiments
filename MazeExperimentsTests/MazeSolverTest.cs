using MazeExperiments;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeExperimentsTests;
[TestClass]
public class MazeSolverTest {
    [TestMethod]
    public void ImpossibleTest() {
        //no connection, so no best path
        var maze = new RectangularMaze(1, 2);
        Assert.IsNull(MazeSolver.GetBestPath((0, 0), (0, 1), maze));
    }

    [TestMethod]
    public void SameNodeTest() {
        var maze = new RectangularMaze(1, 2);
        List<(int, int)> bestPath = MazeSolver.GetBestPath((0, 1), (0, 1), maze);
        Assert.AreEqual(1, bestPath.Count);
        Assert.AreEqual((0, 1), bestPath[0]);
    }

    [TestMethod]
    public void TrivialPathTest() {
        var maze = new RectangularMaze(1, 2);
        maze.SetConnection((0, 0), (0, 1));
        List<(int, int)> bestPath = MazeSolver.GetBestPath((0, 1), (0, 0), maze);
        Assert.AreEqual(2, bestPath.Count);
        Assert.AreEqual((0, 0), bestPath[0]); //path starts at goal
        Assert.AreEqual((0, 1), bestPath[1]); //path ends at start
    }

    [TestMethod]
    public void BacktrackTest() {
        /*-------
          |S|   |
          |   | |
          | --| |
          |   |G|
          -------*/
        //path from goal to start
        var intendedPath = new List<(int, int)>() {
            (2, 2),
            (2, 1),
            (2, 0),
            (1, 0),
            (1, 1),
            (0, 1),
            (0, 0)
        };
        var maze = new RectangularMaze(3, 3);
        (int, int) prev = intendedPath[0];
        foreach((int, int) node in intendedPath.Skip(1)) {
            maze.SetConnection(prev, node);
            prev = node;
        }
        maze.SetConnection((0, 1), (0, 2));
        maze.SetConnection((0, 2), (1, 2));
        Assert.IsTrue(MazeSolver.GetBestPath((0, 0), (2, 2), maze)
            .SequenceEqual(intendedPath));
    }
}
