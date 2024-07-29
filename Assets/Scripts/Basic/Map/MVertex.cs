using System.Collections.Generic;
using UnityEngine;

public class MVertex
{
    public Vector2 position;
    public List<MVertex> adjacents;

    public MVertex(Vector2 position)
    {
        this.position = position;
        adjacents = new List<MVertex>();
    }
    public MVertex(Vector2 position, List<MVertex> adjacents)
    {
        this.position = position;
        this.adjacents = adjacents;
    }

    public void addAdjacent(MVertex vertex)
    {
        if (!adjacents.Contains(vertex))
        {
            adjacents.Add(vertex);
        }
    }

    public bool Equal(MVertex vertex)
    {
        return position == vertex.position;
    }


}
