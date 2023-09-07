using System;
using UnityEngine;

public class EventSystemTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //需要从事件中获得数据的写法
        TypeEventSystem.Instance.Register<KillEvent>(e =>
        {
            Debug.Log(e.Name + " " + e.Age);
        }).UnregisterWhenGameObjectDestroyde(gameObject);
        //只需要响应事件的写法
        //TypeEventSystem.Instance.Register<GameStartEvent>(e =>
        //{
        //    kill();
        //});
    }

    void kill()
    {
        Debug.Log("Kill");
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            KillEvent killEvent = new KillEvent();
            killEvent.Name = "steven";
            killEvent.Age = 18;
            //传递数据的写法
            TypeEventSystem.Instance.Send<KillEvent>(new KillEvent
            {
                Name = "steven",
                Age = 18
            });
            
            //只发送事件的写法
            //TypeEventSystem.Instance.Send<GameStartEvent>();
        }
    }
}
