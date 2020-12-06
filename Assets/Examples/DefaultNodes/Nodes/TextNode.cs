using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Primitives/Text")]
public class TextNode : BaseNode
{
    [Output(name = "Get"), SerializeField]
    public string get;
    [Input(name = "Set"), SerializeField]
    public int set;

    public BindDirection bindDirection;
    public string bindName;
    public override string name => "Text";
}
public enum BindDirection
{
    v2m,
    m2v,
    mvvm
}
