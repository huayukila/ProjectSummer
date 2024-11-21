using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class PaintBubbleController : MonoBehaviour,IExplodable,IItemAffectable
{
    private const int EXPLODE_VERTEX_COUNT = 30;
    private Color _bubbleColor = Color.clear;
    private float _waitForExplodeTime = Global.BUBBLE_EXPLODE_TIME;
    private float _explodeRadius = 0f;
    private int _ownerPlayerID = -1;
    private Material _material;
    private SpriteRenderer _imageRenderer;
    private Collider[] _explodeTargetColliders;
    private Animator _animator;
    private Coroutine _explodeCoroutine;
    public Color Color => _bubbleColor;
    public int OwnerPlayerID => _ownerPlayerID;


    private void Awake()
    {
        Timer explodeTimer = new Timer(Time.time,_waitForExplodeTime,ExplodeBubble);
        explodeTimer.StartTimer(this);
        _imageRenderer = GetComponentInChildren<SpriteRenderer>();
        
        _explodeTargetColliders = new Collider[Global.PLAYER_MAX_COUNT];

        _animator = GetComponent<Animator>();
        _explodeCoroutine = null;
    }
    private void Start()
    {
        _material = _imageRenderer.material;
        _material.color = _bubbleColor;
        _imageRenderer.material = _material;
    }

    private void Update()
    {
        if (_explodeCoroutine != null)
        {
            return;
        }

        _waitForExplodeTime -= Time.deltaTime;
        if (_waitForExplodeTime <= 0f)
        {
            ExplodeBubble();
        }
    }

    public void SetExplodeProperty(int owner, float radius, Color color)
    {
        _ownerPlayerID = owner;
        _explodeRadius = radius;
        _bubbleColor = color;
    }

    private void OnDestroy()
    {
        _explodeCoroutine = null;
        if (_material != null)
        {
            Destroy(_material);
        }
    }
    private void ExplodeBubble()
    {
        PaintExplodeArea();
        _explodeCoroutine ??= StartCoroutine(PlayExplodeAnimAndDestroy());
    }

    private void PaintExplodeArea()
    {
        if (_ownerPlayerID == -1)
        {
            return;
        }

        List<Vector3> explodeAreaVertexes = new List<Vector3>();
        for(int i = 0;i < EXPLODE_VERTEX_COUNT;++i)
        {
            Quaternion angle = Quaternion.Euler(0f, 360f / (float)EXPLODE_VERTEX_COUNT * (float)i, 0f);
            Vector3 vert =  angle * Vector3.right ;
            explodeAreaVertexes.Add(vert.normalized * _explodeRadius + transform.position);
        }

        int cnt = Physics.OverlapSphereNonAlloc(    transform.position,
                                                    _explodeRadius,
                                                    _explodeTargetColliders,
                                                    LayerMask.GetMask("Player")
                                                );

        for(int i = 0; i < cnt; ++i)
        {
            if(_explodeTargetColliders[i].TryGetComponent(out IItemAffectable itemAffectable))
            {
                itemAffectable.OnAffect(this);
            }
        }

        PolygonPaintManager.Instance.Paint(explodeAreaVertexes.ToArray(), _ownerPlayerID, _bubbleColor);
    }

    private IEnumerator PlayExplodeAnimAndDestroy()
    {
        _material.color = Color.white;
        _animator.Play("Explode");

        // wait one frame(Unity need to reset animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
        yield return null;

        // TODO need change explode anim size
        gameObject.transform.localScale *= 5f;

        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        
        Destroy(gameObject);
        yield break;
    }

    void IItemAffectable.OnAffect(StunSilkController stunSilk)
    {
        _waitForExplodeTime = float.PositiveInfinity;
        ExplodeBubble();
    }

    void IItemAffectable.OnAffect(BananaPeelController bananaPeel)
    { 
        //* Do nothing
    }
    void IItemAffectable.OnAffect(PaintBubbleController paintBubble)
    {
      //* Do nothing 
    }
}
