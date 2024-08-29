using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimatableEntity
{
    float GetXSpeed();

    float GetYSpeed();

    float GetZSpeed();

    bool GetIsCrouching();

    bool GetIsRunning();

    bool GetIsGrounded();
}
