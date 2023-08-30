using UnityEngine;
using UnityEngine.Rendering;

namespace PaintOnObject
{
    public class PolygonPaintManager:Singleton<PolygonPaintManager>
    {
        public Shader texturePaint;
        CommandBuffer command;
        private Material paintMaterial;
        int colorID = Shader.PropertyToID("_Color");
        int textureID = Shader.PropertyToID("_MainTex");
        int maxVertNum = Shader.PropertyToID("_MaxVertNum");
        public void ClearRT(RenderTexture rt)
        {
            CommandBuffer command = new CommandBuffer();
            command.SetRenderTarget(rt);
            command.ClearRenderTarget(true, true, Color.clear);
            Graphics.ExecuteCommandBuffer(command);
            command.Clear();
        }

        public void InitUVMask(Paintable paintable)
        {
            RenderTexture mask = paintable.GetMask();
            RenderTexture copy = paintable.GetCopy();
            Renderer rend = paintable.GetRenderer();

            command.SetRenderTarget(mask);
            command.SetRenderTarget(copy);
            command.DrawRenderer(rend, paintMaterial, 0);

            Graphics.ExecuteCommandBuffer(command);
            command.Clear();
        }

        public void Paint(Paintable paintable, Vector3[] worldPosList, Color? color = null)
        {
            RenderTexture mask = paintable.GetMask();
            RenderTexture copy = paintable.GetCopy();
            Renderer rend = paintable.GetRenderer();

            Vector4[] posList=new Vector4[100];
            for(int i=0;i<worldPosList.Length;i++)
            {
                posList[i] = worldPosList[i];
            }
            paintMaterial.SetInt(maxVertNum, worldPosList.Length);
            paintMaterial.SetVectorArray("_worldPosList", posList);
            paintMaterial.SetColor(colorID, color ?? Color.red);
            paintMaterial.SetTexture(textureID, copy);

            command.SetRenderTarget(mask);
            command.DrawRenderer(rend, paintMaterial, 0);

            command.SetRenderTarget(copy);
            command.Blit(mask, copy);

            Graphics.ExecuteCommandBuffer(command);
            command.Clear();
        }

        protected override void Awake()
        {
            base.Awake();
            paintMaterial = new Material(texturePaint);
            command = new CommandBuffer();
            command.name = "CommmandBuffer - " + gameObject.name;
        }
    }
}