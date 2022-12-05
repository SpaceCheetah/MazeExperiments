using System.Diagnostics;

namespace MazeExperiments;

public partial class MainPage : ContentPage {
    (int x, int y) nodeStart = (0, 0);
    (int x, int y) nodeEnd = (19, 19);
    RectangularMaze maze;

	public MainPage() {
		InitializeComponent();
        UpdateMaze(true);
	}

    static void NormalizeNode(ref (int x, int y) node, (int x, int y) max) {
        if(node.x < 0) {
            node.x = 0;
        }
        if(node.y < 0) {
            node.y = 0;
        }
        if(node.x >= max.x) {
            node.x = max.x - 1;
        }
        if (node.y >= max.y) {
            node.y = max.y - 1;
        }
    }

    void UpdateMaze(bool generationUpdated) {
        if(generationUpdated) {
            //Recreate the random each time, as a maze with the same paramaters should be the same
            var rand = new Random((int)SeedSlider.Value);
            maze = new RectangularMaze((int)WidthSlider.Value, (int)HeightSlider.Value);
            MazeGenerator.GenerateMaze(rand, maze, (0, 0), JumpChanceSlider.Value, BreakChanceSlider.Value);
        }
        NormalizeNode(ref nodeStart, (maze.Width, maze.Height));
        NormalizeNode(ref nodeEnd, (maze.Width, maze.Height));
        
        List<(int, int)> solution = ShowSolution.IsChecked ? MazeSolver.GetBestPath(nodeStart, nodeEnd, maze) : null;
        MazeView.Drawable = new RectangularMazeDrawable(maze, nodeStart, nodeEnd, solution);
    }

    void OnValueChanged(object sender, EventArgs e) => UpdateMaze(true);

    void CheckChanged(object sender, CheckedChangedEventArgs e) => UpdateMaze(false);

    void MazeView_StartInteraction(object sender, TouchEventArgs e) {
        nodeStart = ((RectangularMazeDrawable)MazeView.Drawable).HitTest(e.Touches[0].X, e.Touches[0].Y);
        nodeEnd = nodeStart;
        UpdateMaze(false);
    }

    void MazeView_DragInteraction(object sender, TouchEventArgs e) {
        (int x, int y) node = ((RectangularMazeDrawable)MazeView.Drawable).HitTest(e.Touches[0].X, e.Touches[0].Y);
        if (node == nodeEnd || node == (-1, -1)) {
            return;
        }
        nodeEnd = node;
        UpdateMaze(false);
    }
}

