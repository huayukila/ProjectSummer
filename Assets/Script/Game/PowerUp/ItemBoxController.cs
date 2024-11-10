using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxController : MonoBehaviour
{
  private Vector3 _defaultPos;
  private RainbowOutlineEffect _outlineEffect;
  public bool IsInactive => _defaultPos != transform.position;

  // Start is called before the first frame update
  void Start()
  {
    _defaultPos = transform.position;
    var mat = GetComponentInChildren<Renderer>().material;
    _outlineEffect = new RainbowOutlineEffect(mat);
  }

  private void Update()
  {
    _outlineEffect?.UpdateOutline(Time.deltaTime);
  }

  public void SetInactive()
  {
    transform.position = Global.GAMEOBJECT_STACK_POS;
    SetRespawnTimer();
    _outlineEffect?.SetActive(false);
  }

  private void SetRespawnTimer()
  {
    Timer respawnTimer = new Timer(Time.time,Global.ITEM_BOX_SPAWN_TIME,
    () =>
    {
      transform.position = _defaultPos;
      _outlineEffect?.SetActive(true);
    });
    respawnTimer.StartTimer(this);
  }

  private void OnDestroy()
  {
    _outlineEffect?.Dispose();
  }
 
}
