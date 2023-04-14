using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource
{
    public static readonly string[] TYPES = { "Nothing", "Sugar", "Phosphate", "Base Pair" };

    private string type;

    public Resource()
    {
        type = TYPES[0];
    }

    public Resource(int typeIndex)
    {
        type = TYPES[typeIndex];
    }
    
    public override string ToString()
    {
        return type;
    }
}
