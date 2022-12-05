namespace MazeExperiments;
public interface IMaze<TNode> {
    //get nodes that currently connect to the given node
    List<TNode> GetNeighbors(TNode node);
    //get all nodes that may be (but are currently not) connected to the given node
    List<TNode> GetPossibleNeighbors(TNode node);
    //estimate the minimum distance between the two given nodes; this estimation may not be lower than the true distance
    int EstimateDistance(TNode nodeA, TNode nodeB);
    //make or remove a connection between the two nodes, or throw InvalidOperationException if not possible
    void SetConnection(TNode nodeA, TNode nodeB, bool set = true);
}
