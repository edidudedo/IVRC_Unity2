// https://qiita.com/r-ngtm/items/261a529b6f516263cad9

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using System.Runtime.InteropServices;

public class PaintToolManager : MonoBehaviour
{
    private List<Vector3> vertices = new List<Vector3>();   // position of vertex
    public RayInteractor rayInteractorR, rayInteractorL;

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
        // Check if rayInteractor is assigned
        if (rayInteractorR.HasInteractable || rayInteractorL.HasInteractable)
        {
          // get hitPoint of ray and mesh
          RayInteractor rayInteractor;
          if (rayInteractorR.HasInteractable) rayInteractor = rayInteractorR;
          else rayInteractor = rayInteractorL;
          var hitPoint = rayInteractor.CollisionInfo.Value.Point;

          var rayInteractable = rayInteractor.Candidate;
          var temp = rayInteractable.gameObject;
          var hoge = temp.GetComponent<VertexPaintTool>();
          Mesh sharedMesh = hoge.SharedMesh;
          
          hitPoint = temp.transform.InverseTransformPoint(hitPoint);

          // 頂点の取得
          if (vertices.Count != sharedMesh.vertexCount)
              hoge.SharedMesh.GetVertices(vertices);

          // 頂点カラーを取得
          var colors = new List<Color>(sharedMesh.vertexCount);
          sharedMesh.GetColors(colors);

          // 頂点カラーの書き換え
          print(vertices.Count);
          for (var i = 0; i < vertices.Count; i++)
          {
              var v = vertices[i]; // 頂点座標(オブジェクトSpace)
              float sqrDistanceOS = (hitPoint - v).sqrMagnitude; // Rayヒットと頂点の距離 (オブジェクトSpace)
              if (sqrDistanceOS > BrushSettign.brushSize * BrushSettign.brushSize) continue; // 距離がある程度離れている頂点は除外する

              // 色の書き換え
              colors[i] = Color.Lerp(colors[i], BrushSettign.paintColor, BrushSettign.paintColor.a);
          }

          // 頂点カラーを設定
          sharedMesh.SetColors(colors);
        }
        // else
        // {
        //   Debug.LogWarning("Ray Interactor is not assigned.");
        //   return;
        // }
    }
}
