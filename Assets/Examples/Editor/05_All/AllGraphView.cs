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
        var parentNode = BaseNode.CreateFromType<StringNode>(Vector2.zero);
        var parentView = AddNode(parentNode);
        var parent_OutPort_View = parentView.GetFirstPortViewFromFieldName(nameof(parentNode.output));

        var childNode_2 = BaseNode.CreateFromType<ColorNode>(new Vector2(100 * 2.5f, 0));
        var childView_2 = AddNode(childNode_2);
        var child_02_InputPort_View = childView_2.GetFirstPortViewFromFieldName(nameof(childNode_2.input));

        var childNode_1 = BaseNode.CreateFromType<ColorNode>(new Vector2(100 * 7.5f, 100 * 2.5f));
        var childView_1 = AddNode(childNode_1);
        var child_01_InputPort_view = childView_1.GetFirstPortViewFromFieldName(nameof(childNode_1.input));
        var child_01_OutPort_View = childView_1.GetFirstPortViewFromFieldName(nameof(childNode_1.output));

        views.Add(parentView);
        views.Add(childView_2);
        views.Add(childView_1);

        Connect(child_02_InputPort_View, parent_OutPort_View, true);
        Connect(child_01_InputPort_view, parent_OutPort_View, true);

        var grandNode_1 = BaseNode.CreateFromType<ColorNode>(new Vector2(100 * 10.5f, 100 * 0.5f));
        var grandView_1 = AddNode(grandNode_1);
        var grand_01_01_InputView = grandView_1.GetFirstPortViewFromFieldName(nameof(grandNode_1.input));

        Connect(grand_01_01_InputView, child_01_OutPort_View, true);
    }

    List<BaseNodeView> views = new List<BaseNodeView>();

    public void TestAddStack()
    {
        TestConnect();

        var stackNode = new BaseStackNode(Vector2.right * 100);
        var stackNodeView = AddStackNode(stackNode);

        foreach (var item in views){
            stackNodeView.InsertElement(0, item);
        }

    }
}