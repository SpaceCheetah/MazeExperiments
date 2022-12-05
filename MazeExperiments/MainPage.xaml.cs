using System.Diagnostics;

namespace MazeExperiments;

public partial class MainPage : ContentPage {
    (int x, int y) _nodeStart = (0, 0);
    (int x, int y) _nodeEnd = (19, 19);
    RectangularMaze _maze;

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
            _maze = new RectangularMaze((int)WidthSlider.Value, (int)HeightSlider.Value);
            MazeGenerator.GenerateMaze(rand, _maze, (0, 0), JumpChanceSlider.Value, BreakChanceSlider.Value);
        }
        NormalizeNode(ref _nodeStart, (_maze.Width, _maze.Height));
        NormalizeNode(ref _nodeEnd, (_maze.Width, _maze.Height));
        
        List<(int, int)> solution = ShowSolution.IsChecked ? MazeSolver.GetBestPath(_nodeStart, _nodeEnd, _maze) : null;
        MazeView.Drawable = new RectangularMazeDrawable(_maze, _nodeStart, _nodeEnd, solution);
    }

    void OnValueChanged(object sender, EventArgs e) => UpdateMaze(true);

    void CheckChanged(object sender, CheckedChangedEventArgs e) => UpdateMaze(false);

    void MazeView_StartInteraction(object sender, TouchEventArgs e) {
        _nodeStart = ((RectangularMazeDrawable)MazeView.Drawable).HitTest(e.Touches[0].X, e.Touches[0].Y);
        _nodeEnd = _nodeStart;
        UpdateMaze(false);
    }

    void MazeView_DragInteraction(object sender, TouchEventArgs e) {
        (int x, int y) node = ((RectangularMazeDrawable)MazeView.Drawable).HitTest(e.Touches[0].X, e.Touches[0].Y);
        if (node == _nodeEnd || node == (-1, -1)) {
            return;
        }
        _nodeEnd = node;
        UpdateMaze(false);
    }
}

