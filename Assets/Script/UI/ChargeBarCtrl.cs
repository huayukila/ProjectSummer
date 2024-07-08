using UnityEngine;

public partial class ChargeBarCtrl : ViewController
{
    [Range(0, 1)] public float testNums;

    public Animator fireAnimator;

    // Start is called before the first frame update
    void Start()
    {
        Mask.fillAmount = testNums;
    }

    // Update is called once per frame
    void Update()
    {
        Mask.fillAmount = testNums * 0.4f;
    }
}