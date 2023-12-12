using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class PolygonPaintManager : Singleton<PolygonPaintManager>
{
    public Shader texturePaint;
    public Shader areaPaint;
    public ComputeShader computeShader;

    public Sprite player1AreaTexture;
    public Sprite player2AreaTexture;

    Paintable mapPaintable;
    CommandBuffer command;
    ComputeBuffer mCountBuffer;
    Material paintMaterial;
    Material areaMaterial;
    RenderTexture CopyRT;

    int kernelHandle;
    int colorID = Shader.PropertyToID("_Color");
    int textureID = Shader.PropertyToID("_MainTex");
    int maxVertNum = Shader.PropertyToID("_MaxVertNum");
    int playerAreaTextureID = Shader.PropertyToID("_PlayerAreaText");

    int[] CountResultArray = new int[2];

    protected override void Awake()
    {
        paintMaterial = new Material(texturePaint);
        areaMaterial = new Material(areaPaint);

        kernelHandle = computeShader.FindKernel("CSMain");
        mCountBuffer = new ComputeBuffer(2, sizeof(int));

        command = new CommandBuffer();
        command.name = "CommmandBuffer - " + gameObject.name;


        computeShader.SetBuffer(kernelHandle, "CountBuffer", mCountBuffer);
        computeShader.SetVector("TargetColorA", Global.PLAYER_TRACE_COLORS[0]);
        computeShader.SetVector("TargetColorB", Global.PLAYER_TRACE_COLORS[1]);
    }

    /// <summary>
    /// ミニマップを獲得
    /// </summary>
    /// <returns></returns>
    public RenderTexture GetMiniMapRT()
    {
        if (CopyRT == null)
        {
            Debug.LogError("render texture is not already");
            return null;
        }

        return CopyRT;
    }


    private int m_MixVariant = Global.MAP_SIZE_WIDTH * Global.MAP_SIZE_HEIGHT * 10000;//計算負担軽減するため、プリ計算
    /// <summary>
    /// プレイヤーのマップ占有率
    /// </summary>
    /// <returns></returns>
    public float[] GetPlayersAreaPercent()
    {
        float[] temp = new float[CountResultArray.Length];
        for (int i = 0; i < CountResultArray.Length; i++)
        {
            temp[i] = CountResultArray[i] /(float)m_MixVariant;
        }
        return temp;
    }

    public void SetPaintable(Paintable paintable)
    {
        mapPaintable = paintable;
        CopyRT = mapPaintable.GetCopy();
        computeShader.SetTexture(kernelHandle, "Result", CopyRT);
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
        TypeEventSystem.Instance.Send(new RefreshVSBarEvent
            { PlayerPixelNums = CountResultArray });
    }

    #region 内部用

    /// <summary>
    /// 分数計算
    /// </summary>
    /// <param name="color"></param>
    void CountPixelByColor()
    {
        computeShader.Dispatch(kernelHandle, mapPaintable.GetCopy().width / 10,
            mapPaintable.GetCopy().height / 10, 1);
        mCountBuffer.GetData(CountResultArray);
        mCountBuffer.SetData(new int[2] { 0, 0 });
    }

    private void OnDestroy()
    {
        mCountBuffer.Release();
        mCountBuffer = null;
    }

    #endregion
}