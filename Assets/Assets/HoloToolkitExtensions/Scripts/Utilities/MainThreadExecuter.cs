using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainThreadExecuter : MonoBehaviour
{

    private Queue<Action> _executionQueue = new Queue<Action>();

    public void Add(Action action)
    {
        _executionQueue.Enqueue(action);
    }

    void Update()
    {
        if (_executionQueue.Any())
        {
            var action = _executionQueue.Dequeue();
            if (action != null)
            {
                action();
            }
        }
    }
}