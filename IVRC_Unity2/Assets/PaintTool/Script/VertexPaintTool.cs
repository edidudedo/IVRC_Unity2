/* 
    色を塗るメッシュにアタッチするためのスクリプト/ Script for attaching to the mesh to be colored

    https://qiita.com/r-ngtm/items/261a529b6f516263cad9#vertexpainttoolcs
    このurlのプログラムを参考にしています
*/

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    private float timeElapsed = 0.0f;   // ゲーム内の経過時間
    private float timeOut = 0.5f;       // 何秒ごとに、塗り終えたかを確認するか
    private float boader = 0.9f;        // 頂点のうちの何%を塗れば、塗り終わったことにするかのボーダー/Border that determines what percentage of the vertices must be painted in order for the painting to be considered complete
    public bool isPainted = true;       // 色が塗り終わっているかを管理するフラグ / Flag to manage whether the color has been painted or not
    public List<bool> colored;          // 各頂点に色を塗ったかを管理するフラグ / Flag to manage whether each vertex has been painted with color
    public Mesh SharedMesh              // 色を塗るメッシュ / Mesh to be colored
    {
        get
        {
            return meshFilter.sharedMesh;
        }
    }

    /// <summary>
    /// 起動時に、ペイント前の元々の色を保存 / Save original color before coloring
    /// </summary>
    private void Start()
    {
        // 色を塗るメッシュを取得/get mesh for coloring
        meshFilter = GetComponent<MeshFilter>();

        // 元の色を保存/save original color
        colorOrigin = new List<Color>(SharedMesh.vertexCount);
        colored = new List<bool>(SharedMesh.vertexCount);
        SharedMesh.GetColors(colorOrigin);

        // 色が設定されていない頂点を、白色で着色/Colors vertices that have no color to white
        for (int i = colorOrigin.Count; i < SharedMesh.vertexCount; i++) {
            colorOrigin.Add(Color.white);
        }

        // 着色されているという情報をboolで保存/Save the information that it is colored
        for (int i = 0; i < SharedMesh.vertexCount; i++) {
            colored.Add(true);
        }
    }

    /// <summary>
    /// 色が塗り終わっているかを確認
    /// 毎フレーム実行すると重たいので、0.5秒秒ごとに実行
    /// </summary>
    void Update ()
    {
        // 塗り終わってないなら
        if (!isPainted) {
            // 0.5秒ごとに、塗り終わってないかを確認
            timeElapsed += Time.deltaTime;
            if(timeElapsed >= timeOut) {
                timeElapsed = 0.0f;

                // 塗り終えた頂点の割合を求める
                float rate = (float)colored.Count(obj => obj == true) / (float)colored.Count;

                // 頂点をほとんど塗り終えていたら
                if (rate > boader) {
                    ResetColor();                       // 残りを自動で塗る
                    isPainted = true;                   // 塗り終えたことにする
                    FlagManager.uncoloredObjectNum--;   // 塗れてないオブジェクトの数を減らす
                }
            }
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

    /// <summary>
    /// 色を白色にする / turn the object color to white
    /// </summary>
    public void SetColor2White()
    {
        // 全ての頂点の色を白色にする
        List<Color> colors = new List<Color>(SharedMesh.vertexCount);
        for (int i = 0; i < SharedMesh.vertexCount; i++) {
            colors.Add(Color.white);
        }

        SharedMesh.SetColors(colors);

        // フラグを、塗られていない状態に戻す
        isPainted = false;
        for (int i = 0; i < SharedMesh.vertexCount; i++) {
            colored[i] = false;
        }
    }
}
