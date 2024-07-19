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
            // DropPointを管理する親
            public GameObject pointGroup;
        }

        [SerializeField]
        private PlayerDropPoints _playerDropPoints;

        private TrailRenderer _tailTrailRenderer;      // DropPointが繋がっていることを表すTrailRenderer
        private GameObject _pointPrefab;             // DropPointのプレハブ
        private float _tailFadeOutTimer;
        private Timer _dropPointTimer;           // DropPointのインスタンス化することを管理するタイマー

        private float trailOffset;

        //TODO refactorying
        private string _dropPointTag;
        private int _playerID;
        private Color _areaColor;

        private Player _player;
        private GamePlayer _networkPlayer;
        public GameObject DropPointPrefab => _pointPrefab;

        private void Awake()
        {
            _networkPlayer = GetComponent<GamePlayer>();

            _pointPrefab = GameResourceSystem.Instance.GetPrefabResource("DropPoint");
            _tailFadeOutTimer = 0.0f;
            trailOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;
            _player = gameObject.GetOrAddComponent<Player>();
            _dropPointTag = "DropPoint";
            _playerID = -1;
            _areaColor = Color.clear;

            // 尻尾を描画するGameObjectを作る
            GameObject trail = new GameObject(name + "Trail");
            // プレイヤーを親にする
            trail.transform.parent = transform;
            //todo take note
            // ワールド座標をローカル座標に変換する
            Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            trail.transform.localPosition = Vector3.down * 0.5f - localForward * trailOffset;
            trail.transform.localScale = Vector3.one;
            // TrailRendererをアタッチする
            _tailTrailRenderer = trail.gameObject.AddComponent<TrailRenderer>();

            _dropPointTimer = new Timer(Time.time, Global.DROP_POINT_INTERVAL,
                () =>
                {
                    Vector3 spawnPos = transform.position - transform.forward * trailOffset;
                    // タイマーが終わったらDropPointを置く
                    _networkPlayer.CmdOnInstantiateDropPoint(spawnPos);
                }
                );
            _dropPointTimer.StartTimer(this);

            _playerDropPoints = new PlayerDropPoints
            {
                playerPoints = new List<GameObject>(),
                pointGroup = new GameObject("Player drop point group")
            };


        }
        // Update is called once per frame
        private void Update()
        {
            
            _tailFadeOutTimer += Time.deltaTime;
            // プレイヤーが場に一定時間を移動し続けたら（DropPointの生存時間の半分）
            if (_tailFadeOutTimer >= Global.DROP_POINT_ALIVE_TIME / 2.0f && _tailFadeOutTimer < Global.DROP_POINT_ALIVE_TIME)
            {
                // 不透明度を計算する　※　y = -1.9x + 1.95;
                float alpha = (-1.9f / Global.DROP_POINT_ALIVE_TIME) * _tailFadeOutTimer + 1.95f;
                // 不透明度の最小値を0.05に設定する
                if (alpha < 0.05f)
                {
                    alpha = 0.05f;
                }
                SetTrailGradient(alpha);
            }

        }

        private void FixedUpdate()
        {
            DropNewPoint();
        }
        /// <summary>
        /// DropPointをインスタンス化する
        /// </summary>
        [ClientRpc]
        public void RpcAddDropPoint(GameObject pt)
        {
            //GameObject pt = Instantiate(_pointPrefab, transform.position - transform.forward * trailOffset, Quaternion.identity);
            pt.tag = _dropPointTag;
            pt.GetComponent<DropPoint>().SetDestroyCallback(_networkPlayer.CmdOnDestroyDropPoint);
            // TODO 
            AddPoint(pt);

        }

        /// <summary>
        /// TrailRendererの初期設定行う
        /// </summary>
        public void Init()
        {
            _playerID = _player.ID;
            _dropPointTag = "DropPoint" + _playerID.ToString();
            _areaColor = _player.AreaColor;

            _tailTrailRenderer.material = new Material(Shader.Find("Sprites/Default")) { hideFlags = HideFlags.DontSave};
            _tailTrailRenderer.startColor = Global.PLAYER_TRACE_COLORS[_playerID - 1];
            _tailTrailRenderer.endColor = Global.PLAYER_TRACE_COLORS[_playerID - 1];
            _tailTrailRenderer.startWidth = 1.0f;
            _tailTrailRenderer.endWidth = 1.0f;
            _tailTrailRenderer.time = Global.DROP_POINT_ALIVE_TIME;
        }

        /// <summary>
        /// DropPointを置く
        /// </summary>    
        private void DropNewPoint()
        {
            if(_dropPointTimer.IsFinished())
            {
                _dropPointTimer.OnTimerReset();
                _dropPointTimer.StartTimer(this);
            }
        }

        /// <summary>
        /// TrailRendererの状態をリセットする
        /// </summary>
        public void ResetTrail()
        {
            _tailTrailRenderer.Clear();
            _tailFadeOutTimer = 0.0f;
            SetTrailGradient(1.0f);
        }

        /// <summary>
        /// TrailRendererのグラディエントを設定する
        /// </summary>
        /// <param name="alpha">一番後ろの不透明度</param>
        private void SetTrailGradient(float alpha)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(_areaColor, 0.0f), new GradientColorKey(_areaColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
            _tailTrailRenderer.colorGradient = gradient;
        }


        /// <summary>
        /// Listにある全てのDropPoint(GameObject)のワールド座標を返す
        /// </summary>
        /// <returns>Listの全てのGameObjectのワールド座標(Vector3型)</returns>
        private Vector3[] DropPointsGameObjectToVector3()
        {
            // Listのコピーを作る
            //List<GameObject> retList = new List<GameObject>(list);
            // 戻り値用配列を作る
            Vector3[] retPos = new Vector3[_playerDropPoints.playerPoints.Count];
            int index = 0;
            // Listの全てのGameObjectのワールド座標を戻り値用配列に入れる
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
        /// 特定のプレイヤーのDropPoint(GameObject)を管理するListにDropPoint(GameObject)を入れる関数
        /// </summary>
        /// <param name="ID">プレイヤーID</param>
        /// <param name="dropPoint">Listに入れるDropPoint</param>
        private void AddPoint(GameObject dropPoint)
        {
            // dropPointの親を設定して、Listに入れる
            dropPoint.transform.parent = _playerDropPoints.pointGroup.transform;
            _playerDropPoints.playerPoints.Add(dropPoint);
            // 存在しない場合はエラーメッセージを出力
        }

        /// <summary>
        /// 消えたDropPoint(GameObject)をListから消す関数
        /// </summary>
        /// <param name="dropPoint">消えたDropPoint(GameObject)</param>
        public void RemovePoint(GameObject dropPoint)
        {
            if(!_playerDropPoints.playerPoints.Contains(dropPoint))
            {
                return;
            }

            _playerDropPoints.playerPoints.Remove(dropPoint);
        }

        /// <summary>
        /// プレイヤーの全てのDropPoint(GameObject)を消す関数
        /// </summary>
        /// <param name="ID">プレイヤーのID</param>
        [ClientRpc]
        public void RpcClearDropPoints()
        {
            // 全てのDropPoint(GameObject)を破棄する
            foreach (GameObject dropPoint in _playerDropPoints.playerPoints)
            {
                if(_networkPlayer != null)
                    _networkPlayer.CmdOnDestroyDropPoint(dropPoint);
            }
            // Listにある物を全部消す
            _playerDropPoints.playerPoints.Clear();
        }

        /// <summary>
        /// プレイヤーの全てのDropPointのワールド座標を戻す関数
        /// </summary>
        /// <param name="ID">プレイヤーのID</param>
        /// <returns>全てのDropPoint(GameObject)のワールド座標（Vector3型）、プレイヤーが存在しない場合は空の配列を返す</returns>
        public Vector3[] GetPlayerDropPoints()
        {
            return DropPointsGameObjectToVector3();
        }
        /// <summary>
        /// DropPointSystemをデイニシャライゼーションする関数
        /// </summary>
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
    }
}