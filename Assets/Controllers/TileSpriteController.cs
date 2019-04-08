//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================


using UnityEngine;
using System.Collections.Generic;
using System;

public class TileSpriteController : MonoBehaviour {

    public Sprite floorSprite, emptySprite;


    Dictionary<Tile, GameObject> tileGameObjectMap;

    World world { get { return WorldController.Instance.world; } }

    // This runs before Start
    void Start() {
        tileGameObjectMap = new Dictionary<Tile, GameObject>();

        // Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
        for (int x = 0; x < world.Width; x++) {
            for (int y = 0; y < world.Height; y++) {
                // Get the tile data
                Tile tile_data = world.GetTileAt(x, y);

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

        world.RegisterTileChanged(OnTileChanged);
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
}
