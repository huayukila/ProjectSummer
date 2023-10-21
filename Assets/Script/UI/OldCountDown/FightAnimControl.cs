using UnityEngine;
using UnityEngine.UI;

public class FightAnimControl : MonoBehaviour
{
    public Animation TimeOutAnimator;
    public Image FightIMG;

    public bool isPlayDone { get; private set; } = false;

    bool isStartPlay;
    float upSpeed;

    private void Start()
    {
        isStartPlay = false;
    }
    public void PlayFight(float upSpeed)
    {
        isStartPlay=true;
        this.upSpeed = upSpeed;
        TimeOutAnimator.Play("TimeOutAnimation");
    }

    public void PlayFX()
    {
        AudioManager.Instance.PlayFX("TimeOutFX", 1.0f);
    }
    public void PlayDone()
    {
        isPlayDone=true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isStartPlay&&!isPlayDone)
        {
            transform.position += Vector3.up * upSpeed * Time.deltaTime;
        }
    }
}
