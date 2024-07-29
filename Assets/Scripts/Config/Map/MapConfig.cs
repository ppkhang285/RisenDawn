using UnityEngine;

[CreateAssetMenu(menuName ="Config/Map/MapConfig")]
public class MapConfig : ScriptableObject
{
    public Vector2 roomWidthRange;
    public Vector2 roomHeightRange;
    // roomNumber will be moved to LevelConfig
    public int roomNumber;
}
