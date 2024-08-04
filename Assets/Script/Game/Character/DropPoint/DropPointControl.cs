using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace WSV.Character
{
    public class DropPointControl : NetworkBehaviour
    {
        [Serializable]
        private struct PlayerDropPoints
        {
            // DropPointを入れるコンテナ
            public List<GameObject> playerPoints;
            // DropPointをHierarchyでまとめる親オブジェクト
            public GameObject pointGroup;
        }

        [SerializeField]
        private PlayerDropPoints _playerDropPoints;

        private TrailRenderer _tailTrailRenderer;      // 尻尾を描画するレンダラー
        private float _tailFadeOutTimer;

        private float  _dropPointTimerCnt;
        private float _trailOffset;

        //TODO refactoring

        [SerializeField]
        private string _dropPointTag;
        [SerializeField]
        private int _playerID;
        [SerializeField]
        private Color _areaColor;

        private GamePlayer _networkPlayer;

        private void Awake()
        {
            _networkPlayer = GetComponent<GamePlayer>();

            _tailFadeOutTimer = 0.0f;
            _trailOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;
            _dropPointTag = "DropPoint";
            _playerID = 0;
            _areaColor = Color.clear;

            _playerDropPoints = new PlayerDropPoints
            {
                playerPoints = new List<GameObject>(),
                pointGroup = new GameObject("Player drop point group")
            };

            _dropPointTimerCnt = 0f;

            _tailTrailRenderer = GetComponentInChildren<TrailRenderer>();

        }
        // Update is called once per frame
        private void Update()
        {
            if(!isLocalPlayer)
                return;

            _tailFadeOutTimer += Time.deltaTime;
            
            if (_tailFadeOutTimer >= Global.DROP_POINT_ALIVE_TIME / 2.0f && _tailFadeOutTimer < Global.DROP_POINT_ALIVE_TIME)
            {
                
                float alpha = (-1.9f / Global.DROP_POINT_ALIVE_TIME) * _tailFadeOutTimer + 1.95f;
               
                {
                    alpha = 0.05f;
                }
                //_networkPlayer.CmdSetTrailGradient(alpha);
            }

        }

    
        [ClientRpc]
        public void RpcAddDropPoint(GameObject pt)
        {
            pt.GetComponent<DropPoint>().SetDestroyCallback(RpcRemoveDropPoint);
            pt.transform.parent = _playerDropPoints.pointGroup.transform;
            _playerDropPoints.playerPoints.Add(pt);
        }


        public void InitDropPointCtrl(IPlayerInfo playerInfo)
        {

            if(playerInfo == null)
                return;

            {
                _playerID = playerInfo.ID;
                _dropPointTag = "DropPoint" + _playerID.ToString();
                _areaColor = playerInfo.AreaColor;
            }

            Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);

            _tailTrailRenderer.transform.localPosition = Vector3.down * 0.5f - localForward * _trailOffset;
            _tailTrailRenderer.transform.localScale = Vector3.one;

            _tailTrailRenderer.startColor = Global.PLAYER_TRACE_COLORS[_playerID - 1];
            _tailTrailRenderer.endColor = Global.PLAYER_TRACE_COLORS[_playerID - 1];
            _tailTrailRenderer.startWidth = 1.0f;
            _tailTrailRenderer.endWidth = 1.0f;
            _tailTrailRenderer.time = Global.DROP_POINT_ALIVE_TIME;
        }

        public void DropNewPoint()
        {
            _dropPointTimerCnt += Time.fixedDeltaTime;

            if(_dropPointTimerCnt <= Global.DROP_POINT_INTERVAL)
                return;

            Vector3 spawnPos = transform.position - transform.forward * _trailOffset;
            _networkPlayer.CmdInstantiateDropPoint(spawnPos,_dropPointTag);

            //_networkPlayer.CmdSendMessage($"{name} create DropPoint{_playerID}");

            _dropPointTimerCnt = 0f;
        }

        public void ResetTrail()
        {
            _tailTrailRenderer.Clear();
            _tailFadeOutTimer = 0.0f;
            //RpcSetTrailGradient(1.0f);
        }
        
        [ClientRpc]
        public void RpcSetTrailGradient(float alpha)
        {

            _tailTrailRenderer.colorGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(_areaColor, 0.0f), new GradientColorKey(_areaColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        }


        private Vector3[] DropPointsGameObjectToVector3()
        {

            Vector3[] retPos = new Vector3[_playerDropPoints.playerPoints.Count];
            int index = 0;
 
            foreach (GameObject ob in _playerDropPoints.playerPoints)
            {
                if(ob == null)
                    continue;
                
                retPos[index] = ob.transform.position;
                ++index;
            }
            return retPos;
            
        }

        [ClientRpc]
        public void RpcRemoveDropPoint(GameObject obj)
        {

            if(_playerDropPoints.playerPoints.Count > 0)
                _playerDropPoints.playerPoints.RemoveAt(0);
        }

        public void ClearAllDropPoints()
        {
            for(int i = 0; i < _playerDropPoints.playerPoints.Count ; ++i )
            {
                GameObject dropPoint = _playerDropPoints.playerPoints[i];
                dropPoint.GetComponent<DropPoint>().DestroySelf();
            }
        }
        public Vector3[] GetPlayerDropPointsPosition()
        {
            return DropPointsGameObjectToVector3();
        }

        [Command]
        public void CmdOnClearAllDropPoints()
        {
            ClearAllDropPoints();
        }

        private void OnDestroy()
        {
            if(NetworkServer.active)
            {
                CmdOnClearAllDropPoints();
            }
            else
            {
                _playerDropPoints.playerPoints.Clear();
            }

            Destroy(_playerDropPoints.pointGroup);

        }

        private void OnDisable()
        {
            //CmdOnClearAllDropPoints();
        }
      
    }
}