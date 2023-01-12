using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathFinder
{
    public class Node
    {
        public int x;
        public int y;
        public string name;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
            name = $"({x},{y})";
        }

        public bool Equals(Node other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;

            if(other is Node)
            {
                return Equals(other as Node);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        internal Vector2Int ToVector2Int()
        {
            return new Vector2Int(x, y);
        }

        public static bool operator ==(Node lhs, Node rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Node lhs, Node rhs)
        {
            return !(lhs == rhs);
        }
    }

    public class Graph
    {
        private bool[,] _walkMatrix;
        private Dictionary<Node, List<Node>> _neighbours = new Dictionary<Node, List<Node>>();
        private List<Node> _nodes = new List<Node>();

        public Graph(bool[,] walkMatrix)
        {
            _walkMatrix = walkMatrix;
            ShowWalkMap();
            CreateNodes();
        }

        public List<Node> GetConnectedNodes(Node node)
        {
            return _neighbours[node];
        }

        public Node GetAt(int x, int y)
        {
            return _nodes.Find(e => e.x == x && e.y == y);
        }

        private bool IsPosValid(int x, int y)
        {
            if (x < 0 || y < 0 || y >= _walkMatrix.GetLength(0) || x >= _walkMatrix.GetLength(1))
                return false;
            return true;
        }

        private void ShowWalkMap()
        {
            int size = _walkMatrix.GetLength(0);
            string str = "";
            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    str += _walkMatrix[x, y] ? "1" : "0";
                }
                str += "\n";
            }
            Debug.Log($"WalkMatrix: \n{str}");
        }

        private void CreateNodes()
        {
            int size = _walkMatrix.GetLength(0);
            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    Node node = new Node(x, y);
                    List<Node> connected = new List<Node>();
                    int a, b;

                    if (_walkMatrix[x, y])
                    {
                        a = x - 1; b = y;
                        if (IsPosValid(a, b) && _walkMatrix[a, b])
                            connected.Add(new Node(a, b));

                        a = x + 1; b = y;
                        if (IsPosValid(a, b) && _walkMatrix[a, b])
                            connected.Add(new Node(a, b));

                        a = x; b = y - 1;
                        if (IsPosValid(a, b) && _walkMatrix[a, b])
                            connected.Add(new Node(a, b));

                        a = x; b = y + 1;
                        if (IsPosValid(a, b) && _walkMatrix[a, b])
                            connected.Add(new Node(a, b));

                        Debug.Log("Graph. Node: " + node.name);
                        foreach (var elem in connected)
                        {
                            Debug.Log("Graph. Node Child: " + elem.name);
                        }
                    }
                    else
                    {
                        Debug.Log("Graph. Node: " + node.name + ". No connected nodes!");
                    }

                    _neighbours[node] = connected;
                    _nodes.Add(node);
                }
            }
        }
    }

    public class TreeNode
    {
        public List<TreeNode> children = new List<TreeNode>();

        public Node node;

        public TreeNode(Node node)
        {
            this.node = node;
        }

        public void AddChild(TreeNode child)
        {
            children.Add(child);
        }
    }

    [Serializable]
    public class PathInfo
    {
        public bool found;
        public bool valid;
        public List<Vector2Int> path;
    }


    /// <summary>
    /// Search path between two nodes using Depth First Search (DFS) Algorithm.
    /// </summary>
    /// <param name="start">Start node to start from</param>
    /// <param name="end">End node to arrive</param>
    /// <param name="walkMap">Walk map. 0 if can't move, 1 if can move.</param>
    public PathInfo SearchPath(Vector2Int start, Vector2Int end, bool[,] walkMap)
    {
        Graph graph = new Graph(walkMap);
        Node startNode = graph.GetAt(start.y, start.x);
        Node endNode = graph.GetAt(end.y, end.x);

        Debug.Log($"[PathFinder] Start:{startNode.name}; End:{endNode.name}");

        TreeNode tree = DFS(startNode, 0, endNode, new List<Node>(), graph, 90);

        Debug.Log("[PathFinder] Search finished.");

        bool found = tree != null;
        PathInfo pathInfo = new PathInfo();
        pathInfo.found = found;
        if (tree == null)
        {
            Debug.Log("[PathFinder] No path!");
            pathInfo.path = null;
        }
        else
        {
            pathInfo.path = new List<Vector2Int>();
            PrintPath(tree, ref pathInfo.path);
        }

        return pathInfo;
    }

    private TreeNode CreateTreeNode(Node node)
    {
        return new TreeNode(node);
    }

    private TreeNode DFS(Node node, int depth, Node target, List<Node> visited, Graph graph, int maxDepth)
    {
        visited.Add(node);

        if (depth >= maxDepth)
        {
            Debug.LogError("[PathFinder] Max depth!");
            return null;
        }

        if(node == target)
        {
            TreeNode tree = CreateTreeNode(node);
            return tree;
        }

        List<TreeNode> subTrees = new List<TreeNode>();

        foreach(var connectedNode in graph.GetConnectedNodes(node))
        {
            //Debug.Log($"[PathFinder] Checking. Node:{connectedNode.name}; Visited: {visited.Contains(connectedNode)};");
            if (!visited.Contains(connectedNode))
            {

                TreeNode subTree = DFS(connectedNode, depth + 1, target, visited, graph, maxDepth);
                if(subTree != null)
                {
                    subTrees.Add(subTree);
                }
            }
        }

        if(subTrees.Count > 0)
        {
            TreeNode treeNode = CreateTreeNode(node);
            foreach(var subTree in subTrees)
            {
               treeNode.AddChild(subTree);
            }
            return treeNode;
        }

        return null;
    }

    private void PrintPath(TreeNode root, ref List<Vector2Int> path, int order = 0)
    {
        path.Add(root.node.ToVector2Int());
        Debug.Log($"Node: ({root.node.y},{root.node.x}); Order:{order}");
        foreach (var child in root.children)
        {
            PrintPath(child, ref path, order + 1);
        }
    }

}
