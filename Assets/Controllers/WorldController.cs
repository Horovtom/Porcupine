//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================


using UnityEngine;
using System.Collections.Generic;
using System;

public class WorldController : MonoBehaviour {
    public static WorldController Instance { get; protected set; }

    public Sprite floorSprite, emptySprite;

    // The world and tile data
    public World World { get; protected set; }

    Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<Furniture, GameObject> furnitureGameObjectMap;
    Dictionary<string, Sprite> furnitureSprites;

    // This runs before Start
    void OnEnable() {
        if (Instance != null) {
            Debug.LogError("There should never be two world controllers.");
        }
        Instance = this;

        // Create a world with Empty tiles
        World = new World();

        World.RegisterFurnitureCreated(OnFurnitureCreated);

        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();
        furnitureSprites = new Dictionary<string, Sprite>();
        PopulateFurnitureSpritesDictionary();

        // Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
        for (int x = 0; x < World.Width; x++) {
            for (int y = 0; y < World.Height; y++) {
                // Get the tile data
                Tile tile_data = World.GetTileAt(x, y);

                // This creates a new GameObject and adds it to our scene.
                GameObject tile_go = new GameObject();
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, true);

                tileGameObjectMap.Add(tile_data, tile_go);

                // Add a sprite renderer, but don't bother setting a sprite
                // because all the tiles are empty right now.
                // Add a default sprite for empty tiles.
                tile_go.AddComponent<SpriteRenderer>().sprite = emptySprite;
            }
        }

        World.RegisterTileChanged(OnTileChanged);

        // Center the Camera
        Camera.main.transform.position = new Vector3(World.Width / 2, World.Height / 2, Camera.main.transform.position.z);
    }

    void PopulateFurnitureSpritesDictionary() {
        if (furnitureSprites == null) furnitureSprites = new Dictionary<string, Sprite>();
        furnitureSprites.Clear();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furnitures");
        Debug.Log("Loaded resource: ");
        foreach (Sprite s in sprites) {
            Debug.Log(s);
            furnitureSprites[s.name] = s;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    // This function should be called automatically whenever a tile's type gets changed.
    void OnTileChanged(Tile tile) {
        if (!tileGameObjectMap.ContainsKey(tile)) {
            Debug.LogError("There was no GameObject registered for this Tile: " + tile);
            return;
        }
        GameObject tile_go = tileGameObjectMap[tile];
        if (tile.Type == TileType.Floor) {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        } else if (tile.Type == TileType.Empty) {
            tile_go.GetComponent<SpriteRenderer>().sprite = emptySprite;
        } else {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }
    }

    /// <summary>
    /// Gets the tile at the unity-space coordinates
    /// </summary>
    /// <returns>The tile at world coordinate.</returns>
    /// <param name="coord">Unity World-Space coordinates.</param>
    public Tile GetTileAtWorldCoord(Vector3 coord) {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return World.GetTileAt(x, y);
    }

    public void OnFurnitureCreated(Furniture obj) {
        // Create a visual GameObject linked to this data.
        // This creates a new GameObject and adds it to our scene.

        int x = obj.Tile.X;
        int y = obj.Tile.Y;
        GameObject obj_go = new GameObject();
        obj_go.name = obj.objectType + "_" + x + "_" + y;
        obj_go.transform.position = new Vector3(x, y, 0);
        obj_go.transform.SetParent(this.transform, true);

        furnitureGameObjectMap.Add(obj, obj_go);

        // Add a sprite renderer, but don't bother setting a sprite
        // because all the tiles are empty right now.
        obj_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(obj);

        // Use a lambda to create an anonymous function to "wrap" our callback function
        obj.RegisterOnChangedCallback(OnFurnitureChanged);
    }

    Sprite GetSpriteForFurniture(Furniture obj) {
        string spriteName = obj.objectType;
        if (obj.linksToNeighbour == false) {
            if (furnitureSprites.ContainsKey(spriteName) == false) {
                Debug.LogError("Sprite " + spriteName + " was not found!");
                return null;
            }
            return furnitureSprites[spriteName];
        }

        int x = obj.Tile.X;
        int y = obj.Tile.Y;

        spriteName += "_";

        //Check for neighbors North, East, South, West
        Tile t;
        t = World.GetTileAt(x, y + 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "N";
        }
        t = World.GetTileAt(x + 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "E";
        }
        t = World.GetTileAt(x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "S";
        }
        t = World.GetTileAt(x - 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "W";
        }

        if (furnitureSprites.ContainsKey(spriteName) == false) {
            Debug.LogError("Sprite " + spriteName + " was not found!");
            return null;
        }

        return furnitureSprites[spriteName];
    }

    void OnFurnitureChanged(Furniture furn) {


        // Make sure the furniture's graphics are correct.
        if (!furnitureGameObjectMap.ContainsKey(furn)) {
            Debug.LogError("OnFurnitureChanged -- Could not find gameObject for given furniture. ");
            return;
        }

        GameObject furn_go = furnitureGameObjectMap[furn];
        furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
    }

}
