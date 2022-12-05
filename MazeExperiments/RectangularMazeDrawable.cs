namespace MazeExperiments;
internal class RectangularMazeDrawable : IDrawable {
    readonly RectangularMaze _maze;
    readonly List<(int x, int y)> _solution;
    public RectangularMazeDrawable(RectangularMaze maze, List<(int x, int y)> solution = null) {
        _maze = maze;
        _solution = solution;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect) {
        if (_maze.Width == 0 || _maze.Height == 0) return;
        float cellWidth = dirtyRect.Width / _maze.Width;
        float cellHeight = dirtyRect.Height / _maze.Height;
        canvas.StrokeSize = Math.Min(cellWidth, cellHeight) / 10;
        canvas.StrokeColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawRectangle(dirtyRect);
        for(int y = 0; y < _maze.Height; y++) {
            for(int x = 0; x < _maze.Width; x++) {
                RectangularMaze.CellPassages passages = _maze.GetCellPassages(x, y);
                if(!passages.Right && x != _maze.Width - 1) {
                    canvas.DrawLine(dirtyRect.Left + cellWidth * x + cellWidth, dirtyRect.Top + cellHeight * y, dirtyRect.Left + cellWidth * x + cellWidth, dirtyRect.Top + cellHeight * y + cellHeight);
                }
                if(!passages.Down && y != _maze.Height - 1) {
                    canvas.DrawLine(dirtyRect.Left + cellWidth * x, dirtyRect.Top + cellHeight * y + cellHeight, dirtyRect.Left + cellWidth * x + cellWidth, dirtyRect.Top + cellHeight * y + cellHeight);
                }
            }
        }
        if (_solution is null || _solution.Count < 2) return;
        canvas.StrokeColor = Colors.Green;
        (int lastX, int lastY) = _solution.First();
        foreach ((int x, int y) in _solution) {
            canvas.DrawLine(dirtyRect.Left + cellWidth * lastX + cellWidth / 2, dirtyRect.Top + cellHeight * lastY + cellHeight / 2, dirtyRect.Left + cellWidth * x + cellWidth / 2, dirtyRect.Top + cellHeight * y + cellHeight / 2);
            (lastX, lastY) = (x, y);
        }
    }
}
