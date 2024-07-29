using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MEdge
{
    public MVertex v0;
    public MVertex v1;

    public MEdge(MVertex v0, MVertex v1)
    {
        this.v0 = v0;
        this.v1 = v1;
    }
    public bool Equal(MEdge other)
    {
        return (v0.Equal(other.v0) && v1.Equal(other.v1))
            || (v1.Equal(other.v0) && v0.Equal(other.v1));
    }

}