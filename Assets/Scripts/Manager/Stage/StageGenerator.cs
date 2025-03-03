using System.Collections;
using System.Collections.Generic;
using TANK3.Controller;
using TANK3.Data.StageManagement;
using UnityEngine;

public class StageGenerator : Singleton<StageGenerator>
{
    GameObject player;
    public GameObject playerPrefab;
    public PlayerDesc playerDesc;   // 外部から設定.
    public Transform stageRoot;
    Camera MainCamera;
    [SerializeField] float scale = 3.0f;

    protected override void Awake()
    {
        base.Awake();

        // もしプレイヤーのプレハブが未設定なら、一度だけ Resources.Load してキャッシュ
        if (playerPrefab == null)
        {
            playerPrefab = Resources.Load<GameObject>("Player");
            if (playerPrefab == null)
            {
                Debug.LogError("Player prefab が Resources フォルダに見つかりませんでした。");
            }
        }
    }

    public void Generate(StageData sceneData)
    {
        ClearStage();
        GameObject root = new ("StageRoot");
        stageRoot = root.transform;
        Debug.Log($"Generate {sceneData.stageName}");
        Vector3 stageCenter = new Vector3(sceneData.stageWidth/2, 0, sceneData.stageHeight/2);
        GameObject surroundings = new ("Surroundings");
        surroundings.transform.position = Vector3.zero;
        CreateFloor(stageCenter, sceneData).transform.parent = surroundings.transform;
        var tileMap = sceneData.stageDesc;
        int enemyCount = 0;

        for (int x = 0; x < sceneData.stageWidth; x++)
        {
            for (int y = 0; y < sceneData.stageHeight; y++)
            {
                if (At(tileMap, x, y, sceneData.stageWidth, out var type))
                {
                    Vector3 newPos = new Vector3(x, 0.5f, y) - stageCenter;
                    Vector3 offset = new Vector3(0, 12.0f, 0);
                    switch (type)
                    {
                        case TileType.Obstacle:
                            if(At(sceneData.obstacleTiles, x , y, sceneData.stageWidth, out var item))
                            {
                                GameObject newObjectPrefab = item.obstacleData.GameObjectPrefab;
                                if(newObjectPrefab != null)
                                {
                                    var createdObject = Instantiate(newObjectPrefab, newPos, Quaternion.identity);
                                    createdObject.transform.parent = surroundings.transform;
                                }
                            }
                            break;

                        case TileType.Player:
                            StartCoroutine(LoadPlayerObject(newPos * scale + offset, surroundings));
                            break;

                        case TileType.Enemy:
                            if(At(sceneData.enemyTiles, x, y, sceneData.stageWidth, out var enemy))
                            {
                                GameObject newObjectPrefab = enemy.enemyData.GameObjectPrefab;
                                if (newObjectPrefab != null)
                                {
                                    var createdEnemy = Instantiate(newObjectPrefab, newPos * scale + offset, Quaternion.Euler(0,enemy.rotation, 0));
                                    Instance.StartCoroutine(DisableComponentTemporarily(createdEnemy.GetComponent<TankController>(), 1.0f));
                                    createdEnemy.transform.parent = stageRoot;
                                    ++enemyCount;
                                }
                            }
                            break;

                        case TileType.Empty:
                            break;
                        default:
                            break;
                    }
                }    
            }
        }
        surroundings.transform.localScale = Vector3.one * scale;
        surroundings.transform.parent = stageRoot;

        if (sceneData.enemyCount != enemyCount) Debug.LogWarning("StageGerator: 設定された敵の数と生成された敵の数が合いません.");
        EnemyManager.Instance.totalEnemies = enemyCount;
        EnemyManager.Instance.Initialize();
        EnemyManager.Instance.stageData = sceneData;
    }

    bool At<T>(List<T> list, int x, int y, int width, out T item)
    {
        int index = y * width + x;
        if (index >= list.Count || index < 0)
        {
            item = list[0];
            return false;
        }
        else
        {
            item = list[index];
            return true;
        }
    }

    GameObject CreateFloor(Vector3 center, StageData sceneData)
    {
        // 親オブジェクトを作成してColliderを追加
        GameObject floorParent = new GameObject("FloorParent");
        BoxCollider parentCollider = floorParent.AddComponent<BoxCollider>();

        // 親オブジェクトの位置を設定（中心に合わせる）
        floorParent.transform.position = Vector3.zero;

        // 親オブジェクトのサイズをステージサイズに合わせる
        parentCollider.size = new Vector3(sceneData.stageWidth, 0.1f, sceneData.stageHeight);

        // 個々の床ブロックを生成（Colliderは付けない）
        for (int x = 0; x < sceneData.stageWidth; x++)
        {
            for (int y = 0; y < sceneData.stageHeight; y++)
            {
                // ポジションを計算
                Vector3 position = new Vector3(x, -0.05f, y) - center;

                // 各床ブロックを生成
                GameObject floorBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floorBlock.transform.position = position;
                floorBlock.transform.localScale = new Vector3(1, 0.1f, 1);

                // 生成したブロックのColliderを削除
                Collider blockCollider = floorBlock.GetComponent<Collider>();
                if (blockCollider != null)
                {
                    Destroy(blockCollider);  // Colliderを削除
                }

                // 親オブジェクトの子として追加
                floorBlock.transform.SetParent(floorParent.transform);
            }
        }

        return floorParent;
    }

    IEnumerator LoadPlayerObject(Vector3 position, GameObject Parent)
    {
        float rotation = 0.0f;
        if (playerPrefab != null)
        {
            player = Instantiate(playerPrefab, position, Quaternion.Euler(0, rotation, 0));
            Instance.StartCoroutine(DisableComponentTemporarily(player.GetComponent<TankController>(), 1.0f));
            player.transform.parent = stageRoot;
            Vector3 offset = new Vector3(0.0f, 3.0f, -10.0f);
            MainCamera = FindObjectOfType<Camera>();
            MainCamera.transform.Translate(player.transform.position + offset);
            player.GetComponent<PlayerCannonController>().MainCamera = MainCamera;
            CameraFollow cameraFollow = MainCamera.gameObject.AddComponent<CameraFollow>();
            cameraFollow.target = player.transform;
        }
        else
        {
            Debug.LogError("Player prefab がキャッシュされていません。");
        }
        yield return null;
    }

    public void ClearStage()
    {
        if (stageRoot != null)
        {
            // 親オブジェクトを破棄すると、その子も自動的に破棄される
            Destroy(stageRoot.gameObject);
            stageRoot = null;
        }
    }

    private IEnumerator DisableComponentTemporarily(MonoBehaviour component, float duration)
    {
        // Update を無効化
        component.enabled = false;
        yield return new WaitForSeconds(duration);
        // Update を再度有効化
        component.enabled = true;
    }
}
