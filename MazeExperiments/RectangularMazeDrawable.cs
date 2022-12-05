namespace MazeExperiments;
internal class RectangularMazeDrawable : IDrawable {
    readonly RectangularMaze _maze;
    readonly List<(int x, int y)> _solution;
    float _cellWidth = -1;
    float _cellHeight = -1;
    (int x, int y) _start, _end;

    public RectangularMazeDrawable(RectangularMaze maze, (int x, int y) start, (int x, int y) end, List<(int x, int y)> solution = null) {
        _maze = maze;
        _solution = solution;
        _start = start;
        _end = end;
    }

    public (int x, int y) HitTest(float x, float y) {
        if (_cellWidth <= 0 || _cellHeight <= 0) return (-1, -1);
        return ((int)(x / _cellWidth),(int)( y / _cellHeight));
    }

    public void Draw(ICanvas canvas, RectF dirtyRect) {
        if (_maze.Width == 0 || _maze.Height == 0) return;
        _cellWidth = dirtyRect.Width / _maze.Width;
        _cellHeight = dirtyRect.Height / _maze.Height;

        canvas.FillColor = Colors.Blue;
        canvas.FillRectangle(_start.x * _cellWidth, _start.y * _cellHeight, _cellWidth, _cellHeight);
        canvas.FillColor = Colors.Gold;
        canvas.FillRectangle(_end.x * _cellWidth, _end.y * _cellHeight, _cellWidth, _cellHeight);

        canvas.StrokeSize = Math.Min(_cellWidth, _cellHeight) / 10;
        canvas.StrokeColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawRectangle(dirtyRect);
        for(int y = 0; y < _maze.Height; y++) {
            for(int x = 0; x < _maze.Width; x++) {
                RectangularMaze.CellPassages passages = _maze.GetCellPassages(x, y);
                if(!passages.Right && x != _maze.Width - 1) {
                    canvas.DrawLine(dirtyRect.Left + _cellWidth * x + _cellWidth, dirtyRect.Top + _cellHeight * y, dirtyRect.Left + _cellWidth * x + _cellWidth, dirtyRect.Top + _cellHeight * y + _cellHeight);
                }
                if(!passages.Down && y != _maze.Height - 1) {
                    canvas.DrawLine(dirtyRect.Left + _cellWidth * x, dirtyRect.Top + _cellHeight * y + _cellHeight, dirtyRect.Left + _cellWidth * x + _cellWidth, dirtyRect.Top + _cellHeight * y + _cellHeight);
                }
            }
        }
        if (_solution is null || _solution.Count < 2) return;
        canvas.StrokeColor = Colors.Green;
        (int lastX, int lastY) = _solution.First();
        foreach ((int x, int y) in _solution) {
            canvas.DrawLine(dirtyRect.Left + _cellWidth * lastX + _cellWidth / 2, dirtyRect.Top + _cellHeight * lastY + _cellHeight / 2, dirtyRect.Left + _cellWidth * x + _cellWidth / 2, dirtyRect.Top + _cellHeight * y + _cellHeight / 2);
            (lastX, lastY) = (x, y);
        }
    }
}
