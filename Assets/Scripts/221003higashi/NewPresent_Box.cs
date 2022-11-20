using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//=========================================================
// �R�������ĊJ����v���[���g
//=========================================================
public class NewPresent_Box : MonoBehaviour
{
    [Header("References")]
    GameManager gameManager;
    NewPresentBoxManager newPresentBoxManager;
    //PresentAction presentAction;

    Vector2 mouseStart;     // �}�E�X���N���b�N�����ꏊ
    Vector2 mouseStop;      // �}�E�X�𗣂����ꏊ
    float mouseAngle;       // �}�E�X���h���b�O�����p�x
    float mouseLength;      // �}�E�X���h���b�O��������

    [Header("if Open Present")]
    [SerializeField] public float minMouseLength = 150.0f;      // �h���b�O���Ăق����ŒZ����
    [SerializeField] public float minAngle = 90.0f;            // �h���b�O���Ăق����Œ�p�x
    [SerializeField] public float maxAngle = 170.0f;            // �h���b�O���Ăق����ō��p�x



    private void Awake()
    {
        newPresentBoxManager = FindObjectOfType<NewPresentBoxManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    { }
    //    // �}�E�X�̃h���b�O�������擾�ł����̂ŁA
    //    // ���ꂭ�炢�������������Ńv���[���g�J����邩�ǂ���������
    //    if (mouseLength >= 300.0f)
    //    {
    //        Debug.Log("�v���[���g���J���邱�Ƃ��ł�����I");
    //        Debug.Log(mouseLength);
    //    }
    //    // ������200�O�オ�Ó�����...
    //    else if (mouseLength < 200.0f && mouseLength > 0)
    //    {
    //        Debug.Log("�J���邱�Ƃ��ł��Ȃ�����...");
    //        Debug.Log(mouseLength);
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (newPresentBoxManager.presentTimeNow < 5.0f && newPresentBoxManager.presentTimeNow > 0.0f)
        {
            // �}�E�X�h���b�O�̋������擾����
            if (Input.GetMouseButtonDown(0))
            {
                mouseStart = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mouseStop = Input.mousePosition;
                mouseLength = (mouseStop - mouseStart).magnitude;
                mouseAngle = GetAngle(mouseStart, mouseStop);
                Debug.Log(mouseAngle);
            }
        }

        // �}�E�X�̃h���b�O�����Ƃ��̊p�x����v���[���g���J�����邩�ǂ���
        if (mouseLength >= minMouseLength)
        {
            mouseLength = 0.0f;
            //if ((mouseAngle > minAngle && mouseAngle <= maxAngle) ||
            //    (mouseAngle > -maxAngle && mouseAngle <= -minAngle))
            if (mouseAngle > minAngle && mouseAngle <= maxAngle)
            {
                // �v���[���g���J��
                Debug.Log("�v���[���g���J��");
                StartCoroutine(newPresentBoxManager.GetPresent());
            }

        }



    }


    // �}�E�X���h���b�O�����Ƃ��̂Q�_�̊p�x���擾����
    float GetAngle(Vector2 start, Vector2 end)
    {
        Vector2 dir = end - start;
        float rad = Mathf.Atan2(dir.x, dir.y);
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

}