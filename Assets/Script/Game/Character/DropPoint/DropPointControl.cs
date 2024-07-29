using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Org.BouncyCastle.Security;

namespace Character
{
    public class DropPointControl : NetworkBehaviour
    {
        [Serializable]
        private struct PlayerDropPoints
        {
            // DropPointãŒä¿å­˜ã•ã‚Œã‚‹List
            public List<GameObject> playerPoints;
            // DropPointã‚’ç®¡???¿½?¿½??¿½?¿½?ã™ã‚‹è¦ª
            public GameObject pointGroup;
        }

        [SerializeField]
        private PlayerDropPoints _playerDropPoints;

        private TrailRenderer _tailTrailRenderer;      // DropPointãŒç¹‹ãŒã£ã¦???¿½?¿½??¿½?¿½?ã‚‹ã“ã¨ã‚’è¡¨ã™TrailRenderer
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
            if (!isLocalPlayer) 
                return;
            
            _tailFadeOutTimer += Time.deltaTime;
            // ãƒ—ãƒ¬ã‚¤ãƒ¤ãƒ¼ãŒï¿½????¿½?¿½??¿½?¿½ã«ä¸€å®šæ™‚é–“ã‚’ç§»å‹•ã—ç¶šã‘ãŸã‚‰????¿½?¿½??¿½?¿½?DropPointã®ç”Ÿå­˜æ™‚é–“ï¿½????¿½?¿½??¿½?¿½åŠï¿½??????¿½?¿½??¿½?¿½?
            if (_tailFadeOutTimer >= Global.DROP_POINT_ALIVE_TIME / 2.0f && _tailFadeOutTimer < Global.DROP_POINT_ALIVE_TIME)
            {
                // ä¸é€ï¿½????¿½?¿½??¿½?¿½åº¦ã‚’è¨ˆç®—ã™ã‚‹ã€€â€»ã€€y = -1.9x + 1.95;
                float alpha = (-1.9f / Global.DROP_POINT_ALIVE_TIME) * _tailFadeOutTimer + 1.95f;
                // ä¸é€ï¿½????¿½?¿½??¿½?¿½åº¦ã®æœ€å°å€¤???¿½?¿½??¿½?¿½?0.05ã«è¨­å®šã™???¿½?¿½??¿½?¿½?
                if (alpha < 0.05f)
                {
                    alpha = 0.05f;
                }
                //_networkPlayer.CmdSetTrailGradient(alpha);
            }

        }

        /// <summary>
        /// DropPoint???¿½?¿½??¿½?¿½???¿½?¿½??¿½?¿½??¿½?¿½???¿½?¿½??¿½?¿½???¿½?¿½??¿½?¿½???¿½?¿½??¿½?¿½???¿½?¿½??¿½?¿½
        /// </summary>
        [ClientRpc]
        public void RpcAddDropPoint(GameObject pt)
        {
            pt.GetComponent<DropPoint>().SetDestroyCallback(RpcRemoveDropPoint);
            // dropPointã®è¦ªã‚’è¨­å®šã—ã¦ã€Listã«å…¥ã‚Œã‚‹
            pt.transform.parent = _playerDropPoints.pointGroup.transform;
            _playerDropPoints.playerPoints.Add(pt);
        }

        /// <summary>
        /// TrailRendererã®åˆæœŸè¨­å®šè¡Œã†
        /// </summary>
        public void InitDropPointCtrl(IPlayerInfo playerInfo)
        {

            if(playerInfo == null)
                return;

            {
                _playerID = playerInfo.ID;
                _dropPointTag = "DropPoint" + _playerID.ToString();
                _areaColor = playerInfo.AreaColor;
            }

            //TODO take note
            // ãƒ¯ãƒ¼ãƒ«ãƒ‰åº§æ¨™ã‚’ãƒ­ãƒ¼ã‚«ãƒ«åº§æ¨™ã«å¤‰æ›ã™ã‚‹
            Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);

            _tailTrailRenderer.transform.localPosition = Vector3.down * 0.5f - localForward * _trailOffset;
            _tailTrailRenderer.transform.localScale = Vector3.one;

