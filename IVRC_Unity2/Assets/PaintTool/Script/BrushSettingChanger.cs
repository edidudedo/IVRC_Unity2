using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ブラシの設定（塗る色、ブラシのサイズ）を管理するスクリプト / Script to manage brush settings (paint color, brush size)
/// </summary>
public class BrushSettingChanger : MonoBehaviour
{
    [SerializeField] private Color paintColor = Color.white;
    [SerializeField] private float brushSize = 0.003f;

    public void ChangeBrushSetting()
    {
        BrushSettign.paintColor = paintColor;
        BrushSettign.brushSize = brushSize;
    }
}
