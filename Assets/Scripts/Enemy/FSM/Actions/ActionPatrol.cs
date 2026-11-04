using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPatrol : FSMAction
{
    [Header("Config")] 
    [SerializeField] private float speed;

    private Waypoint _waypoint;
    private int _pointIndex;
    private Vector3 _nextPosition;
    
    private void Awake()
    {
        _waypoint = GetComponent<Waypoint>();
    }

    public override void Act()
    {
        FollowPath();
    }

    private void FollowPath()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            GetCurrentPosition(), speed * Time.deltaTime);
        if (Vector3.Distance(transform.position,GetCurrentPosition()) <= 0.1f)
        {
            UpdateNextPosition();
        }
    }

    private void UpdateNextPosition()
    {
        _pointIndex++;
        if (_pointIndex > _waypoint.Points.Length - 1)
        {
            _pointIndex = 0;
        }
    }
    
    private Vector3 GetCurrentPosition()
    {
        return _waypoint.GetPosition(_pointIndex);
    }
}



