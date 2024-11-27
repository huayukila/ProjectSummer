using UnityEngine;
using UnityEngine.UI;

public class TimeBarCtrl : MonoBehaviour
{
    public Image timeBar;
    private float time;

    void Start()
    {
        time = Global.SET_GAME_TIME;
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0f)
        {
            time -= Time.deltaTime;
            timeBar.fillAmount = time / Global.SET_GAME_TIME;
        }
    }
}