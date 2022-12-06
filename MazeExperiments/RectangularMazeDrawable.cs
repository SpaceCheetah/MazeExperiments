namespace MazeExperiments;
internal class RectangularMazeDrawable : IDrawable {
    readonly RectangularMaze maze;
    readonly List<(int x, int y)> solution;
    float cellWidth = -1;
    float cellHeight = -1;
    (int x, int y) start, end;

    public RectangularMazeDrawable(RectangularMaze maze, (int x, int y) start, (int x, int y) end, List<(int x, int y)> solution = null) {
        this.maze = maze;
        this.solution = solution;
        this.start = start;
        this.end = end;
    }

    public (int x, int y) HitTest(float x, float y) {
        if (cellWidth <= 0 || cellHeight <= 0) return (-1, -1);
        return ((int)(x / cellWidth),(int)( y / cellHeight));
    }

    public void Draw(ICanvas canvas, RectF dirtyRect) {
        if (maze.Width == 0 || maze.Height == 0) return;
        cellWidth = dirtyRect.Width / maze.Width;
        cellHeight = dirtyRect.Height / maze.Height;

        canvas.FillColor = Colors.Blue;
        canvas.FillRectangle(start.x * cellWidth, start.y * cellHeight, cellWidth, cellHeight);
        canvas.FillColor = Colors.Gold;
        canvas.FillRectangle(end.x * cellWidth, end.y * cellHeight, cellWidth, cellHeight);

        canvas.StrokeSize = Math.Min(cellWidth, cellHeight) / 10;
        canvas.StrokeColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawRectangle(dirtyRect);
        for(int y = 0; y < maze.Height; y++) {
            for(int x = 0; x < maze.Width; x++) {
                RectangularMaze.CellPassages passages = maze.GetCellPassages(x, y);
                if(!passages.Right && x != maze.Width - 1) {
                    canvas.DrawLine(dirtyRect.Left + cellWidth * x + cellWidth, dirtyRect.Top + cellHeight * y, dirtyRect.Left + cellWidth * x + cellWidth, dirtyRect.Top + cellHeight * y + cellHeight);
                }
                if(!passages.Down && y != maze.Height - 1) {
                    canvas.DrawLine(dirtyRect.Left + cellWidth * x, dirtyRect.Top + cellHeight * y + cellHeight, dirtyRect.Left + cellWidth * x + cellWidth, dirtyRect.Top + cellHeight * y + cellHeight);
                }
            }
        }
        if (solution is null || solution.Count < 2) return;
        canvas.StrokeColor = Colors.Green;
        (int lastX, int lastY) = solution.First();
        foreach ((int x, int y) in solution) {
            canvas.DrawLine(dirtyRect.Left + cellWidth * lastX + cellWidth / 2, dirtyRect.Top + cellHeight * lastY + cellHeight / 2, dirtyRect.Left + cellWidth * x + cellWidth / 2, dirtyRect.Top + cellHeight * y + cellHeight / 2);
            (lastX, lastY) = (x, y);
        }
    }
}
