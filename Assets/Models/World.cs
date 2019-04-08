//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections.Generic;
using System;

public class World {

    // A two-dimensional array to hold our tile data.
    Tile[,] tiles;

    Dictionary<string, Furniture> furniturePrototypes;

    // The tile width of the world.
    public int Width { get; protected set; }

    // The tile height of the world
    public int Height { get; protected set; }

    Action<Furniture> cbFurnitureCreated;
    Action<Tile> cbTileChanged;

    public JobQueue jobQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="World"/> class.
    /// </summary>
    /// <param name="width">Width in tiles.</param>
    /// <param name="height">Height in tiles.</param>
    public World(int width = 100, int height = 100) {
        Width = width;
        Height = height;

        jobQueue = new JobQueue();

        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
            }
        }

        Debug.Log("World created with " + (Width * Height) + " tiles.");
        CreateIsntalledObjectPrototypes();
    }

    void CreateIsntalledObjectPrototypes() {
        furniturePrototypes = new Dictionary<string, Furniture>();

        furniturePrototypes.Add("Wall",
            Furniture.CreatePrototype("Wall", 0, 1, 1, true)
        );
    }


    /// <summary>
    /// A function for testing out the system
    /// </summary>
    public void RandomizeTiles() {
        Debug.Log("RandomizeTiles");
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {

                if (UnityEngine.Random.Range(0, 2) == 0) {
                    tiles[x, y].Type = TileType.Empty;
                } else {
                    tiles[x, y].Type = TileType.Floor;
                }

            }
        }
    }

    /// <summary>
    /// Gets the tile data at x and y.
    /// </summary>
    /// <returns>The <see cref="Tile"/>.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public Tile GetTileAt(int x, int y) {
        if (x > Width || x < 0 || y > Height || y < 0) {
            Debug.LogError("Tile (" + x + "," + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    public bool PlaceFurniture(string objectType, Tile t) {
        if (furniturePrototypes == null) {
            Debug.LogError("FurniturePrototypes were not created yet!");
            return false;
        }
        if (!furniturePrototypes.ContainsKey(objectType)) {
            Debug.LogError("furniturePrototypes doesn't contain a proto for key: " + objectType);
            return false;
        }

        Furniture obj = Furniture.PlaceInstance(furniturePrototypes[objectType], t);

        if (obj == null) {
            return false;
        }

        cbFurnitureCreated?.Invoke(obj);
        return true;
    }

    public void RegisterFurnitureCreated(Action<Furniture> callbackFunc) {
        cbFurnitureCreated += callbackFunc;
    }

    public void UnregisterFurnitureCreated(Action<Furniture> callbackFunc) {
        cbFurnitureCreated -= callbackFunc;
    }

    public void RegisterTileChanged(Action<Tile> callbackFunc) {
        cbTileChanged += callbackFunc;
    }

    public void UnregisterTileChanged(Action<Tile> callbackFunc) {
        cbTileChanged -= callbackFunc;
    }

    void OnTileChanged(Tile t) {
        cbTileChanged?.Invoke(t);
    }

    public bool IsFurniturePlacementValid(string furnitureType, Tile t) {
        if (!furniturePrototypes.ContainsKey(furnitureType)) {
            Debug.LogError("IsFurniturePlacementValid -- invalid furniture type: " + furnitureType);
            return false;
        }
        return furniturePrototypes[furnitureType].IsValidPosition(t);
    }

    public Furniture GetFurniturePrototype(string objectType) {
        if (!furniturePrototypes.ContainsKey(objectType)) {
            Debug.LogError("GetFurniturePrototype -- there was no such furniture in the prototypes dict.");
            return null;
        }
        return furniturePrototypes[objectType];
    }
}
