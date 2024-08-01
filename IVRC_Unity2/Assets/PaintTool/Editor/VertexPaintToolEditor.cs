// https://qiita.com/r-ngtm/items/261a529b6f516263cad9

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(VertexPaintTool))]
public class VertexPaintToolEditor : Editor
{
    private List<Vector3> vertices = new List<Vector3>();   // position of vertex

    // Reference to the XR Ray Interactor, assigned manually
    public XRRayInteractor rayInteractor;

    // VertexPaintToolのGUIを設定
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Reset Colorボタンを追加
        if (GUILayout.Button("Reset Color"))
        {
            var component = target as VertexPaintTool;
            component.ResetColor();
        }

        // Field to assign the XR Ray Interactor manually
        rayInteractor = EditorGUILayout.ObjectField("Ray Interactor", rayInteractor, typeof(XRRayInteractor), true) as XRRayInteractor;
    }

    // Scene Viewでの動作を定義
    private void OnSceneGUI()
    {
        // Check if rayInteractor is assigned
        if (rayInteractor == null)
        {
            Debug.LogWarning("Ray Interactor is not assigned.");
            return;
        }

        // Check if rayInteractor is hitting something
        if (rayInteractor.TryGetCurrent3DRaycastHit(out var hit))
        {
            var component = target as VertexPaintTool;

            // マウスの延長線上とメッシュとの交点を取得
            var hitPositionOS = component.transform.InverseTransformPoint(hit.point);

            Mesh sharedMesh = component.SharedMesh;

            // 頂点の取得
            if (vertices.Count != sharedMesh.vertexCount)
                component.SharedMesh.GetVertices(vertices);

            // 頂点カラーを取得
            var colors = new List<Color>(sharedMesh.vertexCount);
            sharedMesh.GetColors(colors);

            // 頂点カラーの書き換え
            for (var i = 0; i < vertices.Count; i++)
            {
                var v = vertices[i]; // 頂点座標(オブジェクトSpace)
                float sqrDistanceOS = (hitPositionOS - v).sqrMagnitude; // Rayヒットと頂点の距離 (オブジェクトSpace)
                if (sqrDistanceOS > BrushSettign.brushSize * BrushSettign.brushSize) continue; // 距離がある程度離れている頂点は除外する

                // 色の書き換え
                colors[i] = Color.Lerp(colors[i], BrushSettign.paintColor, BrushSettign.paintColor.a);
            }

            // 頂点カラーを設定
            sharedMesh.SetColors(colors);
        }
    }
}
