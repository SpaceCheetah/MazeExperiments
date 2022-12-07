namespace MazeExperiments;

public partial class CircularMazePage : ContentPage
{
    (int level, int section) nodeStart = (3, 0);
    (int level, int section) nodeEnd = (0, 0);
    CircularMaze maze;

	public CircularMazePage()
	{
		InitializeComponent();
		UpdateMaze(true);
	}

	void UpdateMaze(bool generationUpdated) {
        if(generationUpdated) {
            var rand = new Random((int)SeedSlider.Value);
            maze = new CircularMaze((int)LayerSlider.Value, (int)BaseNodesSlider.Value);
            MazeGenerator.GenerateMaze(rand, maze, (0, 0), JumpChanceSlider.Value, BreakChanceSlider.Value);
        }
        NormalizeNode(ref nodeStart, maze);
        NormalizeNode(ref nodeEnd, maze);

        List<(int, int)> solution = ShowSolution.IsChecked ? MazeSolver.GetBestPath(nodeStart, nodeEnd, maze) : null;
        MazeView.Drawable = new CircularMazeDrawable(maze, nodeStart, nodeEnd, solution);
    }

    static void NormalizeNode(ref (int level, int section) node, CircularMaze maze) {
        if (node.level < 0) {
            node.level = 0;
        }
        if (node.section < 0) {
            node.section = 0;
        }
        if (node.level >= maze.Levels) {
            node.level = maze.Levels - 1;
        }
        if (node.section >= maze.GetNumSections(node.level)) {
            node.section = maze.GetNumSections(node.level) - 1;
        }
    }

    void OnValueChanged(object sender, EventArgs e) => UpdateMaze(true);

    void CheckChanged(object sender, CheckedChangedEventArgs e) => UpdateMaze(false);

    void MazeView_StartInteraction(object sender, TouchEventArgs e) {
        nodeStart = ((CircularMazeDrawable)MazeView.Drawable).HitTest(e.Touches[0].X, e.Touches[0].Y);
        nodeEnd = nodeStart;
        UpdateMaze(false);
    }

    void MazeView_DragInteraction(object sender, TouchEventArgs e) {
        (int x, int y) node = ((CircularMazeDrawable)MazeView.Drawable).HitTest(e.Touches[0].X, e.Touches[0].Y);
        if (node == nodeEnd || node == (-1, -1)) {
            return;
        }
        nodeEnd = node;
        UpdateMaze(false);
    }
}