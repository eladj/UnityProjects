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
	Player,
	Enemy
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
    public List<TileObj> tilesTypes = new List<TileObj>(6);

	private TileType[,] map;
	private Dictionary<TileType, Tile> tilesDict;
	private Vector2Int playerPos;
	private HashSet<Vector2Int> trail = new HashSet<Vector2Int>();  // Holds the trail of the player in the field

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
		// trail = new HashSet<Vector2>();
		
		Debug.Log("GameManager: Initialization finished.");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space")){
            Fill(new Vector2Int(2, 2));
		}
	}

	public void UpdatePlayerPosition(Vector2Int position){
		if (position.x < 0 || position.x > mapSizeX || position.y < 0 || position.y > mapSizeY){
			Debug.LogError("UpdatePlayerPosition: Invalid position: " + position.ToString());
		}
		playerPos = position;

		if (!IsBorder(position)){
			// Player is on the field
			AddToTrail(position);
		} else {
			if (trail.Count > 0){
				// Player was on the field and now back to border

				// TODO: Close the area according to the enemies
			}
		}
		// SetTileAtPosition(position, TileType.Player);	
	}

	// Sets Tile at TileMap according to tile type
	void SetTileAtPosition(Vector2Int position, TileType type){
		if (position.x < 0 || position.x > mapSizeX || position.y < 0 || position.y > mapSizeY){
			Debug.LogError("UpdateTileAtPosition: Invalid position: " + position.ToString());
		}
		map[position.x, position.y] = type;
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

	public bool IsBorder(Vector2Int position) {
		return map[position.x, position.y] == TileType.Border;
	}

	public void AddToTrail(Vector2Int position){
		bool new_item = trail.Add(position);
		if (new_item){
			SetTileAtPosition(position, TileType.Trail);
		}
	}

	// Flood fill tiles map
	public void Fill(Vector2Int position){
		// tilemap.FloodFill(new Vector3Int(position.x, position.y, 0), tilesDict[TileType.Filled]);
		if (position.x <= 0 || position.x >= mapSizeX || position.y <= 0 || position.y >= mapSizeY){
			Debug.LogError("FloodFill: Invalid position: " + position.ToString());
		}
		if (map[position.x, position.y] != TileType.Empty) return;

		// Define valid tile types we want to fill
		List<TileType> validTiles = new List<TileType>{TileType.Empty};

		// TileType curTileType = map[position.x, position.y];
		List<Vector2Int> queue = new List<Vector2Int>();
		queue.Add(position);
		while (queue.Count > 0){
			int num_elements = queue.Count;
			for (int n = 0; n < num_elements; n++){
				int y = queue[n].y;
				int west = queue[n].x;
				int east = queue[n].x;
				while (validTiles.Contains(map[west, y])){
					west--;
				}
				west++;
				while (validTiles.Contains(map[east, y])){
					east++;
				}
				east--;
				for (int i = west; i <= east; i++){
					// Change tile
					SetTileAtPosition(new Vector2Int(i, y), TileType.Filled);

					// Check color above and below and add to queue
					if (validTiles.Contains(map[i, y+1])){
						queue.Add(new Vector2Int(i, y+1));
					}
					if (validTiles.Contains(map[i, y-1])){
						queue.Add(new Vector2Int(i, y-1));
					}
				}
			}
			// Remove all previous positions
			queue.RemoveRange(0, num_elements);
		}

		foreach (var t in trail){
			SetTileAtPosition(t, TileType.Filled);
		}
		trail.Clear();
	}
}
