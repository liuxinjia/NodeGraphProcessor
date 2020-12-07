using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using GraphProcessor;
using System;
using UnityEditor;
using System.Collections.Generic;

public class AllGraphView : BaseGraphView
{
    // Nothing special to add for now
    public AllGraphView(EditorWindow window) : base(window) { }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        BuildStackNodeContextualMenu(evt);
        base.BuildContextualMenu(evt);
    }

    /// <summary>
    /// Add the New Stack entry to the context menu
    /// </summary>
    /// <param name="evt"></param>
    protected void BuildStackNodeContextualMenu(ContextualMenuPopulateEvent evt)
    {
        Vector2 position = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        evt.menu.AppendAction("New Stack", (e) => AddStackNode(new BaseStackNode(position)), DropdownMenuAction.AlwaysEnabled);
    }


    public void TestConnect()
    {
        var parentNode = BaseNode.CreateFromType<TextNode>(Vector2.zero);
        var parentView = AddNode(parentNode);
        // var parent_OutPort_View = parentView.GetFirstPortViewFromFieldName(nameof(parentNode.output));

        var childNode_2 = BaseNode.CreateFromType<TextNode>(new Vector2(100 * 2.5f, 0));
        var childView_2 = AddNode(childNode_2);
        // var child_02_InputPort_View = childView_2.GetFirstPortViewFromFieldName(nameof(childNode_2.input));

        var childNode_1 = BaseNode.CreateFromType<TextNode>(new Vector2(100 * 7.5f, 100 * 2.5f));
        var childView_1 = AddNode(childNode_1);
        // var child_01_InputPort_view = childView_1.GetFirstPortViewFromFieldName(nameof(childNode_1.input));
        // var child_01_OutPort_View = childView_1.GetFirstPortViewFromFieldName(nameof(childNode_1.output));

        views.Add(parentView);
        views.Add(childView_2);
        views.Add(childView_1);

        // Connect(child_02_InputPort_View, parent_OutPort_View, true);
        // Connect(child_01_InputPort_view, parent_OutPort_View, true);

        var grandNode_1 = BaseNode.CreateFromType<ColorNode>(new Vector2(100 * 10.5f, 100 * 0.5f));
        var grandView_1 = AddNode(grandNode_1);
        var grand_01_01_InputView = grandView_1.GetFirstPortViewFromFieldName(nameof(grandNode_1.input));

        // Connect(grand_01_01_InputView, child_01_OutPort_View, true);
    }

    internal void TestAll()
    {
        InitRNodes();
        CreateRNodes();
    }

    const int Width = 200;
    const int Height = 97;
    const float WidthSpace = .3f;
    const float HeightSpace = .3f;
    const float lengthSpace = .55f;

    private void CreateRNodes()
    {
        var startPos = Vector2.zero;
        var rootNode = BaseNode.CreateFromType<ColorNode>(startPos);
        rootNode.go = RNodes[0].transform;
        var rootNodeView = AddNode(rootNode);
        RNodes[0].pos = startPos;
        RNodes[0].nodeView = rootNodeView;

        for (int i = 1; i < RNodes.Count; i++)
        {
            RNode parentNode = RNodes[RNodes[i].parentLevel];
            Vector2 parentPos = parentNode.pos;
            int yDir = RNodes[i].index - parentNode.count / 2;
            float y = (yDir + (parentNode.count % 2 == 0 ? 0.5f : 0)) * Height;
            float x = (RNodes[i].level - parentNode.level - 1) * lengthSpace * Width + Width;
            var pos = parentPos + new Vector2(x + WidthSpace * Width, y + HeightSpace * Height * Mathf.Sign(yDir));
            var node = BaseNode.CreateFromType<ColorNode>(pos);
            node.go = RNodes[i].transform;
            var nodeView = AddNode(node);
            nodeView.name = $"{RNodes[i].level}";
            RNodes[i].pos = pos;
            RNodes[i].nodeView = nodeView;

            //Connect
            var inputPortView = nodeView.GetFirstPortViewFromFieldName(nameof(node.input));
            var outputPortView = parentNode.nodeView.GetFirstPortViewFromFieldName(nameof(node.output));
            Connect(inputPortView, outputPortView, true);
        }
    }

    private void InitRNodes()
    {
        var childrens = Selection.activeGameObject.GetComponentsInChildren<RectTransform>();
        var root = Selection.activeGameObject.transform;

        foreach (var child in childrens)
        {
            if (child.transform == root) continue;
            int index = 1;
            var parent = child.transform.parent;
            while (parent != null && parent != root)
            {
                parent = parent.parent;
                index++;
            }
            RNodes.Add(new RNode()
            {
                level = index,
                transform = child.transform
            });
        }

        RNodes.Add(new RNode()
        {
            level = 0,
            transform = root
        });

        RNodes.Sort((r1, r2) => r1.level.CompareTo(r2.level));

        var map = new Dictionary<Transform, RNode>();
        for (int i = 0; i < RNodes.Count; i++)
        {
            map.Add(RNodes[i].transform, RNodes[i]);
        }

        for (int i = 0; i < RNodes.Count; i++)
        {
            if (RNodes[i].transform == root) continue;

            var parent = RNodes[i].transform.parent;
            while (parent != null && !map.ContainsKey(parent))
            {
                parent = parent.parent;
            }
            RNodes[i].parentLevel = parent == null ? 0 : map[parent].level;
            RNodes[i].index = map[parent].count++;
        }
    }

    List<RNode> RNodes = new List<RNode>();

    public class RNode
    {
        public int level;
        public Transform transform;

        public int parentLevel;
        public int index;
        public int count;
        public Vector2 pos;
        public BaseNodeView nodeView;
    }

    List<BaseNodeView> views = new List<BaseNodeView>();

    public void TestAddStack()
    {
        TestConnect();

        var stackNode = new BaseStackNode(Vector2.right * 100);
        var stackNodeView = AddStackNode(stackNode);
        stackNodeView.name = "Text";

        foreach (var item in views)
        {
            stackNodeView.InsertElement(0, item);
        }

    }

    protected override void HandleClick(List<ISelectable> selections)
    {
        foreach (var item in selections)
        {
            if (item is ColorNodeView nodeView)
            {
                nodeView.OpenNode();
            }
        }

    }
}