using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviour
{
    public enum PlayerID
    {
        Left = 1,
        Right,
    }

    public PlayerID id;
    public Image ChargeBar;
    public Image ItemImg;
    private bool isBoost = false;


// Start is called before the first frame update
    void Start()
    {
        ItemImg.gameObject.SetActive(false);
        TypeEventSystem.Instance.Register<BoostStart>(e =>
            {
                if (e.Number == (int)id)
                {
                    ChargeBar.DOFillAmount(0, Global.BOOST_DURATION_TIME);
                }
            })
            .UnregisterWhenGameObjectDestroyed(gameObject);
    }

// Update is called once per frame
    void Update()
    {
        if (ChargeBar.fillAmount <= 0)
        {
            ChargeBar.DOFillAmount(0.4f, Global.BOOST_COOLDOWN_TIME);
        }
    }
}