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

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        Timer explodeTimer = new Timer(Time.time,_waitForExplodeTime,ExplodeBubble);
        explodeTimer.StartTimer(this);
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        _material = new Material(_meshRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave};
        _material.color = _bubbleColor;
        _meshRenderer.sharedMaterial = _material;
    }

    private void Update()
    {
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
        if (_material != null)
        {
            Destroy(_material);
        }
    }
    private void ExplodeBubble()
    {
        PaintExplodeArea();
        Destroy(gameObject);
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

        PolygonPaintManager.Instance.Paint(explodeAreaVertexes.ToArray(), _ownerPlayerID, _bubbleColor);
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
