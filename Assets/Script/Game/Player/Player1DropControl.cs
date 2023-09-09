using UnityEngine;

public class Player1DropControl : DropPointControl
{
    protected override void InstantiateDropPoint()
    {
        GameObject pt = Instantiate(_pointPrefab,transform.position,transform.rotation);
        pt.tag = "DropPoint1";
        DropPointManager.Instance.PlayerOneAddPoint(pt);
    }

    protected override void SetTRProperties()
    {
        TR.material = new Material(Shader.Find("Sprites/Default"));
        TR.startColor = Global.PLAYER_ONE_COLOR;
        TR.endColor = Global.PLAYER_ONE_COLOR;
        TR.startWidth = 0.5f;
        TR.endWidth = 0.5f;
        TR.time = 3.0f;
    }

}
