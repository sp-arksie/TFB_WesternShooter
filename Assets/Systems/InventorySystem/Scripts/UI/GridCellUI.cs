using UnityEngine;

public class GridCellUI : MonoBehaviour
{
    public Vector2Int cellPosition { get; private set; }

    public int Id { get; private set; } = -1;

    public void SetLocationInGrid(int x, int y)
    {
        cellPosition = new Vector2Int(x, y);
    }

    public void SetId(int id)
    {
        this.Id = id;
    }
}
