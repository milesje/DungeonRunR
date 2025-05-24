using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private int _mazeWidth;
    [SerializeField] private int _mazeDepth;

    private MazeCell[,] _mazeGrid;

    [SerializeField]
    private int _seed;
    [SerializeField]
    private bool _useSeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_useSeed)
        {
            Random.InitState(_seed);
        }
        else
        {
            int randomSeed = Random.Range(1, int.MaxValue);
            Random.InitState(randomSeed);
            Debug.Log(randomSeed);
        }
           
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity,transform);
                _mazeGrid[x, z].transform.localPosition = new Vector3(x, 0, z);
                
            }
        }
        GenerateMaze(null, _mazeGrid[0,0]);

        // Instantiate the Player into the Maze...
        var player = Instantiate(playerPrefab, new Vector3(_mazeGrid[0, 0].transform.position.x, 1, _mazeGrid[0, 0].transform.position.z), Quaternion.identity);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);
            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        }while(nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(_ => Random.Range(5, 500)).FirstOrDefault();

    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.localPosition.x;
        int z = (int)currentCell.transform.localPosition.z;

        // Check if cell to the right has been visited
        if(x+1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x+1, z];
            if(cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }
        // Check if cell to the left has been visited
        if (x-1 >= 0)
        {
            var cellToLeft = _mazeGrid[x-1, z];
            if(cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }

        // Check if cell to the front has been visited
        if (z+1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z+1];
            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }

        // Check if cell to the back has been visited
        if (z -1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];
            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }


    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null) { return; }

        if(previousCell.transform.localPosition.x < currentCell.transform.localPosition.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if(previousCell.transform.localPosition.x > currentCell.transform.localPosition.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if(previousCell.transform.localPosition.z < currentCell.transform.localPosition.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if(previousCell.transform.localPosition.z > currentCell.transform.localPosition.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

}
