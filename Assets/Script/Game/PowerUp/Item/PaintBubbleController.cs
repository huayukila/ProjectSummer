using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Mirror;
using UnityEngine;


public class PaintBubbleController : NetworkBehaviour,IExplodable
{
    private Color _bubbleColor = Color.clear;

    private float _waitForExplodeTime = Global.BUBBLE_EXPLODE_TIME;

    [SerializeField]
    private float _explodeRadius = 0f;

    private int _ownerPlayerID = -1;

    private Material _material;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {

        _meshRenderer = GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        _material = new Material(_meshRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave};
        _material.color = _bubbleColor;
        _meshRenderer.sharedMaterial = _material;
    }
    // Update is called once per frame
    public void SetupExplode(int owner, Color color)
    {
        _ownerPlayerID = owner;
        _bubbleColor = color;

        Timer explodeTimer = new Timer(Time.time,_waitForExplodeTime,ExplodeBubble);
        explodeTimer.StartTimer(this);
    }

    private void OnDestroy()
    {
        if (_material != null)
        {
            Destroy(_material);
        }
    }

    [Server]
    private void ExplodeBubble()
    {
        PaintExplodeArea();
        JamEnemyPlayerScreen();
        NetworkServer.Destroy(gameObject);
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

        PaintAreaEvent paintEvent = new PaintAreaEvent
        {
            Verts = explodeAreaVertexes.ToArray(),
            PlayerID = _ownerPlayerID,
            PlayerAreaColor = _bubbleColor
        };
        //TypeEventSystem.Instance.Send(paintEvent);
    }

    private void JamEnemyPlayerScreen()
    {

    }

}
