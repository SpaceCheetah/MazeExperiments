namespace MazeExperiments;

public partial class CircularMazePage : ContentPage
{
	public CircularMazePage()
	{
		InitializeComponent();
		UpdateMaze(true);
	}

	void UpdateMaze(bool generationUpdated) {
		var maze = new CircularMaze(15, 4);
		MazeGenerator.GenerateMaze(new Random(0), maze, (0, 0), 0, 0);
		MazeView.Drawable = new CircularMazeDrawable(maze, (14, 3), (0, 0), MazeSolver.GetBestPath((14, 3), (0, 0), maze));
	}
}