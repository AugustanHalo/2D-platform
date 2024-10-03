using UnityEngine;
using CodeMonkey.Utils;
using System.IO;
using System.Collections.Generic;

public class Testing : MonoBehaviour
{
    private Grid grid;

    [Header("Map")]
    [SerializeField] int width = 24;
    [SerializeField] int height = 15;
    [SerializeField] float cellSize = 10f;
    [SerializeField] int horizontalSegmentLength;
    [SerializeField] int verticalSegmentLength;
    [SerializeField] Sprite[] tileSprites;


    [Header("Enemy")]
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Path")]
    [SerializeField] Sprite[] pathSprites;

    [Header("Economy")]
    [SerializeField] float money = 100f;
    [SerializeField] float towerCost = 50f;

    [Header("UI")]
    [SerializeField] TowerDefenseOverlay towerDefenseOverlay;

    [Header("Health")]
    [SerializeField] float health = 100f;

    private List<Vector3[]> listPathPositions = new List<Vector3[]>();
    private Vector3[] pathPositions;

    private void Start()
    {

        grid = new Grid(width, height, cellSize, new Vector3(-120, -70, 0));

        //CreateMesh(width, height, cellSize);
        if(pathPositions == null)
        {
            GeneratePathPositions();
        }

        CreatePathSprite();
        CreateGridSprite();
       
        towerDefenseOverlay.SetMoneyAmmount((int)money);
        towerDefenseOverlay.SetHealthAmmount((int)health);

        Debug.Log($"Path generated with {pathPositions.Length} positions");
        
        //Inialize the enemy spawner
        InitializeEnemySpawner();
    }
    private void InitializeEnemySpawner()
    {
        if (enemySpawner == null)
        {
            enemySpawner = gameObject.AddComponent<EnemySpawner>();
        }
        System.Random random = new System.Random();
        enemySpawner.Initialize(listPathPositions);
    }

