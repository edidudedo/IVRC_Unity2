using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ブラシの設定（塗る色、ブラシのサイズ）を保存するスクリプト / Script to save brush settings (paint color, brush size)
/// </summary>
public static class BrushSettign
{
    [SerializeField] public static Color paintColor = Color.gray;           // ブラシカラー/brush collar
    [SerializeField, Range(0f, 1f)] public static float brushSize = 0.3f;   // ブラシサイズ/brush size
}
