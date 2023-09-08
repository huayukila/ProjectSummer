using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PolygonPaintManager : Singleton<PolygonPaintManager>
{
    public Shader texturePaint;
    CommandBuffer command;
    private Material paintMaterial;
    int colorID = Shader.PropertyToID("_Color");
    int textureID = Shader.PropertyToID("_MainTex");
    int maxVertNum = Shader.PropertyToID("_MaxVertNum");

    /// <summary>
    /// ‰˜‚³‚ê‚½RT‚ğãY—í‚É‚·‚é
    /// </summary>
    /// <param name="rt"></param>
    public void ClearRT(RenderTexture rt)
    {
        CommandBuffer command = new CommandBuffer();
        command.SetRenderTarget(rt);
        command.ClearRenderTarget(true, true, Color.clear);
        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }
    /// <summary>
    /// ƒ|ƒŠƒSƒ“•`‰æŠÖ”
    /// </summary>
    /// <param name="worldPosList">—A“ü‚Ì¢ŠEÀ•W‚Ì“_</param>
    /// <param name="color">•`‚«‚½‚¢‚ÌF</param>
    public void Paint(Vector3[] worldPosList, Color? color = null)
    {
        Paintable mapPaintable=GameManager.Instance.mapPaintable;
        RenderTexture mask = mapPaintable.GetMask();
        RenderTexture copy = mapPaintable.GetCopy();
        Renderer rend = mapPaintable.GetRenderer();
        //shader‚Í4ŸŒ³‚Ì”z—ñ‚µ‚©ó‚¯‚ç‚ê‚È‚¢‚Ì‚ÅAˆê‰“]Š·
        Vector4[] posList = new Vector4[100];
        for (int i = 0; i < worldPosList.Length; i++)
        {
            posList[i] = worldPosList[i];
        }
        //shader•Ï”İ’u
        paintMaterial.SetInt(maxVertNum, worldPosList.Length);
        paintMaterial.SetVectorArray("_worldPosList", posList);
        paintMaterial.SetColor(colorID, color ?? Color.red);
        paintMaterial.SetTexture(textureID, copy);

        //render‚Ì–Ú•W‚ğmask‚Éİ’è‚·‚é
        command.SetRenderTarget(mask);
        //renderŠJn
        command.DrawRenderer(rend, paintMaterial, 0);

        //–Ú•W‚ğcopy‚Éİ’è
        command.SetRenderTarget(copy);
        //mask‚Ì•`‰æ‚ğcopy‚É‰Á‚¦‚é
        command.Blit(mask, copy);

        //render‚Ì–½—ß‚ğ—¬‚ê‚³‚¹‚é
        Graphics.ExecuteCommandBuffer(command);
        //–½—ß‘à—ñƒNƒŠƒA
        command.Clear();
    }
    /// <summary>
    /// debug—pŠÖ”
    /// </summary>
    /// <param name="paintable"></param>
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
    protected override void Awake()
    {
        base.Awake();
        paintMaterial = new Material(texturePaint);
        command = new CommandBuffer();
        command.name = "CommmandBuffer - " + gameObject.name;
    }
}
