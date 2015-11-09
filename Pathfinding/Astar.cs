using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Astar
{
    public GameManager game;
    public HashSet<Node> closedSet = new HashSet<Node>();
    private int maxSize = 1000;
    private Heap<Node> openSet;

    public List<Node> FindPath(Node start, Node destination)
    {
        // an open set to store nodes with the potentional to lead towards the goal
        openSet = new Heap<Node>(game.nodeGrid.nodesInGridWidth * game.nodeGrid.nodesInGridHeight);

        // a closed set to keep track of nodes already expanded from
        closedSet = new HashSet<Node>();
        openSet.Add(start);

        // as long as the openset contains nodes we keep searching a path
        while (openSet.Count > 0)
        {
            // get the node with the lowest F value
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            // if we find the goal, retrace our path and send it to the requester
            if (currentNode == destination)
                return RetracePath(start, destination);

            // expand from the current node, get neighbours in all 8 directions
            foreach (Node neighbour in game.nodeGrid.GetAllNeighbours(currentNode))
            {
                if (neighbour.blocked || closedSet.Contains(neighbour))
                    continue;

                // the new cost is the current Node G value + the distance to the neighbour (either 10 or 14)
                int newGCost = currentNode.G + GetDistance(currentNode, neighbour);
                if (newGCost < neighbour.G || !openSet.Contains(neighbour))
                {
                    neighbour.G = newGCost;
                    neighbour.H = GetDistance(neighbour, destination);
                    neighbour.parent = currentNode;

                    // the openSet maintains itself in such away that the node with the lowest F value is always the first element.
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
        }

        // we went through every node we could reach and havent found the goal. it is unreachable
        return null;
    }


    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(startNode);
        path.Reverse();

        return path;
    }

    public int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public void DebugExploredNodes()
    {
		// explored & expanded
		Texture2D t = game.gameObject.GetComponent<Renderer>().material.mainTexture as Texture2D;
		game.gameObject.GetComponent<Renderer>().material.mainTexture = t;
		foreach (Node node in closedSet)
			t.SetPixel(node.gridX, game.nodeGrid.nodesInGridHeight - 1 - node.gridY, Color.red);
		t.Apply();


		//added but not expanded
        while (openSet.Count > 0)
        {
            Node current = openSet.RemoveFirst();
                game.AssignDebugPixel(current.gridX, current.gridY, Color.red);
        }
    }


    public List<Node> SmoothPath(List<Node> oldPath)
    {
        if (oldPath.Count > 2)
        {
            List<Node> newPath = new List<Node>();
            newPath.Add(oldPath[0]);

            int check = 2;
            Node checkPoint = oldPath[0];
            Node currentPoint = oldPath[check];

            while (check < oldPath.Count - 1)
            {
                if (game.bresenhamLine.IsPathClear(checkPoint, currentPoint))
                {
                    check++;
                    currentPoint = oldPath[check];
                }
                else
                {
                    checkPoint = oldPath[check - 1];

                    // re add removed node
                    newPath.Add(checkPoint);
                }
            }

            newPath.Add(oldPath[oldPath.Count - 1]);
            return newPath;
        }
        else
            return oldPath;
    }

}

