//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using UnityEngine;
using System.Collections;
using System;

public class Furniture {
    /// <summary>
    /// This represents the BASE tile of the object -- but in practice, large objects may actually occupy multiple tiles.
    /// </summary>
    public Tile tile { get; protected set; }

    /// <summary>
    /// This "objectType" will be queried by the visual system to know what sprite to render for this object.
    /// </summary>
    public string objectType { get; protected set; } 

    /// <summary>
    /// This is a multiplier, so a value of "2" here, means you move twice as slowly (i.e. at half speed)
    /// Tile types and other environmental effect may be combined.
    /// For example, a "rough" tile (cost of 2) with a table (cost of 3) that is on fire (cost of 3)
    /// would have a total movement cost of (2 + 3 + 3 = 8), so you'd move through this tile at 1/8th normal speed
    /// SPECIAL: If movementCost = 0, then this tile is impassable. (e.g. a wall).
    /// </summary>
    float movementCost;

    // For example, a sofa might be 3x2 (actual graphics only appear to cover the 3x1 area, but the extra row is for leg room.
    int width;
    int height;

    public bool linksToNeighbour { get; protected set; }

    Action<Furniture> cbOnChanged;
    Func<Tile, bool> funcPositionValidation;

    protected Furniture() { }

    static public Furniture CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false) {
        Furniture obj = new Furniture();
        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbour = linksToNeighbour;

        obj.funcPositionValidation = obj.IsValidPosition;
        return obj;
    }

    static public Furniture PlaceInstance(Furniture proto, Tile tile) {
        if (proto.funcPositionValidation(tile) == false) {
            Debug.LogError("PlaceInstance -- Position validity function returned False.");
            return null;
        }
        Furniture obj = new Furniture();
        obj.objectType = proto.objectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.linksToNeighbour = proto.linksToNeighbour;

        obj.tile = tile;

        if (!tile.PlaceFurniture(obj)) {
            // For some reason, we weren't able to place our object in this tile.
            // (Probably it was already occupied.)

            // Do NOT return our newly instantiated object.
            // (It will be garbage collected.)
            return null;
        }

        if (obj.linksToNeighbour) {
            // This type of furniture links itself to its neighbours, we have to notify them.
            triggerNeighbours(obj, tile);
        }

        return obj;
    }

    static void triggerNeighbours(Furniture obj, Tile tile) {
        int x = tile.X;
        int y = tile.Y;
        Tile t;
        t = tile.world.GetTileAt(x, y + 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            t.furniture.cbOnChanged(t.furniture);
        }

        t = tile.world.GetTileAt(x+1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            t.furniture.cbOnChanged(t.furniture);
        }

        t = tile.world.GetTileAt(x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            t.furniture.cbOnChanged(t.furniture);
        }

        t = tile.world.GetTileAt(x-1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
            t.furniture.cbOnChanged(t.furniture);
        }

    }

    public void RegisterOnChangedCallback(Action<Furniture> callbackFunc) {
        cbOnChanged += callbackFunc;
    }

    public void UnregisterOnChangedCallback(Action<Furniture> callbackFunc) {
        cbOnChanged -= callbackFunc;
    }

    public bool IsValidPosition(Tile t) {
        if (t.Type != TileType.Floor) return false;
        if (t.furniture != null) return false;

        return true;
    }

    public bool IsValidPosition_Door(Tile t) {
        // Make sure we have a pair of E/W walls or N/S walls
        if (IsValidPosition(t) == false) return false;
        return true;
    }
}   
