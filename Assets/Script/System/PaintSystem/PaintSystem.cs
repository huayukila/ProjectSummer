using UnityEngine;

interface IPaintSystem: ISystem
{
    void RegisterManager(PolygonPaintManager paintManager);

    RenderTexture GetMiniMap();
    void Paint(Vector3[] worldPosList, int index, Color32 color);
    float[] GetPlayerAreaPercent();
}
public class PaintSystem : AbstractSystem,IPaintSystem
{
    private PolygonPaintManager m_PaintManager;
    protected override void OnInit()
    {
    }

    public void RegisterManager(PolygonPaintManager paintManager)
    {
        m_PaintManager = paintManager;
    }

    public RenderTexture GetMiniMap()
    {
        return m_PaintManager.GetMiniMapRT();
    }

    public void Paint(Vector3[] worldPosList, int index, Color32 color)
    {
        m_PaintManager.CmdPaint(worldPosList,index,color);
    }

    public float[] GetPlayerAreaPercent()
    {
        return m_PaintManager.GetPlayersAreaPercent();
    }
}
