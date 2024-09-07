using UnityEngine;

[ExecuteInEditMode]
public class OilFilterPostEffect : MonoBehaviour {

    [SerializeField] private Material _material;

    private void OnRenderImage(RenderTexture src, RenderTexture dst){
        Graphics.Blit(src, dst, _material);
    }
}