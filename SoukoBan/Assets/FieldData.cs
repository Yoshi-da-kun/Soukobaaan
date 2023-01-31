using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldData : MonoBehaviour
{
    [SerializeField, Header("ブロックマス(壁)のオブジェクト")]
    private GameObject _blockSquaresObj;
    [SerializeField, Header("空のマスのオブジェクト")]
    private GameObject _nullSquaresObj;

    [Header("値の入力時は必ずブロックマスからスタートする")]
    [SerializeField, TextArea, Header("(2,5,1) ブロックマス(壁),通れるマス,ブロックマス")]
    private string _inputFieldData;

    //フィールドの列数と行数
    private int _fieldColumn = default;
    private int _fieldRow = default;

    //フィールドデータを格納している配列
    public int[,] _fieldDataArray { get; private set; }

    //ブロックが入っていないマス(通れるマス)を0、入っている場所を1とする
    private const int _storeBlockNumber = 1;
    private const int _nullSquaresNumber = 0;


    /// <summary>
    ///入力されたテキストに応じて、マップ生成を行う 
    /// </summary>
    private void Awake()
    {
        GetFieldDataArraySize();

        //フィールドデータを抽出する変数
        string inputFieldDataValue = default;

        //データ格納中の行
        int blockSetRow = 0;

        //マスに値をセットする際の行中のスタート位置
        int blockSetStartColumn = 0;

        //格納するマスに値を入れる変数とそれに対応する生成オブジェクト
        int squareValue = default;
        GameObject instantObj = default;

        //フィールドデータを格納する処理
        for (int i = 0; i < _inputFieldData.Length; i++)
        {
            //数値をひとつ記録する処理
            for (int m = i; i < _inputFieldData.Length && _inputFieldData[m] != ',' && _inputFieldData[m] != '\n'; m++)
            {
                //テキストで入力された数値を記録する
                inputFieldDataValue = inputFieldDataValue + _inputFieldData[m].ToString();
                i++;
            }

            //抽出されたデータを値型に変換する処理
            int blockSetNumber = int.Parse(inputFieldDataValue);
            inputFieldDataValue = default;

            //格納を始める列位置が0ならブロックマスの値を入れる(行の最初がブロックマスであるため)
            if (blockSetStartColumn == 0)
            {
                squareValue = _storeBlockNumber;
                instantObj = _blockSquaresObj;
            }
            else
            {
                //前回格納した値がブロックマスなら、空マスの値を入れる
                if (squareValue == _storeBlockNumber)
                {
                    squareValue = _nullSquaresNumber;
                    instantObj = _nullSquaresObj;
                }
                //前回格納した値が空マスなら、ブロックマスの値を入れる
                else
                {
                    squareValue = _storeBlockNumber;
                    instantObj = _blockSquaresObj;
                }
            }

            //配列に格納し、オブジェクトを配置する処理
            for (int k = 0; k < blockSetNumber; k++)
            {
                //配列(_fieldDataArray)に値をセットする
                _fieldDataArray[blockSetStartColumn + k, blockSetRow] = squareValue;
                //フィールド上にオブジェクトを配置する
                Instantiate(instantObj).transform.position = new Vector3(blockSetStartColumn + k, blockSetRow, 0);
            }

            //格納を始める列位置を更新
            blockSetStartColumn += blockSetNumber;

            //列数と格納を始める列位置が一致したら、次の行に更新する
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
        //フィールドデータから列数を抽出するための変数
        string FieldDataCulumnValue = default;

        //フィールドの列数を取得する(すべてのマスにブロックマスが入るの一行目から抽出)
        for (int i = 0; _inputFieldData[i] != ',' && _inputFieldData[i] != '\n'; i++)
        {
            //列数を記録する
            FieldDataCulumnValue += _inputFieldData[i].ToString();
        }
        //列数を数値化する
        _fieldColumn += int.Parse(FieldDataCulumnValue);
        FieldDataCulumnValue = default;


        //フィールドの行数を取得する
        _fieldRow = 1;
        for (int i = 0; i < _inputFieldData.Length; i++)
        {
            //改行があったら行数をプラスする
            if (_inputFieldData[i] == '\n')
            {
                _fieldRow += 1;
            }
        }
        //フィールドデータ(二次元配列)の大きさを定義する
        _fieldDataArray = new int[_fieldColumn, _fieldRow];

    }
}