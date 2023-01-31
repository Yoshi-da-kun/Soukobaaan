using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ---------------------------------------------------------
// FieldData.cs
//
// 作成者:吉田雄伍
// プレイヤーのコントローラ入力に応じた操作を行うスクリプト
// ---------------------------------------------------------

public class PlayerBoxControl : MonoBehaviour
{
    [SerializeField, Tooltip("フィールドのデータのスクリプト")]
    private FieldData _fieldData;

    [SerializeField, Header("プレイヤーのオブジェクト")]
    private GameObject _playerObj;

    [SerializeField, Header("プレイヤーの初期位置")]
    private Vector2 _playerInitialPos;


    [SerializeField, Header("箱のオブジェクト")]
    private GameObject _boxObj;

    [SerializeField, Header("箱の初期位置")]
    private Vector2[] _boxInitialPos;

    [SerializeField, Header("箱のゴール位置のオブジェクト")]
    private GameObject _boxGoalObj;

    [SerializeField, Header("箱のゴール位置")]
    private Vector2[] _boxGoalPos;


    [SerializeField, Header("ゴールテキスト")]
    private GameObject _goalTextObj;

    [SerializeField, Header("このScene名")]
    private string _thisSceneName;

    // ブロックが入っていないマス(通れるマス)を0、入っている場所を1、箱が入っているマスを2とする
    private const int STORE_BLOCK_NUMBER = 1;
    private const int EMPTY_BLOCK_NUMBER = 0;
    private const int STORE_BOX_NUMBER = 2;


    // 箱のオブジェクトと位置情報を格納している変数
    private GameObject[] _boxObjects = default;
    private int[] _boxCurrentPosX = default;
    private int[] _boxCurrentPosY = default;

    // 箱がゴールしているかを判定するフラグ
    private bool[] _isBoxGoal = default;

    // プレイヤーの現在位置情報
    private int _playerCurrentPosX = default;
    private int _playerCurrentPosY = default;

    // フィールドデータを格納する変数
    private int[,] _fieldDataArray = default;


    // プレイヤーの移動先予定の位置
    private int _playerDestinationPosX = default;
    private int _playerDestinationPosY = default;

    // 移動先に箱があった場合の箱の移動先予定の位置
    private int _boxDestinationPosX = default;
    private int _boxDestinationPosY = default;

    private float _beforeVerticalInput = default;
    private float _beforeHorizontalInput = default;


    private void Start()
    {
        // ブロックが入っていないマス(通れるマス)を0、入っている場所を1、箱があるマスを2
        _fieldDataArray = _fieldData._fieldDataArray;

        // プレイヤーを初期位置に移動する
        PlayerStart();

        // 箱とゴールを生成する
        BoxStart();
    }


    /// <summary>
    /// プレイヤーのStartメソッドでの処理
    /// </summary>
    private void PlayerStart()
    {
        // プレイヤーを初期位置に移動する
        _playerCurrentPosX = (int)_playerInitialPos.x;
        _playerCurrentPosY = (int)_playerInitialPos.y;
        _playerObj.transform.position = _playerInitialPos;
    }


