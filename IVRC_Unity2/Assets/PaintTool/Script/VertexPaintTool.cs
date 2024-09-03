/* 
    色を塗るメッシュにアタッチするためのスクリプト/ Script for attaching to the mesh to be colored

    https://qiita.com/r-ngtm/items/261a529b6f516263cad9#vertexpainttoolcs
    このurlのプログラムを参考にしています
*/

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// このスクリプトをアタッチすると、自動でMeshFilterが追加される
// add MeshFilter automatically
[RequireComponent(typeof(MeshFilter))]


/// <summary>
/// 色を塗るメッシュにアタッチするためのスクリプト/ Script for attaching to the mesh to be colored
/// </summary>
public class VertexPaintTool : MonoBehaviour
{
    private MeshFilter meshFilter;      // 色を塗るメッシュを保持するクラス / A Mesh Filter component holds a reference to a mesh
    private List<Color> colorOrigin;    // 色を塗るメッシュの元々の色 / Original color of the mesh
    public Mesh SharedMesh              // 色を塗るメッシュ / Mesh to be colored
    {
        get
        {
            return meshFilter.sharedMesh;
        }
    }

    /// <summary>
    /// 起動時に、ペイント前の元々の色を保存 / Save original color before painting
    /// </summary>
    private void Start()
    {
        // 色を塗るメッシュを取得
        meshFilter = GetComponent<MeshFilter>();

        // 元の色を保存
        colorOrigin = new List<Color>(SharedMesh.vertexCount);;
        SharedMesh.GetColors(colorOrigin);

        // 色が設定されていない頂点を、白色で着色
        for (int i = colorOrigin.Count; i < SharedMesh.vertexCount; i++) {
            colorOrigin.Add(Color.white);
        }
    }
    

    /// <summary>
    /// メッシュの色をリセット / Reset color of mesh 
    /// </summary>
    public void ResetColor()
    {
        SharedMesh.SetColors(colorOrigin);
    }

    /// <summary>
    /// 指定された頂点の色を元に戻す
    /// </summary>
    public Color GetOriginVertexColor(int i)
    {
        return colorOrigin[i];
    }

    /// <summary>
    /// play mode終了時に、色を元に戻す / Return to original color at the end of play mode.
    /// </summary>
    private void OnApplicationQuit()
    {
        SharedMesh.SetColors(colorOrigin);
    }

    public void SetColor2White()
    {
        List<Color> colors = new List<Color>(SharedMesh.vertexCount);
        for (int i = 0; i < SharedMesh.vertexCount; i++) {
            colors.Add(Color.white);
        }
        
        SharedMesh.SetColors(colors);
    }
}
