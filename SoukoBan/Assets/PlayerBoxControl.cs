using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBoxControl : MonoBehaviour
{
    [SerializeField]
    private FieldData _fieldData;

    [SerializeField, Header("�v���C���[�̃I�u�W�F�N�g")]
    private GameObject _playerObj;

    [SerializeField, Header("�v���C���[�̏����ʒu")]
    private Vector2 _playerInisialPos;

    [SerializeField, Header("���̃I�u�W�F�N�g")]
    private GameObject _boxObj;

    [SerializeField, Header("���̏����ʒu")]
    private Vector2[] _boxInisialPos;

    [SerializeField, Header("���̃S�[���ʒu�̃I�u�W�F�N�g")]
    private GameObject _boxGoalObj;

    [SerializeField, Header("���̃S�[���ʒu")]
    private Vector2[] _boxGoalPos;


    [SerializeField, Header("�S�[���e�L�X�g")]
    private GameObject _goalTextObj;

    [SerializeField, Header("����Scene��")]
    private string _thisSceneName;

    //�u���b�N�������Ă��Ȃ��}�X(�ʂ��}�X)��0�A�����Ă���ꏊ��1�A���������Ă���}�X��2�Ƃ���
    private const int _storeBlockNumber = 1;
    private const int _nullSquaresNumber = 0;
    private const int _storeBoxNumber = 2;


    //���̃I�u�W�F�N�g�ƈʒu�����i�[���Ă���ϐ�
    private GameObject[] _boxObjects;
    private int[] _boxCurrentPosX;
    private int[] _boxCurrentPosY;

    //�����S�[�����Ă��邩�𔻒肷��t���O
    private bool[] _isBoxGoal;

    //�v���C���[�̌��݈ʒu���
    private int _playerCurrentPosX;
    private int _playerCurrentPosY;

    //�t�B�[���h�f�[�^���i�[����ϐ�
    private int[,] _fieldDataArray;


    //�v���C���[�̈ړ���\��̈ʒu
    private int playerDestinationPosX�@= default;
    private int playerDestinationPosY�@= default;

    //�ړ���ɔ����������ꍇ�̔��̈ړ���\��̈ʒu
    private int boxDestinationPosX = default;
    private int boxDestinationPosY = default;

    private void Start()
    {
        //�u���b�N�������Ă��Ȃ��}�X(�ʂ��}�X)��0�A�����Ă���ꏊ��1�A��������}�X��2
        _fieldDataArray = _fieldData._fieldDataArray;

        PlayerStart();

        BoxStart();
    }


    /// <summary>
    /// �v���C���[��Start���\�b�h�ł̏���
    /// </summary>
    private void PlayerStart()
    {
        //�v���C���[�������ʒu�Ɉړ�����
        _playerCurrentPosX = (int)_playerInisialPos.x;
        _playerCurrentPosY = (int)_playerInisialPos.y;
        _playerObj.transform.position = _playerInisialPos;
    }


    /// <summary>
    /// ����Start���\�b�h�ł̏���
    /// </summary>
    private void BoxStart()
    {
        //�z��̑傫�����`
        _boxObjects = new GameObject[_boxInisialPos.Length];
        _boxCurrentPosX = new int[_boxInisialPos.Length];
        _boxCurrentPosY = new int[_boxInisialPos.Length];

        _isBoxGoal = new bool[_boxGoalPos.Length];


        //���̏����ʒu�̐ݒ�����鏈��
        for (int i = 0; i < _boxInisialPos.Length; i++)
        {
            //���̃S�[���n�_�𐶐�����
            Instantiate(_boxGoalObj).transform.position = _boxGoalPos[i];
            
            //���̏����ʒu�𔠂̌��ݒn�Ɋi�[����
            _boxCurrentPosX[i] = (int)_boxInisialPos[i].x;
            _boxCurrentPosY[i] = (int)_boxInisialPos[i].y;

            //�t�B�[���h���ɔ��̒l��ݒ肷��
            _fieldDataArray[(int)_boxInisialPos[i].x, (int)_boxInisialPos[i].y] = _storeBoxNumber;

            //���������ʒu�ɐ�������
            _boxObjects[i] = Instantiate(_boxObj);
            _boxObjects[i].transform.position = _boxInisialPos[i];

            //�������������S�[���ɓ��������ǂ��������m����(j�̓S�[���̐�)
            for (int j = 0; j < _boxGoalPos.Length; j++)
            {
                //���̌��ݒn���i�[����
                Vector2 boxCurrentPos = new Vector2(_boxCurrentPosX[i], _boxCurrentPosY[i]);

                //���ƃS�[���n�_����v�����炷�ׂăS�[�����Ă��邩
                if (_boxGoalPos[j] == boxCurrentPos)
                {
                    _isBoxGoal[i] = true;
                }
            }
        }
    }

    private void Update()
    {
        //�v���C���[�̈ړ���\��̈ʒu
        playerDestinationPosX = _playerCurrentPosX;
        playerDestinationPosY = _playerCurrentPosY;

        //�ړ���ɔ����������ꍇ�̔��̈ړ���\��̈ʒu
        boxDestinationPosX = _playerCurrentPosX;
        boxDestinationPosY = _playerCurrentPosY;

        ///�{�^�����������Ƃ��̏���///
        //���̓��͂�����̈ړ���
        int _oneInputMoveMass = 1;

        //��Ɉړ�
        if (Input.GetKeyDown(KeyCode.W))
        {
            //�v���C���[�̈ړ��\��ʒu�Ɣ��̈ړ��\��ʒu���i�[����
            playerDestinationPosY = _playerCurrentPosY + _oneInputMoveMass;
            boxDestinationPosY = playerDestinationPosY + _oneInputMoveMass;

            MoveProcessing();
            return;
        }
        //���Ɉړ�
        if (Input.GetKeyDown(KeyCode.S))
        {
            playerDestinationPosY = _playerCurrentPosY - _oneInputMoveMass;
            boxDestinationPosY = playerDestinationPosY - _oneInputMoveMass;

            MoveProcessing();
            return;
        }
        //�E�Ɉړ�
        if (Input.GetKeyDown(KeyCode.D))
        {
            playerDestinationPosX = _playerCurrentPosX + _oneInputMoveMass;
            boxDestinationPosX = playerDestinationPosX + _oneInputMoveMass;

            MoveProcessing();
            return;
        }
        //���Ɉړ�
        if (Input.GetKeyDown(KeyCode.A))
        {
            playerDestinationPosX = _playerCurrentPosX - _oneInputMoveMass;
            boxDestinationPosX = playerDestinationPosX - _oneInputMoveMass;

            MoveProcessing();
            return;
        }

        //���X�^�[�g
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(_thisSceneName);
        }
        //�Q�[���̏I��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    /// <summary>
    /// �ړ����邩�𔻒肷�鏈���Ɣ����ړ����鏈�����s��
    /// </summary>
    private void MoveProcessing()
    {
        //�ړ��悪��̃}�X�̎��̏���
        if (_fieldDataArray[playerDestinationPosX, playerDestinationPosY] == _nullSquaresNumber)
        {
            PlayerMove();

            return;
        }

        //�ړ���ɔ����Ȃ��Ƃ��͏������Ȃ�
        if (_fieldDataArray[playerDestinationPosX, playerDestinationPosY] != _storeBoxNumber)
        {
            return;
        }
        //�����ړ��ł��Ȃ��Ƃ��͏������Ȃ�
        else if(_fieldDataArray[boxDestinationPosX, boxDestinationPosY] != _nullSquaresNumber)
        {
            return;
        }

        //�����ړ��ł���Ƃ��̏���
        for (int i = 0; i < _boxObjects.Length; i++)
        {
            //�v���C���[�������o�������������A���̔��𓮂���
            if (playerDestinationPosX == _boxCurrentPosX[i] && playerDestinationPosY == _boxCurrentPosY[i])
            {
                //�t�B�[���h���̒l��ύX����
                _fieldDataArray[boxDestinationPosX, boxDestinationPosY] = _storeBoxNumber;
                _fieldDataArray[_boxCurrentPosX[i], _boxCurrentPosY[i]] = _nullSquaresNumber;

                //���𓮂���
                _boxObjects[i].transform.position = new Vector2(boxDestinationPosX, boxDestinationPosY);

                //���̌��ݒn���X�V
                _boxCurrentPosX[i] = boxDestinationPosX;
                _boxCurrentPosY[i] = boxDestinationPosY;

                PlayerMove();

                //�������������S�[���ɓ��������ǂ��������m����(j�̓S�[���̐�)
                for (int j = 0; j < _boxGoalPos.Length; j++)
                {
                    //���̌��ݒn���i�[����
                    Vector2 boxCurrentPos = new Vector2(_boxCurrentPosX[i], _boxCurrentPosY[i]);
                
                    //�������������S�[���������̏���
                    if (_boxGoalPos[j] == boxCurrentPos)
                    {
                        //�S�[���t���O���Z�b�g����
                        _isBoxGoal[i] = true;

                        BoxGoalCheck();
                        return;
                    }
                }

                //�S�[�����Ă��Ȃ��������̏���
                _isBoxGoal[i] = false;
            }
        }
    }


    /// <summary>
    /// �v���C���[�����ۂɈړ����鏈��
    /// </summary>
    private void PlayerMove()
    {
        //�v���C���[���ړ�����
        _playerObj.transform.position = new Vector2(playerDestinationPosX, playerDestinationPosY);

        //�v���C���[�̌��ݒn���X�V
        _playerCurrentPosX = playerDestinationPosX;
        _playerCurrentPosY = playerDestinationPosY;
    }


    /// <summary>
    /// �����S�[���n�_�ɓ����Ă��邩�𔻒肷�郁�\�b�h
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

        //�S�[������
        _goalTextObj.SetActive(true);
    }
}
