using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldData : MonoBehaviour
{
    [SerializeField, Header("�u���b�N�}�X(��)�̃I�u�W�F�N�g")]
    private GameObject _blockSquaresObj;
    [SerializeField, Header("��̃}�X�̃I�u�W�F�N�g")]
    private GameObject _nullSquaresObj;

    [Header("�l�̓��͎��͕K���u���b�N�}�X����X�^�[�g����")]
    [SerializeField, TextArea, Header("(2,5,1) �u���b�N�}�X(��),�ʂ��}�X,�u���b�N�}�X")]
    private string _inputFieldData;

    //�t�B�[���h�̗񐔂ƍs��
    private int _fieldColumn = default;
    private int _fieldRow = default;

    //�t�B�[���h�f�[�^���i�[���Ă���z��
    public int[,] _fieldDataArray { get; private set; }

    //�u���b�N�������Ă��Ȃ��}�X(�ʂ��}�X)��0�A�����Ă���ꏊ��1�Ƃ���
    private const int _storeBlockNumber = 1;
    private const int _nullSquaresNumber = 0;


    /// <summary>
    ///���͂��ꂽ�e�L�X�g�ɉ����āA�}�b�v�������s�� 
    /// </summary>
    private void Awake()
    {
        GetFieldDataArraySize();

        //�t�B�[���h�f�[�^�𒊏o����ϐ�
        string inputFieldDataValue = default;

        //�f�[�^�i�[���̍s
        int blockSetRow = 0;

        //�}�X�ɒl���Z�b�g����ۂ̍s���̃X�^�[�g�ʒu
        int blockSetStartColumn = 0;

        //�i�[����}�X�ɒl������ϐ��Ƃ���ɑΉ����鐶���I�u�W�F�N�g
        int squareValue = default;
        GameObject instantObj = default;

        //�t�B�[���h�f�[�^���i�[���鏈��
        for (int i = 0; i < _inputFieldData.Length; i++)
        {
            //���l���ЂƂL�^���鏈��
            for (int m = i; i < _inputFieldData.Length && _inputFieldData[m] != ',' && _inputFieldData[m] != '\n'; m++)
            {
                //�e�L�X�g�œ��͂��ꂽ���l���L�^����
                inputFieldDataValue = inputFieldDataValue + _inputFieldData[m].ToString();
                i++;
            }

            //���o���ꂽ�f�[�^��l�^�ɕϊ����鏈��
            int blockSetNumber = int.Parse(inputFieldDataValue);
            inputFieldDataValue = default;

            //�i�[���n�߂��ʒu��0�Ȃ�u���b�N�}�X�̒l������(�s�̍ŏ����u���b�N�}�X�ł��邽��)
            if (blockSetStartColumn == 0)
            {
                squareValue = _storeBlockNumber;
                instantObj = _blockSquaresObj;
            }
            else
            {
                //�O��i�[�����l���u���b�N�}�X�Ȃ�A��}�X�̒l������
                if (squareValue == _storeBlockNumber)
                {
                    squareValue = _nullSquaresNumber;
                    instantObj = _nullSquaresObj;
                }
                //�O��i�[�����l����}�X�Ȃ�A�u���b�N�}�X�̒l������
                else
                {
                    squareValue = _storeBlockNumber;
                    instantObj = _blockSquaresObj;
                }
            }

            //�z��Ɋi�[���A�I�u�W�F�N�g��z�u���鏈��
            for (int k = 0; k < blockSetNumber; k++)
            {
                //�z��(_fieldDataArray)�ɒl���Z�b�g����
                _fieldDataArray[blockSetStartColumn + k, blockSetRow] = squareValue;
                //�t�B�[���h��ɃI�u�W�F�N�g��z�u����
                Instantiate(instantObj).transform.position = new Vector3(blockSetStartColumn + k, blockSetRow, 0);
            }

            //�i�[���n�߂��ʒu���X�V
            blockSetStartColumn += blockSetNumber;

            //�񐔂Ɗi�[���n�߂��ʒu����v������A���̍s�ɍX�V����
            if (blockSetStartColumn == _fieldColumn)
            {
                blockSetRow += 1;
                blockSetStartColumn = 0;
            }
        }
    }


    /// <summary>
    /// �t�B�[���h�f�[�^�̔z��̑傫�����`����
    /// </summary>
    private void GetFieldDataArraySize()
    {
        //�t�B�[���h�f�[�^����񐔂𒊏o���邽�߂̕ϐ�
        string FieldDataCulumnValue = default;

        //�t�B�[���h�̗񐔂��擾����(���ׂẴ}�X�Ƀu���b�N�}�X������̈�s�ڂ��璊�o)
        for (int i = 0; _inputFieldData[i] != ',' && _inputFieldData[i] != '\n'; i++)
        {
            //�񐔂��L�^����
            FieldDataCulumnValue += _inputFieldData[i].ToString();
        }
        //�񐔂𐔒l������
        _fieldColumn += int.Parse(FieldDataCulumnValue);
        FieldDataCulumnValue = default;


        //�t�B�[���h�̍s�����擾����
        _fieldRow = 1;
        for (int i = 0; i < _inputFieldData.Length; i++)
        {
            //���s����������s�����v���X����
            if (_inputFieldData[i] == '\n')
            {
                _fieldRow += 1;
            }
        }
        //�t�B�[���h�f�[�^(�񎟌��z��)�̑傫�����`����
        _fieldDataArray = new int[_fieldColumn, _fieldRow];

    }
}