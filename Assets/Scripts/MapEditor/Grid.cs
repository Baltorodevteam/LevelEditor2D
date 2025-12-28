 using UnityEngine;
 
public class Grid : MonoBehaviour
{
    public static Grid Instance = null;

    public Texture gridTexture;

    public static int gridWidth = 10;
    public static int gridHeight = 10;
    public static float gridDepth = 0.05f;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //GenerateGrid(gridWidth, gridHeight);
    }

    private void Update()
    {
    }
    
    public void GenerateGrid(RoomData rd)
    {
        GameObject go;
        Renderer r;

        ClearGrid();
        //LayersPanel.Instance.GetRoomData().ClearLayers();

        gridWidth = (int)rd.GetRoomWidth();
        gridHeight = (int)rd.GetRoomHeight();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.transform.parent = this.transform;
                r = go.GetComponent<Renderer>();
                r.material.mainTexture = gridTexture;
                r.name = "Tile_" + x + "x" + y;
                go.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
                go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                go.transform.localPosition = new Vector3(x, y, gridDepth);

                Collider col = go.GetComponent<Collider>();
                if(col)
                {
                    col.enabled = false;
                }
            }
        }

        if(rd == null)
        {
            transform.position = Vector3.zero;
        }
        else
        {
            transform.position = new Vector3((float)rd.GetRoomGridX() * RoomData.defaultRoomWidth, (float)rd.GetRoomGridY() * RoomData.defaultRoomHeight, 0.0f);
        }
    }

    public void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}