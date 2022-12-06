namespace MazeExperiments;

public partial class CircularMazePage : ContentPage
{
	public CircularMazePage()
	{
		InitializeComponent();
		UpdateMaze(true);
	}

	void UpdateMaze(bool generationUpdated) {
		var maze = new CircularMaze(15, 7);
		MazeGenerator.GenerateMaze(new Random(0), maze, (0, 0), 0, 0);
		MazeView.Drawable = new CircularMazeDrawable(maze);
	}
}