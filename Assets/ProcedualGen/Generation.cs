using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static RoomTypes;
using Direction = RoomTypes.Direction;
public class Generation : MonoBehaviour
{

    [SerializeField] public GenData GenData;

    [SerializeField] private List<RoomData> _curRooms;

    [SerializeField] private ContactFilter2D _filter;

    [SerializeField] private int _stepBackResetMargin = 10;

    [SerializeField] private int _maxIterations = 10000;

    [SerializeField] private GameObject _dynamicRoomPrefab;

    [SerializeField] public Tilemap GlobalGroundTileMap;
    [SerializeField] public Tilemap GlobalWallTileMap;
    [SerializeField] public Tilemap GlobalDecoTileMap;
    [SerializeField] private GameObject PlayerObj;
    [SerializeField] public Transform TeleportObj;


    [Header("Debug Options")]
    [SerializeField] private Tilemap _debugGroundTileMap;
    [SerializeField] private bool _debugButton;

    [SerializeField] private bool _debugButton2;
    [SerializeField] private bool _debugResetButton;

    [SerializeField] private bool _debugTraversel;
    [SerializeField] private GameObject _heatMapObjPrefab;




    public static System.Random GenRnd;
    public static Generation Instance;

    private int _roomsBuild = 0;
    private List<RoomData> _validRooms;

    private List<GameObject> _tileMapObject = new();

    private Dictionary<RoomType, int> _quotas;
    private GameStarter _gameStarter;
    private int _totalQouta;

    private int _stepBackAmount = 1;
    private int _curBuildStreak = 0;
    private int _bestBuildStreak = 0;

    private Dictionary<RoomData, int> _traversalData = new();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GenRnd = new();


        _quotas = GenData.BuildDict();
        foreach (var item in _quotas)
        {
            if (item.Value == -1)
            {
                continue;
            }
            _totalQouta += item.Value;
        }

        _validRooms = _curRooms.ToList();

        Instance = this;


