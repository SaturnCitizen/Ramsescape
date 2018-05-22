using System.Collections;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    // The type of tile that will be laid in a specific position.
    public enum TileType
    {
        Wall, Floor, Item, Enemy, EnemySpawner, Player, Exit,
    }


    public int columns = 100;                                 // The number of columns on the board (how wide it will be).
    public int rows = 100;                                    // The number of rows on the board (how tall it will be).
    public IntRange numRooms = new IntRange(15, 20);         // The range of the number of rooms there can be.
    public IntRange roomWidth = new IntRange(3, 10);         // The range of widths rooms can have.
    public IntRange roomHeight = new IntRange(3, 10);        // The range of heights rooms can have.
    public IntRange corridorLength = new IntRange(6, 10);    // The range of lengths corridors between rooms can have.
    public GameObject[] floorTiles;                           // An array of floor tile prefabs.
    public GameObject[] wallTiles;                            // An array of wall tile prefabs.
    public GameObject[] itemTiles;                            // An array of item tile prefabs.
    public GameObject[] enemyTiles;                           // An array of enemy tile prefabs.
    public GameObject player;
    public GameObject exit;
    public GameObject playerFloor;
    

    private TileType[][] tiles;                               // A jagged array of tile types representing the board, like a grid.
    private Room[] rooms;                                     // All the rooms that are created for this board.
    private Corridor[] corridors;                             // All the corridors that connect the rooms.
    private GameObject boardHolder;                           // GameObject that acts as a container for all other tiles.
    private float itemRandomValue = 0.95f;
    private float enemyRandomValue = 0.85f;
    private Vector3 checkPlayerValue;
    private Vector3 checkExitValue;



    public void Awake()
    {
        SetupTilesArray();

        CreateRoomsAndCorridors();

        SetTilesValuesForRooms();

        SetTilesValuesForCorridors();
    }

    public void Start()
    {
        // Create the board holder.

        boardHolder = new GameObject("BoardHolder");

        InstantiateTiles();

    }

    void SetupTilesArray()
    {
        // Set the tiles jagged array to the correct width.
        tiles = new TileType[columns][];

        // Go through all the tile arrays...
        for (int i = 0; i < tiles.Length; i++)
        {
            // ... and set each tile array is the correct height.
            tiles[i] = new TileType[rows];
        }
    }


    void CreateRoomsAndCorridors()
    {
        // Create the rooms array with a random size.
        rooms = new Room[numRooms.Random];

        // There should be one less corridor than there is rooms.
        corridors = new Corridor[rooms.Length - 1];

        // Create the first room and corridor.
        rooms[0] = new Room();
        corridors[0] = new Corridor();

        // Setup the first room, there is no previous corridor so we do not use one.
        rooms[0].SetupRoom(roomWidth, roomHeight, columns, rows);

        // Setup the first corridor using the first room.
        corridors[0].SetupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, columns, rows, true);

        for (int i = 1; i < rooms.Length; i++)
        {
            // Create a room.
            rooms[i] = new Room();

            // Setup the room based on the previous corridor.
            rooms[i].SetupRoom(roomWidth, roomHeight, columns, rows, corridors[i - 1]);

            // If we haven't reached the end of the corridors array...
            if (i < corridors.Length)
            {
                // ... create a corridor.
                corridors[i] = new Corridor();

                // Setup the corridor based on the room that was just created.
                corridors[i].SetupCorridor(rooms[i], corridorLength, roomWidth, roomHeight, columns, rows, false);
            }

            if (i == Mathf.Ceil(rooms.Length * .5f))
            {
                Vector3 playerPos = new Vector3(rooms[i].xPos, rooms[i].yPos, 0);
                checkPlayerValue = playerPos;
                Instantiate(playerFloor, playerPos, Quaternion.identity);
                Instantiate(player, playerPos, Quaternion.identity);
            }
            if (i == rooms.Length-1)
            {
                Vector3 exitPos = new Vector3(rooms[i].xPos + Mathf.Floor((Random.value * rooms[i].roomWidth) 
                % (rooms[i].roomWidth - 1)) , rooms[i].yPos + Mathf.Floor((Random.value * rooms[i].roomHeight)
                % (rooms[i].roomHeight - 1)), 0);
                checkExitValue = exitPos;
                Instantiate(playerFloor, exitPos, Quaternion.identity);
                Instantiate(exit, exitPos, Quaternion.identity);  
            }
        }

    }


    void SetTilesValuesForRooms()
    {
        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currentRoom = rooms[i];

            // ... and for each room go through it's width.
            for (int j = 0; j < currentRoom.roomWidth; j++)
            {
                int xCoord = currentRoom.xPos + j;

                // For each horizontal tile, go up vertically through the room's height.
                for (int k = 0; k < currentRoom.roomHeight; k++)
                {
                    int yCoord = currentRoom.yPos + k;

                    // The coordinates in the jagged array are based on the room's position and it's width and height.

                    //switch case needed
                    if (Random.value > itemRandomValue)
                    {
                        tiles[xCoord][yCoord] = TileType.Item;
                    }
                    else if (Random.value > enemyRandomValue)
                    {
                        tiles[xCoord][yCoord] = TileType.Enemy;
                    }
                    else
                    {
                        tiles[xCoord][yCoord] = TileType.Floor;
                    }
                }
            }
        }
    }



    void SetTilesValuesForCorridors()
    {
        // Go through every corridor...
        for (int i = 0; i < corridors.Length; i++)
        {
            Corridor currentCorridor = corridors[i];

            // and go through it's length.
            for (int j = 0; j < currentCorridor.corridorLength; j++)
            {
                // Start the coordinates at the start of the corridor.
                int xCoord = currentCorridor.startXPos;
                int yCoord = currentCorridor.startYPos;

                // Depending on the direction, add or subtract from the appropriate
                // coordinate based on how far through the length the loop is.
                switch (currentCorridor.direction)
                {
                    case Direction.North:
                        yCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        yCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }

                // Set the tile at these coordinates to Floor.
                tiles[xCoord][yCoord] = TileType.Floor;
            }
        }
    }


    void InstantiateTiles()
    {

        // Go through all the tiles in the jagged array...
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                // If the tile type is Wall...
                if (tiles[i][j] == TileType.Wall)
                {
                    // ... instantiate a wall.
                    InstantiateFromArray(wallTiles, i, j);
                }
                else
                {
                    // ... instantiate a floor tile.
                    InstantiateFromArray(floorTiles, i, j);

                    if (tiles[i][j] == TileType.Item)
                    {
                        // ... instantiate an item tile.
                        InstantiateFromArray(itemTiles, i, j);
                    }
                    else if (tiles[i][j] == TileType.Enemy)
                    {
                        // ... instantiate an enemy tile.
                        InstantiateFromArray(enemyTiles, i, j);
                    }
                }
            }
        }  
    }


    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        // Create an instance of the prefab from the random index of the array.
        if (!(position == checkExitValue || position == checkPlayerValue))
        {
            GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;
            // Set the tile's parent to the board holder.
            tileInstance.transform.parent = boardHolder.transform;
        }
    }
}