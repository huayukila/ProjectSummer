using UnityEngine;

public class TestPostProcess : MonoBehaviour
{
  public Material mat;
  private void OnRenderImage(RenderTexture src, RenderTexture dest) {
    Graphics.Blit(src, dest, mat);
  }
}