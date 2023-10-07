using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PolygonPaintManager : Singleton<PolygonPaintManager>
{
    public Shader texturePaint;
    public Shader areaPaint;
    public ComputeShader computeShader;

    public Sprite player1AreaTexture;
    public Sprite player2AreaTexture;

    public Paintable mapPaintable;

    CommandBuffer command;
    ComputeBuffer mCountBuffer;
    private Material paintMaterial;
    private Material areaMaterial;
    int kernelHandle;

    int colorID = Shader.PropertyToID("_Color");
    int textureID = Shader.PropertyToID("_MainTex");
    int maxVertNum = Shader.PropertyToID("_MaxVertNum");
    int playerAreaTextureID = Shader.PropertyToID("_PlayerAreaText");

    float redScore = 0.0f;
    float greenScore = 0.0f;
    protected override void Awake()
    {
        base.Awake();
        paintMaterial = new Material(texturePaint);
        areaMaterial = new Material(areaPaint);

        kernelHandle = computeShader.FindKernel("CSMain");
        mCountBuffer = new ComputeBuffer(2, sizeof(int));

        command = new CommandBuffer();
        command.name = "CommmandBuffer - " + gameObject.name;
    }
    private void Start()
    {
        computeShader.SetTexture(kernelHandle, "Result", mapPaintable.GetCopy());
        computeShader.SetBuffer(kernelHandle, "CountBuffer", mCountBuffer);
        computeShader.SetVector("_TargetColorA",
            new Vector4(Global.PLAYER_ONE_AREA_COLOR.r / 255.0f,
            Global.PLAYER_ONE_AREA_COLOR.g / 255.0f,
            Global.PLAYER_ONE_AREA_COLOR.b / 255.0f,
            Global.PLAYER_ONE_AREA_COLOR.a / 255.0f));
        computeShader.SetVector("_TargetColorB", 
            new Vector4(Global.PLAYER_TWO_AREA_COLOR.r / 255.0f,
            Global.PLAYER_TWO_AREA_COLOR.g / 255.0f,
            Global.PLAYER_TWO_AREA_COLOR.b / 255.0f,
            Global.PLAYER_TWO_AREA_COLOR.a / 255.0f));
    }

    private void OnGUI()
    {
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(3.0f, 3.0f, 1));
        GUILayout.BeginArea(new Rect(10, 10, 1000, 200));
        GUILayout.Label("Game Data Test", GUILayout.Width(1000));
        GUILayout.Label("Blue:" + redScore + "%", GUILayout.Width(1000));
        GUILayout.Label("Green:" + greenScore + "%", GUILayout.Width(1000));
        GUILayout.EndArea();
    }

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
    /// <param name="index">プレイヤーの番号(1-2)</param>
    public void Paint(Vector3[] worldPosList, int index, Color32? color = null)
    {
        RenderTexture mask = mapPaintable.GetMask();
        RenderTexture copy = mapPaintable.GetCopy();
        RenderTexture areaMask = mapPaintable.GetAreaMask();
        RenderTexture areaCopy = mapPaintable.GetAreaCopy();
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
        paintMaterial.SetColor(colorID, color ?? new Color32(255, 0, 0, 255));
        paintMaterial.SetTexture(textureID, copy);

        //shader変数設置
        areaMaterial.SetInt(maxVertNum, worldPosList.Length);
        areaMaterial.SetVectorArray("_worldPosList", posList);
        areaMaterial.SetTexture(textureID, areaCopy);

        //プレイヤー１なら
        switch (index)
        {
            case 1:
                areaMaterial.SetTexture(playerAreaTextureID, player1AreaTexture.texture);
                break;
            case 2:
                areaMaterial.SetTexture(playerAreaTextureID, player2AreaTexture.texture);
                break;
        }

        //色
        {
            //renderの目標をmaskに設定する
            command.SetRenderTarget(mask);
            //render開始
            command.DrawRenderer(rend, paintMaterial, 0);
            //目標をcopyに設定
            command.SetRenderTarget(copy);
            //maskの描画をcopyに加える
            command.Blit(mask, copy);
        }

        //家紋
        {
            //renderの目標をmaskに設定する
            command.SetRenderTarget(areaMask);
            //render開始
            command.DrawRenderer(rend, areaMaterial, 0);
            //目標をcopyに設定
            command.SetRenderTarget(areaCopy);
            //maskの描画をcopyに加える
            command.Blit(areaMask, areaCopy);
        }

        //renderの命令を流れさせる
        Graphics.ExecuteCommandBuffer(command);
        //命令隊列クリア
        command.Clear();
        CountPixelByColor();
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

    /// <summary>
    /// 分数計算
    /// </summary>
    /// <param name="color"></param>
    void CountPixelByColor()
    {
        computeShader.Dispatch(kernelHandle, mapPaintable.GetCopy().width / 8,
           mapPaintable.GetCopy().height / 8, 1);
        int[] CountResultArray = new int[2];
        mCountBuffer.GetData(CountResultArray);
        redScore = (CountResultArray[0] / (1920.0f * 1080.0f)) * 1000.0f;
        greenScore = (CountResultArray[1] / (1920.0f * 1080.0f)) * 1000.0f;
        mCountBuffer.SetData(new int[2] { 0, 0 });
    }

    private void OnDestroy()
    {
        mCountBuffer.Release();
        mCountBuffer = null;
    }
}
