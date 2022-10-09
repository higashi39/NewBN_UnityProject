using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Map Generator Explanation
    //----------------------------------------------------------------------------------------------
    //  MapGenerator（マップ生成）について
    //----------------------------------------------------------------------------------------------
    //  １のマップの設定
    //  このマップは見えない壁に囲われる
    //  囲われるのはFloor(Grass)だけ
    //
    //  Floor -> 普通のPlaneの３Ｄ物体
    //  Floor(House) -> 茶色のPlane
    //  Floor(Grass) -> 緑色のPlane
    //  Floorは mapX と mapZ　の大きさによって変わる

    //  2Dの配列みたいに描画すると
    //  例　mapX = 2, mapZ = 3;
    //  ---FH---
    //  eeeeeeee
    //  eFG  FGe
    //  eFG  FGe
    //  eFG  FGe
    //  eeeeeeee
    //  一番左下FGは（0，0，0）
    //  eは Extra Field
    //  FG -> Field(Grass)
    //  FH -> Field(House)

    //　Floor(Grass)について
    //  最初は全Floor(Grass)の座標は x:10 y:0 z:10　（後で移動する）
    //　Floor(Grass)の中身は自動的に生成する
    //  １つのFloor(Grass)の中身はDecoration Settingsで設定する
    //  注意：計算ミスとマップの生成は失敗です。

    //  Floor(Grass)のdecoration「飾り」計算
    //  decorGrassと他のdecorのルールが違い

    //  !!Planeの大きさは10*10
    //  floor(Grass)の飾りの生成方法
    //  1：decorGrass以外の飾りを先に生成する
    //  そのために１つの配列5*5の大きさを準備する
    //  その配列は配置する座標のためです。（私の場合1D配列で長さは25）
    //　２：その配列の中身は0～25を入れて、中身の数値の場所をランダムする
    //  ３：decorBushを生成する
    //  ４：decorStumpを生成する
    //  ５：decorFlowerを生成する
    //  ６：decorSmallRockを生成する
    //  生成するの時そこで座標計算があります。
    //  例（イメージとして）：5*5
    //　--------------------------------
    //  |  x  |  x  |  x  |  x  |  x  |         各自物体の配置の距離は4.0f;Planeの長さはは20.0f。20.0f/5.0f　は４.0f
    //  |  x  |  x  |  x  |  x  |  x  |         後でその場所から　-2.0f~2.0fにずらす(xは真ん中に配置する)
    //  |  x  |  x  |  x  |  x  |  x  |         -10.0ｆの理由はこのfloor(Grass)の今の座標は Vector3(10,0,10)
    //  |  x  |  x  |  x  |  x  |  x  |         後でfloor(Grass)の子供になるのでこのロカル座標->Vector3の範囲 は（0,0,0） ~ (20,0,20)
    //  |  x  |  x  |  x  |  x  |  x  |         -10しないと配置するVector3の範囲は -> Vector3(-10,0,-10) ~ (10,0,10)　ややこしいになる
    //  |  x  |  x  |  x  |  x  |  x  |
    //  -------------------------------

    //  decorGrassの生成
    //  １つの配列51010の大きさを準備する(私の場合１Dの配列、長さは100)
    //  この配列はdecorGrassの座標のため
    //  2：中身は0～100のを入れて、中身の数値の場所をランダムする
    //  ３：decorGrassを生成する
    //  生成するの時そこで座標計算があります。
    //  decorGrassの最初置き場所は左下0，0、上のdecorと違い（上のdecorの座標は0，0ではない）

    //  Floor(House)について
    //  まだ何もない、ただ描画だけ

    //  プレイヤーのスポーン場所
    //  いつも上から真ん中
    //  例　mapX = 2, mapZ = 3;　
    //  プレイヤーは x = 1.5f , z= 2.8f;
    //  プレイヤーはスポーン前に周りのBushesやStumpを消す

    //----------------------------------------------------------------------------------------------
    #endregion

    //マップの大きさ・広さ
    [Header("Map Setting")]
    [SerializeField] int mapX;
    [SerializeField] int mapZ;

    [Header("Present Settings")]
    [SerializeField] int presentNum;        //本物
    [SerializeField] int presentHazureNum;   //外れ
    [SerializeField] int presentTotal;      //全部プレゼント数

    //各マスの飾り設定
    //decorGrassは最大数は100
    //decorFlower,decorBushes,decorStump,decorSmallRockの合計は最大数は25
    //decorBushの数はpresentTotalより多いに設定してください。
    [Header("Decoration Settings")]
    [SerializeField] GardenDecoration decorGrass;
    [SerializeField] GardenDecoration decorFlower;
    [SerializeField] GardenDecoration decorBushes;
    [SerializeField] GardenDecoration decorStump;
    [SerializeField] GardenDecoration decorSmallRock;

    //プレハブ設定
    [Header("Prefab Settings")]
    [SerializeField] GameObject prefFloorGrass;
    [SerializeField] GameObject prefFloorHouse;
    [SerializeField] GameObject prefPresent;
    [SerializeField] GameObject prefPresentHazure;
    [SerializeField] GameObject prefFence;
    [SerializeField] GameObject prefFencePost;
    [SerializeField] GameObject prefHouse;
    [SerializeField] GameObject prefChargingZone;


    //Floor（床）とHouse（家）の配列
    [Header("Floor Array")]
    GameObject[] floorGrassArr;
    GameObject floorHouse;

    //Floor（床）のサイズや最初座標
    [Header("Floor Size & First Coordinate")]
    const float floorGrassDistance = 20.0f;
    const float floorGrassSize = 10.0f;
    const float floorHouseSize = 5.0f;
    const float floorHouseSizeZ = 10.0f;
    float firstPosGrassX;
    float firstPosGrassZ;
    float firstPosHouseX;
    float firstPosHouseZ;

    //プレゼントやマップの親物体
    [Header("Game Object Parent")]
    [SerializeField] GameObject mapParent;
    [SerializeField] GameObject presentParent;
    [SerializeField] GameObject extraZoneParent;
    [SerializeField] GameObject floorGrassParent;
    [SerializeField] GameObject fenceParent;
    [SerializeField] GameObject coverZoneParent;

    //エクストラエリア
    [Header("Extra Zone")]
    const float extraZoneSize = 1.0f;
    const float extraZoneHalfSize = extraZoneSize * 0.5f;
    const float extraZoneDoubleSize = extraZoneSize * 2.0f;
    [SerializeField] GameObject extraZoneLeft;
    [SerializeField] GameObject extraZoneRight;
    [SerializeField] GameObject extraZoneTop;
    [SerializeField] GameObject extraZoneBottom;

    //見えない壁（プレイヤーは庭の中から出ないために）
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
        //Debugのため
        //Gizmos.DrawSphere(playerClearObjectLoc , playerClearObjectDistance);
    }

    #region Map Generator Rule & Map Generator Rule Explanation

    #region Map Generator Rule Explanation
    //----------------------------------------------------------------------------------------------
    //  「CheckRule（）」関数の説明
    //----------------------------------------------------------------------------------------------
    //  4の大事ルールがある。

    //  １つ目  
    //  マップの大きさ（mapX,mapY）1つにも値が０以下に設定禁止
    //  マップの大きさ　1ｘ1は禁止、でも1*2、2*1はＯＫ

    //  ２つ目
    //  草の最大数は100以上禁止

    //  ３つ目
    //  decorFlower + decorBushes + decorStump + decorSmallRock
    //  全最大数の合計が25以上は禁止

    //  4つ目
    //  presentTotal数は「全Floor(Grass)-1」　より多い
    //  恐らくプレイヤーがスポーン時Bushesとぶつかるので
    //  だから、１つのFloor(Grass)のBushesを計算に含めない
    //　全Floor(Grass)は　mapX * mapZ
    //----------------------------------------------------------------------------------------------
    #endregion
    bool CheckRule()
    {
        bool isAnyRuleViolated = false;
        if (mapX <= 0 || mapZ <= 0)
        {
            Debug.Log("Warning!! The map size is <=0（マップのサイズは　<=0）");
            Debug.Log("Please increase the map size(マップのサイズを大きくしてください。)");
            isAnyRuleViolated = true;
        }

        if (mapX == 1 && mapZ == 1)
        {
            Debug.Log("Warning!! The map size cannot 1x1（マップのサイズは1x1は禁止）");
            Debug.Log("Please increase the map size(マップのサイズを大きくしてください。)");
            isAnyRuleViolated = true;
        }

        if (decorGrass.GetDecorMaxNum() > 100)
        {
            Debug.Log("Warning!! The total of decoration grass is over 100（草の数は100以上です。）");
            Debug.Log("Please reduce the grass number(草の数を減らしてください。)");
            isAnyRuleViolated = true;
        }
        int totalDecor = decorFlower.GetDecorMaxNum() + decorBushes.GetDecorMaxNum() + decorSmallRock.GetDecorMaxNum() + decorStump.GetDecorMaxNum();
        if (totalDecor > 25)
        {
            Debug.Log("Warning!! The total of decoration flower,bushes,stump,　and small rock is over 25（草以外の飾り数は25以上です。）");
            Debug.Log("Please reduce the decoration max num（草以外の飾りの最大数を減らしてください。）");
            isAnyRuleViolated = true;
        }

        int bushMinNum = decorBushes.GetDecorMinNum() * ((mapX * mapZ) - 1);
        presentTotal = presentNum + presentHazureNum;
        if (presentTotal >= bushMinNum)
        {
            Debug.Log("Warning!! Bush Number is less than the total present number（Bush数は全プレゼント数より小さい）");
            Debug.Log("Please decrease present num or increase bush min num（全プレゼント数を減らすかbushの最低数を加えるをしてください。）");
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
                //右と左
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