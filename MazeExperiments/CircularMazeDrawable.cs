using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeExperiments;
public class CircularMazeDrawable : IDrawable {
    readonly CircularMaze maze;

    public CircularMazeDrawable(CircularMaze maze) {
        this.maze = maze;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect) {
        if (maze.Levels == 0 || maze.LevelDivisionFactor == 0) return;
        float levelWidth = Math.Min(dirtyRect.Width, dirtyRect.Height) / maze.Levels * 0.4f;
        PointF center = dirtyRect.Center;
        canvas.StrokeSize = levelWidth / maze.Levels / 2;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black;
        //draw rings for levels
        int sectionCount = 0;
        for (int level = 0; level < maze.Levels; level++) {
            float radius = levelWidth * level + levelWidth;
            if(level == maze.Levels - 1) {
                canvas.DrawCircle(center, radius);
            }
            //draw vertical barriers
            //sections on the current level
            var x = center.X - radius;
            var y = center.Y - radius;
            for (int section = 0; section < (sectionCount == 0 ? 1 : sectionCount); section++) {
                //possibly connected sections on the next level
                if (level != maze.Levels - 1) {
                    int sectionAngleStart = level == 0 ? 360 : 360 - section * 360 / sectionCount;
                    int sectionAngleEnd = level == 0 ? 0 : 360 - (section + 1) * 360 / sectionCount;
                    int lastNeighborEnd = sectionAngleStart;
                    foreach(((int level, int section) node, bool connected) in maze.GetAllNeighbors(level, section)) {
                        if (node.level != level + 1) continue; //only consider neighbors on the next level
                        int neighborAngleStart = 360 - node.section * 360 / (sectionCount + maze.LevelDivisionFactor);
                        //deal with non-touching neighbors (neighbors don't neccesarily line up)
                        if(neighborAngleStart < lastNeighborEnd) {
                            canvas.DrawArc(x, y, radius * 2, radius * 2, lastNeighborEnd, neighborAngleStart, true, false);
                        }
                        int neighborAngleEnd = 360 - (node.section + 1) * 360 / (sectionCount + maze.LevelDivisionFactor);
                        if (!connected) {
                            canvas.DrawArc(x, y, radius * 2, radius * 2, Math.Min(neighborAngleStart, sectionAngleStart), Math.Max(neighborAngleEnd, sectionAngleEnd), true, false);
                        } else {
                            //tempoary, testing drawing passages
                            canvas.StrokeColor = Colors.Green;
                            canvas.DrawArc(x, y, radius * 2, radius * 2, Math.Min(neighborAngleStart, sectionAngleStart), Math.Max(neighborAngleEnd, sectionAngleEnd), true, false);
                            canvas.StrokeColor = Colors.White;
                        }
                        lastNeighborEnd = neighborAngleEnd;
                    }
                    //deal with last neighbor not lining up with section
                    if (sectionAngleEnd < lastNeighborEnd) {
                        canvas.DrawArc(x, y, radius * 2, radius * 2, lastNeighborEnd, sectionAngleEnd, true, false);
                    }
                }
                if(level == 0 || maze.GetNeighbors((level, section)).Contains((level, (section + 1) % sectionCount))) {
                    continue;
                }
                //draw horizontal barriers
                double currentSectionEndRads = 2 * Math.PI - (section + 1) * 2 * Math.PI / sectionCount;
                //I'm going to assume that the compiler can optimize out duplicated pure function calls
                double prevRadius = level * levelWidth;
                float xStart = (float)(prevRadius * Math.Cos(currentSectionEndRads)) + center.X;
                //higher y is lower on the screen, so have to invert y
                float yStart = center.Y - (float)(prevRadius * Math.Sin(currentSectionEndRads));
                float xEnd = (float)(radius * Math.Cos(currentSectionEndRads)) + center.X;
                float yEnd = center.Y - (float)(radius * Math.Sin(currentSectionEndRads));
                canvas.DrawLine(xStart, yStart, xEnd, yEnd);
                //canvas.DrawString($"L{level}S{section}", xStart, yStart, HorizontalAlignment.Left);
            }
            sectionCount += maze.LevelDivisionFactor;
        }
    }
}
