using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MazeExperiments;

namespace MazeExperiments.Test {
    [TestClass]
    public class RectangularMazeTest {
        [TestMethod]
        public void TestSetConnection() {
            var maze = new RectangularMaze(5, 5);
        }
    }
}
