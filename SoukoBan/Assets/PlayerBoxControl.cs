using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ---------------------------------------------------------
// FieldData.cs
//
// �쐬��:�g�c�Y��
// �v���C���[�̃R���g���[�����͂ɉ�����������s���X�N���v�g
// ---------------------------------------------------------

public class PlayerBoxControl : MonoBehaviour
{
    [SerializeField, Tooltip("�t�B�[���h�̃f�[�^�̃X�N���v�g")]
    private FieldData _fieldData;

    [SerializeField, Header("�v���C���[�̃I�u�W�F�N�g")]
    private GameObject _playerObj;

    [SerializeField, Header("�v���C���[�̏����ʒu")]
    private Vector2 _playerInitialPos;


    [SerializeField, Header("���̃I�u�W�F�N�g")]
    private GameObject _boxObj;

    [SerializeField, Header("���̏����ʒu")]
    private Vector2[] _boxInitialPos;

    [SerializeField, Header("���̃S�[���ʒu�̃I�u�W�F�N�g")]
    private GameObject _boxGoalObj;

    [SerializeField, Header("���̃S�[���ʒu")]
    private Vector2[] _boxGoalPos;


    [SerializeField, Header("�S�[���e�L�X�g")]
    private GameObject _goalTextObj;

    [SerializeField, Header("����Scene��")]
    private string _thisSceneName;

    // �u���b�N�������Ă��Ȃ��}�X(�ʂ��}�X)��0�A�����Ă���ꏊ��1�A���������Ă���}�X��2�Ƃ���
    private const int STORE_BLOCK_NUMBER = 1;
    private const int EMPTY_BLOCK_NUMBER = 0;
    private const int STORE_BOX_NUMBER = 2;


    // ���̃I�u�W�F�N�g�ƈʒu�����i�[���Ă���ϐ�
    private GameObject[] _boxObjects = default;
    private int[] _boxCurrentPosX = default;
    private int[] _boxCurrentPosY = default;

    // �����S�[�����Ă��邩�𔻒肷��t���O
    private bool[] _isBoxGoal = default;

    // �v���C���[�̌��݈ʒu���
    private int _playerCurrentPosX = default;
    private int _playerCurrentPosY = default;

    // �t�B�[���h�f�[�^���i�[����ϐ�
    private int[,] _fieldDataArray = default;


    // �v���C���[�̈ړ���\��̈ʒu
    private int _playerDestinationPosX = default;
    private int _playerDestinationPosY = default;

    // �ړ���ɔ����������ꍇ�̔��̈ړ���\��̈ʒu
    private int _boxDestinationPosX = default;
    private int _boxDestinationPosY = default;

    private float _beforeVerticalInput = default;
    private float _beforeHorizontalInput = default;


    private void Start()
    {
        // �u���b�N�������Ă��Ȃ��}�X(�ʂ��}�X)��0�A�����Ă���ꏊ��1�A��������}�X��2
        _fieldDataArray = _fieldData._fieldDataArray;

        // �v���C���[�������ʒu�Ɉړ�����
        PlayerStart();

        // ���ƃS�[���𐶐�����
        BoxStart();
    }


    /// <summary>
    /// �v���C���[��Start���\�b�h�ł̏���
    /// </summary>
    private void PlayerStart()
    {
        // �v���C���[�������ʒu�Ɉړ�����
        _playerCurrentPosX = (int)_playerInitialPos.x;
        _playerCurrentPosY = (int)_playerInitialPos.y;
        _playerObj.transform.position = _playerInitialPos;
    }


