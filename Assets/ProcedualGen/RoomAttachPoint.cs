using UnityEngine;
using static RoomTypes;

public class RoomAttachPoint : MonoBehaviour
{
    public RoomType[] ValidRoomTypes;
    public Direction Direction;

    public int ConectingDir
    {
        get
        {
            return ((int)Direction + 180) % 360;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