    /// <summary>
    /// 箱のStartメソッドでの処理
    /// </summary>
    private void BoxStart()
    {
        // 配列の大きさを定義
        _boxObjects = new GameObject[_boxInitialPos.Length];
        _boxCurrentPosX = new int[_boxInitialPos.Length];
        _boxCurrentPosY = new int[_boxInitialPos.Length];

        _isBoxGoal = new bool[_boxGoalPos.Length];


        // 箱の初期位置の設定をする処理
        for (int i = 0; i < _boxInitialPos.Length; i++)
        {
            // 箱のゴール地点を生成する
            Instantiate(_boxGoalObj).transform.position = _boxGoalPos[i];
            
            // 箱の初期位置を箱の現在地に格納する
            _boxCurrentPosX[i] = (int)_boxInitialPos[i].x;
            _boxCurrentPosY[i] = (int)_boxInitialPos[i].y;

            // フィールド情報に箱の値を設定する
            _fieldDataArray[(int)_boxInitialPos[i].x, (int)_boxInitialPos[i].y] = STORE_BOX_NUMBER;

            // 箱を初期位置に生成する
            _boxObjects[i] = Instantiate(_boxObj);
            _boxObjects[i].transform.position = _boxInitialPos[i];

            // 動かした箱がゴールに入ったかどうかを検知する(jはゴールの数)
            for (int j = 0; j < _boxGoalPos.Length; j++)
            {
                // 箱の現在地を格納する
                Vector2 boxCurrentPos = new Vector2(_boxCurrentPosX[i], _boxCurrentPosY[i]);

                // 箱とゴールの位置が一致しているか
                if (_boxGoalPos[j] == boxCurrentPos)
                {
                    // ゴールフラグをセットする
                    _isBoxGoal[i] = true;

                    // 全ての箱がゴールしているかをチェックする
                    BoxGoalCheck();
                }
            }
        }
    }

    private void Update()
    {
        // プレイヤーの移動先予定の位置
        _playerDestinationPosX = _playerCurrentPosX;
        _playerDestinationPosY = _playerCurrentPosY;

        // 移動先に箱があった場合の箱の移動先予定の位置
        _boxDestinationPosX = _playerCurrentPosX;
        _boxDestinationPosY = _playerCurrentPosY;

        /// ボタンを押したときの処理 ///
        
        // 入力に応じて移動処理を行う処理
        PlayerVerticalInputProcess();
        PlayerHorizontalInputProcess();

        // リスタート
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(_thisSceneName);
        }
        // ゲームの終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    /// <summary>
    /// 縦軸の入力があった時の処理
    /// </summary>
    private void PlayerVerticalInputProcess()
    {
        // 一回の入力あたりの移動量
        int oneInputMoveMass = 1;

        // 縦軸の入力値
        float verticalInput = Input.GetAxis("Vertical");

        // 前フレームに縦軸の入力をしていた場合、移動をしない
        if (_beforeVerticalInput != 0)
        {
            // 現在フレームの入力値を格納する
            _beforeVerticalInput = verticalInput;

            return;
        }

        // 現在フレームの入力値を格納する
        _beforeVerticalInput = verticalInput;

        // 上に移動
        if (verticalInput > 0)
        {
            // プレイヤーの移動予定位置と箱の移動予定位置を格納する
            _playerDestinationPosY = _playerCurrentPosY + oneInputMoveMass;
            _boxDestinationPosY = _playerDestinationPosY + oneInputMoveMass;

            // プレイヤーと箱の移動処理を行う
            MoveProcessing();
            return;
        }
        // 下に移動
        if (verticalInput < 0)
        {
            _playerDestinationPosY = _playerCurrentPosY - oneInputMoveMass;
            _boxDestinationPosY = _playerDestinationPosY - oneInputMoveMass;

            MoveProcessing();
            return;
        }
    }


    /// <summary>
    /// 横軸の入力があった時の処理
    /// </summary>
    private void PlayerHorizontalInputProcess()
    {
        // 一回の入力あたりの移動量
        int oneInputMoveMass = 1;

        // コントローラとキーの入力量
        float horizontalInput = Input.GetAxis("Horizontal");

        // 前フレームに横軸の入力をしていた場合、移動をしない
        if (_beforeHorizontalInput != 0)
        {
            // 現在フレームの入力値を格納する
            _beforeHorizontalInput = horizontalInput;
            return;
        }

        // 現在フレームの入力値を格納する
        _beforeHorizontalInput = horizontalInput;

        // 右に移動
        if (horizontalInput > 0)
        {
            // プレイヤーの移動予定位置と箱の移動予定位置を格納する
            _playerDestinationPosX = _playerCurrentPosX + oneInputMoveMass;
            _boxDestinationPosX = _playerDestinationPosX + oneInputMoveMass;

            // プレイヤーと箱の移動処理を行う
            MoveProcessing();
            return;
        }
        // 左に移動
        if (horizontalInput < 0)
        {
            _playerDestinationPosX = _playerCurrentPosX - oneInputMoveMass;
            _boxDestinationPosX = _playerDestinationPosX - oneInputMoveMass;

            MoveProcessing();
            return;
        }
    }


