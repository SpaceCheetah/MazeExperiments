 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeExperiments;
public class CircularMaze : IMaze<(int level, int section)> {
    /* Sections per level: LevelDivisionFactor^level
     * Potential neighbors: same level +- section (section wraps), level below at section / LevelDivisionFactor, level above at section * LevelDivisionFactor + n, where n = [0,LevelDivisionFactor)
     * So, every node has LevelDivisionFactor + 3 neighbors max
     */
    public int Levels { get; }
    public int LevelDivisionFactor { get; }
    //downPassages[level - 1][section] says whether that node has a connection to the node at [level - 2][section / LevelDivisionFactor]; - 1, because level 0 has no levels below it
    //using [][] instead of [,], as each level has a different number of sections
    readonly bool[][] downPassages;
    //rightPassages[level - 1][section] says whether that node has a connection to [level - 1][(section + 1) % rightPassages[level - 1].Length], same reasoning as downPassages
    readonly bool[][] rightPassages;

    public CircularMaze(int levels, int levelDivisionFactor) {
        Levels = levels;
        LevelDivisionFactor = levelDivisionFactor;
        downPassages = new bool[Levels - 1][];
        rightPassages= new bool[Levels - 1][];
        int sectionCount = LevelDivisionFactor;
        for (int level = 1; level < Levels; level++) {
            downPassages[level - 1] = new bool[sectionCount];
            rightPassages[level - 1] = new bool[sectionCount];
            sectionCount *= LevelDivisionFactor;
        }
    }

    public bool ConnectedDown(int level, int section) {
        if (level == 0) return false;
        return downPassages[level - 1][section];
    }

    public bool ConnectedRight(int level, int section) {
        if (level == 0) return true; //the inner circle is connected to itself
        return rightPassages[level - 1][section];
    }

    //all currently connected or possible neighbors
    List<(int level, int section, bool connected)> AllNeighbors((int level, int section) node) {
        var neighbors = new List<(int level, int section, bool connected)>();
        if(node.level > 0) {
            neighbors.Add((node.level - 1, node.section / LevelDivisionFactor, downPassages[node.level - 1][node.section / LevelDivisionFactor]));
        }
        if (node.level < Levels - 1) {
            for (int section = node.section * LevelDivisionFactor; section < node.section * LevelDivisionFactor + LevelDivisionFactor; section++) {
                neighbors.Add((node.level + 1, section, downPassages[node.level][section]));
            }
        }
        int levelSections = rightPassages[node.level - 1].Length;
        if(levelSections == 2) { //only has one neighbor on this level, and is connected if either it or its neighbor has rightPassage true
            neighbors.Add((node.level, (node.section + 1) % 2, rightPassages[node.level - 1][0] || rightPassages[node.level - 1][1]));
        } else if(levelSections > 2) { //has both left and right neighbors
            int leftNeighbor = node.section == 0 ? levelSections - 1 : node.section - 1;
            int rightNeighbor = (node.section + 1) % levelSections;
            neighbors.Add((node.level, leftNeighbor, rightPassages[node.level - 1][leftNeighbor]));
            neighbors.Add((node.level, rightNeighbor, rightPassages[node.level - 1][node.section]));
        }
        return neighbors;
    }

    public List<(int level, int section)> GetNeighbors((int level, int section) node) {
        var neighbors = new List<(int level, int section)>();
        foreach ((int level, int section, bool connected) in AllNeighbors(node)) {
            if (connected) {
                neighbors.Add((level, section));
            }
        }
        return neighbors;
    }

    public List<(int level, int section)> GetPossibleNeighbors((int level, int section) node) {
        var neighbors = new List<(int level, int section)>();
        foreach ((int level, int section, bool connected) in AllNeighbors(node)) {
            if (!connected) {
                neighbors.Add((level, section));
            }
        }
        return neighbors;
    }

    public int EstimateDistance((int level, int section) nodeA, (int level, int section) nodeB) {
        if(nodeA.level != nodeB.level) {
            return Math.Abs(nodeA.level - nodeB.level);
        }
        return nodeA.section == nodeB.section ? 0 : 1;
        //could return 2 if nodeA and nodeB aren't neighbors, but this is probably good enough; the heuristic, as long as it never returns a too high value, doesn't matter too much for small-ish mazes
    }

    public void SetConnection((int level, int section) nodeA, (int level, int section) nodeB, bool set = true) {
        if(nodeA.level != nodeB.level) {
            int diff = nodeA.level - nodeB.level;
            if(diff == 1) {
                if(nodeA.section / LevelDivisionFactor == nodeB.section) {
                    downPassages[nodeA.level - 1][nodeA.section] = set;
                    return;
                }
            } else if(diff == -1) {
                if(nodeB.section / LevelDivisionFactor == nodeA.section) {
                    downPassages[nodeB.level - 1][nodeB.section] = set;
                    return;
                }
            }
        } else {
            int levelSections = rightPassages[nodeA.level - 1].Length;
            int nodeALeft = nodeA.section == 0 ? levelSections - 1 : nodeA.section - 1;
            if(nodeB.section == nodeALeft) {
                rightPassages[nodeA.level - 1][nodeALeft] = set;
                return;
            }
            int nodeARight = (nodeA.section + 1) % levelSections;
            if(nodeB.section == nodeARight) {
                rightPassages[nodeA.level - 1][nodeA.section] = set;
                return;
            }
        }
        throw new InvalidOperationException();
    }
}