    /// <summary>
    /// ����Start���\�b�h�ł̏���
    /// </summary>
    private void BoxStart()
    {
        // �z��̑傫�����`
        _boxObjects = new GameObject[_boxInitialPos.Length];
        _boxCurrentPosX = new int[_boxInitialPos.Length];
        _boxCurrentPosY = new int[_boxInitialPos.Length];

        _isBoxGoal = new bool[_boxGoalPos.Length];


        // ���̏����ʒu�̐ݒ�����鏈��
        for (int i = 0; i < _boxInitialPos.Length; i++)
        {
            // ���̃S�[���n�_�𐶐�����
            Instantiate(_boxGoalObj).transform.position = _boxGoalPos[i];
            
            // ���̏����ʒu�𔠂̌��ݒn�Ɋi�[����
            _boxCurrentPosX[i] = (int)_boxInitialPos[i].x;
            _boxCurrentPosY[i] = (int)_boxInitialPos[i].y;

            // �t�B�[���h���ɔ��̒l��ݒ肷��
            _fieldDataArray[(int)_boxInitialPos[i].x, (int)_boxInitialPos[i].y] = STORE_BOX_NUMBER;

            // ���������ʒu�ɐ�������
            _boxObjects[i] = Instantiate(_boxObj);
            _boxObjects[i].transform.position = _boxInitialPos[i];

            // �������������S�[���ɓ��������ǂ��������m����(j�̓S�[���̐�)
            for (int j = 0; j < _boxGoalPos.Length; j++)
            {
                // ���̌��ݒn���i�[����
                Vector2 boxCurrentPos = new Vector2(_boxCurrentPosX[i], _boxCurrentPosY[i]);

                // ���ƃS�[���̈ʒu����v���Ă��邩
                if (_boxGoalPos[j] == boxCurrentPos)
                {
                    // �S�[���t���O���Z�b�g����
                    _isBoxGoal[i] = true;

                    // �S�Ă̔����S�[�����Ă��邩���`�F�b�N����
                    BoxGoalCheck();
                }
            }
        }
    }

    private void Update()
    {
        // �v���C���[�̈ړ���\��̈ʒu
        _playerDestinationPosX = _playerCurrentPosX;
        _playerDestinationPosY = _playerCurrentPosY;

        // �ړ���ɔ����������ꍇ�̔��̈ړ���\��̈ʒu
        _boxDestinationPosX = _playerCurrentPosX;
        _boxDestinationPosY = _playerCurrentPosY;

        /// �{�^�����������Ƃ��̏��� ///
        
        // ���͂ɉ����Ĉړ��������s������
        PlayerVerticalInputProcess();
        PlayerHorizontalInputProcess();

        // ���X�^�[�g
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(_thisSceneName);
        }
        // �Q�[���̏I��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    /// <summary>
    /// �c���̓��͂����������̏���
    /// </summary>
    private void PlayerVerticalInputProcess()
    {
        // ���̓��͂�����̈ړ���
        int oneInputMoveMass = 1;

        // �c���̓��͒l
        float verticalInput = Input.GetAxis("Vertical");

        // �O�t���[���ɏc���̓��͂����Ă����ꍇ�A�ړ������Ȃ�
        if (_beforeVerticalInput != 0)
        {
            // ���݃t���[���̓��͒l���i�[����
            _beforeVerticalInput = verticalInput;

            return;
        }

        // ���݃t���[���̓��͒l���i�[����
        _beforeVerticalInput = verticalInput;

        // ��Ɉړ�
        if (verticalInput > 0)
        {
            // �v���C���[�̈ړ��\��ʒu�Ɣ��̈ړ��\��ʒu���i�[����
            _playerDestinationPosY = _playerCurrentPosY + oneInputMoveMass;
            _boxDestinationPosY = _playerDestinationPosY + oneInputMoveMass;

            // �v���C���[�Ɣ��̈ړ��������s��
            MoveProcessing();
            return;
        }
        // ���Ɉړ�
        if (verticalInput < 0)
        {
            _playerDestinationPosY = _playerCurrentPosY - oneInputMoveMass;
            _boxDestinationPosY = _playerDestinationPosY - oneInputMoveMass;

            MoveProcessing();
            return;
        }
    }


    /// <summary>
    /// �����̓��͂����������̏���
    /// </summary>
    private void PlayerHorizontalInputProcess()
    {
        // ���̓��͂�����̈ړ���
        int oneInputMoveMass = 1;

        // �R���g���[���ƃL�[�̓��͗�
        float horizontalInput = Input.GetAxis("Horizontal");

        // �O�t���[���ɉ����̓��͂����Ă����ꍇ�A�ړ������Ȃ�
        if (_beforeHorizontalInput != 0)
        {
            // ���݃t���[���̓��͒l���i�[����
            _beforeHorizontalInput = horizontalInput;
            return;
        }

        // ���݃t���[���̓��͒l���i�[����
        _beforeHorizontalInput = horizontalInput;

        // �E�Ɉړ�
        if (horizontalInput > 0)
        {
            // �v���C���[�̈ړ��\��ʒu�Ɣ��̈ړ��\��ʒu���i�[����
            _playerDestinationPosX = _playerCurrentPosX + oneInputMoveMass;
            _boxDestinationPosX = _playerDestinationPosX + oneInputMoveMass;

            // �v���C���[�Ɣ��̈ړ��������s��
            MoveProcessing();
            return;
        }
        // ���Ɉړ�
        if (horizontalInput < 0)
        {
            _playerDestinationPosX = _playerCurrentPosX - oneInputMoveMass;
            _boxDestinationPosX = _playerDestinationPosX - oneInputMoveMass;

            MoveProcessing();
            return;
        }
    }


