using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Map Generator Explanation
    //----------------------------------------------------------------------------------------------
    //  MapGenerator�i�}�b�v�����j�ɂ���
    //----------------------------------------------------------------------------------------------
    //  �P�̃}�b�v�̐ݒ�
    //  ���̃}�b�v�͌����Ȃ��ǂɈ͂���
    //  �͂���̂�Floor(Grass)����
    //
    //  Floor -> ���ʂ�Plane�̂R�c����
    //  Floor(House) -> ���F��Plane
    //  Floor(Grass) -> �ΐF��Plane
    //  Floor�� mapX �� mapZ�@�̑傫���ɂ���ĕς��

    //  2D�̔z��݂����ɕ`�悷���
    //  ��@mapX = 2, mapZ = 3;
    //  ---FH---
    //  eeeeeeee
    //  eFG  FGe
    //  eFG  FGe
    //  eFG  FGe
    //  eeeeeeee
    //  ��ԍ���FG�́i0�C0�C0�j
    //  e�� Extra Field
    //  FG -> Field(Grass)
    //  FH -> Field(House)

    //�@Floor(Grass)�ɂ���
    //  �ŏ��͑SFloor(Grass)�̍��W�� x:10 y:0 z:10�@�i��ňړ�����j
    //�@Floor(Grass)�̒��g�͎����I�ɐ�������
    //  �P��Floor(Grass)�̒��g��Decoration Settings�Őݒ肷��
    //  ���ӁF�v�Z�~�X�ƃ}�b�v�̐����͎��s�ł��B

    //  Floor(Grass)��decoration�u����v�v�Z
    //  decorGrass�Ƒ���decor�̃��[�����Ⴂ

    //  !!Plane�̑傫����10*10
    //  floor(Grass)�̏���̐������@
    //  1�FdecorGrass�ȊO�̏�����ɐ�������
    //  ���̂��߂ɂP�̔z��5*5�̑傫������������
    //  ���̔z��͔z�u������W�̂��߂ł��B�i���̏ꍇ1D�z��Œ�����25�j
    //�@�Q�F���̔z��̒��g��0�`25�����āA���g�̐��l�̏ꏊ�������_������
    //  �R�FdecorBush�𐶐�����
    //  �S�FdecorStump�𐶐�����
    //  �T�FdecorFlower�𐶐�����
    //  �U�FdecorSmallRock�𐶐�����
    //  ��������̎������ō��W�v�Z������܂��B
    //  ��i�C���[�W�Ƃ��āj�F5*5
    //�@--------------------------------
    //  |  x  |  x  |  x  |  x  |  x  |         �e�����̂̔z�u�̋�����4.0f;Plane�̒����͂�20.0f�B20.0f/5.0f�@�͂S.0f
    //  |  x  |  x  |  x  |  x  |  x  |         ��ł��̏ꏊ����@-2.0f~2.0f�ɂ��炷(x�͐^�񒆂ɔz�u����)
    //  |  x  |  x  |  x  |  x  |  x  |         -10.0���̗��R�͂���floor(Grass)�̍��̍��W�� Vector3(10,0,10)
    //  |  x  |  x  |  x  |  x  |  x  |         ���floor(Grass)�̎q���ɂȂ�̂ł��̃��J�����W->Vector3�͈̔� �́i0,0,0�j ~ (20,0,20)
    //  |  x  |  x  |  x  |  x  |  x  |         -10���Ȃ��Ɣz�u����Vector3�͈̔͂� -> Vector3(-10,0,-10) ~ (10,0,10)�@��₱�����ɂȂ�
    //  |  x  |  x  |  x  |  x  |  x  |
    //  -------------------------------

    //  decorGrass�̐���
    //  �P�̔z��51010�̑傫������������(���̏ꍇ�PD�̔z��A������100)
    //  ���̔z���decorGrass�̍��W�̂���
    //  2�F���g��0�`100�̂����āA���g�̐��l�̏ꏊ�������_������
    //  �R�FdecorGrass�𐶐�����
    //  ��������̎������ō��W�v�Z������܂��B
    //  decorGrass�̍ŏ��u���ꏊ�͍���0�C0�A���decor�ƈႢ�i���decor�̍��W��0�C0�ł͂Ȃ��j

    //  Floor(House)�ɂ���
    //  �܂������Ȃ��A�����`�悾��

    //  �v���C���[�̃X�|�[���ꏊ
    //  �����ォ��^��
    //  ��@mapX = 2, mapZ = 3;�@
    //  �v���C���[�� x = 1.5f , z= 2.8f;
    //  �v���C���[�̓X�|�[���O�Ɏ����Bushes��Stump������

    //----------------------------------------------------------------------------------------------
    #endregion

    //�}�b�v�̑傫���E�L��
    [Header("Map Setting")]
    [SerializeField] int mapX;
    [SerializeField] int mapZ;

    [Header("Present Settings")]
    [SerializeField] int presentNum;        //�{��
    [SerializeField] int presentHazureNum;   //�O��
    [SerializeField] int presentTotal;      //�S���v���[���g��

    //�e�}�X�̏���ݒ�
    //decorGrass�͍ő吔��100
    //decorFlower,decorBushes,decorStump,decorSmallRock�̍��v�͍ő吔��25
    //decorBush�̐���presentTotal��葽���ɐݒ肵�Ă��������B
    [Header("Decoration Settings")]
    [SerializeField] GardenDecoration decorGrass;
    [SerializeField] GardenDecoration decorFlower;
    [SerializeField] GardenDecoration decorBushes;
    [SerializeField] GardenDecoration decorStump;
    [SerializeField] GardenDecoration decorSmallRock;

    //�v���n�u�ݒ�
    [Header("Prefab Settings")]
    [SerializeField] GameObject prefFloorGrass;
    [SerializeField] GameObject prefFloorHouse;
    [SerializeField] GameObject prefPresent;
    [SerializeField] GameObject prefPresentHazure;
    [SerializeField] GameObject prefFence;
    [SerializeField] GameObject prefFencePost;
    [SerializeField] GameObject prefHouse;
    [SerializeField] GameObject prefChargingZone;


    //Floor�i���j��House�i�Ɓj�̔z��
    [Header("Floor Array")]
    GameObject[] floorGrassArr;
    GameObject floorHouse;

    //Floor�i���j�̃T�C�Y��ŏ����W
    [Header("Floor Size & First Coordinate")]
    const float floorGrassDistance = 20.0f;
    const float floorGrassSize = 10.0f;
    const float floorHouseSize = 5.0f;
    const float floorHouseSizeZ = 10.0f;
    float firstPosGrassX;
    float firstPosGrassZ;
    float firstPosHouseX;
    float firstPosHouseZ;

    //�v���[���g��}�b�v�̐e����
    [Header("Game Object Parent")]
    [SerializeField] GameObject mapParent;
    [SerializeField] GameObject presentParent;
    [SerializeField] GameObject extraZoneParent;
    [SerializeField] GameObject floorGrassParent;
    [SerializeField] GameObject fenceParent;
    [SerializeField] GameObject coverZoneParent;

    //�G�N�X�g���G���A
    [Header("Extra Zone")]
    const float extraZoneSize = 1.0f;
    const float extraZoneHalfSize = extraZoneSize * 0.5f;
    const float extraZoneDoubleSize = extraZoneSize * 2.0f;
    [SerializeField] GameObject extraZoneLeft;
    [SerializeField] GameObject extraZoneRight;
    [SerializeField] GameObject extraZoneTop;
    [SerializeField] GameObject extraZoneBottom;

    //�����Ȃ��ǁi�v���C���[�͒�̒�����o�Ȃ����߂Ɂj
    [Header("Invisible Wall")]
    const float invisibleWallSize = 1.0f;
    const float invisibleWallHalfSize = invisibleWallSize * 0.5f;
    [SerializeField] GameObject wallLeft;
    [SerializeField] GameObject wallRight;
    [SerializeField] GameObject wallTop;
    [SerializeField] GameObject wallBottom;

    [Header("Placing Player")]
    [SerializeField] float playerClearObjectDistance = 10.0f;
    Vector3 playerFirstLoc;

    [Header("Fence Settings")]
    Vector3 fenceFirstPos = new Vector3(-1.0f, 0.0f, -1.0f);
    const float fenceSize = 2.0f;
    const float fenctNextRot = 90.0f;
    int fenceNum;

    [Header("House Settings")]
    const float houseSize = 6.0f;
    const float houseSizeHalf = houseSize * 0.5f;
    int removeFenceNum = 3;
    int removeFenceCounter = 0;
    bool isRemoveFence = false;

    [Header("Particle Plane")]
    [SerializeField] GameObject particlePlane;

    // Start is called before the first frame update
    void Start()
    {
        if (CheckRule()) return;

        floorGrassArr = new GameObject[mapX * mapZ];

        CalculateFloorFirstCoordinate();
        GenerateMap();
        CalculateWallSizeAndCoordinate();
        PlacingPlayer();
        PlacingHouseAndChargeZone();
        GenerateFence();
        //PresentGenerator();
        SetParticlePlane();
    }

    private void OnDrawGizmos()
    {
        //Debug�̂���
        //Gizmos.DrawSphere(playerClearObjectLoc , playerClearObjectDistance);
    }

    #region Map Generator Rule & Map Generator Rule Explanation

    #region Map Generator Rule Explanation
    //----------------------------------------------------------------------------------------------
    //  �uCheckRule�i�j�v�֐��̐���
    //----------------------------------------------------------------------------------------------
    //  4�̑厖���[��������B

    //  �P��  
    //  �}�b�v�̑傫���imapX,mapY�j1�ɂ��l���O�ȉ��ɐݒ�֎~
    //  �}�b�v�̑傫���@1��1�͋֎~�A�ł�1*2�A2*1�͂n�j

    //  �Q��
    //  ���̍ő吔��100�ȏ�֎~

    //  �R��
    //  decorFlower + decorBushes + decorStump + decorSmallRock
    //  �S�ő吔�̍��v��25�ȏ�͋֎~

    //  4��
    //  presentTotal���́u�SFloor(Grass)-1�v�@��葽��
    //  ���炭�v���C���[���X�|�[����Bushes�ƂԂ���̂�
    //  ������A�P��Floor(Grass)��Bushes���v�Z�Ɋ܂߂Ȃ�
    //�@�SFloor(Grass)�́@mapX * mapZ
    //----------------------------------------------------------------------------------------------
    #endregion
    bool CheckRule()
    {
        bool isAnyRuleViolated = false;
        if (mapX <= 0 || mapZ <= 0)
        {
            Debug.Log("Warning!! The map size is <=0�i�}�b�v�̃T�C�Y�́@<=0�j");
            Debug.Log("Please increase the map size(�}�b�v�̃T�C�Y��傫�����Ă��������B)");
            isAnyRuleViolated = true;
        }

        if (mapX == 1 && mapZ == 1)
        {
            Debug.Log("Warning!! The map size cannot 1x1�i�}�b�v�̃T�C�Y��1x1�͋֎~�j");
            Debug.Log("Please increase the map size(�}�b�v�̃T�C�Y��傫�����Ă��������B)");
            isAnyRuleViolated = true;
        }

        if (decorGrass.GetDecorMaxNum() > 100)
        {
            Debug.Log("Warning!! The total of decoration grass is over 100�i���̐���100�ȏ�ł��B�j");
            Debug.Log("Please reduce the grass number(���̐������炵�Ă��������B)");
            isAnyRuleViolated = true;
        }
        int totalDecor = decorFlower.GetDecorMaxNum() + decorBushes.GetDecorMaxNum() + decorSmallRock.GetDecorMaxNum() + decorStump.GetDecorMaxNum();
        if (totalDecor > 25)
        {
            Debug.Log("Warning!! The total of decoration flower,bushes,stump,�@and small rock is over 25�i���ȊO�̏��萔��25�ȏ�ł��B�j");
            Debug.Log("Please reduce the decoration max num�i���ȊO�̏���̍ő吔�����炵�Ă��������B�j");
            isAnyRuleViolated = true;
        }

        int bushMinNum = decorBushes.GetDecorMinNum() * ((mapX * mapZ) - 1);
        presentTotal = presentNum + presentHazureNum;
        if (presentTotal >= bushMinNum)
        {
            Debug.Log("Warning!! Bush Number is less than the total present number�iBush���͑S�v���[���g����菬�����j");
            Debug.Log("Please decrease present num or increase bush min num�i�S�v���[���g�������炷��bush�̍Œᐔ������������Ă��������B�j");
            isAnyRuleViolated = true;
        }
        return isAnyRuleViolated;
    }
    #endregion

    #region Calculate Methods
    void CalculateFloorFirstCoordinate()
    {
        firstPosGrassX = floorGrassSize;
        firstPosGrassZ = floorGrassSize;
        firstPosHouseX = mapX * floorGrassSize;
        firstPosHouseZ = floorHouseSize + mapZ * floorGrassDistance + extraZoneSize;
    }

    void CalculateWallSizeAndCoordinate()
    {
        //Left Side
        {
            Vector3 newSize = new Vector3();
            newSize.x = 1.0f;
            newSize.y = 10.0f;
            newSize.z = mapZ * floorGrassDistance + extraZoneDoubleSize;

            Vector3 newPos = new Vector3();
            newPos.x = -invisibleWallHalfSize - extraZoneSize;
            newPos.y = 0.0f;
            newPos.z = mapZ * floorGrassSize;

            wallLeft.transform.position = newPos;
            wallLeft.transform.localScale = newSize;
        }

        //Right Side
        {
            Vector3 newSize = new Vector3();
            newSize.x = 1.0f;
            newSize.y = 10.0f;
            newSize.z = mapZ * floorGrassDistance + extraZoneDoubleSize;

            Vector3 newPos = new Vector3();
            newPos.x = mapX * floorGrassDistance + invisibleWallHalfSize + extraZoneSize;
            newPos.y = 0.0f;
            newPos.z = mapZ * floorGrassSize;

            wallRight.transform.position = newPos;
            wallRight.transform.localScale = newSize;
        }

        //Top Side
        {
            Vector3 newSize = new Vector3();
            newSize.x = mapX * floorGrassDistance + extraZoneDoubleSize;
            newSize.y = 10.0f;
            newSize.z = 1.0f;

            Vector3 newPos = new Vector3();
            newPos.x = mapX * floorGrassSize;
            newPos.y = 0.0f;
            newPos.z = mapZ * floorGrassDistance + invisibleWallHalfSize + extraZoneSize;

            wallTop.transform.position = newPos;
            wallTop.transform.localScale = newSize;
        }

        //Bottom Side
        {
            Vector3 newSize = new Vector3();
            newSize.x = mapX * floorGrassDistance + extraZoneDoubleSize;
            newSize.y = 10.0f;
            newSize.z = 1.0f;

            Vector3 newPos = new Vector3();
            newPos.x = mapX * floorGrassSize - extraZoneSize;
            newPos.y = 0.0f;
            newPos.z = -invisibleWallHalfSize - extraZoneSize;

            wallBottom.transform.position = newPos;
            wallBottom.transform.localScale = newSize;
        }
    }

    #endregion

    #region Map Generator Methods
    void GenerateMap()
    {
        //Creating Floor
        CreatingFloorGrass();
        CreatingFloorHouse();

        //Placing Floor
        PlacingFloorGrass();
        PlacingFloorHouse();

        //Placing Extra And Cover Zone
        GenerateExtraZone();
        GenerateCoverZone();
    }

    #region Get Random Rotation
    float GetRandomRotationY360()
    {
        float rot = Random.Range(0.0f, 360.0f);
        return rot;
    }
    float GetRandomRotationMultipleOf90()
    {
        float[] rot = { 0.0f, 90.0f, 180.0f, 270.0f };
        return rot[Random.Range(0, rot.Length)];
    }
    Vector3 GetRandomVectorRotationY360()
    {
        Vector3 rot = new Vector3();
        rot.y = GetRandomRotationY360();
        return rot;
    }
    #endregion

    #region Generate Floor Grass
    void CreatingFloorGrass()
    {
        int floorCount = floorGrassArr.Length;
        for (int i = 0; i < floorCount; ++i)
        {
            floorGrassArr[i] = GenerateFloorGrass();
        }
    }
    void PlacingFloorGrass()
    {
        int indexFloorArr = 0;
        for (int i = 0; i < mapZ; ++i)
        {
            Vector3 newPos = new Vector3();
            newPos.z = firstPosGrassZ + (i * floorGrassDistance);
            for (int j = 0; j < mapX; ++j)
            {
                newPos.x = firstPosGrassX + (j * floorGrassDistance);
                GameObject obj = floorGrassArr[indexFloorArr];
                obj.transform.position = newPos;
                ++indexFloorArr;
            }
        }
    }
    GameObject GenerateFloorGrass()
    {
        GameObject parent = CreateFloorGrass();
        GenerateFloorGrassDecoration(parent);
        return parent;
    }
    GameObject CreateFloorGrass()
    {
        GameObject obj = Instantiate(prefFloorGrass);
        obj.transform.position = new Vector3(0, 0, 0);
        obj.transform.parent = floorGrassParent.transform;
        return obj;
    }
    void GenerateFloorGrassDecoration(GameObject parent)
    {
        {
            // 5*5 array
            const int arrLength = 25;
            const float distanceBetweenObj = 4.0f;

            const float randomAreaMin = -1.25f;
            const float randomAreaMax = 1.25f;

            int[] randomArr = new int[arrLength];
            int nowRandomArrNum = 0;

            for (int i = 0; i < arrLength; ++i)
            {
                randomArr[i] = i;
            }
            for (int i = 0; i < arrLength; ++i)
            {
                int tmpRandomNum = Random.Range(0, arrLength);
                int tmpVal = randomArr[i];
                randomArr[i] = randomArr[tmpRandomNum];
                randomArr[tmpRandomNum] = tmpVal;
            }

            //Create Bushes
            {
                int objNum = decorBushes.GetRandomDecorNum();
                for (int i = 0; i < objNum; ++i)
                {
                    GameObject obj = Instantiate(decorBushes.GetRandomDecorGameObject());

                    Vector3 newPos = new Vector3();
                    newPos.x = (randomArr[nowRandomArrNum] % 5) * distanceBetweenObj - 10.0f + 2.0f + Random.Range(randomAreaMin, randomAreaMax);
                    newPos.z = ((int)(randomArr[nowRandomArrNum] / 5)) * distanceBetweenObj - 10.0f + 2.0f + Random.Range(randomAreaMin, randomAreaMax);

                    obj.transform.position = newPos;
                    obj.transform.parent = parent.transform;
                    obj.transform.eulerAngles = GetRandomVectorRotationY360();

                    ++nowRandomArrNum;
                }

            }

            //Create Stump
            {
                int objNum = decorStump.GetRandomDecorNum();
                for (int i = 0; i < objNum; ++i)
                {
                    GameObject obj = Instantiate(decorStump.GetRandomDecorGameObject());

                    Vector3 newPos = new Vector3();
                    newPos.x = (randomArr[nowRandomArrNum] % 5) * distanceBetweenObj - 10.0f + 2.0f + Random.Range(randomAreaMin, randomAreaMax);
                    newPos.z = ((int)(randomArr[nowRandomArrNum] / 5)) * distanceBetweenObj - 10.0f + 2.0f + Random.Range(randomAreaMin, randomAreaMax);

                    obj.transform.position = newPos;
                    obj.transform.parent = parent.transform;
                    obj.transform.eulerAngles = GetRandomVectorRotationY360();
                    ++nowRandomArrNum;
                }
            }

            //Create Flower
            {
                int objNum = decorFlower.GetRandomDecorNum();
                for (int i = 0; i < objNum; ++i)
                {
                    GameObject obj = Instantiate(decorFlower.GetRandomDecorGameObject());

                    Vector3 newPos = new Vector3();
                    newPos.x = (randomArr[nowRandomArrNum] % 5) * distanceBetweenObj - 10.0f + 2.0f + Random.Range(randomAreaMin, randomAreaMax);
                    newPos.z = ((int)(randomArr[nowRandomArrNum] / 5)) * distanceBetweenObj - 10.0f + 2.0f + Random.Range(randomAreaMin, randomAreaMax);

                    obj.transform.position = newPos;
                    obj.transform.parent = parent.transform;
                    obj.transform.eulerAngles = GetRandomVectorRotationY360();
                    ++nowRandomArrNum;
                }
            }

            //Create Small Rock
            {
                int objNum = decorSmallRock.GetRandomDecorNum();
                for (int i = 0; i < objNum; ++i)
                {
                    GameObject obj = Instantiate(decorSmallRock.GetRandomDecorGameObject());

                    Vector3 newPos = new Vector3();
                    newPos.x = (randomArr[nowRandomArrNum] % 5) * distanceBetweenObj - 10.0f + 2.0f + Random.Range(randomAreaMin, randomAreaMax);
                    newPos.z = ((int)(randomArr[nowRandomArrNum] / 5)) * distanceBetweenObj - 10.0f + 2.0f + Random.Range(randomAreaMin, randomAreaMax);
                    obj.transform.position = newPos;
                    obj.transform.parent = parent.transform;
                    obj.transform.eulerAngles = GetRandomVectorRotationY360();
                    ++nowRandomArrNum;
                }
            }
        }

        //Grass Diffrent Rule
        //10*10 array
        {
            const int arrLength = 100;
            const float distanceBetweenObj = 2.0f;
            int[] randomArr = new int[arrLength];
            int nowRandomArrNum = 0;

            for (int i = 0; i < arrLength; ++i)
            {
                randomArr[i] = i;
            }
            for (int i = 0; i < arrLength; ++i)
            {
                int tmpRandomNum = Random.Range(0, arrLength);
                int tmpVal = randomArr[i];
                randomArr[i] = randomArr[tmpRandomNum];
                randomArr[tmpRandomNum] = tmpVal;
            }

            //Create Grass
            {
                int objNum = decorGrass.GetRandomDecorNum();
                for (int i = 0; i < objNum; ++i)
                {
                    GameObject obj = Instantiate(decorGrass.GetRandomDecorGameObject());

                    Vector3 newPos = new Vector3();
                    newPos.x = (randomArr[nowRandomArrNum] % 10) * distanceBetweenObj - 10.0f + Random.Range(0.0f, 2.0f);
                    newPos.z = ((int)(randomArr[nowRandomArrNum] / 10)) * distanceBetweenObj - 10.0f + Random.Range(0.0f, 2.0f);

                    obj.transform.position = newPos;
                    obj.transform.parent = parent.transform;
                    ++nowRandomArrNum;
                }
            }
        }

    }
    #endregion

    #region Generate Floor House
    void CreatingFloorHouse()
    {
        floorHouse = GenerateFloorHouse();
    }
    void PlacingFloorHouse()
    {
        Vector3 newPos = new Vector3();
        newPos.x = firstPosHouseX;
        newPos.z = firstPosHouseZ + floorHouseSizeZ * 0.5f;
        floorHouse.transform.position = newPos;
    }
    GameObject GenerateFloorHouse()
    {
        GameObject parent = CreateFloorHouse();
        return parent;
    }
    GameObject CreateFloorHouse()
    {
        const float extraSizeX = 4.2f;

        GameObject obj = Instantiate(prefFloorHouse);
        obj.transform.position = new Vector3(0, 0, 0);

        Vector3 newScale = new Vector3(0, 0, 0);
        newScale.x = mapX * 2.0f + extraSizeX;
        newScale.y = 1.0f;
        newScale.z = 2.0f;

        obj.transform.localScale = newScale;

        obj.transform.parent = mapParent.transform;
        return obj;
    }
    #endregion

    #region Generate Extra Zone
    void GenerateExtraZone()
    {
        {
            float extraSizeZ = 2.2f;
            float extraPosZ = 1.0f;
            //Left
            {
                extraZoneLeft = Instantiate(prefFloorGrass);

                // +1.2f newSize.z
                Vector3 newSize = new Vector3();
                newSize.x = 0.1f;
                newSize.y = 1.0f;
                newSize.z = mapZ * 2.0f + extraSizeZ;

                Vector3 newPos = new Vector3();
                newPos.x = -extraZoneHalfSize;
                newPos.z = mapZ * floorGrassSize - extraSizeZ * floorGrassSize * 0.5f + extraPosZ;

                extraZoneLeft.transform.position = newPos;
                extraZoneLeft.transform.localScale = newSize;
                extraZoneLeft.transform.parent = extraZoneParent.transform;
            }

            //Right
            {
                extraZoneRight = Instantiate(prefFloorGrass);

                Vector3 newSize = new Vector3();
                newSize.x = 0.1f;
                newSize.y = 1.0f;
                newSize.z = mapZ * 2.0f + extraSizeZ;

                Vector3 newPos = new Vector3();
                newPos.x = mapX * floorGrassDistance + extraZoneHalfSize;
                newPos.z = mapZ * floorGrassSize - extraSizeZ * floorGrassSize * 0.5f + extraPosZ; ;

                extraZoneRight.transform.position = newPos;
                extraZoneRight.transform.localScale = newSize;
                extraZoneRight.transform.parent = extraZoneParent.transform;
            }
        }

        //Top
        {
            float extraSizeX = 4.2f;
            {
                //�E�ƍ�
                extraZoneTop = Instantiate(prefFloorGrass);

                Vector3 newSize = new Vector3();
                newSize.x = mapX * 2.0f + extraSizeX;
                newSize.y = 1.0f;
                newSize.z = 0.1f;

                Vector3 newPos = new Vector3();
                newPos.x = mapX * floorGrassSize;
                newPos.z = mapZ * floorGrassDistance + extraZoneHalfSize;

                extraZoneTop.transform.position = newPos;
                extraZoneTop.transform.localScale = newSize;
                extraZoneTop.transform.parent = extraZoneParent.transform;
            }

            //Bottom
            {
                extraZoneBottom = Instantiate(prefFloorGrass);

                Vector3 newSize = new Vector3();
                newSize.x = mapX * 2.0f + extraSizeX;
                newSize.y = 1.0f;
                newSize.z = 0.1f;

                Vector3 newPos = new Vector3();
                newPos.x = mapX * floorGrassSize;
                newPos.z = -extraZoneHalfSize;

                extraZoneBottom.transform.position = newPos;
                extraZoneBottom.transform.localScale = newSize;
                extraZoneBottom.transform.parent = extraZoneParent.transform;
            }
        }
    }

    void GenerateCoverZone()
    {
        //Left & right
        {
            for (int i = 0; i < mapZ; ++i)
            {
                //Left
                {
                    GameObject floor = GenerateFloorGrass();
                    Vector3 newPos = new Vector3();
                    newPos.x = -(firstPosGrassX + extraZoneSize);
                    newPos.z = firstPosGrassZ + (i * floorGrassDistance);

                    floor.transform.position = newPos;
                    floor.transform.parent = coverZoneParent.transform;
                }

                //Right
                {
                    GameObject floor = GenerateFloorGrass();
                    Vector3 newPos = new Vector3();
                    newPos.x = (mapZ + 1) * floorGrassDistance + extraZoneSize - firstPosGrassX;
                    newPos.z = firstPosGrassZ + (i * floorGrassDistance);

                    floor.transform.position = newPos;
                    floor.transform.parent = coverZoneParent.transform;
                }

            }
        }

        //int indexFloorArr = 0;
        //for (int i = 0; i < mapZ; ++i)
        //{
        //    Vector3 newPos = new Vector3();
        //    newPos.z = firstPosGrassZ + (i * floorGrassDistance);
        //    for (int j = 0; j < mapX; ++j)
        //    {
        //        newPos.x = firstPosGrassX + (j * floorGrassDistance);
        //        GameObject obj = floorGrassArr[indexFloorArr];
        //        obj.transform.position = newPos;
        //        ++indexFloorArr;
        //    }
        //}

        //Bottom(Middle)
        {
            for (int i = 0; i < mapX; ++i)
            {
                //Left
                {
                    GameObject floor = GenerateFloorGrass();
                    Vector3 newPos = new Vector3();
                    newPos.x = firstPosGrassX + (i * floorGrassDistance);
                    newPos.z = -(firstPosGrassZ + extraZoneSize);

                    floor.transform.position = newPos;
                    floor.transform.parent = coverZoneParent.transform;
                }

            }
        }
        //Bottom(Left and Right)
        {
            //Left
            {
                GameObject floor = GenerateFloorGrass();
                Vector3 newPos = new Vector3();
                newPos.x = -(firstPosGrassX + extraZoneSize);
                newPos.z = -(firstPosGrassZ + extraZoneSize);

                floor.transform.position = newPos;
                floor.transform.parent = coverZoneParent.transform;
            }
            //Right
            {
                GameObject floor = GenerateFloorGrass();
                Vector3 newPos = new Vector3();
                newPos.x = (mapZ + 1) * floorGrassDistance + extraZoneSize - firstPosGrassX;
                newPos.z = -(firstPosGrassZ + extraZoneSize);

                floor.transform.position = newPos;
                floor.transform.parent = coverZoneParent.transform;
            }
        }
    }
    #endregion

    #region Generate Fence
    void GenerateFence()
    {
        // +1 -> extra zone
        int fenceNumHorizontal = mapX * (int)floorGrassSize + 1;
        int fenceNumVertical = mapZ * (int)floorGrassSize + 1;

        //Create Horizontal Bottom
        for (int i = 0; i < fenceNumHorizontal; ++i)
        {
            GameObject obj = Instantiate(prefFence, fenceParent.transform);
            Vector3 newRot = new Vector3(0.0f, 270.0f, 0.0f);
            Vector3 newPos = fenceFirstPos;
            newPos.x += 2.0f + i * fenceSize;

            obj.transform.position = newPos;
            obj.transform.eulerAngles = newRot;
        }

        //Create Horizontal Top
        for (int i = 0; i < fenceNumHorizontal; ++i)
        {
            if (isRemoveFence)
            {
                if (removeFenceCounter < removeFenceNum)
                {
                    ++removeFenceCounter;
                    continue;
                }
                else
                {
                    isRemoveFence = false;
                }
            }

            Vector3 newRot = new Vector3(0.0f, 90.0f, 0.0f);
            Vector3 newPos = fenceFirstPos;
            newPos.x += i * fenceSize;
            newPos.z = (mapZ * floorGrassSize * 2.0f) + extraZoneSize;

            if (newPos.x == playerFirstLoc.x - houseSizeHalf)
            {
                Vector3 fencePostPos = new Vector3(0.0f, 0.0f, 0.0f);
                fencePostPos.x = newPos.x;
                fencePostPos.z = newPos.z;
                GameObject objFencePost = Instantiate(prefFencePost, fenceParent.transform);
                objFencePost.transform.position = fencePostPos;

                isRemoveFence = true;
                ++removeFenceCounter;
                continue;
            }

            GameObject obj = Instantiate(prefFence, fenceParent.transform);
            obj.transform.position = newPos;
            obj.transform.eulerAngles = newRot;
        }

        //Create Vertical Left
        for (int i = 0; i < fenceNumVertical; ++i)
        {
            GameObject obj = Instantiate(prefFence, fenceParent.transform);
            Vector3 newRot = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 newPos = fenceFirstPos;
            newPos.z += i * fenceSize;

            obj.transform.position = newPos;
            obj.transform.eulerAngles = newRot;
        }

        //Create Vertical Left
        for (int i = 0; i < fenceNumVertical; ++i)
        {
            GameObject obj = Instantiate(prefFence, fenceParent.transform);
            Vector3 newRot = new Vector3(0.0f, 180.0f, 0.0f);
            Vector3 newPos = fenceFirstPos;
            newPos.x = (mapX * floorGrassSize * 2.0f) + extraZoneSize;
            newPos.z += 2.0f + i * fenceSize;

            obj.transform.position = newPos;
            obj.transform.eulerAngles = newRot;
        }

    }
    #endregion

    #region Player Placement
    void PlacingPlayer()
    {
        const float firstPosZOffset = 2.0f;
        Transform player = FindObjectOfType<PlayerMain>().transform;

        Vector3 newPlayerPos = new Vector3();
        newPlayerPos.x = mapX * floorGrassSize;
        newPlayerPos.y = player.position.y;
        newPlayerPos.z = mapZ * floorGrassDistance - firstPosZOffset;
        playerFirstLoc = newPlayerPos;

        GameObject[] objBush = GameObject.FindGameObjectsWithTag("Bush");
        GameObject[] objStump = GameObject.FindGameObjectsWithTag("Stump");

        foreach (GameObject obj in objBush)
        {
            float distance = Vector3.Distance(newPlayerPos, obj.transform.position);
            if (distance <= playerClearObjectDistance)
            {
                obj.SetActive(false);
                Destroy(obj);
            }
        }

        foreach (GameObject obj in objStump)
        {
            float distance = Vector3.Distance(newPlayerPos, obj.transform.position);
            if (distance <= playerClearObjectDistance)
            {
                obj.SetActive(false);
                Destroy(obj.gameObject);
            }
        }
        player.position = newPlayerPos;

    }

    #endregion

    #region House Placement
    void PlacingHouseAndChargeZone()
    {
        {
            const float posXOffset = -2.0f;
            const float posZOffset = 7.0f;

            Vector3 housePos = new Vector3();
            housePos.x = mapX * floorGrassSize + posXOffset;
            housePos.y = 0.0f;
            housePos.z = mapZ * floorGrassDistance + posZOffset;

            GameObject house = Instantiate(prefHouse, this.transform);
            house.transform.position = housePos;
        }

        {
            const float posZOffset = 0.5f;

            Vector3 chargingZonePos = new Vector3();
            chargingZonePos.x = mapX * floorGrassSize;
            chargingZonePos.y = 0.5f;
            chargingZonePos.z = mapZ * floorGrassDistance + posZOffset;

            GameObject obj = Instantiate(prefChargingZone, this.transform);
            obj.transform.position = chargingZonePos;
        }

    }

    #endregion

    #endregion

    #region Present Generator
    void PresentGenerator()
    {
        Bushes[] objBush = FindObjectsOfType<Bushes>();
        int bushNum = objBush.Length;
        int[] arrRandom = new int[bushNum];
        int nowRandomCounter = 0;
        for (int i = 0; i < bushNum; ++i)
        {
            arrRandom[i] = i;
        }

        for (int i = 0; i < bushNum; ++i)
        {
            int randomIndex = Random.Range(0, bushNum);
            int tmpVal = arrRandom[i];
            arrRandom[i] = arrRandom[randomIndex];
            arrRandom[randomIndex] = tmpVal;
        }

        PresentBoxManager presentBoxManager = FindObjectOfType<PresentBoxManager>();
        presentBoxManager.Init(presentNum);
        //Generete Present Box
        for (int i = 0; i < presentNum; ++i)
        {
            GameObject present = Instantiate(prefPresent, presentParent.transform);
            Transform bushPresentLoc = objBush[arrRandom[nowRandomCounter]].transform.Find("PresentLocation");
            Vector3 newPos = bushPresentLoc.position;
            present.transform.position = newPos;

            PresentBox presentBox = present.GetComponent<PresentBox>();
            presentBox.presentType = PresentBox.PRESENT_TYPE.PRESENT;
            presentBoxManager.AddPresentListRef(presentBox);

            ++nowRandomCounter;
        }

        //Generate Present Box Hazure
        for (int i = 0; i < presentHazureNum; ++i)
        {
            GameObject present = Instantiate(prefPresentHazure, presentParent.transform);
            Transform bushPresentLoc = objBush[arrRandom[nowRandomCounter]].transform.Find("PresentLocation");
            Vector3 newPos = bushPresentLoc.position;
            newPos.y += 0.6f;
            present.transform.position = newPos;

            PresentBox presentBox = present.GetComponent<PresentBox>();
            presentBox.presentType = PresentBox.PRESENT_TYPE.HAZURE;

            ++nowRandomCounter;
        }

    }
    #endregion

    #region Particle Plane
    void SetParticlePlane()
    {
        Vector3 newSize = new Vector3(mapX * 3.0f, 1.0f, mapZ * 3.5f);
        particlePlane.transform.localScale = newSize;

        Vector3 newPos = new Vector3(mapX * floorGrassSize, 0.0f, mapZ * floorGrassSize);
        particlePlane.transform.position = newPos;
    }
    #endregion
}

[System.Serializable]
struct GardenDecoration
{
    [Header("Number Settings")]
    [SerializeField] int decorMinNum;
    [SerializeField] int decorMaxNum;

    [Header("Prefab Settings")]
    [SerializeField] GameObject[] prefDecor;

    public int GetRandomDecorNum()
    {
        int number = Random.Range(decorMinNum, decorMaxNum + 1);
        return number;
    }

    public int GetDecorMinNum()
    {
        return decorMinNum;
    }
    public int GetDecorMaxNum()
    {
        return decorMaxNum;
    }

    public GameObject GetRandomDecorGameObject()
    {
        GameObject obj = prefDecor[Random.Range(0, prefDecor.Length)];
        return obj;
    }
}