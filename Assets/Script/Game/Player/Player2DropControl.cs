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
        TR.startColor = Global.PLAYER_TWO_COLOR;
        TR.endColor = Global.PLAYER_TWO_COLOR;
        TR.startWidth = 0.5f;
        TR.endWidth = 0.5f;
        TR.time = 3.0f;
    }
}
