using UnityEngine;
using UnityEngine.Rendering;

public class PolygonPaintManager : Singleton<PolygonPaintManager>
{
    public Shader texturePaint;
    public Shader areaPaint;

    public RenderTexture playerAreaTexture;
    public RenderTexture playerBreaTexture;

    public Paintable mapPaintable;

    CommandBuffer command;
    private Material paintMaterial;
    private Material areaMaterial;
    int colorID = Shader.PropertyToID("_Color");
    int textureID = Shader.PropertyToID("_MainTex");
    int maxVertNum = Shader.PropertyToID("_MaxVertNum");

    /// <summary>
    /// 汚されたRTを綺麗にする
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
    /// ポリゴン描画関数
    /// </summary>
    /// <param name="worldPosList">輸入の世界座標の点</param>
    /// <param name="color">描きたいの色</param>
    public void Paint(Vector3[] worldPosList, Color? color = null)
    {
        RenderTexture mask = mapPaintable.GetMask();
        RenderTexture copy = mapPaintable.GetCopy();
        Renderer rend = mapPaintable.GetRenderer();

        //shaderは4次元の配列しか受けられないので、一応転換
        Vector4[] posList = new Vector4[100];
        for (int i = 0; i < worldPosList.Length; i++)
        {
            posList[i] = worldPosList[i];
        }
        //shader変数設置
        paintMaterial.SetInt(maxVertNum, worldPosList.Length);
        paintMaterial.SetVectorArray("_worldPosList", posList);
        paintMaterial.SetColor(colorID, color ?? Color.red);
        paintMaterial.SetTexture(textureID, copy);

        //renderの目標をmaskに設定する
        command.SetRenderTarget(mask);
        //render開始
        command.DrawRenderer(rend, paintMaterial, 0);

        //目標をcopyに設定
        command.SetRenderTarget(copy);
        //maskの描画をcopyに加える
        command.Blit(mask, copy);

        //renderの命令を流れさせる
        Graphics.ExecuteCommandBuffer(command);
        //命令隊列クリア
        command.Clear();
    }

    public void PaintArea(Vector3[] worldPosList)
    {
        RenderTexture areaMask = mapPaintable.GetAreaMask();
        RenderTexture areaCopy=mapPaintable.GetAreaCopy();
        Renderer rend = mapPaintable.GetRenderer();

        Vector4[] posList = new Vector4[100];
        for (int i = 0; i < worldPosList.Length; i++)
        {
            posList[i] = worldPosList[i];
        }
        //shader変数設置
        areaMaterial.SetInt(maxVertNum, worldPosList.Length);
        areaMaterial.SetVectorArray("_worldPosList", posList);
        areaMaterial.SetTexture(textureID, playerAreaTexture);

        //renderの目標をmaskに設定する
        command.SetRenderTarget(areaMask);
        //render開始
        command.DrawRenderer(rend, areaMaterial, 0);

        //目標をcopyに設定
        command.SetRenderTarget(areaCopy);
        //maskの描画をcopyに加える
        command.Blit(areaMask, areaCopy);

        //renderの命令を流れさせる
        Graphics.ExecuteCommandBuffer(command);
        //命令隊列クリア
        command.Clear();
    }

    /// <summary>
    /// debug用関数
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
        areaMaterial = new Material(areaPaint);

        command = new CommandBuffer();
        command.name = "CommmandBuffer - " + gameObject.name;
    }
}
