// https://qiita.com/r-ngtm/items/261a529b6f516263cad9
// を一部参考にしています

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Oculus.Interaction;
using System.Runtime.InteropServices;

public class PaintToolManager : MonoBehaviour
{
  private List<Vector3> vertices = new List<Vector3>();   // position of vertex
  public RayInteractor rayInteractorR, rayInteractorL;
  [SerializeField] public bool isColorFixMode = true;
  private RayInteractor rayInteractor;

  // 壁とビームとの交点を保持するクラスrayInterator　を取得
  public void Start()
  {
    GameObject objR = GameObject.Find("[BuildingBlock] Right Ray");
    GameObject objL = GameObject.Find("[BuildingBlock] Left Ray");
    rayInteractorR = objR.GetComponent<RayInteractor>();
    rayInteractorL = objL.GetComponent<RayInteractor>();
  }

  private void Update()
  {
    // Check if right hand rayInteractor is assigned
    if (rayInteractorR.HasInteractable)
    {
      rayInteractor = rayInteractorR;
      if (rayInteractor.CollisionInfo != null) {
        // get hitPoint of ray and mesh
        var hitPoint = rayInteractor.CollisionInfo.Value.Point;

        // get ray hitted object
        var rayInteractable = rayInteractor.Candidate;
        var obj = rayInteractable.gameObject;

        // get ray hitted mesh
        var vertexPaintTool = obj.GetComponent<VertexPaintTool>();
        Mesh sharedMesh = vertexPaintTool.SharedMesh;
        
        // get ray hitted point/rayの当たった座標の取得
        hitPoint = obj.transform.InverseTransformPoint(hitPoint); // convert world position to local position(object space)

        // rayが当たったmeshを構成する頂点の取得
        if (vertices.Count != sharedMesh.vertexCount)
            vertexPaintTool.SharedMesh.GetVertices(vertices);

        // 頂点カラーを取得
        var colors = new List<Color>(sharedMesh.vertexCount);
        sharedMesh.GetColors(colors);

        // 頂点カラーの書き換え
        for (var i = 0; i < vertices.Count; i++)
        {
          var v = vertices[i]; // 頂点の座標(object space)
          float sqrDistanceOS = (hitPoint - v).sqrMagnitude; // Rayのhitした点とmeshの各頂点の距離 (object space)
          if (sqrDistanceOS > BrushSettign.brushSize * BrushSettign.brushSize) continue; // 距離がある程度離れている頂点は除外する

          if (isColorFixMode) {
            // 色を元に戻す
            colors[i] = vertexPaintTool.GetOriginVertexColor(i);
          }
          else {
            // 色を書き換える
            colors[i] = Color.Lerp(colors[i], BrushSettign.paintColor, BrushSettign.paintColor.a);
          }
        }

        // 頂点カラーを適用
        sharedMesh.SetColors(colors);
      }
    }
  }
}
