using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour {
    bool buildModeIsObjects = false;
    string buildModeObjectType;
    TileType buildModeTile = TileType.Floor;

    // Use this for initialization
    void Start() {
    }

    public void SetMode_BuildFloor() {
        buildModeIsObjects = false;
        buildModeTile = TileType.Floor;
    }

    public void SetMode_Bulldoze() {
        buildModeIsObjects = false;
        buildModeTile = TileType.Empty;
    }

    public void SetMode_BuildFurniture(string objectType) {
        buildModeIsObjects = true;
        buildModeObjectType = objectType;
    }

    public void DoBuild(Tile t) {
        if (buildModeIsObjects) {
            string type = buildModeObjectType;

            // Can we even build the furniture in the selected tile?
            // Run the ValidPlacement function!
            if (!WorldController.Instance.world.IsFurniturePlacementValid(type, t) ||
                t.pendingFurnitureJob != null) {
                // This tile isn't valid for this furniture
                return;
            }

            Job j = new Job(t, (job) => {
                WorldController.Instance.world.PlaceFurniture(type, job.Tile);
                job.Tile.pendingFurnitureJob = null;
            });
            t.pendingFurnitureJob = j;
            j.RegisterJobCancelCallback((job) => { job.Tile.pendingFurnitureJob = null; });
            WorldController.Instance.world.jobQueue.Enqueue(j);
        } else {
            // We are in tile-changing mode.
            t.Type = buildModeTile;
        }
    }
}
