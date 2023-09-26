using UnityEngine;
using UnityEngine.UI;

public class MenuPanelExample : MonoBehaviour
{
    //这里写获取的ui组件，绑定到panel上时记得把组件拖进去
    //否则使用ui组件时会报空
    public Button startBtn;
    public Text showText;

    private void Start()
    {
        startBtn.onClick.AddListener(() =>
        {
            //写业务逻辑，这里输出一个测试的log
            Debug.Log("GameStart");
        });
        showText.text= string.Empty;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) )
        {
            showText.text = "hello";
        }
    }
}
