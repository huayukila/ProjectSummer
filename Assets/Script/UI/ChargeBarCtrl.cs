using UnityEngine;
using UnityEngine.UI;

public partial class ChargeBarCtrl : ViewController
{
    public Animator fireAnimator;
    private IPlayerBoost boost;

    // Start is called before the first frame update
    void Start()
    {
        TypeEventSystem.Instance.Register<SendInitializedPlayerEvent>(e => { boost = e.Player as IPlayerBoost; });
        Mask.fillAmount = 0.4f;
    }

    // Update is called once per frame
    void Update()
    {
        if (boost != null)
        {
            Mask.fillAmount = boost.ChargeBarPercentage * 0.4f;
        }
    }
}