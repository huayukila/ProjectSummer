using UnityEngine;

public class Player2DropControl : DropPointControl
{
    protected override void InstantiateDropPoint()
    {
        GameObject pt = Instantiate(_pointPrefab, transform.position, transform.rotation);
        pt.tag = "DropPoint2";
        DropPointManager.Instance.PlayerTwoAddPoint(pt);
    }
    protected override void SetTRProperties()
    {
        TR.material = new Material(Shader.Find("Sprites/Default"));
        TR.startColor = Color.green;
        TR.endColor = Color.green;
        TR.startWidth = 0.5f;
        TR.endWidth = 0.5f;
        TR.time = Global.DROP_POINT_ALIVE_TIME;
    }
}
