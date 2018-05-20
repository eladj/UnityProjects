using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public enum TileType{
    Empty,
    Border,
    Filled,
    Trail,
	Player
}

// Holds both TileType and and Tile object (Sprite).
// Only used to expose this in the Inspector in a convenient way.
[System.Serializable]
public class TileObj {
	public TileType type;
	public Tile tile;
}

public class GameManager : MonoBehaviour {

	public int mapSizeX;
	public int mapSizeY;
	public Tilemap tilemap;	
    public List<TileObj> tilesTypes = new List<TileObj>(5);

	private TileType[,] map;
	private Dictionary<TileType, Tile> tilesDict;
	private Vector2Int playerPos;
	private List<Vector2> trail;

    // Use this for initialization
    void Start () {
		Debug.Log("GameManager: Start initialization...");
		// Create tiles dictionary for easy mapping between Tile object to TileType enumeration
		tilesDict = new Dictionary<TileType, Tile>();
		for (int n = 0; n < tilesTypes.Count; n++){
			tilesDict.Add(tilesTypes[n].type, tilesTypes[n].tile);
		}

		// Initialize our tiles map (not the TileMap GameObject)
		map = new TileType[mapSizeX, mapSizeY];
		InitTilesMap();

		// Set initial player position
		UpdatePlayerPosition(new Vector2Int(0, 0));

		// Initialize general stuff
		trail = new List<Vector2>();
		
		Debug.Log("GameManager: Initialization finished.");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void UpdatePlayerPosition(Vector2Int position){
		if (position.x < 0 || position.x > mapSizeX || position.y < 0 || position.y > mapSizeY){
			Debug.LogError("UpdatePlayerPosition: Invalid position: " + position.ToString());
		}
		playerPos = position;
		map[position.x, position.y] = TileType.Player;
		SetTileAtPosition(position, TileType.Player);	
	}

	// Sets Tile at TileMap according to tile type
	void SetTileAtPosition(Vector2Int position, TileType type){
		if (position.x < 0 || position.x > mapSizeX || position.y < 0 || position.y > mapSizeY){
			Debug.LogError("UpdateTileAtPosition: Invalid position: " + position.ToString());
		}
		tilemap.SetTile(new Vector3Int(position.x, position.y, 0), tilesDict[type]);
	}

	void InitTilesMap(){
		long n = 0;
		for (int y = 0; y < mapSizeY ; y++) {
        	for (int x = 0; x < mapSizeX ; x++) {
				if (x == 0 || x == (mapSizeX - 1) || y == 0 || y == (mapSizeY - 1)){
					map[x, y] = TileType.Border;
				} else {
					map[x, y] = TileType.Empty;
				}	
				n++;
			}
        }
		MapToTileMap();
	}

	// Update the TileMap from our map
	 void MapToTileMap(){
		TileBase[] tileArray = new TileBase[mapSizeX * mapSizeY];
		long n = 0;
		for (int y = 0; y < mapSizeY ; y++) {
        	for (int x = 0; x < mapSizeX ; x++) {
				tileArray[n] = tilesDict[map[x, y]];
				n++;
			}
        }
		BoundsInt area = new BoundsInt(0, 0, 0, mapSizeX, mapSizeY, 1);
 		tilemap.SetTilesBlock(area, tileArray);		
	}

	public bool IsBorder(int x, int y) {
		return map[x, y] == TileType.Border;
	}

	public void AddToTrail(Vector2Int position){
		trail.Add(position);
		SetTileAtPosition(position, TileType.Trail);
	}

}