        //Step();
    }

    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_debugButton)
        {
            GenerateMap();
        }

        if (_debugButton2)
        {
            _debugButton2 = false;
            StartTileMapMerge();
        }
        if (_debugResetButton)
        {
            _debugResetButton = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }




    #region  Main Loop
    void GenerateMap()
    {
        while (LoopRooms())
        {


        }
        _debugButton = false;
        Cleanup();
        AttachRoomsConnections();
        List<(RoomData, int)> traversalData;
        ReValidateAll();
        RoomData startRoom = FindFarthestRoomAndAttach(_curRooms[0], GenData.StartRoom);
        ReValidateAll();
        RoomData endRoom = FindFarthestRoomAndAttach(startRoom, out traversalData, GenData.EndRoom);

        foreach (var item in traversalData)
        {
            _traversalData.Add(item.Item1, item.Item2);
        }

        if (_debugTraversel)
        {
            DebugHeatMap(startRoom.transform.position, new(0, 1, 0));
            DebugHeatMap(endRoom.transform.position, new(0, 0, 1));
        }


        StartTileMapMerge();
        RemoveConnectionWalls();

        RoomDirectorInit();

        _gameStarter = _curRooms[^2].GetComponent<GameStarter>();
        if (_gameStarter == null)
        {
            //print(startRoom.gameObject.name);
            //print("????");
            return;
        }

        Physics2D.SyncTransforms();

        _gameStarter.PlayerObj = PlayerObj;
        _gameStarter.StartGame();

    }

    bool LoopRooms()
    {
        GameObject roomObj = null;

        RoomAttachPoint point = null;
        RoomData curRoom = null;
        int iterator = 0;
        int problemCount = 0;

        while (true)
        {
            iterator++;

            if (_totalQouta <= 0)
            {
                return false;
            }

            if (iterator > _maxIterations)
            {
                throw new System.Exception("Loop exceted 9000 | LoopRooms");
            }
            if (_validRooms.Count <= 0)
            {
                //
                //Debug.Log($"Problems | {_validRooms.Count}");
                if (problemCount >= 100)
                {

                    throw new System.Exception("No valid rooms found while qouta has not been meet");
                }
                problemCount++;
                StepBack();

                return true;

            }


            curRoom = PickRandomValidRoom();




            if (!TryFindValidPrefabAndAttach(curRoom, out roomObj, out point))
            {
                //print("room was invalid");
                _validRooms.Remove(curRoom);
                continue;
            }
            break;


        }
        TryAddRoom(curRoom, roomObj, point);
        return true;
    }



    #endregion

    #region Room Generation
    bool TryAddRoom(RoomData curRoom, GameObject roomObj, RoomAttachPoint point, bool bypassCollsions = false, RoomAttachPoint oRoomPoint = null)
    {

        _roomsBuild++;

        // Generates the corridor data
        int corridorLength = GenRnd.Next((int)point.MinCorridorLength, (int)point.MaxCorridorLength);
        Vector2 pointDirection = point.GetVectorDirection();

        //minus one because pointDirection already has a 1
        Vector2 buildOffset = pointDirection * (corridorLength - 1);


        //Instansiates the other room object and sets the postion
        GameObject iRoomObj = Instantiate(roomObj);
        RoomData iRoomData = iRoomObj.GetComponent<RoomData>();
        RoomAttachPoint otherPoint = iRoomData.PickRandomValidAttachPoint(point, iRoomData);

        Vector3 ConnectedOfset = otherPoint.transform.position - iRoomObj.transform.position;
        Vector3 AttachPointPos = point.transform.position;
        Vector3 finalPos = AttachPointPos - ConnectedOfset + (Vector3)buildOffset;

        iRoomObj.transform.position = finalPos;

        iRoomObj.name += _roomsBuild;

        if (iRoomData.BoundingBox == null)
        {
            //print("Colider was null");
            iRoomData.BoundingBox = iRoomData.GetComponent<BoxCollider2D>();
        }
        //!Physics2D.IsTouching(iRoomData.BoundingBox, curRoom.BoundingBox)
        Physics2D.SyncTransforms();
        //List<Collider2D> hits = new();
        Collider2D[] hits;
        // I sure hope this works
        //iRoomData.BoundingBox.Overlap(hits);

        hits = Physics2D.OverlapBoxAll(
            iRoomData.transform.position + (Vector3)iRoomData.BoundingBox.offset,
            iRoomData.BoundingBox.bounds.size,
            0f
        );
        bool hasHitSomething = false;

        foreach (var hit in hits)
        {
            //If what we hit has no room data we skip it as it is not a room
            if (hit.gameObject.GetComponent<RoomData>() == null)
            {
                continue;
            }
            //print(hit.name);
            if (hit.gameObject != curRoom.gameObject && hit.gameObject != iRoomData.gameObject)
            {
                //print($"This thing has hit something i don't know what but it has {hit.gameObject.name} | {curRoom.gameObject.name} | {iRoomData.gameObject.name}");
                hasHitSomething = true;
                break;
            }
        }

        if (hasHitSomething && !bypassCollsions)
        {

            //print($"Something Was Touching {point.transform.parent.name} | {point.transform.name}");
            point.ValidRoomTypes.Remove(iRoomData.RoomType);
            if (point.ValidRoomTypes.Count <= 0)
            {
                curRoom.ValidRoomAttachPoints.Remove(point);
                //point.BlackListedRooms.Add(iRoomData.Identifyer);
            }
            Destroy(iRoomObj);
            return false;
        }
        else
        {

            _validRooms.Add(iRoomData);
            iRoomData.ValidRoomAttachPoints.Remove(otherPoint);
            iRoomData.CurAttachments++;
            iRoomData.ConnectedPoints.Add(point);
            otherPoint.Connected = curRoom;

            curRoom.CurAttachments++;
            _curRooms.Add(iRoomData);
            point.Connected = iRoomData;

            if (oRoomPoint != null)
            {
                RoomAttachPoint oRoomOtherPoint = iRoomData.PickRandomValidAttachPoint(oRoomPoint, iRoomData);
                oRoomPoint.Connected = iRoomData;
                oRoomPoint.Parrent.CurAttachments++;

                iRoomData.ValidRoomAttachPoints.Remove(oRoomOtherPoint);
                iRoomData.CurAttachments++;
                iRoomData.ConnectedPoints.Add(oRoomPoint);
                oRoomOtherPoint.Connected = oRoomPoint.Parrent;

            }

            GameObject dynCorObj = Instantiate(_dynamicRoomPrefab, transform.position, Quaternion.identity);
            DynamicCorridor dynamicCorridor = dynCorObj.GetComponent<DynamicCorridor>();
            dynamicCorridor.GenCorridor(point, corridorLength);
            _tileMapObject.Add(dynCorObj);

            curRoom.AttachedCorridors.Add((point, dynamicCorridor));
            iRoomData.AttachedCorridors.Add((otherPoint, dynamicCorridor));

            curRoom.ValidRoomAttachPoints.Remove(point);
            if (_quotas.ContainsKey(iRoomData.RoomType))
            {
                if (!(_quotas[iRoomData.RoomType] == -1))
                {
                    _quotas[iRoomData.RoomType]--;
                    _totalQouta--;
                }
            }

            _curBuildStreak++;

        }
        return true;
    }

    #endregion

    #region Connection Generation


    [Header("Extra Generation")]
    [SerializeField]
    [Range(0f, 1f)]
    private float _extraConectionChance;
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private float _maxRaycastLength;
    void AttachRoomsConnections()
    {
        List<RoomData> copyCurRoom = _curRooms.ToList();
        foreach (RoomData room in copyCurRoom)
        {
            if (room.CurAttachments < room.MaxAttachments)
            {
                int count = room.ValidRoomAttachPoints.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i >= room.ValidRoomAttachPoints.Count)
                    {
                        break;
                    }
                    var point = room.ValidRoomAttachPoints[i];
                    if (room.CurAttachments >= room.MaxAttachments)
                    {
                        break;
                    }
                    double rndNum = GenRnd.NextDouble();
                    if (rndNum < _extraConectionChance)
                    {
                        CheckRoomExtraConnection(point);
                    }
                }
            }
        }
    }

    void CheckRoomExtraConnection(RoomAttachPoint point)
    {
        Physics2D.SyncTransforms();
        Vector2 direction = point.GetVectorDirection();

        RaycastHit2D[] hits = Physics2D.RaycastAll(point.transform.position, direction, _maxRaycastLength, _layerMask);
        RoomAttachPoint hitPoint = null;
        if (hits.Length <= 0)
        {
            return;
        }
        foreach (var hit in hits)
        {
            RoomData hitRoom = hit.collider.gameObject.GetComponentInParent<RoomData>();
            if (hit.collider.gameObject.GetComponent<BoxCollider2D>() != null)
            {
                //If it hits a corridor skip
                if (hit.collider.GetComponent<DynamicCorridor>() != null)
                {
                    return;
                }
                continue;
            }

            if (!hitRoom) { return; }

            if (hitRoom == point.Parrent)
            {
                continue;
            }
            if (hitRoom.CurAttachments == hitRoom.MaxAttachments)
            {
                return;
            }



            foreach (var rPoint in hitRoom.RoomAttachPoints)
            {

                if (Vector2.Distance(hit.point, rPoint.transform.position) <= 0.5)
                {
                    //Debug.DrawLine(point.transform.position, rPoint.transform.position, Color.yellow, 10000);
                    hitPoint = rPoint;
                }
            }

            if (hitPoint == null)
            {
                //Debug.DrawLine(point.transform.position, hit.point, Color.red, 10000);

                return;
            }
            //Debug.DrawLine(point.transform.position, hitPoint.transform.position, Color.green, 10000);





            AttachCorridor(point, hitPoint);
            return;
        }



    }




    void AttachCorridor(RoomAttachPoint roomPoint, RoomAttachPoint otherPoint)
    {
        GameObject dynCorObj = Instantiate(_dynamicRoomPrefab, transform.position, Quaternion.identity);
        DynamicCorridor dynamicCorridor = dynCorObj.GetComponent<DynamicCorridor>();
        //i add one to this, why? I DON'T FUCKING KNOW
        int corridorLength = Mathf.RoundToInt((roomPoint.transform.position - otherPoint.transform.position).magnitude + 1);
        dynamicCorridor.GenCorridor(roomPoint, corridorLength);

        _tileMapObject.Add(dynCorObj);

        roomPoint.Connected = otherPoint.Parrent;
        otherPoint.Connected = roomPoint.Parrent;
        roomPoint.Parrent.ValidRoomAttachPoints.Remove(roomPoint);
        otherPoint.Parrent.ValidRoomAttachPoints.Remove(otherPoint);
        roomPoint.Parrent.CurAttachments++;
        otherPoint.Parrent.CurAttachments++;

        roomPoint.Parrent.AttachedCorridors.Add((roomPoint, dynamicCorridor));
        otherPoint.Parrent.AttachedCorridors.Add((otherPoint, dynamicCorridor));

    }

    #endregion

    #region Validation
    bool TryFindValidPrefabAndAttach(RoomData roomData, out GameObject roomObj, out RoomAttachPoint curPoint, bool ignoreMaxAttach = false, GameObject roomObjOveride = null)
    {
        roomObj = null;
        if (roomObjOveride != null)
        {
            roomObj = roomObjOveride;
        }
        curPoint = null;
        /* 
        Loops over every attach point and checks if it has any valid prefabs, if not it is removed from the pool
        and another attach point is choosen. This repeats until either a valid prefab is found or every point is removed
        at which point it returns
        */
        int iterator = 0;


        while (true)
        {
            iterator++;

            if (iterator > _maxIterations)
            {
                throw new System.Exception("Loop exceted 9000 | TryFindValidPrefabAndAttach");
            }
            //Guard Statements
            if (roomData.ValidRoomAttachPoints.Count <= 0)
            {
                //Debug.Log("Nothing Valid Found");
                return false;
            }
            if (roomData.CurAttachments >= roomData.MaxAttachments)
            {
                //Debug.Log("To many attachments :(");
                if (!ignoreMaxAttach)
                {
                    return false;
                }

            }


            curPoint = roomData.GetRandomAttachPoint();

            if (roomObjOveride != null)
            {
                RoomData overideData = roomObjOveride.GetComponent<RoomData>();
                if (!IsRoomValid(overideData, curPoint, false, true))
                {
                    roomData.ValidRoomAttachPoints.Remove(curPoint);
                    continue;
                }
                break;
            }
            else if (TryFindRandomValidAttachRoom(out roomObj, curPoint))
            {
                break;
            }
            else
            {
                roomData.ValidRoomAttachPoints.Remove(curPoint);
                curPoint = null;
            }
        }
        return true;
    }





    bool TryFindRandomValidAttachRoom(out GameObject obj, RoomAttachPoint point)
    {

        obj = null;
        //This is so stupid and i hate lists
        List<WeightedObject> validRoomPrefabs = GenData.Objects.ToList();
        List<WeightedObject> RoomPrefabs = GenData.Objects.ToList();

        // Loops over rooms and removes invalid rooms
        for (int i = 0; i < RoomPrefabs.Count; i++)
        {
            var room = RoomPrefabs[i].Object;
            RoomData roomData = room.GetComponent<RoomData>();

            if (!IsRoomValid(roomData, point))
            {
                validRoomPrefabs.Remove(RoomPrefabs[i]);
                continue;
            }

        }

        if (validRoomPrefabs.Count <= 0)
        {
            //Debug.Log("No valid rooms found");
            return false;
        }



        int TotalWeight = 0;
        foreach (var room in validRoomPrefabs)
        {
            TotalWeight += room.Weight;
            //print($"{room.Object.name} | {Time.time}");
        }

        //print($"total weight {TotalWeight} | {Time.time}");

        int rndWeight = GenRnd.Next(TotalWeight);

        //print($"rnd weight {rndWeight} | {Time.time}");

        foreach (var room in validRoomPrefabs)
        {
            rndWeight -= room.Weight;
            if (rndWeight <= 0)
            {
                obj = room.Object;
                break;
            }
        }

        //int index = GenRnd.Next(weightedRoomPrefabs.Count);
        //obj = weightedRoomPrefabs[index];

        return true;
    }

    bool IsRoomValid(RoomData roomData, RoomAttachPoint point, bool overideRoomType = false, bool ignoreQouta = false)
    {
        if (!point.ValidRoomTypes.Contains(roomData.RoomType) && !overideRoomType)
        {
            //print($"Room doesn't have corect data type | {roomData.gameObject.name}");
            return false;
        }
        if (point.BlackListedRooms.Contains(roomData.Identifyer))
        {
            return false;
        }


        bool hasValidAttachPoint = false;
        foreach (var roomPrefabPoint in roomData.RoomAttachPoints)
        {
            if (roomPrefabPoint.ConectingDir == (int)point.Direction)
            {
                hasValidAttachPoint = true;
            }
        }
        if (!hasValidAttachPoint)
        {
            //print($"Room doesn't have point with valid direction | {roomData.gameObject.name}");
            return false;
        }

        if (ignoreQouta)
        {
            return true;
        }

        if (!_quotas.ContainsKey(roomData.RoomType))
        {
            return false;
        }

        //Checks if there is still qouta left
        if (_quotas[roomData.RoomType] == -1)
        {
            return true;
        }
        if (_quotas[roomData.RoomType] <= 0)
        {
            return false;
        }

        return true;

    #endregion
    }


    #region  Random
    RoomData PickRandomValidRoom()
    {
        if (_validRooms.Count <= 0)
        {
            Debug.LogWarning("THIS THING FUCKED UP");
            Reset();
        }
        int index = GenRnd.Next(_validRooms.Count);
        return _validRooms[index];
    }
    #endregion

    #region Traversal

    RoomData FindFarthestRoomAndAttach(RoomData roomData, GameObject spawnPrefab = null)
    {
        return FindFarthestRoomAndAttach(roomData, out _, spawnPrefab);
    }
    RoomData FindFarthestRoomAndAttach(RoomData roomData, out List<(RoomData, int)> traversalData, GameObject spawnPrefab = null)
    {
        traversalData = null;
        List<(RoomData, int)> ToTraverse = new();
        List<(RoomData, int)> TraversedItems = new();
        List<RoomData> TraversedRooms = new();
        ToTraverse.Add((roomData, 0));
        int _debugFuthersDistance = 0;

        int iterator = 0;
        while (ToTraverse.Count > 0)
        {
            iterator++;
            if (iterator > _maxIterations)
            {
                throw new System.Exception("Loop exceted 9000 | FindFarthestRoom");
            }




            RoomData curRoom = ToTraverse[0].Item1;
            if (TraversedRooms.Contains(curRoom))
            {
                ToTraverse.RemoveAt(0);
                continue;
            }
            int depth = ToTraverse[0].Item2;
            TraversedItems.Add((curRoom, ToTraverse[0].Item2));
            TraversedRooms.Add(curRoom);

            if (depth > _debugFuthersDistance)
            {
                _debugFuthersDistance = depth;
            }

            ToTraverse.RemoveAt(0);



            foreach (RoomAttachPoint point in curRoom.RoomAttachPoints)
            {
                if (point.Connected != null)
                {
                    if (TraversedRooms.Contains(point.Connected))
                    {
                        continue;
                    }
                    ToTraverse.Add((point.Connected, depth + 1));

                }
            }
        }
        RoomData furthestRoom = null;
        RoomAttachPoint opoint = null;

        //We copy the list before we abosulute ruin the list
        traversalData = TraversedItems.ToList();
        if (spawnPrefab != null)
        {
            int count = TraversedItems.Count;

            (RoomData, int) objData = (null, 0);


            for (int i = 0; i < count; i++)
            {
                furthestRoom = GetFurthestFromList(TraversedItems, out objData);
                if (furthestRoom == null || TraversedItems.Count <= 0)
                {
                    Debug.LogError("Failed to find a valid room");
                    return null;
                }
                bool returnvalue = TryFindValidPrefabAndAttach(furthestRoom, out GameObject obj, out opoint, true, spawnPrefab);
                if (returnvalue)
                {
                    if (TryAddRoom(furthestRoom, spawnPrefab, opoint))
                    {
                        break;
                    }
                }

                TraversedItems.Remove(objData);
                if (i == count - 1)
                {
                    throw new Exception("Something smth error message i don't care any more i just want to go home");
                }
            }
        }
        else
        {
            furthestRoom = GetFurthestFromList(TraversedItems, out _);
        }



        return furthestRoom;
    }

    private RoomData GetFurthestFromList(List<(RoomData, int)> TraversedItems, out (RoomData, int) outdata)
    {
        int distance = 0;
        RoomData furthestRoom = null;
        (RoomData, int) objData = (null, 0);
        int count = TraversedItems.Count;
        outdata = (null, 0);

        foreach (var item in TraversedItems)
        {
            if (_debugTraversel)
            {
                //DebugHeatMap(item.Item1.transform.position, new((float)item.Item2 / (float)_debugFuthersDistance, 0, 0));
            }
            if (item.Item2 > distance)
            {
                if (item.Item1 == null)
                {
                    Debug.LogWarning("Item was null?");
                    continue;
                }
                distance = item.Item2;
                furthestRoom = item.Item1;
                objData = item;
            }
        }

        // if (furthestRoom.RoomType == RoomType.Treasure)
        // {
        //     print($"furthestRoomtype:  {furthestRoom.RoomType}");
        //     distance = 0;
        //     furthestRoom = null;
        //     TraversedItems.Remove(objData);
        //     continue;
        // }
        outdata = objData;

        return furthestRoom;
    }

    void DebugHeatMap(Vector3 point, Color color)
    {
        GameObject iobj = Instantiate(_heatMapObjPrefab, point, Quaternion.identity);
        iobj.GetComponent<SpriteRenderer>().color = color;
    }

    #endregion

    #region Cleanup

    //This is such a bad idea
    void Reset()
    {
        //If somthing doesn't work try try and try again :)
        RoomData startRoom = _curRooms[0];
        _validRooms.Add(startRoom);
        startRoom.ValidRoomAttachPoints = startRoom.RoomAttachPoints.ToList();
        foreach (var item in startRoom.ValidRoomAttachPoints)
        {
            item.ResetValidRoomTypes();
        }

    }


    void Cleanup()
    {
        List<RoomData> ToDelete = new();
        foreach (RoomData room in _curRooms)
        {
            //print(room.RoomType);

            if (GenData.CleanUpTypes.Contains(room.RoomType))
            {
                if (room.CurAttachments != room.MaxAttachments)
                {
                    ToDelete.Add(room);
                }
            }
        }

        foreach (RoomData room in ToDelete)
        {
            foreach (var point in room.ConnectedPoints)
            {
                point.Parrent.CurAttachments--;
                point.Connected = null;
            }
            _curRooms.Remove(room);
            Destroy(room.gameObject);
        }
    }
    void StepBack()
    {

        //If build streak above the margin reset it
        if (_curBuildStreak > _stepBackResetMargin)
        {
            _bestBuildStreak = 0;
        }

        if (_curBuildStreak < _bestBuildStreak)
        {
            _stepBackAmount++;

        }
        else
        {
            //Only assign if cur buildStreak is better than the last
            _bestBuildStreak = _curBuildStreak;
            _stepBackAmount = 1;

        }




        _curBuildStreak = 0;

        int count = _curRooms.Count;
        //RoomData lastRemovedRoom = null;
        List<RoomData> roomsToRemove = new();

        count = _curRooms.Count;
        RoomData removedRoom = _curRooms.Last();
        // if (removedRoom == _curRooms[0])
        // {
        //     break;
        // }



        _curRooms.RemoveAt(count - 1);
        foreach (var point in removedRoom.ConnectedPoints)
        {
            point.Connected = null;
            point.Parrent.CurAttachments--;
            point.BlackListedRooms.Add(removedRoom.Identifyer);
            //_validRooms.Add(point.Parrent);
            //point.Parrent.ValidRoomAttachPoints.Add(point);
        }

        //Adds the qoutas for the removed objects
        if (!(_quotas[removedRoom.RoomType] == -1))
        {
            _totalQouta++;
            _quotas[removedRoom.RoomType]++;
        }

        roomsToRemove.Add(removedRoom);


        foreach (var rRoom in roomsToRemove)
        {
            foreach (var item in rRoom.AttachedCorridors)
            {
                if (item.Item2 != null)
                {
                    _tileMapObject.Remove(item.Item2.gameObject);
                    Destroy(item.Item2.gameObject);
                }
            }
            Destroy(rRoom.gameObject);
            Destroy(rRoom);
        }

        count = _curRooms.Count;

        ReValidateAll();


        // for (int i = 0; i < 4; i++)      
        // {
        //     if ((count - i) > -1)
        //     {
        //         RoomData room = _curRooms[count - i - 1];
        //         if (_validRooms.Contains(room))
        //         {
        //             _validRooms.Add(room);
        //         }
        //     }
        // }
    }


    void ReValidateAll()
    {
        foreach (RoomData room in _curRooms)
        {
            if (room.CurAttachments >= room.MaxAttachments)
            {
                continue;
            }
            _validRooms.Add(room);
            room.ValidRoomAttachPoints = new();
            foreach (RoomAttachPoint point in room.RoomAttachPoints)
            {
                if (point.Connected == null)
                {
                    room.ValidRoomAttachPoints.Add(point);
                }
            }

        }
    }
    #endregion 

    #region Merge Tilemaps

    void StartTileMapMerge()
    {
        foreach (var room in _curRooms)
        {
            _tileMapObject.Add(room.gameObject);
        }

        foreach (var obj in _tileMapObject)
        {

            Tilemap[] tilemaps = obj.GetComponentsInChildren<Tilemap>();
            Tilemap ground = null;
            Tilemap wall = null;
            Tilemap deco = null;

            foreach (var tmap in tilemaps)
            {
                float zpos = tmap.transform.localPosition.z;

                if (zpos == 0)
                {
                    ground = tmap;
                    continue;
                }
                if (zpos == -1)
                {
                    deco = tmap;
                    continue;
                }
                if (zpos == -2)
                {
                    wall = tmap;
                    continue;
                }
            }
            if (ground == null || deco == null || wall == null)
            {

                continue;

            }
            MergeTileMaps(ground, wall, deco);
        }
    }

    void MergeTileMaps(Tilemap ground, Tilemap wall, Tilemap deco)
    {
        MergeTileMap(ground, GlobalGroundTileMap);
        MergeTileMap(wall, GlobalWallTileMap);
        MergeTileMap(deco, GlobalDecoTileMap);
    }

    void MergeTileMap(Tilemap localTMap, Tilemap globalTMap)
    {
        BoundsInt area = localTMap.cellBounds;


        TileBase[] tiles = localTMap.GetTilesBlock(area);

        Vector3Int newAreaPos = globalTMap.WorldToCell(localTMap.CellToWorld(area.position));

        area.position = newAreaPos;

        MergeTileBlock(area, tiles, globalTMap);
        Destroy(localTMap.gameObject);
    }

    void MergeTileBlock(BoundsInt area, TileBase[] tiles, Tilemap tilemap)
    {
        TileBase[] curTiles = tilemap.GetTilesBlock(area);

        if (tiles.Length != curTiles.Length)
        {
            throw new System.Exception("Array Lengths do not match");
        }

        for (int i = 0; i < curTiles.Length; i++)
        {
            //Checks if there is already is a tile in the current spot, if yes skip that tile
            if (curTiles[i] != null)
            {
                tiles[i] = curTiles[i];
            }
        }

        tilemap.SetTilesBlock(area, tiles);

    }

    void RemoveConnectionWalls()
    {
        foreach (var room in _curRooms)
        {
            foreach (var attachPoint in room.RoomAttachPoints)
            {
                if (attachPoint.Connected != null)
                {
                    Vector3Int pos = GlobalWallTileMap.WorldToCell(attachPoint.transform.position);
                    GlobalWallTileMap.SetTile(pos, null);
                }
            }
        }
    }

    #endregion

    void RoomDirectorInit()
    {
        foreach (RoomData room in _curRooms)
        {
            room.Generation = this;
            if (_traversalData.ContainsKey(room))
            {
                int distanceFS = _traversalData[room];
                room.RoomDirector.SpawnCredits = (int)(GenData.StartingCredits + distanceFS * GenData.DifScalingMultiplyer);
                //print($"Obj Does have data {room.RoomDirector.SpawnCredits}");
                room.RoomDirector.Init();

            }
            else
            {
                //print("Obj Doesn't have traversal data");
            }


        }

    }

}

