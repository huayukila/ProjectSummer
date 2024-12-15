using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    enum State
    {
        DEFAULT,
        RED,
        BLUE
    }

    public SpriteRenderer defaultBase;
    public SpriteRenderer blueBase;

    private float redPrecent;
    private float bluePrecent;
    private float durationTime = 1.5f;

    private State currentState = State.DEFAULT;

    private void Start()
    {
        TypeEventSystem.Instance.Register<RefreshVSBarEvent>(e =>
        {
            float[] values = PolygonPaintManager.Instance.GetPlayersAreaPercent();
            redPrecent = values[0];
            bluePrecent = values[1];
            TryChange();
        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    void TryChange()
    {
        if (bluePrecent > 2f || redPrecent > 2f)
        {
            if (bluePrecent / redPrecent > 2)
            {
                ChangeToBlue();
                currentState = State.BLUE;
            }
            else if (redPrecent / bluePrecent > 2)
            {
                currentState = State.RED;
                ChangeToRed();
            }
            return;
        }

        if (bluePrecent < 20 && currentState == State.BLUE)
        {
            ChangeToDefault();
        }
        else if (redPrecent < 20 && currentState == State.RED)
        {
            ChangeToDefault();
        }
    }

    void ChangeToDefault()
    {
        defaultBase.DOFade(1, durationTime);
        blueBase.DOFade(1, durationTime);
    }

    void ChangeToBlue()
    {
        defaultBase.DOFade(0, durationTime);
        blueBase.DOFade(1, durationTime);
    }

    void ChangeToRed()
    {
        defaultBase.DOFade(0, durationTime);
        blueBase.DOFade(0, durationTime);
    }
}