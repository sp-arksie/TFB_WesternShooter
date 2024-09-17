using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2IntExtensions
{
    /// <summary>
    /// Returns a Vector2Int with value (-1, -1).
    /// </summary>
    public static Vector2Int Empty { get; } = new(-1, -1);
}