    /// <summary>
    /// 移動するかを判定する処理と箱を移動する処理を行う
    /// </summary>
    private void MoveProcessing()
    {
        // 移動先が空のマスの時の処理
        if (_fieldDataArray[_playerDestinationPosX, _playerDestinationPosY] == EMPTY_BLOCK_NUMBER)
        {
            // プレイヤーを移動する処理
            PlayerMove();

            return;
        }

        // 移動先に箱がないときは処理しない
        if (_fieldDataArray[_playerDestinationPosX, _playerDestinationPosY] != STORE_BOX_NUMBER)
        {
            return;
        }
        // 箱が移動できないときは処理しない
        else if(_fieldDataArray[_boxDestinationPosX, _boxDestinationPosY] != EMPTY_BLOCK_NUMBER)
        {
            return;
        }

        // 箱が移動できるときの処理
        for (int i = 0; i < _boxObjects.Length; i++)
        {
            // プレイヤーが押し出す箱を検索し、その箱を動かす
            if (_playerDestinationPosX == _boxCurrentPosX[i] && _playerDestinationPosY == _boxCurrentPosY[i])
            {
                // フィールド情報の値を変更する
                _fieldDataArray[_boxDestinationPosX, _boxDestinationPosY] = STORE_BOX_NUMBER;
                _fieldDataArray[_boxCurrentPosX[i], _boxCurrentPosY[i]] = EMPTY_BLOCK_NUMBER;

                // 箱を動かす
                _boxObjects[i].transform.position = new Vector2(_boxDestinationPosX, _boxDestinationPosY);

                // 箱の現在地を更新
                _boxCurrentPosX[i] = _boxDestinationPosX;
                _boxCurrentPosY[i] = _boxDestinationPosY;

                // プレイヤーを移動する処理
                PlayerMove();

                // 動かした箱がゴールに入ったかどうかを検知する(jはゴールの数)
                for (int j = 0; j < _boxGoalPos.Length; j++)
                {
                    // 箱の現在地を格納する
                    Vector2 boxCurrentPos = new Vector2(_boxCurrentPosX[i], _boxCurrentPosY[i]);
                
                    // 動かした箱がゴールした時の処理
                    if (_boxGoalPos[j] == boxCurrentPos)
                    {
                        // ゴールフラグをセットする
                        _isBoxGoal[i] = true;

                        // 全ての箱がゴールしているかをチェックする
                        BoxGoalCheck();

                        return;
                    }
                }

                // ゴールしていなかった時の処理
                _isBoxGoal[i] = false;
            }
        }
    }


    /// <summary>
    /// プレイヤーを実際に移動する処理
    /// </summary>
    private void PlayerMove()
    {
        // プレイヤーを移動する
        _playerObj.transform.position = new Vector2(_playerDestinationPosX, _playerDestinationPosY);

        // プレイヤーの現在地を更新
        _playerCurrentPosX = _playerDestinationPosX;
        _playerCurrentPosY = _playerDestinationPosY;
    }


    /// <summary>
    /// 箱がゴール地点に入っているかを判定するメソッド
    /// </summary>
    private void BoxGoalCheck()
    {
        // すべてゴールしているかを判定する
        for(int i = 0; i < _isBoxGoal.Length; i++)
        {
            if(!_isBoxGoal[i])
            {
                return;
            }
        }

        // ゴール処理
        _goalTextObj.SetActive(true);
    }
}
