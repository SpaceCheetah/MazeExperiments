namespace MazeExperiments;
public static class MazeGenerator {
    //backtracker maze generator
    //jumpChance: chance to randomly jump to another cell when running the algorithm, 0-1
    //breakChance: after generating, chance for a wall to randomly be broken, causing loops, 0-1
    //on bidirectional mazes, the actual chance for a wall to break will be 1 - (1 - breakChance)^2, since each wall will be checked twice
    public static void GenerateMaze<TNode>(Random rand, IMaze<TNode> maze, TNode start, double jumpChance, double breakChance) {
        //could optimize for breakChance 1, but that just means a maze where every possible connection is an actual connection, not very useful
        var visited = new HashSet<TNode>();
        visited.Add(start);
        var visitChain = new List<TNode>();
        visitChain.Add(start);

        while (visitChain.Count > 0) {
            int index = jumpChance == 0 || (jumpChance != 1 && rand.NextDouble() > jumpChance) ? visitChain.Count - 1 : rand.Next(visitChain.Count);
            TNode node = visitChain[index];
            List<TNode> neighbors = maze.GetPossibleNeighbors(node);
            neighbors.RemoveAll(neighbor => visited.Contains(neighbor));
            if (neighbors.Count == 0) {
                visitChain.RemoveAt(index); //if the current node has no more neighbors, it can be safely removed
                continue;
            }
            TNode neighbor = neighbors[rand.Next(neighbors.Count)];
            maze.SetConnection(node, neighbor);
            visited.Add(neighbor);
            visitChain.Add(neighbor);
        }

        if (breakChance == 0) return;
        foreach(TNode node in visited) {
            foreach (TNode neighbor in maze.GetPossibleNeighbors(node)) {
                if (rand.NextDouble() < breakChance) {
                    maze.SetConnection(node, neighbor);
                }
            }
        }
    }
}
