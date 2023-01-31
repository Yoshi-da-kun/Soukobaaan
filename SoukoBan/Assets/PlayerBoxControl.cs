using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBoxControl : MonoBehaviour
{
    [SerializeField]
    private FieldData _fieldData;

    [SerializeField, Header("プレイヤーのオブジェクト")]
    private GameObject _playerObj;

    [SerializeField, Header("プレイヤーの初期位置")]
    private Vector2 _playerInisialPos;

    [SerializeField, Header("箱のオブジェクト")]
    private GameObject _boxObj;

    [SerializeField, Header("箱の初期位置")]
    private Vector2[] _boxInisialPos;

    [SerializeField, Header("箱のゴール位置のオブジェクト")]
    private GameObject _boxGoalObj;

    [SerializeField, Header("箱のゴール位置")]
    private Vector2[] _boxGoalPos;


    [SerializeField, Header("ゴールテキスト")]
    private GameObject _goalTextObj;

    [SerializeField, Header("このScene名")]
    private string _thisSceneName;

    //ブロックが入っていないマス(通れるマス)を0、入っている場所を1、箱が入っているマスを2とする
    private const int _storeBlockNumber = 1;
    private const int _nullSquaresNumber = 0;
    private const int _storeBoxNumber = 2;


    //箱のオブジェクトと位置情報を格納している変数
    private GameObject[] _boxObjects;
    private int[] _boxCurrentPosX;
    private int[] _boxCurrentPosY;

    //箱がゴールしているかを判定するフラグ
    private bool[] _isBoxGoal;

    //プレイヤーの現在位置情報
    private int _playerCurrentPosX;
    private int _playerCurrentPosY;

    //フィールドデータを格納する変数
    private int[,] _fieldDataArray;


    //プレイヤーの移動先予定の位置
    private int playerDestinationPosX　= default;
    private int playerDestinationPosY　= default;

    //移動先に箱があった場合の箱の移動先予定の位置
    private int boxDestinationPosX = default;
    private int boxDestinationPosY = default;

    private void Start()
    {
        //ブロックが入っていないマス(通れるマス)を0、入っている場所を1、箱があるマスを2
        _fieldDataArray = _fieldData._fieldDataArray;

        PlayerStart();

        BoxStart();
    }


    /// <summary>
    /// プレイヤーのStartメソッドでの処理
    /// </summary>
    private void PlayerStart()
    {
        //プレイヤーを初期位置に移動する
        _playerCurrentPosX = (int)_playerInisialPos.x;
        _playerCurrentPosY = (int)_playerInisialPos.y;
        _playerObj.transform.position = _playerInisialPos;
    }


    /// <summary>
    /// 箱のStartメソッドでの処理
    /// </summary>
    private void BoxStart()
    {
        //配列の大きさを定義
        _boxObjects = new GameObject[_boxInisialPos.Length];
        _boxCurrentPosX = new int[_boxInisialPos.Length];
        _boxCurrentPosY = new int[_boxInisialPos.Length];

        _isBoxGoal = new bool[_boxGoalPos.Length];


        //箱の初期位置の設定をする処理
        for (int i = 0; i < _boxInisialPos.Length; i++)
        {
            //箱のゴール地点を生成する
            Instantiate(_boxGoalObj).transform.position = _boxGoalPos[i];
            
            //箱の初期位置を箱の現在地に格納する
            _boxCurrentPosX[i] = (int)_boxInisialPos[i].x;
            _boxCurrentPosY[i] = (int)_boxInisialPos[i].y;

            //フィールド情報に箱の値を設定する
            _fieldDataArray[(int)_boxInisialPos[i].x, (int)_boxInisialPos[i].y] = _storeBoxNumber;

            //箱を初期位置に生成する
            _boxObjects[i] = Instantiate(_boxObj);
            _boxObjects[i].transform.position = _boxInisialPos[i];

            //動かした箱がゴールに入ったかどうかを検知する(jはゴールの数)
            for (int j = 0; j < _boxGoalPos.Length; j++)
            {
                //箱の現在地を格納する
                Vector2 boxCurrentPos = new Vector2(_boxCurrentPosX[i], _boxCurrentPosY[i]);

                //箱とゴール地点が一致したらすべてゴールしているか
                if (_boxGoalPos[j] == boxCurrentPos)
                {
                    _isBoxGoal[i] = true;
                }
            }
        }
    }

    private void Update()
    {
        //プレイヤーの移動先予定の位置
        playerDestinationPosX = _playerCurrentPosX;
        playerDestinationPosY = _playerCurrentPosY;

        //移動先に箱があった場合の箱の移動先予定の位置
        boxDestinationPosX = _playerCurrentPosX;
        boxDestinationPosY = _playerCurrentPosY;

        ///ボタンを押したときの処理///
        //一回の入力あたりの移動量
        int _oneInputMoveMass = 1;

        //上に移動
        if (Input.GetKeyDown(KeyCode.W))
        {
            //プレイヤーの移動予定位置と箱の移動予定位置を格納する
            playerDestinationPosY = _playerCurrentPosY + _oneInputMoveMass;
            boxDestinationPosY = playerDestinationPosY + _oneInputMoveMass;

            MoveProcessing();
            return;
        }
        //下に移動
        if (Input.GetKeyDown(KeyCode.S))
        {
            playerDestinationPosY = _playerCurrentPosY - _oneInputMoveMass;
            boxDestinationPosY = playerDestinationPosY - _oneInputMoveMass;

            MoveProcessing();
            return;
        }
        //右に移動
        if (Input.GetKeyDown(KeyCode.D))
        {
            playerDestinationPosX = _playerCurrentPosX + _oneInputMoveMass;
            boxDestinationPosX = playerDestinationPosX + _oneInputMoveMass;

            MoveProcessing();
            return;
        }
        //左に移動
        if (Input.GetKeyDown(KeyCode.A))
        {
            playerDestinationPosX = _playerCurrentPosX - _oneInputMoveMass;
            boxDestinationPosX = playerDestinationPosX - _oneInputMoveMass;

            MoveProcessing();
            return;
        }

        //リスタート
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(_thisSceneName);
        }
        //ゲームの終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    /// <summary>
    /// 移動するかを判定する処理と箱を移動する処理を行う
    /// </summary>
    private void MoveProcessing()
    {
        //移動先が空のマスの時の処理
        if (_fieldDataArray[playerDestinationPosX, playerDestinationPosY] == _nullSquaresNumber)
        {
            PlayerMove();

            return;
        }

        //移動先に箱がないときは処理しない
        if (_fieldDataArray[playerDestinationPosX, playerDestinationPosY] != _storeBoxNumber)
        {
            return;
        }
        //箱が移動できないときは処理しない
        else if(_fieldDataArray[boxDestinationPosX, boxDestinationPosY] != _nullSquaresNumber)
        {
            return;
        }

        //箱が移動できるときの処理
        for (int i = 0; i < _boxObjects.Length; i++)
        {
            //プレイヤーが押し出す箱を検索し、その箱を動かす
            if (playerDestinationPosX == _boxCurrentPosX[i] && playerDestinationPosY == _boxCurrentPosY[i])
            {
                //フィールド情報の値を変更する
                _fieldDataArray[boxDestinationPosX, boxDestinationPosY] = _storeBoxNumber;
                _fieldDataArray[_boxCurrentPosX[i], _boxCurrentPosY[i]] = _nullSquaresNumber;

                //箱を動かす
                _boxObjects[i].transform.position = new Vector2(boxDestinationPosX, boxDestinationPosY);

                //箱の現在地を更新
                _boxCurrentPosX[i] = boxDestinationPosX;
                _boxCurrentPosY[i] = boxDestinationPosY;

                PlayerMove();

                //動かした箱がゴールに入ったかどうかを検知する(jはゴールの数)
                for (int j = 0; j < _boxGoalPos.Length; j++)
                {
                    //箱の現在地を格納する
                    Vector2 boxCurrentPos = new Vector2(_boxCurrentPosX[i], _boxCurrentPosY[i]);
                
                    //動かした箱がゴールした時の処理
                    if (_boxGoalPos[j] == boxCurrentPos)
                    {
                        //ゴールフラグをセットする
                        _isBoxGoal[i] = true;

                        BoxGoalCheck();
                        return;
                    }
                }

                //ゴールしていなかった時の処理
                _isBoxGoal[i] = false;
            }
        }
    }


    /// <summary>
    /// プレイヤーを実際に移動する処理
    /// </summary>
    private void PlayerMove()
    {
        //プレイヤーを移動する
        _playerObj.transform.position = new Vector2(playerDestinationPosX, playerDestinationPosY);

        //プレイヤーの現在地を更新
        _playerCurrentPosX = playerDestinationPosX;
        _playerCurrentPosY = playerDestinationPosY;
    }


    /// <summary>
    /// 箱がゴール地点に入っているかを判定するメソッド
    /// </summary>
    private void BoxGoalCheck()
    {
        for(int i = 0; i < _isBoxGoal.Length; i++)
        {
            if(!_isBoxGoal[i])
            {
                return;
            }
        }

        //ゴール処理
        _goalTextObj.SetActive(true);
    }
}
