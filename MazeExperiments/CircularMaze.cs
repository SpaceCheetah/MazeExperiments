 using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MazeExperiments;
public class CircularMaze : IMaze<(int level, int section)> {
    public int Levels { get; }

    //nodes[level][section] contains all nodes it could or has connected to
    Dictionary<(int level, int section), bool>[][] nodes;

    public CircularMaze(int levels, int level1Sections) {
        Levels = levels;
        nodes = new Dictionary<(int level, int section), bool>[levels][];
        int sectionCount = level1Sections;
        nodes[0] = new Dictionary<(int, int), bool>[1];
        nodes[0][0] = new Dictionary<(int, int), bool>();
        for(int level = 1; level < levels; level++) {
            if(level1Sections * level >= sectionCount * 2) {
                sectionCount *= 2;
            }
            nodes[level] = new Dictionary<(int level, int section), bool>[sectionCount];
            for(int section = 0; section < nodes[level].Length; section++) {
                nodes[level][section] = new Dictionary<(int level, int section), bool>();
            }
        }
        if (levels == 1) return;
        for(int section = 0; section < nodes[1].Length; section++) {
            nodes[0][0][(1, section)] = false;
            nodes[1][section][(0, 0)] = false;
        }
        for (int level = 1; level < Levels; level++) {
            for(int section = 0; section < nodes[level].Length; section++) {
                var neighbors = nodes[level][section];
                //add horizontal neighbors
                if (nodes[level].Length == 2) {
                    neighbors[(level, (section + 1) % 2)] = false;
                } else if(nodes[level].Length > 2) {
                    int leftNeighbor = section == 0 ? nodes[level].Length - 1 : section - 1;
                    int rightNeighbor = (section + 1) % nodes[level].Length;
                    neighbors[(level, leftNeighbor)] = false;
                    neighbors[(level, rightNeighbor)] = false;
                }
                //add neighbors on the next level
                if (level == Levels - 1) continue; //no next level to add connections to 
                //with the current scheme, each node either has 1 or 2 connections on the next layer
                if (nodes[level].Length == nodes[level + 1].Length) {
                    neighbors[(level + 1, section)] = false;
                    nodes[level + 1][section][(level, section)] = false;
                } else {
                    neighbors[(level + 1, section * 2)] = false;
                    neighbors[(level + 1, section * 2 + 1)] = false;
                    nodes[level + 1][section * 2][(level, section)] = false;
                    nodes[level + 1][section * 2 + 1][(level, section)] = false;
                }
            }
        }
    }

    public int GetNumSections(int level) => nodes[level].Length;

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
