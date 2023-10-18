using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CDAnimControl : MonoBehaviour
{
    public List<Sprite> cdSprites = new List<Sprite>();
    public Animation cdAnimator;
    public Image cdIMG;
    public FightAnimControl fightAnimControl;
    public float upSpeed = 40f;

    int Index;
    int countdownTimes;
    bool isPlayFX = false;
    bool isStartCountDown;
    Vector3 rootTransform;


    private void Start()
    {
        isStartCountDown = false;
        rootTransform = transform.position;
        Index = 0;
        countdownTimes = 3;
        cdIMG.sprite = cdSprites[Index];
    }

    private void Update()
    {
        if (isStartCountDown)
        {
            if (!isPlayFX && countdownTimes != 0)
            {
                cdAnimator.Play("CDAnimation");
            }
            if (countdownTimes == 0 && !isPlayFX)
            {
                fightAnimControl.PlayFight(upSpeed);
            }
            transform.position += Vector3.up * upSpeed * Time.deltaTime;
            if (fightAnimControl.isPlayDone)
            {
                gameObject.SetActive(false);
            }
        }
    }

    [Button]
    public void StartCD()
    {
        isStartCountDown = true;
    }

    public void ChangeSprite()
    {
        Index++;
        if (Index < cdSprites.Count)
        {
            cdIMG.sprite = cdSprites[Index];
        }
        countdownTimes--;
        isPlayFX = false;
        transform.position= rootTransform;
    }
    public void PlayFX()
    {
        AudioManager.Instance.PlayFX("CDFX", 1f);
    }
}
