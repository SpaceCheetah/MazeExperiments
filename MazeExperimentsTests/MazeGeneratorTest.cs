using Microsoft.VisualStudio.TestTools.UnitTesting;
using MazeExperiments;

namespace MazeExperimentsTests;
[TestClass]
public class MazeGeneratorTest {
    [TestMethod]
    public void TestSmallMaze() {
        Random rand = new Random();
        var maze = new RectangularMaze(1, 2);
        MazeGenerator.GenerateMaze(rand, maze, (0, 0), 0, 0);
        var expected = new RectangularMaze.CellPassages();
        expected.Down = true;
        Assert.AreEqual(expected, maze.GetCellPassages(0, 0));
        expected.Down = false;
        expected.Up = true;
        Assert.AreEqual(expected, maze.GetCellPassages(0, 1));

        maze = new RectangularMaze(2, 2);
        /*Starting at bottom right, no jump chance, so garunteed to be one of these
         * -----
         * |   |
         * |  -|
         * |   |
         * -----
         * -----
         * |   |
         * | | |
         * -----
         */
        MazeGenerator.GenerateMaze(rand, maze, (1, 1), 0, 0);
        var passages = maze.GetCellPassages(1, 1);
        Assert.IsTrue(passages.Up ^ passages.Left); //one of, but not both, up or left
    }

    [TestMethod]
    public void TestSolvable() {
        var maze = new RectangularMaze(20, 20);
        MazeGenerator.GenerateMaze(new Random(), maze, (0, 0), 0.1, 0);
        for(int y = 0; y < maze.Height; y++) {
            for(int x = 0; x < maze.Width; x++) {
                Assert.IsFalse(MazeSolver.GetBestPath((0, 0), (x, y), maze) is null);
            }
        }
    }
}
