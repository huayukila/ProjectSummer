using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemBoxController : MonoBehaviour
{
  private enum EBoxState
  {
    Spawn,
    Idle,
    Open,
    Unavailable,
  }

  private EBoxState _boxState;
  private Vector3 _defaultPos;
  private RainbowOutlineEffect _outlineEffect;
  [SerializeField]
  private SpriteRenderer _boxRenderer;
  [SerializeField]
  private SpriteRenderer _shadowRenderer;
  private Animator _boxAnimator;
  private Coroutine _boxAnimCoroutine;

  public bool IsInactive => _defaultPos != transform.position;

  // Start is called before the first frame update
  void Start()
  {
    _boxState = EBoxState.Idle;
    _defaultPos = transform.position;
    _outlineEffect = new RainbowOutlineEffect(_boxRenderer.material);

    _boxAnimator = GetComponent<Animator>();
  }

  private void Update()
  {
    switch (_boxState)
    {
      case EBoxState.Idle:
      {
        _outlineEffect?.UpdateOutline(Time.deltaTime);
      }
      break;
      case EBoxState.Unavailable:
      {
        return;
      }
      case EBoxState.Spawn:
      {
        _boxAnimCoroutine ??= StartCoroutine(SpawnBox());
      }
      break;
      case EBoxState.Open:
      {
        _boxAnimCoroutine ??= StartCoroutine(OpenBox());
      }
      break;
    }
  }

  public void SetInactive()
  {
    _boxState = EBoxState.Open;
    _outlineEffect?.SetActive(false);
  }

  private void SetRespawnTimer()
  {
    Timer respawnTimer = new Timer(Time.time,Global.ITEM_BOX_SPAWN_TIME,
    () =>
    {
      transform.position = _defaultPos;
      _boxState = EBoxState.Spawn;
    });
    respawnTimer.StartTimer(this);
  }

  private IEnumerator SpawnBox()
  {
    _boxAnimator.Play("Spawn");
    yield return null;

    yield return new WaitUntil(() => _boxAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

    _boxState = EBoxState.Idle;
    _boxAnimCoroutine = null;
    _boxAnimator.Play("Idle");
    _outlineEffect?.SetActive(true);

    yield break;
  }

  private IEnumerator OpenBox()
  {
    _boxAnimator.Play("Open");
    yield return null;

    yield return new WaitUntil(() => _boxAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

    _boxState = EBoxState.Unavailable;
    _boxAnimCoroutine = null;
    transform.position = Global.GAMEOBJECT_STACK_POS;
    SetRespawnTimer();

    yield break;    
  }

  private void OnDestroy()
  {
    _outlineEffect?.Dispose();
  }
 
}
