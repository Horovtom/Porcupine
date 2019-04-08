using System.Collections;
using System.Collections.Generic;
using System;

public class JobQueue {

    public Queue<Job> jobQueue;
    Action<Job> cbJobCreated;


    public JobQueue() {
        jobQueue = new Queue<Job>();
    }

    public void Enqueue(Job j) {
        jobQueue.Enqueue(j);

        cbJobCreated?.Invoke(j);
    }

    public void RegisterJobCreationCallback(Action<Job> cb) {
        cbJobCreated += cb;
    }

    public void UnregisterJobCreationCallback(Action<Job> cb) {
        cbJobCreated -= cb;
    }
}
