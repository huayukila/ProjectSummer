using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;


public class PaintBubbleController : MonoBehaviour,IExplodable
{
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
    // Update is called once per frame
    void Update()
    {
        
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
        Debug.LogWarning("Explode!!!");
        PaintExplodeArea();
        JamEnemyPlayerScreen();
        Destroy(gameObject);
    }

    private void PaintExplodeArea()
    {
        if (_ownerPlayerID == -1)
        {
            return;
        }

        List<Vector3> explodeAreaVertexes = new List<Vector3>();
        for(int i = 0;i < 30;++i)
        {
            Quaternion angle = Quaternion.Euler(0, 12f * i, 0);
            Vector3 vert =  angle * Vector3.right ;
            explodeAreaVertexes.Add(vert.normalized * _explodeRadius + transform.position);
        }

        //PolygonPaintManager.Instance.Paint(explodeAreaVertexes.ToArray(),_ownerPlayerID,_bubbleColor);
        foreach(var pos in explodeAreaVertexes)
        {
            Debug.Log(pos);
        }
    }

    private void JamEnemyPlayerScreen()
    {

    }

}
