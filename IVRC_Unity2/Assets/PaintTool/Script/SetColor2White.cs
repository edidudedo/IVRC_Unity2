using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 指定したメッシュの色をなくすスクリプト
/// </summary>
[DefaultExecutionOrder(1)]  // 色の保存より後に実行
public class SetColor2White : MonoBehaviour
{
    [SerializeField] private VertexPaintTool[] vertexPaints;

    private void Start()
    {
      for(int i = 0; i < vertexPaints.Length; i++) {
        vertexPaints[i].SetColor2White();
      }
    }
}
