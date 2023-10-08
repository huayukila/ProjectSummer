using UnityEngine;

public class Player1DropControl : DropPointControl
{
    protected override void InstantiateDropPoint()
    {
        GameObject pt = Instantiate(pointPrefab,transform.position,transform.rotation);
        pt.tag = "DropPoint1";
        DropPointManager.Instance.PlayerOneAddPoint(pt);
    }
    protected override void SetTRProperties()
    {
        tr.material = new Material(Shader.Find("Sprites/Default"));
        tr.startColor = Global.PLAYER_ONE_TRACE_COLOR;
        tr.endColor = Global.PLAYER_ONE_TRACE_COLOR;
        tr.startWidth = 0.5f;
        tr.endWidth = 0.5f;
        tr.time = Global.DROP_POINT_ALIVE_TIME;
    }

    protected override void Update()
    {
        base.Update();
        fadeOutTimer += Time.deltaTime;
        if(fadeOutTimer >= Global.DROP_POINT_ALIVE_TIME / 2.0f && fadeOutTimer < Global.DROP_POINT_ALIVE_TIME)
        {
            float alpha = (-1.9f / Global.DROP_POINT_ALIVE_TIME) * fadeOutTimer + 1.95f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Global.PLAYER_ONE_TRACE_COLOR, 0.0f), new GradientColorKey(Global.PLAYER_ONE_TRACE_COLOR, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(alpha >= 0.05f ? alpha : 0.05f, 1.0f) }
            );
            tr.colorGradient = gradient;
        }
    }

    public void ResetTrail()
    {
        fadeOutTimer = 0.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Global.PLAYER_ONE_TRACE_COLOR, 0.0f), new GradientColorKey(Global.PLAYER_ONE_TRACE_COLOR, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        tr.colorGradient = gradient;
    }
}
