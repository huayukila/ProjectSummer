using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

public class PolygonPaintManager : View
{
    readonly SyncList<int> CountResultList = new SyncList<int>();

    public Paintable mapPaintable;
    public Shader texturePaint;
    public Shader areaPaint;
    public ComputeShader computeShader;
    public Sprite player1AreaTexture;
    public Sprite player2AreaTexture;

    CommandBuffer command;
    ComputeBuffer countBuffer;
    Material paintMaterial;
    Material areaMaterial;
    RenderTexture copyRT;

    int kernelHandle;
    int colorID = Shader.PropertyToID("_Color");
    int textureID = Shader.PropertyToID("_MainTex");
    int maxVertNum = Shader.PropertyToID("_MaxVertNum");
    int playerAreaTextureID = Shader.PropertyToID("_PlayerAreaText");

    protected void Awake()
    {
        mapPaintable.Init();
        if (isServer)
        {
            foreach (var connection in NetworkServer.connections)
            {
                //接続しているクライアント数によってデータ増加
                CountResultList.Add(0);
            }
        }

        CountResultList.Callback += OnsyncListChanged;

        paintMaterial = new Material(texturePaint);
        areaMaterial = new Material(areaPaint);

        kernelHandle = computeShader.FindKernel("CSMain");
        countBuffer = new ComputeBuffer(2, sizeof(int));

        command = new CommandBuffer();
        command.name = "CommmandBuffer - " + gameObject.name;

        computeShader.SetBuffer(kernelHandle, "CountBuffer", countBuffer);
        computeShader.SetVector("TargetColorA", Global.PLAYER_TRACE_COLORS[0]);
        computeShader.SetVector("TargetColorB", Global.PLAYER_TRACE_COLORS[1]);
        GetSystem<IPaintSystem>().RegisterManager(this);
    }

    /// <summary>
    /// ミニマップを獲得
    /// </summary>
    /// <returns></returns>
    public RenderTexture GetMiniMapRT()
    {
        if (copyRT == null)
        {
            Debug.LogError("render texture is not already");
            return null;
        }
        return copyRT;
    }


    private int m_MixVariant = Global.MAP_SIZE_WIDTH * Global.MAP_SIZE_HEIGHT * 10000; //計算負担軽減するため、プリ計算

    /// <summary>
    /// プレイヤーのマップ占有率
    /// </summary>
    /// <returns></returns>
    public float[] GetPlayersAreaPercent()
    {
        float[] temp = new float[CountResultList.Count];
        for (int i = 0; i < CountResultList.Count; i++)
        {
            temp[i] = CountResultList[i] / (float)m_MixVariant;
        }

        return temp;
    }

    /// <summary>
    /// ポリゴン描画関数
    /// </summary>
    /// <param name="worldPosList">輸入の世界座標の点</param>
    /// <param name="color">描きたいの色</param>
    /// <param name="index">プレイヤーの番号(1-2)</param>
    [Command]
    public void CmdPaint(Vector3[] worldPosList, int index, Color32 color)
    {
        RpcPaintPolygon(worldPosList, index, color);
        CountPixelByColor();
    }

    #region 内部用

    /// <summary>
    /// 分数計算
    /// </summary>
    /// <param name="color"></param>
    [Server]
    void CountPixelByColor()
    {
        computeShader.Dispatch(kernelHandle,
            mapPaintable.GetCopy().width / 10,
            mapPaintable.GetCopy().height / 10, 1);
        countBuffer.GetData(CountResultList.ToArray());
        countBuffer.SetData(new int[2] { 0, 0 });
    }

    [ClientRpc]
    void RpcPaintPolygon(Vector3[] worldPosList, int index, Color32 color)
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
        paintMaterial.SetColor(colorID, color);
        paintMaterial.SetTexture(textureID, copy);

        //shader変数設置
        areaMaterial.SetInt(maxVertNum, worldPosList.Length);
        areaMaterial.SetVectorArray("_worldPosList", posList);
        areaMaterial.SetTexture(textureID, areaCopy);


        switch (index)
        {
            //プレイヤー１なら
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
    }

    //syncList変わったときのCallback
    void OnsyncListChanged(SyncList<int>.Operation op, int Index, int oldItem, int newItem)
    {
        TypeEventSystem.Instance.Send(new RefreshVSBarEvent
            { PlayerPixelNums = CountResultList.ToArray() });
    }

    private void OnDestroy()
    {
        countBuffer.Release();
        countBuffer = null;
    }

    #endregion
}