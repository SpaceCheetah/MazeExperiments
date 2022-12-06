 using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MazeExperiments;
public class CircularMaze : IMaze<(int level, int section)> {
    /* Sections per level: LevelDivisionFactor*level, except level 0 which has 1
     * Potential neighbors: same level +- section (section wraps), level below at section / LevelDivisionFactor, level above at section * LevelDivisionFactor + n, where n = [0,LevelDivisionFactor)
     * So, every node has LevelDivisionFactor + 3 neighbors max
     */
    public int Levels { get; }
    public int LevelDivisionFactor { get; }

    //nodes[level][section] contains all nodes it could or has connected to
    Dictionary<(int level, int section), bool>[][] nodes;

    public CircularMaze(int levels, int levelDivisionFactor) {
        double minOverlap = 0.5; //might make this configurable later
        Levels = levels;
        LevelDivisionFactor = levelDivisionFactor;
        nodes = new Dictionary<(int level, int section), bool>[levels][];
        for(int level = 0; level < levels; level++) {
            nodes[level] = new Dictionary<(int level, int section), bool>[level == 0 ? 1 : level * LevelDivisionFactor];
            for(int section = 0; section < nodes[level].Length; section++) {
                nodes[level][section] = new Dictionary<(int level, int section), bool>();
            }
        }
        if (levels == 1) return;
        int sectionsCurrent = levelDivisionFactor;
        for(int section = 0; section < sectionsCurrent; section++) {
            nodes[0][0][(1, section)] = false;
            nodes[1][section][(0, 0)] = false;
        }
        for (int level = 1; level < Levels; level++) {
            int sectionsNext = sectionsCurrent + levelDivisionFactor;
            double minOverlapLevel = 1.0 / sectionsNext * minOverlap;
            for(int section = 0; section < sectionsCurrent; section++) {
                var neighbors = nodes[level][section];
                //add horizontal neighbors
                if (sectionsCurrent == 2) {
                    neighbors[(level, (section + 1) % 2)] = false;
                } else if(sectionsCurrent > 2) {
                    int leftNeighbor = section == 0 ? sectionsCurrent - 1 : section - 1;
                    int rightNeighbor = (section + 1) % sectionsCurrent;
                    neighbors[(level, leftNeighbor)] = false;
                    neighbors[(level, rightNeighbor)] = false;
                }
                //add neighbors on the next level
                if(level < Levels - 1) {
                    //faction comparison without doubles: multiply by denominators. Num is numerator in this case
                    //int fractionStartNum = section * sectionsNext;
                    //int fractionEndNum = (section + 1) * sectionsNext;
                    double fractionStart = (double)section / sectionsCurrent;
                    double fractionEnd = (section + 1.0) / sectionsCurrent;
                    for(int upperSection = 0; upperSection < sectionsNext; upperSection++) {
                        //int upperFractionStartNum = upperSection * sectionsCurrent;
                        //int upperFractionEndNum = (upperSection + 1) * sectionsCurrent;
                        double upperFractionStart = (double)upperSection / sectionsNext;
                        double upperFractionEnd = (upperSection + 1.0) / sectionsNext;
                        if (upperFractionStart > fractionEnd) break;
                        double overlap = Math.Min(fractionEnd, upperFractionEnd) - Math.Max(fractionStart, upperFractionStart);
                        if(overlap > minOverlapLevel) {
                            neighbors[(level + 1, upperSection)] = false;
                            nodes[level + 1][upperSection][(level, section)] = false;
                        }
                    }
                }
            }

            sectionsCurrent += levelDivisionFactor;
        }
    }

    //not part of the IMaze interface; need to get all neighbors, connected or not, to draw properly
    public ReadOnlyDictionary<(int level, int section), bool> GetAllNeighbors(int level, int section) => new(nodes[level][section]);

    public List<(int level, int section)> GetNeighbors((int level, int section) node) {
        var neighbors = new List<(int level, int section)>();
        foreach (((int level, int section), bool connected) in nodes[node.level][node.section]) {
            if (connected) {
                neighbors.Add((level, section));
            }
        }
        return neighbors;
    }

    public List<(int level, int section)> GetPossibleNeighbors((int level, int section) node) {
        var neighbors = new List<(int level, int section)>();
        foreach (((int level, int section), bool connected) in nodes[node.level][node.section]) {
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
        //on same level, without doing some non-trivial operations, only thing garunteed to know is at least 1 (if not just same node)
    }

    public void SetConnection((int level, int section) nodeA, (int level, int section) nodeB, bool set = true) {
        if (nodes[nodeA.level][nodeA.section].ContainsKey(nodeB)) {
            nodes[nodeA.level][nodeA.section][(nodeB.level, nodeB.section)] = set;
            nodes[nodeB.level][nodeB.section][(nodeA.level, nodeA.section)] = set;
            return;
        }
        throw new InvalidOperationException();
    }
}
