using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Primitives/Color")]
public class ColorNode : BaseNode
{
    // [Output(name = "Color"), SerializeField]
    // public Color				color;

    [Input("Input")]
    public string input;

    [Output(name = "Output")]
    public string output;

    public Transform go;
    public override string name => "Color";
}
