using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---------------------------------------------------------
// FieldData.cs
//
// 作成者:吉田雄伍
// 入力されたデータ通りにフィールドを生成するスクリプト
// ---------------------------------------------------------

public class FieldData : MonoBehaviour
{
    [SerializeField, Header("ブロックマス(壁)のオブジェクト")]
    private GameObject _blockSquaresObj;
    [SerializeField, Header("空のマスのオブジェクト")]
    private GameObject _emptySquaresObj;

    [Header("値の入力時は必ずブロックマスからスタートする。 ※上下反転します")]
    [SerializeField, TextArea, Header("(2,5,1) ブロックマス(壁),通れるマス,ブロックマス")]
    private string _inputFieldData;

    // フィールドの列数と行数
    private int _fieldColumn = default;
    private int _fieldRow = default;

    // フィールドデータを格納している配列
    public int[,] _fieldDataArray { get; private set; }

    // ブロックが入っていないマス(通れるマス)を0、入っている場所を1とする
    private const int STORE_BLOCK_NUMBER = 1;
    private const int EMPTY_BLOCK_NUMBER = 0;


    /// <summary>
    /// 入力されたテキストに応じて、マップ生成を行う 
    /// </summary>
    private void Awake()
    {
        // フィールドデータの配列の大きさを定義する
        GetFieldDataArraySize();

        // フィールドデータを抽出する変数
        string inputFieldDataValue = default;

        // データ格納中の行
        int blockSetRow = 0;

        // マスに値をセットする際の行中のスタート位置
        int blockSetStartColumn = 0;

        // 格納するマスに値を入れる変数とそれに対応する生成オブジェクト
        int squareValue = default;
        GameObject instantObj = default;

        // フィールドデータを格納する処理
        for (int i = 0; i < _inputFieldData.Length; i++)
        {
            // 入力されたデータ中の数値を一つずつ抽出する処理
            for (int m = i; i < _inputFieldData.Length && _inputFieldData[m] != ',' && _inputFieldData[m] != '\n'; m++)
            {
                // 抽出した数値を格納する
                inputFieldDataValue = inputFieldDataValue + _inputFieldData[m].ToString();
                i++;
            }

            // 抽出されたデータを値型に変換する処理
            int blockSetNumber = int.Parse(inputFieldDataValue);
            inputFieldDataValue = default;

            // 格納する列位置が０(入力データの最初がブロックマス)または、前回格納した値が空マスならブロックマスの値を入れる
            if (squareValue == EMPTY_BLOCK_NUMBER || blockSetStartColumn == 0)
            {
                squareValue = STORE_BLOCK_NUMBER;
                instantObj = _blockSquaresObj;
            }
            // 前回格納した値がブロックマスなら、空マスの値を入れる
            else
            {
                squareValue = EMPTY_BLOCK_NUMBER;
                instantObj = _emptySquaresObj;
            }

            // 配列に格納し、オブジェクトを配置する処理
            for (int k = 0; k < blockSetNumber; k++)
            {
                // 配列(_fieldDataArray)に値をセットする
                _fieldDataArray[blockSetStartColumn + k, blockSetRow] = squareValue;
                // フィールド上にオブジェクトを配置する
                Instantiate(instantObj).transform.position = new Vector3(blockSetStartColumn + k, blockSetRow, 0);
            }

            // 格納を始める列位置を更新
            blockSetStartColumn += blockSetNumber;

            // 列数と格納を始める列位置が一致したら、次の行に更新する
            if (blockSetStartColumn == _fieldColumn)
            {
                blockSetRow += 1;
                blockSetStartColumn = 0;
            }
        }
    }


    /// <summary>
    /// フィールドデータの配列の大きさを定義する
    /// </summary>
    private void GetFieldDataArraySize()
    {
        // フィールドデータから列数を抽出するための変数
        string FieldDataCulumnValue = default;

        // フィールドの列数を取得する(すべてのマスにブロックマスが入るの一行目から抽出)
        for (int i = 0; _inputFieldData[i] != ',' && _inputFieldData[i] != '\n'; i++)
        {
            // 列数を記録する
            FieldDataCulumnValue += _inputFieldData[i].ToString();
        }
        // 列数を数値化する
        _fieldColumn += int.Parse(FieldDataCulumnValue);
        FieldDataCulumnValue = default;


        // フィールドの行数を取得する
        _fieldRow = 1;
        for (int i = 0; i < _inputFieldData.Length; i++)
        {
            // 改行があったら行数をプラスする
            if (_inputFieldData[i] == '\n')
            {
                _fieldRow += 1;
            }
        }
        // フィールドデータ(二次元配列)の大きさを定義する
        _fieldDataArray = new int[_fieldColumn, _fieldRow];

    }
}