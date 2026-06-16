using System;
using UnityEngine;

public interface IPlatform
{
    bool IsActivated { get; }
    Vector3 CurrentVelocity { get; }

    void Move();
    void ResetPlatform();
}