    /// <summary>
    /// �ړ����邩�𔻒肷�鏈���Ɣ����ړ����鏈�����s��
    /// </summary>
    private void MoveProcessing()
    {
        // �ړ��悪��̃}�X�̎��̏���
        if (_fieldDataArray[_playerDestinationPosX, _playerDestinationPosY] == EMPTY_BLOCK_NUMBER)
        {
            // �v���C���[���ړ����鏈��
            PlayerMove();

            return;
        }

        // �ړ���ɔ����Ȃ��Ƃ��͏������Ȃ�
        if (_fieldDataArray[_playerDestinationPosX, _playerDestinationPosY] != STORE_BOX_NUMBER)
        {
            return;
        }
        // �����ړ��ł��Ȃ��Ƃ��͏������Ȃ�
        else if(_fieldDataArray[_boxDestinationPosX, _boxDestinationPosY] != EMPTY_BLOCK_NUMBER)
        {
            return;
        }

        // �����ړ��ł���Ƃ��̏���
        for (int i = 0; i < _boxObjects.Length; i++)
        {
            // �v���C���[�������o�������������A���̔��𓮂���
            if (_playerDestinationPosX == _boxCurrentPosX[i] && _playerDestinationPosY == _boxCurrentPosY[i])
            {
                // �t�B�[���h���̒l��ύX����
                _fieldDataArray[_boxDestinationPosX, _boxDestinationPosY] = STORE_BOX_NUMBER;
                _fieldDataArray[_boxCurrentPosX[i], _boxCurrentPosY[i]] = EMPTY_BLOCK_NUMBER;

                // ���𓮂���
                _boxObjects[i].transform.position = new Vector2(_boxDestinationPosX, _boxDestinationPosY);

                // ���̌��ݒn���X�V
                _boxCurrentPosX[i] = _boxDestinationPosX;
                _boxCurrentPosY[i] = _boxDestinationPosY;

                // �v���C���[���ړ����鏈��
                PlayerMove();

                // �������������S�[���ɓ��������ǂ��������m����(j�̓S�[���̐�)
                for (int j = 0; j < _boxGoalPos.Length; j++)
                {
                    // ���̌��ݒn���i�[����
                    Vector2 boxCurrentPos = new Vector2(_boxCurrentPosX[i], _boxCurrentPosY[i]);
                
                    // �������������S�[���������̏���
                    if (_boxGoalPos[j] == boxCurrentPos)
                    {
                        // �S�[���t���O���Z�b�g����
                        _isBoxGoal[i] = true;

                        // �S�Ă̔����S�[�����Ă��邩���`�F�b�N����
                        BoxGoalCheck();

                        return;
                    }
                }

                // �S�[�����Ă��Ȃ��������̏���
                _isBoxGoal[i] = false;
            }
        }
    }


    /// <summary>
    /// �v���C���[�����ۂɈړ����鏈��
    /// </summary>
    private void PlayerMove()
    {
        // �v���C���[���ړ�����
        _playerObj.transform.position = new Vector2(_playerDestinationPosX, _playerDestinationPosY);

        // �v���C���[�̌��ݒn���X�V
        _playerCurrentPosX = _playerDestinationPosX;
        _playerCurrentPosY = _playerDestinationPosY;
    }


    /// <summary>
    /// �����S�[���n�_�ɓ����Ă��邩�𔻒肷�郁�\�b�h
    /// </summary>
    private void BoxGoalCheck()
    {
        // ���ׂăS�[�����Ă��邩�𔻒肷��
        for(int i = 0; i < _isBoxGoal.Length; i++)
        {
            if(!_isBoxGoal[i])
            {
                return;
            }
        }

        // �S�[������
        _goalTextObj.SetActive(true);
    }
}
