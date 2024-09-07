using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OilPaintingPass : ScriptableRenderPass
{
  private const string ProfilerTag = nameof(OilPaintingPass);

  private readonly Material material;

  private RTHandle cameraColorTarget;

  public OilPaintingPass(Shader shader)
  {
      material = CoreUtils.CreateEngineMaterial(shader);
      renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
  }

  public void SetRenderTarget(RTHandle target)
  {
      cameraColorTarget = target;
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
  {
      if (renderingData.cameraData.isSceneViewCamera)
      {
          return;
      }

      var cmd = CommandBufferPool.Get(ProfilerTag);
      // Blitterで描画する
      Blitter.BlitCameraTexture(cmd, cameraColorTarget, cameraColorTarget, material, 0);
      context.ExecuteCommandBuffer(cmd);
      CommandBufferPool.Release(cmd);
  }
}