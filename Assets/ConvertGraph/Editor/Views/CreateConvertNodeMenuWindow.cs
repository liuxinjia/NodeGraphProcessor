namespace Cr7Sund.ConvertGraph
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using GraphProcessor;
    using UnityEditor;
    using System.Linq;
    using UnityEngine.UIElements;
    using System;
    using System.CodeDom;

    public class CreateConvertNodeMenuWindow : CreateNodeMenuWindow
    {
        protected override string GetEdgeNodePath(IEnumerable<(string path, Type type)> nodePaths, NodeProvider.PortDescription nodePortDes) =>
         nodePaths.FirstOrDefault(k => k.path == nodePortDes.extraInfo).path;

        protected override SearchTreeEntry AddStandardSearchEntry((string path, Type type) nodeMenuItem, string nodeName, int level)
        {
            if (nodeMenuItem.type == typeof(ConvertNode))
            {
                string[] vs = nodeMenuItem.path.Split('$');
                var typeName = vs[0].Split('/').LastOrDefault().Split('.').LastOrDefault();

                var treeEntry = base.AddStandardSearchEntry(nodeMenuItem, nodeName, level);
                treeEntry.userData = nodeMenuItem.path;
                treeEntry.content.text = typeName;

                return treeEntry;
            }
            else if (nodeMenuItem.type.IsAssignableFrom(typeof(SourceNode)))
            {
                var treeEntry = base.AddStandardSearchEntry(nodeMenuItem, nodeName, level);
                if (graphView is ConvertGraphView cView)
                {
                    treeEntry.content.text = GraphNodeType.MNode.ToString();
                    return treeEntry;
                }
                else
                {
                    throw new Exception("OK, Congratulations! You have come into Bermudua");
                }
            }
            else
            {
                return base.AddStandardSearchEntry(nodeMenuItem, nodeName, level);
            }
        }

        protected override SearchTreeEntry AddEdgeNodeSearchEntry(NodeProvider.PortDescription nodeMenuItem, string nodeName, int level)
        {

            if (nodeMenuItem.nodeType.IsAssignableFrom(typeof(ConvertNode)))
            {

                string[] vs = nodeMenuItem.extraInfo.Split('$');
                return new SearchTreeEntry(new GUIContent($"{vs[0].Split('/').LastOrDefault().Split('.').LastOrDefault()}:  {nodeMenuItem.portDisplayName}", icon))
                {
                    level = level + 1,
                    userData = nodeMenuItem
                };
            }
            else
            {

                return base.AddEdgeNodeSearchEntry(nodeMenuItem, nodeName, level);
            }
        }

        protected override PortView CreateEdgePort(Vector2 graphMousePosition, NodeProvider.PortDescription desc)
        {
            if (desc.nodeType.IsAssignableFrom(typeof(ConvertNode)))
            {
                var node = BaseNode.CreateFromType(typeof(ConvertNode), graphMousePosition) as ConvertNode;

                string[] vs = desc.extraInfo.Split('$');
                node.classTypeInfo.assemblyName = vs[2];

                node.description = vs[1];
                string[] cFuncInfo = vs[0].Split('/');
                node.convertFuncName = cFuncInfo[cFuncInfo.Length - 1];
                node.classTypeInfo.fullName = cFuncInfo[cFuncInfo.Length - 2];


                var view = graphView.AddNode(node) as ConvertNodeView;
                if (view.PortFileKeys.Length != 2 && view.PortFileKeys[0] != "inputs")
                {
                    throw new Exception("You are not modifying a convert node or \n you have change the order about inputs and outputs");
                }

                return view.GetPortViewFromFieldName(
                    desc.isInput ? view.PortFileKeys[0] : view.PortFileKeys[1],
                    desc.portIdentifier
                );
            }
            else if (desc.nodeType.IsAssignableFrom(typeof(RelayNode)))
            {
                return null;
            }
            else
            {
                return base.CreateEdgePort(graphMousePosition, desc);
            }
        }

        protected override BaseNode CreateNode(Type nodeType, Vector2 graphMousePosition)
        {
            if (nodeType.IsAssignableFrom(typeof(SourceNode)))
            {
                if (graphView is ConvertGraphView cGraphView)
                {
                    return cGraphView.AddSourceNode(typeof(int), !cGraphView.isM2V, graphMousePosition, GraphNodeType.MNode.ToString(), true).nodeTarget;
                }
                else
                {
                    throw new Exception("You will never drop into such case");
                }
            }
            else
            {
                return base.CreateNode(nodeType, graphMousePosition);
            }
        }

        protected override BaseNode CreateCustomNode(string menuItem, Vector2 graphMousePosition)
        {
            var node = BaseNode.CreateFromType(typeof(ConvertNode), graphMousePosition) as ConvertNode;

            string[] vs = menuItem.Split('$');
            node.classTypeInfo.assemblyName = vs[2];
            node.description = vs[1];

            string[] cFuncInfo = vs[0].Split('/');
            node.convertFuncName = cFuncInfo[cFuncInfo.Length - 1];
            node.classTypeInfo.fullName = cFuncInfo[cFuncInfo.Length - 2];

            graphView.RegisterCompleteObjectUndo($"Added {node.classTypeInfo.fullName}{node.convertFuncName}");
            return node;
        }
    }
}