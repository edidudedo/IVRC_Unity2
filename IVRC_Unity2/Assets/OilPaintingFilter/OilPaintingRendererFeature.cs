using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OilPaintingRendererFeature : ScriptableRendererFeature
{
    [SerializeField]
    private Shader shader;

    private OilPaintingPass grayscalePass;

    public override void Create()
    {
        grayscalePass = new OilPaintingPass(shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(grayscalePass);
    }

    // renderer.cameraColorTargetはSetupRenderPasses内で呼ぶ
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        // cameraColorTarget -> cameraColorTargetHandleにする
        grayscalePass.SetRenderTarget(renderer.cameraColorTargetHandle);
    }
}