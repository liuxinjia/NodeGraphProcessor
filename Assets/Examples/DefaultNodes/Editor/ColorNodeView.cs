using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphProcessor;
using AV.Hierarchy;

[NodeCustomEditor(typeof(ColorNode))]
public class ColorNodeView : BaseNodeView
{
    public override void Enable()
    {
        var colorNode = nodeTarget as ColorNode;

        var hoverPreview = new HoverPreview();
        var preview = new Image();
        UpdatePreviewImage(preview, colorNode.go.gameObject);
        hoverPreview.OnItemPreview(new ViewItem(colorNode.go.gameObject));
        controlsContainer.Add(hoverPreview);

        // AddControlField(nameof(ColorNode.input));
        // controlsContainer.Add(preview);
        // contentContainer.Add(CreatePreviewGUI(colorNode.go.gameObject));
    }

    public VisualElement CreatePreviewGUI(GameObject go)
    {
        var imguiContainer = new IMGUIContainer(
            () =>
            {
                var editor = Editor.CreateEditor(go);
                editor.OnPreviewGUI(GUILayoutUtility.GetRect(256, 256), null);
            }
        );
        return imguiContainer;
    }

    void UpdatePreviewImage(Image image, Object obj)
    {
        image.image = AssetPreview.GetAssetPreview(obj) ?? AssetPreview.GetMiniThumbnail(obj);
    }
    public void OpenNode()
    {
        owner.ClearGraphElements();

        (owner as AllGraphView).TestAddStack();
    }
}