    private void CreatePathSprite()
    {
        if (pathSprites == null || pathSprites.Length == 0)
        {
            Debug.LogError("No tile sprites provided!");
            return;
        }

        GameObject pathSprite = new GameObject("PathSprite");
        pathSprite.transform.position = new Vector3(-115, -65, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid.GetValue(x, y) == 1)  // Only create sprites where path is draw
                {
                    GameObject pathObject = new GameObject($"Path_{x}_{y}");
                    pathObject.transform.SetParent(pathSprite.transform);
                    pathObject.transform.localPosition = new Vector3(x * cellSize, y * cellSize, 0);

                    SpriteRenderer spriteRenderer = pathObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = pathSprites[Random.Range(0, pathSprites.Length)];
                    spriteRenderer.sortingLayerName = "Foreground";
                    spriteRenderer.sortingOrder = -1;

                    // Adjust the scale to fit the cell size
                    float scaleX = cellSize / spriteRenderer.sprite.bounds.size.x;
                    float scaleY = cellSize / spriteRenderer.sprite.bounds.size.y;
                    pathObject.transform.localScale = new Vector3(scaleX, scaleY, 1);
                }
            }
        }

        // Set the grid object's scale
        pathSprite.transform.localScale = Vector3.one;
    }

    private void CreateGridSprite()
    {
        if (tileSprites == null || tileSprites.Length == 0)
        {
            Debug.LogError("No tile sprites provided!");
            return;
        }

        GameObject gridObject = new GameObject("GridSprite");
        gridObject.transform.position = new Vector3(- 115, - 65, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid.GetValue(x, y) == 0)  // Only create sprites where towers can be placed
                {
                    GameObject tileObject = new GameObject($"Tile_{x}_{y}");
                    tileObject.transform.SetParent(gridObject.transform);
                    tileObject.transform.localPosition = new Vector3(x * cellSize, y * cellSize, 0);

                    SpriteRenderer spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = tileSprites[Random.Range(0, tileSprites.Length)];
                    spriteRenderer.sortingLayerName = "Foreground";
                    spriteRenderer.sortingOrder = -1;

                    // Adjust the scale to fit the cell size
                    float scaleX = cellSize / spriteRenderer.sprite.bounds.size.x;
                    float scaleY = cellSize / spriteRenderer.sprite.bounds.size.y;
                    tileObject.transform.localScale = new Vector3(scaleX, scaleY, 1);
                }
            }
        }

        // Set the grid object's scale
        gridObject.transform.localScale = Vector3.one;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnTower("Fire");
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            SpawnTower("Ice");
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            Kitchen.Create(transform.position + new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), 1) );
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            SpawnTower("Wizard");
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            SpawnTower("Barrack");
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 position = UtilsClass.GetMouseWorldPosition();
            Debug.Log(grid.GetValue(position));
        }
    }
    void GenerateRandomPath(out int[,] path, int height, int width)
    {
        path = new int[width, height];
        System.Random random = new System.Random();

        List<Vector3> positions = new List<Vector3>();
        int x = 0;
        int y = 0;
        int direction = 0; // 0 = right, 1 = up, 2 = down

        int maxIterations = 1000;
        int iterations = 0;

        for (int a = 1;a<= verticalSegmentLength;a++)
        {
            y = random.Next((height / verticalSegmentLength) * (a - 1), (height / verticalSegmentLength) * a - 1);
            if (x == 0)
            {
                path[x, y] = 1;
                positions.Add(grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0));
                Debug.DrawLine(grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0), grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0) + Vector3.right * cellSize, Color.red, 100f);
            }

            while (x < width - 1 && iterations < maxIterations)
            {
                // Move in the current direction for the segment length
                for (int i = 0; i < horizontalSegmentLength && x < width - 1; i++)
                {
                    x++;
                    path[x, y] = 1;
                    positions.Add(grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0));
                    if(i<horizontalSegmentLength-1)
                    Debug.DrawLine(grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0), grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0) + Vector3.right * cellSize, Color.red, 100f);
                }
                // Change direction after each segment
                if (x < width - 1)
                {
                    if (y <= 1) // Too close to top, force downward
                        direction = 1;
                    else if (y >= height - 2) // Too close to bottom, force upward
                        direction = 2;
                    else
                        direction = random.Next(1, 3); // If moving right, choose up or down; otherwise, move right

                    switch (direction)
                    {
                        case 0: // Move right
                            x++;
                            //DrawLine(grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0), grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0) + Vector3.right * cellSize, Color.red, 100f);
                            break;
                        case 1: // Move up
                            int temp =y;
                            y = Mathf.Min(y + random.Next(0, 3), height - 1);
                            for(int b = temp ; b<=y;b++)
                            {
                                path[x, b] = 1;
                                positions.Add(grid.GetWorldPosition(x, b) + new Vector3(cellSize / 2, cellSize / 2, 0));
                                if(b<y)
                                Debug.DrawLine(grid.GetWorldPosition(x, b) + new Vector3(cellSize / 2, cellSize / 2, 0), grid.GetWorldPosition(x, b) + new Vector3(cellSize / 2, cellSize / 2, 0) + Vector3.up * cellSize, Color.red, 100f);
                            }
                            break;
                        case 2: // Move down
                            temp = y;
                            y = Mathf.Max(y - random.Next(0, 3), 0);
                            for(int b = temp ; b>=y;b--)
                            {
                                path[x, b] = 1;
                                positions.Add(grid.GetWorldPosition(x, b) + new Vector3(cellSize / 2, cellSize / 2, 0));
                                if(b>y)
                                Debug.DrawLine(grid.GetWorldPosition(x, b) + new Vector3(cellSize / 2, cellSize / 2, 0), grid.GetWorldPosition(x, b) + new Vector3(cellSize / 2, cellSize / 2, 0) + Vector3.down * cellSize, Color.red, 100f);
                            }
                            break;
                    }
                    path[x, y] = 1;
                    positions.Add(grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0));
                    Debug.DrawLine(grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0), grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0) + Vector3.right * cellSize, Color.red, 100f);
                }
                iterations++;
            }
            // Ensure the path reaches the right edge
            path[width-1, y] = 1;
            for (int i = 0; i < positions.Count; i++)
            {
                Vector3 pos = positions[i];
                int gridX = Mathf.FloorToInt((pos.x + 120) / cellSize);
                int gridY = Mathf.FloorToInt((pos.y + 70) / cellSize);
                grid.SetValue(gridX, gridY, 1);
            }
            positions.Add(grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0));
            x = -1;
            pathPositions = positions.ToArray();
            listPathPositions.Add(pathPositions);
            positions.Clear();
        }

    }
    void GeneratePathPositions()
    {
        int[,] pathGrid;
        GenerateRandomPath(out pathGrid, height, width);
        //CreateMesh(width, height, cellSize, pathGrid);

    }



    public float Money
    {
        get { return money; }
        set
        {
            money = value;
            towerDefenseOverlay.SetMoneyAmmount((int)money);
        }
    }

    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            towerDefenseOverlay.SetHealthAmmount((int)health);
            if (health <= 0)
            {
                Debug.Log("Game Over");
                FindAnyObjectByType<UIManager>().GameOver();
            }
        }
    }


    void SpawnTower(string _towerName)
    {
        int x, y;
        grid.GetXY(UtilsClass.GetMouseWorldPosition(),out x,out y);

        if(x>=0 && y>=0 && x<width && y<height && money >= towerCost)
        {
            if(grid.GetValue(x,y) == 0)
            {
                grid.SetValue(x, y, 1);
                Vector3 spawnPosition = grid.GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2);
                Instantiate(GameAssets.GetGetTowerPrefab(_towerName), spawnPosition, Quaternion.identity);
                money -= towerCost;
                towerDefenseOverlay.SetMoneyAmmount((int)money);
            }
        }
    }

    public Grid GetGrid()
    {
        return grid;
    }

    public float GetCellSize()
    {
        return cellSize;
    }   


    #region Mesh Generation
    private void CreateMesh(int width, int height, float cellSize, int[,] path)
    {
        Mesh mesh = new Mesh();
        //Count the number of cells to be drawn
        int cellCount = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (path[y, x] == 1)
                {
                    cellCount++;
                }
            }
        }

        // Initialize arrays with the correct size
        Vector3[] vertices = new Vector3[cellCount * 4];
        Vector2[] uv = new Vector2[cellCount * 4];
        int[] triangles = new int[cellCount * 6];

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (path[y, x] == 1)
                {
                    // Add vertices for this cell
                    vertices[vertexIndex] = new Vector3(cellSize * x - 120, cellSize * y - 70);
                    vertices[vertexIndex + 1] = new Vector3(cellSize * x - 120, cellSize * (y + 1) - 70);
                    vertices[vertexIndex + 2] = new Vector3(cellSize * (x + 1) - 120, cellSize * (y + 1) - 70);
                    vertices[vertexIndex + 3] = new Vector3(cellSize * (x + 1) - 120, cellSize * y - 70);


                    // Add UVs
                    uv[vertexIndex] = new Vector2(0, 0);
                    uv[vertexIndex + 1] = new Vector2(0, 1);
                    uv[vertexIndex + 2] = new Vector2(1, 1);
                    uv[vertexIndex + 3] = new Vector2(1, 0);

                    // Add triangles
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + 1;
                    triangles[triangleIndex + 2] = vertexIndex + 2;
                    triangles[triangleIndex + 3] = vertexIndex;
                    triangles[triangleIndex + 4] = vertexIndex + 2;
                    triangles[triangleIndex + 5] = vertexIndex + 3;

                    vertexIndex += 4;
                    triangleIndex += 6;
                }
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        // Set the path mesh to render before other objects
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingLayerName = "Background";
            meshRenderer.sortingOrder = -1;

            Debug.Log(meshRenderer.sortingLayerName + " " + meshRenderer.sortingOrder);
        }

        GetComponent<MeshFilter>().mesh = mesh;


    }

    #endregion
}
