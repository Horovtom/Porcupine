using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// This class holds info for a queued up job, which can include things like 
/// placing furniture, moving stored inventory, working at a desk, and maybe 
/// even fighting enemies.
/// </summary>
public class Job {
    // Tile reference on which the job is to be done.
    public Tile Tile { get; protected set; }
    // Time to complete the job
    float jobTime = 1f;

    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;

    public Job(Tile tile, Action<Job> cbJobComplete, float jobTime = 1f) {
        this.Tile = tile;
        this.jobTime = jobTime;
        this.cbJobComplete += cbJobComplete;
    }

    public void RegisterJobCompleteCallback(Action<Job> cb) {
        cbJobComplete += cb;
    }

    public void RegisterJobCancelCallback(Action<Job> cb) {
        cbJobCancel += cb;
    }

    public void DoWork(float workTime) {
        jobTime -= workTime;
        if (jobTime <= 0) {
            CompleteJob();
        }
    }

    void CompleteJob() {
        cbJobComplete?.Invoke(this);
    }

    public void Canceljob() {
        cbJobCancel?.Invoke(this);
    }
}

