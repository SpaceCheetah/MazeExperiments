namespace MazeExperiments;
public static class MazeSolver {
    //A* implementation
    //Returns the best path if there is a valid one, or null if there are no valid paths
    //The path is returned is all the nodes from the goal to the start
    //For most mazes, can just reverse start and goal to get the path in forward order. However, some mazes may have directional nodes,
    //and as such the path from start to goal may not be the same as the path from goal to start.
    public static List<TNode> GetBestPath<TNode>(TNode start, TNode goal, IMaze<TNode> maze) {
        EqualityComparer<TNode> comparer = EqualityComparer<TNode>.Default;
        var predecessor = new Dictionary<TNode, TNode>();
        var distance = new Dictionary<TNode, int>();
        distance[start] = 0;
        var queue = new PriorityQueue<(TNode, int), int>();
        queue.Enqueue((start, 0), 0);
        while (queue.Count > 0) {
            (TNode node, int distanceWhenAdded) = queue.Dequeue();
            if(comparer.Equals(node, goal)) {
                return ReconstructPath(start, goal, predecessor, comparer);
            }
            if (distanceWhenAdded > distance[node]) {
                continue; //might happen with an admissable but not consistent heuristic, or when there's no valid path
            }
            int neighborDistance = distanceWhenAdded + 1;
            foreach(TNode neighbor in maze.GetNeighbors(node)) {
                bool hasDistance = distance.TryGetValue(neighbor, out int dist);
                if(!hasDistance || neighborDistance < dist) {
                    predecessor[neighbor] = node;
                    distance[neighbor] = neighborDistance;
                    queue.Enqueue((neighbor, neighborDistance), neighborDistance + maze.EstimateDistance(neighbor, goal));
                }
            }
        }
        return null;
    }

    private static List<TNode> ReconstructPath<TNode>(TNode start, TNode goal, Dictionary<TNode, TNode> predecessor, EqualityComparer<TNode> comparer) {
        var path = new List<TNode>();
        path.Add(goal);
        TNode current = goal;
        while(!comparer.Equals(current, start)) {
            current = predecessor[current];
            path.Add(current);
        }
        return path;
    }
}
