using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This bare-bones controller is mostly just going to piggyback on FurnitureSpriteController 
/// because we don't yet fully know what our job system is going to look like in the end
/// </summary>
public class JobSpriteController : MonoBehaviour {
    FurnitureSpriteController fsc;
    Dictionary<Job, GameObject> jobGameObjectMap;

    // Start is called before the first frame update
    void Start() {
        fsc = GameObject.FindObjectOfType<FurnitureSpriteController>();
        jobGameObjectMap = new Dictionary<Job, GameObject>();
        WorldController.Instance.world.jobQueue.RegisterJobCreationCallback(OnJobCreated);
    }

    void OnJobCreated(Job j) {


        GameObject job_go = new GameObject();

        jobGameObjectMap.Add(j, job_go);

        job_go.name = "JOB_" + j.jobObjectType + "_" + j.Tile.X + "_" + j.Tile.Y;
        job_go.transform.position = new Vector3(j.Tile.X, j.Tile.Y, 0);
        job_go.transform.SetParent(this.transform, true);

        SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>();
        sr.sprite = fsc.GetSpriteForFurniture(j.jobObjectType);
        sr.color = new Color(0.5f, 1f, 0.5f, 0.25f);

        j.RegisterJobCompleteCallback(OnJobEnded);
        j.RegisterJobCancelCallback(OnJobEnded);
    }

    /// <summary>
    /// This executes whether a job was Completed or Cancelled
    /// </summary>
    /// <param name="j"></param>
    void OnJobEnded(Job j) {
        GameObject job_go = jobGameObjectMap[j];

        j.UnregisterJobCancelCallback(OnJobEnded);
        j.UnregisterJobCompleteCallback(OnJobCreated);

        Destroy(job_go);
    }

    // Update is called once per frame
    void Update() {

    }
}
