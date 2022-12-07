using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace MazeExperiments;
public class CircularMazeDrawable : IDrawable {
    readonly CircularMaze maze;
    readonly (int level, int section) start, goal;
    List<(int level, int section)> solution;

    public CircularMazeDrawable(CircularMaze maze, (int level, int section) start, (int level, int section) goal, List<(int level, int section)> solution = null) {
        this.maze = maze;
        this.start = start;
        this.goal = goal;
        this.solution = solution;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect) {
        if (maze.Levels == 0) return;
        float levelWidth = Math.Min(dirtyRect.Width, dirtyRect.Height) / maze.Levels * 0.4f;
        PointF center = dirtyRect.Center;
        //mark start and goal
        canvas.FillColor = Colors.Blue;
        (float x, float y) startCoords = GetCellCoords(start.level, start.section, maze.GetNumSections(start.level), center, levelWidth);
        canvas.FillCircle(startCoords.x, startCoords.y, levelWidth / 3);
        (float x, float y) goalCoords = GetCellCoords(goal.level, goal.section, maze.GetNumSections(goal.level), center, levelWidth);
        canvas.FillColor = Colors.Gold;
        canvas.FillCircle(goalCoords.x, goalCoords.y, levelWidth / 3);

        canvas.StrokeSize = levelWidth / maze.Levels;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black;
        //draw rings for levels
        for (int level = 0; level < maze.Levels; level++) {
            float radius = levelWidth * level + levelWidth;
            if(level == maze.Levels - 1) {
                canvas.DrawCircle(center, radius);
            }
            //draw vertical barriers
            //sections on the current level
            var x = center.X - radius;
            var y = center.Y - radius;
            int sectionCount = maze.GetNumSections(level);
            for (int section = 0; section < sectionCount; section++) {
                //possibly connected sections on the next level
                if (level != maze.Levels - 1) {
                    int sectionAngleStart = level == 0 ? 360 : 360 - section * 360 / sectionCount;
                    int sectionAngleEnd = level == 0 ? 0 : 360 - (section + 1) * 360 / sectionCount;
                    //int lastNeighborEnd = sectionAngleStart;
                    foreach(((int level, int section) node, bool connected) in maze.GetAllNeighbors(level, section)) {
                        if (node.level != level + 1 || connected) continue; //only have to draw barriers for unconnected neighbors on the next level
                        int neighborAngleStart = 360 - node.section * 360 / (maze.GetNumSections(level + 1));
                        int neighborAngleEnd = 360 - (node.section + 1) * 360 / maze.GetNumSections(level + 1);
                        canvas.DrawArc(x, y, radius * 2, radius * 2, Math.Min(neighborAngleStart, sectionAngleStart), Math.Max(neighborAngleEnd, sectionAngleEnd), true, false);
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
            }
        }

        if (solution is null) return;
        //draw solution
        canvas.StrokeColor = Colors.Green;
        PathF path = new PathF();
        path.MoveTo(goalCoords.x, goalCoords.y);
        for (int i = 1; i < solution.Count; i++) {
            if (solution[i].level == solution[i - 1].level) {
                float lastAngle = 360 - 360 * (solution[i - 1].section + 0.5f) / maze.GetNumSections(solution[i].level);
                float angle = 360 - 360 * (solution[i].section + 0.5f) / maze.GetNumSections(solution[i].level);
                float radius = solution[i].level == 0 ? 0 : levelWidth * (solution[i].level + 0.5f);
                float x = center.X - radius;
                float y = center.Y - radius;
                //this is a way too complicated way to determine if the arc should be clockwise or not
                bool clockwise = (solution[i].section > solution[i - 1].section || 
                    (solution[i].section == 0 && solution[i - 1].section == maze.GetNumSections(solution[i].level) - 1)) &&
                    !(solution[i - 1].section == 0 && solution[i].section == maze.GetNumSections(solution[i].level) - 1);
                path.AddArc(x, y, x + radius * 2, y + radius * 2, lastAngle, angle, clockwise);
            } else {
                (float x, float y) coords = GetCellCoords(solution[i].level, solution[i].section, maze.GetNumSections(solution[i].level), center, levelWidth);
                path.LineTo(coords.x, coords.y);
            }
        }
        canvas.DrawPath(path);
    }

    (float x, float y) GetCellCoords(int level, int section, int numSections, PointF center, float levelWidth) {
        double angle = Math.Tau - Math.Tau * (section + 0.5) / numSections;
        double radius = level == 0 ? 0 : levelWidth * (level + 0.5);
        float x = (float)(radius * Math.Cos(angle) + center.X);
        //inverse y
        float y = (float)(center.Y - radius * Math.Sin(angle));
        return (x, y);
    }
}
