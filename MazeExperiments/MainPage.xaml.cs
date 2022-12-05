namespace MazeExperiments;

public partial class MainPage : ContentPage {
	public MainPage() {
		InitializeComponent();
        UpdateMaze(20, 20, 0, 0, 19, 19, 0, 0, true, new Random(0));
	}

	void UpdateMaze(int width, int height, int startX, int startY, int endX, int endY, double jumpChance, double breakChance, bool showSolution, Random rand) {
        var maze = new RectangularMaze(width, height);
        MazeGenerator.GenerateMaze(rand, maze, (0, 0), jumpChance, breakChance);
        List<(int, int)> solution = showSolution ? MazeSolver.GetBestPath((startX, startY), (endX, endY), maze) : null;
        MazeView.Drawable = new RectangularMazeDrawable(maze, solution);
    }

    void OnValueChanged(object sender, EventArgs e) {
        UpdateMaze((int)WidthSlider.Value, (int)HeightSlider.Value, 0, 0, (int)WidthSlider.Value - 1, (int)HeightSlider.Value - 1, JumpChanceSlider.Value, BreakChanceSlider.Value, ShowSolution.IsChecked, new Random((int)SeedSlider.Value));
    }

    private void CheckChanged(object sender, CheckedChangedEventArgs e) {
        OnValueChanged(null, null);
    }
}

