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
  private VertexPaintTool vertexPaintTool;


  // 壁とビームとの交点を保持するクラスrayInterator　を取得
  public void Start()
  {
    GameObject objR = GameObject.Find("[BuildingBlock] Right Ray");
    GameObject objL = GameObject.Find("[BuildingBlock] Left Ray");
    rayInteractorR = objR.GetComponent<RayInteractor>();
    rayInteractorL = objL.GetComponent<RayInteractor>();
  }

  /// <summary>
  /// コントローラのrayで色を塗る
  /// </summary>
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

        // Yellow/Brown/Greenを触った状態かどうかを判定
        if (obj.CompareTag("Yellow") && TouchedObjectController.CheeseIsTouched ||
            obj.CompareTag("Brown") && TouchedObjectController.BaguetteIsTouched ||
            obj.CompareTag("Green") && TouchedObjectController.WineIsTouched ||
            obj.CompareTag("Untagged")) {
            
          // get ray hitted mesh
          vertexPaintTool = obj.GetComponent<VertexPaintTool>();
          Mesh sharedMesh = vertexPaintTool.SharedMesh;

          // 色がまだ塗られていない OR 自分で違う色を塗りたいなら
          if (!vertexPaintTool.isPainted && isColorFixMode || !isColorFixMode) {
            // get ray hitted point/rayの当たった座標の取得
            hitPoint = obj.transform.InverseTransformPoint(hitPoint); // convert world position to local position(object space)

            // rayが当たったmeshを構成する頂点の取得
            if (vertices.Count != sharedMesh.vertexCount) vertexPaintTool.SharedMesh.GetVertices(vertices);

            // rayが当たった付近の色を変更
            ChangeVertexColor(sharedMesh, hitPoint);
          }
        }
      }
    }
  }

  /// <summary>
  /// rayのhitPoint付近のsharedMeshの色を塗る関数
  /// </summary>
  /// <param name="sharedMesh">色を塗るmesh</param>
  /// <param name="hitPoint">色を塗る中心の座標（rayのhitした場所）</param>
  private void ChangeVertexColor(Mesh sharedMesh, Vector3 hitPoint)
  {
    // 頂点カラーを取得
    var colors = new List<Color>(sharedMesh.vertexCount);
    sharedMesh.GetColors(colors);

    // 頂点カラーの書き換え
    for (var i = 0; i < vertices.Count; i++)
    {
      var v = vertices[i]; // 頂点の座標(object space)
      float sqrDistanceOS = (hitPoint - v).sqrMagnitude; // Rayのhitした点とmeshの各頂点の距離 (object space)
      if (sqrDistanceOS > BrushSettign.brushSize * BrushSettign.brushSize) continue; // 距離がある程度離れている頂点は除外する

      // 色を基に戻すモードなら
      if (isColorFixMode) {
        // 色を元に戻す
        colors[i] = vertexPaintTool.GetOriginVertexColor(i);
        vertexPaintTool.colored[i] = true;
      }
      // 色を塗るモードなら
      else {
        // 色を書き換える
        colors[i] = Color.Lerp(colors[i], BrushSettign.paintColor, BrushSettign.paintColor.a);
        vertexPaintTool.colored[i] = false;
      }
    }

    // 頂点カラーを適用
    sharedMesh.SetColors(colors);
    if (!FlagManager.startColoring) FlagManager.startColoring = true;
  }
}
