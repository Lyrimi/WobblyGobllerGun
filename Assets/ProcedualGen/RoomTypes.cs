using UnityEngine;

public static class RoomTypes
{
    public enum RoomType
    {
        Base,
        Coridor,
        Circle,

    };

    public enum Direction : int
    {
        North = 0,
        East = 90,
        South = 180,
        West = 270,
    }
}
