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
            // DropPointが保存されるList
            public List<GameObject> playerPoints;
            // DropPointを管??��?��?する親
            public GameObject pointGroup;
        }

        [SerializeField]
        private PlayerDropPoints _playerDropPoints;

        private TrailRenderer _tailTrailRenderer;      // DropPointが繋がって??��?��?ることを表すTrailRenderer
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
            // プレイヤーが�???��?��に一定時間を移動し続けたら???��?��?DropPointの生存時間�???��?��半�?????��?��?
            if (_tailFadeOutTimer >= Global.DROP_POINT_ALIVE_TIME / 2.0f && _tailFadeOutTimer < Global.DROP_POINT_ALIVE_TIME)
            {
                // 不透�???��?��度を計算する　※　y = -1.9x + 1.95;
                float alpha = (-1.9f / Global.DROP_POINT_ALIVE_TIME) * _tailFadeOutTimer + 1.95f;
                // 不透�???��?��度の最小値??��?��?0.05に設定す??��?��?
                if (alpha < 0.05f)
                {
                    alpha = 0.05f;
                }
                //_networkPlayer.CmdSetTrailGradient(alpha);
            }

        }

        void FixedUpdate()
        {
            if(!isLocalPlayer)
                return;

            //DropNewPoint();
        }


        /// <summary>
        /// DropPoint??��?��??��?��?��??��?��??��?��??��?��??��?��
        /// </summary>
        [ClientRpc]
        public void RpcAddDropPoint(GameObject pt)
        {
            // dropPointの親を設定して、Listに入れる
            pt.transform.parent = _playerDropPoints.pointGroup.transform;
            _playerDropPoints.playerPoints.Add(pt);
        }

        /// <summary>
        /// TrailRendererの初期設定行う
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
            // ワールド座標をローカル座標に変換する
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
        /// DropPointを置??��?��?
        /// </summary>    
        public void DropNewPoint()
        {
            _dropPointTimerCnt += Time.fixedDeltaTime;

            if(_dropPointTimerCnt <= Global.DROP_POINT_INTERVAL)
                return;

            Vector3 spawnPos = transform.position - transform.forward * _trailOffset;
            _networkPlayer.CmdOnInstantiateDropPoint(spawnPos,_dropPointTag);

            _dropPointTimerCnt = 0f;
        }

        /// <summary>
        /// TrailRendererの状態をリセ??��?��?トす??��?��?
        /// </summary>
        public void ResetTrail()
        {
            _tailTrailRenderer.Clear();
            _tailFadeOutTimer = 0.0f;
            RpcSetTrailGradient(1.0f);
        }

        /// <summary>
        /// TrailRendererのグラ??��?��?ィエントを設定す??��?��?
        /// </summary>
        /// <param name="alpha">一番後ろの不透�???��?��度</param>
        
        [ClientRpc]
        public void RpcSetTrailGradient(float alpha)
        {

            _tailTrailRenderer.colorGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(_areaColor, 0.0f), new GradientColorKey(_areaColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        }


        /// <summary>
        /// Listにある全てのDropPoint(GameObject)のワールド座標を返す
        /// </summary>
        /// <returns>Listの全てのGameObjectのワールド座??��?��?(Vector3??��?��?)</returns>
        private Vector3[] DropPointsGameObjectToVector3()
        {
            // Listのコピ�???��?��を作る
            //List<GameObject> retList = new List<GameObject>(list);
            // 戻り値用配�???��?��を作る
            Vector3[] retPos = new Vector3[_playerDropPoints.playerPoints.Count];
            int index = 0;
            // Listの全てのGameObjectのワールド座標を戻り値用配�???��?��に入れる
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
        /// 消えたDropPoint(GameObject)をListから消す関数
        /// </summary>
        /// <param name="dropPoint">消えたDropPoint(GameObject)</param>
        
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
        /// プレイヤーの全てのDropPoint(GameObject)を消す関数
        /// </summary>
        /// <param name="ID">プレイヤーのID</param>

        public void ClearAllDropPoints()
        {
            // 全てのDropPoint(GameObject)を�???��?��??��?��?する
            int index = 0;
            while(index > _playerDropPoints.playerPoints.Count)
            {
                GameObject dropPoint = _playerDropPoints.playerPoints[index];
                if(dropPoint == null)
                {
                    ++index;
                    continue;
                }

                dropPoint.GetComponent<DropPoint>().DestroySelf();
            }
            RpcClearAllDropPoints();
        }

        [ClientRpc]
        private void RpcClearAllDropPoints()
        {
            _playerDropPoints.playerPoints.Clear();
        }

        /// <summary>
        /// �v���C���[�̑S�Ă�DropPoint�̍��W���擾
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