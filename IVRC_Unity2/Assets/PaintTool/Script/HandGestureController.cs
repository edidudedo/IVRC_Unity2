using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特定の手の形を取得するスクリプト
/// </summary>
public class HandGestureController : MonoBehaviour
{
    [SerializeField] private VertexPaintTool[] vertexPaints;

    private void Start()
    {
      for(int i = 0; i < vertexPaints.Length; i++) {
        vertexPaints[i].SetColor2White();
      }
    }
}
