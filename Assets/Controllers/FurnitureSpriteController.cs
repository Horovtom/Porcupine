//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================


using UnityEngine;
using System.Collections.Generic;
using System;

public class FurnitureSpriteController : MonoBehaviour {
    Dictionary<Furniture, GameObject> furnitureGameObjectMap;
    Dictionary<string, Sprite> furnitureSprites;

    World world { get { return WorldController.Instance.world; } }

    // This runs before Start
    void Start() {
        LoadSprites();

        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        world.RegisterFurnitureCreated(OnFurnitureCreated);
    }

    void LoadSprites() {
        furnitureSprites = new Dictionary<string, Sprite>();
        furnitureSprites.Clear();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furnitures");
        Debug.Log("Loaded resource: ");
        foreach (Sprite s in sprites) {
            Debug.Log(s);
            furnitureSprites[s.name] = s;
        }
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

    void OnFurnitureChanged(Furniture furn) {
        // Make sure the furniture's graphics are correct.
        if (!furnitureGameObjectMap.ContainsKey(furn)) {
            Debug.LogError("OnFurnitureChanged -- Could not find gameObject for given furniture. ");
            return;
        }

        GameObject furn_go = furnitureGameObjectMap[furn];
        furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
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
        t = world.GetTileAt(x, y + 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "N";
        }
        t = world.GetTileAt(x + 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "E";
        }
        t = world.GetTileAt(x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "S";
        }
        t = world.GetTileAt(x - 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            spriteName += "W";
        }

        if (furnitureSprites.ContainsKey(spriteName) == false) {
            Debug.LogError("Sprite " + spriteName + " was not found!");
            return null;
        }

        return furnitureSprites[spriteName];
    }
}