            _tailTrailRenderer.startColor = Global.PLAYER_TRACE_COLORS[_playerID - 1];
            _tailTrailRenderer.endColor = Global.PLAYER_TRACE_COLORS[_playerID - 1];
            _tailTrailRenderer.startWidth = 1.0f;
            _tailTrailRenderer.endWidth = 1.0f;
            _tailTrailRenderer.time = Global.DROP_POINT_ALIVE_TIME;
        }
        /// <summary>
        /// DropPointã‚’ç½®???¿½?¿½??¿½?¿½?
        /// </summary>    
        public void DropNewPoint()
        {
            _dropPointTimerCnt += Time.fixedDeltaTime;

            if(_dropPointTimerCnt <= Global.DROP_POINT_INTERVAL)
                return;

            Vector3 spawnPos = transform.position - transform.forward * _trailOffset;
            _networkPlayer.CmdInstantiateDropPoint(spawnPos,_dropPointTag);

            _networkPlayer.CmdSendMessage($"{name} create DropPoint{_playerID}");

            _dropPointTimerCnt = 0f;
        }

        /// <summary>
        /// TrailRendererã®çŠ¶æ…‹ã‚’ãƒªã‚»???¿½?¿½??¿½?¿½?ãƒˆã™???¿½?¿½??¿½?¿½?
        /// </summary>
        public void ResetTrail()
        {
            _tailTrailRenderer.Clear();
            _tailFadeOutTimer = 0.0f;
            RpcSetTrailGradient(1.0f);
        }

        /// <summary>
        /// TrailRendererã®ã‚°ãƒ©???¿½?¿½??¿½?¿½?ã‚£ã‚¨ãƒ³ãƒˆã‚’è¨­å®šã™???¿½?¿½??¿½?¿½?
        /// </summary>
        /// <param name="alpha">ä¸€ç•ªå¾Œã‚ã®ä¸é€ï¿½????¿½?¿½??¿½?¿½åº¦</param>
        
        [ClientRpc]
        public void RpcSetTrailGradient(float alpha)
        {

            _tailTrailRenderer.colorGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(_areaColor, 0.0f), new GradientColorKey(_areaColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        }


        /// <summary>
        /// Listã«ã‚ã‚‹å…¨ã¦ã®DropPoint(GameObject)ã®ãƒ¯ãƒ¼ãƒ«ãƒ‰åº§æ¨™ã‚’è¿”ã™
        /// </summary>
        /// <returns>Listã®å…¨ã¦ã®GameObjectã®ãƒ¯ãƒ¼ãƒ«ãƒ‰åº§???¿½?¿½??¿½?¿½?(Vector3???¿½?¿½??¿½?¿½?)</returns>
        private Vector3[] DropPointsGameObjectToVector3()
        {
            // Listã®ã‚³ãƒ”ï¿½????¿½?¿½??¿½?¿½ã‚’ä½œã‚‹
            //List<GameObject> retList = new List<GameObject>(list);
            // æˆ»ã‚Šå€¤ç”¨é…ï¿½????¿½?¿½??¿½?¿½ã‚’ä½œã‚‹
            Vector3[] retPos = new Vector3[_playerDropPoints.playerPoints.Count];
            int index = 0;
            // Listã®å…¨ã¦ã®GameObjectã®ãƒ¯ãƒ¼ãƒ«ãƒ‰åº§æ¨™ã‚’æˆ»ã‚Šå€¤ç”¨é…ï¿½????¿½?¿½??¿½?¿½ã«å…¥ã‚Œã‚‹
            foreach (GameObject ob in _playerDropPoints.playerPoints)
            {
                if(ob == null)
                    continue;
                
                retPos[index] = ob.transform.position;
                ++index;
            }
            return retPos;
            
        }

        /// <summary>
        /// æ¶ˆãˆãŸDropPoint(GameObject)ã‚’Listã‹ã‚‰æ¶ˆã™é–¢æ•°
        /// </summary>
        /// <param name="dropPoint">æ¶ˆãˆãŸDropPoint(GameObject)</param>
        
        [ClientRpc]
        public void RpcRemoveDropPoint(GameObject obj)
        {
            // if(_playerDropPoints.playerPoints.Contains(obj))
            //     return;

            // //TODO
            // _playerDropPoints.playerPoints.Remove(obj);
            if(_playerDropPoints.playerPoints.Count > 0)
                _playerDropPoints.playerPoints.RemoveAt(0);
        }

        /// <summary>
        /// ãƒ—ãƒ¬ã‚¤ãƒ¤ãƒ¼ã®å…¨ã¦ã®DropPoint(GameObject)ã‚’æ¶ˆã™é–¢æ•°
        /// </summary>
        /// <param name="ID">ãƒ—ãƒ¬ã‚¤ãƒ¤ãƒ¼ã®ID</param>

        public void ClearAllDropPoints()
        {
            // å…¨ã¦ã®DropPoint(GameObject)ã‚’ï¿½????¿½?¿½??¿½?¿½???¿½?¿½??¿½?¿½?ã™ã‚‹
            for(int i = 0; i < _playerDropPoints.playerPoints.Count ; ++i )
            {
                GameObject dropPoint = _playerDropPoints.playerPoints[i];
                dropPoint.GetComponent<DropPoint>().DestroySelf();
            }
        }


        /// <summary>
        /// ?¿½v?¿½?¿½?¿½C?¿½?¿½?¿½[?¿½Ì‘S?¿½Ä‚ï¿½DropPoint?¿½Ìï¿½?¿½W?¿½?¿½?¿½æ“¾
        /// </summary>
        public Vector3[] GetPlayerDropPointsPosition()
        {
            return DropPointsGameObjectToVector3();
        }

        private void OnDestroy()
        {
            if(NetworkServer.active)
            {
                _networkPlayer.CmdOnClearAllDropPoints();
            }
            else
            {
                _playerDropPoints.playerPoints.Clear();
            }

            Destroy(_playerDropPoints.pointGroup);
            
        }

        private void OnDisable()
        {
            _networkPlayer.CmdOnClearAllDropPoints();
        }
    }
}