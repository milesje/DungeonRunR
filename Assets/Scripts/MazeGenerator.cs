using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance;
    [SerializeField] private MazeCell _mazeCellPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField]  private GameObject finishPrefab;
    [SerializeField] private GameObject _startGameCanvas;
    [SerializeField] private GameObject _uiCanvas;
    [SerializeField] private GameObject _uiCamera;
    [SerializeField] private GameObject _endScreenCanvas;

    [SerializeField]
    private int _mazeWidth;
    [SerializeField] private int _mazeDepth;

    private MazeCell[,] _mazeGrid;

    [SerializeField]
    private int _seed;
    [SerializeField]
    private bool _useSeed;

    private GameObject player;
    private GameObject exit;
    private float elapsedTime = 0;
    private bool isTimerRunning = false;

    public TextMeshProUGUI timeText;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startGameCanvas.SetActive(true);
    }

    public void EasyGame()
    {
        _mazeDepth = 8;
        _mazeWidth = 8;
        GameStart();
    }

    public void MediumGame()
    {
        _mazeDepth = 16;
        _mazeWidth = 16;
        GameStart();
    }

    public void HardGame()
    {
        _mazeDepth = 32;
        _mazeWidth = 32;
        GameStart();
    }
    public void GameStart()
    {
        _endScreenCanvas.SetActive(false);
        _startGameCanvas.SetActive(false);
        _uiCamera.SetActive(false);
        _uiCanvas.SetActive(true);
        if (_useSeed)
        {
            UnityEngine.Random.InitState(_seed);
        }
        else
        {
            int randomSeed = UnityEngine.Random.Range(1, int.MaxValue);
            UnityEngine.Random.InitState(randomSeed);
            Debug.Log(randomSeed);
        }

        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity, transform);
                _mazeGrid[x, z].transform.localPosition = new Vector3(x, 0, z);

            }
        }
        GenerateMaze(null, _mazeGrid[0, 0]);

        // Instantiate the Player into the Maze...
        Debug.Log("lets create a player");
        
        player = Instantiate(playerPrefab, new Vector3(_mazeGrid[0, 0].transform.position.x, 1, _mazeGrid[0, 0].transform.position.z), Quaternion.identity);
        
        exit = Instantiate(finishPrefab, new Vector3(_mazeGrid[_mazeWidth - 1, _mazeDepth - 1].transform.position.x, 0.001f, _mazeGrid[_mazeWidth - 1, _mazeDepth - 1].transform.position.z), Quaternion.identity);
        Time.timeScale = 1;
        isTimerRunning = true;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UIManager.Instance.UpdateTimer(elapsedTime);
        }
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
        
        //var finish = Instantiate(finishPrefab, new Vector3(currentCell.transform.position.x, 0.001f, currentCell.transform.position.z), Quaternion.identity);

    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(_ => UnityEngine.Random.Range(5, 500)).FirstOrDefault();

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

    public void EndGame()
    {
        isTimerRunning = false;
        Time.timeScale = 0;
        _endScreenCanvas.SetActive(true);
        // disable player controller
        player.GetComponentInChildren<StarterAssets.FirstPersonController>().enabled = false;
        timeText.text = $"Time: {elapsedTime:F2}";
    }

    public void PlayAgain()
    {
        ResetGame();
        GameStart();
    }

    public void QuitGame()
    {
        // Show the Start Game Canvas
        ResetGame();
        _endScreenCanvas.SetActive(false);
    }

    private void ResetGame()
    {
        _startGameCanvas.SetActive(true);
        _uiCamera.SetActive(true);
        Destroy(player);
        Destroy(exit);
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                Destroy(_mazeGrid[x, z].gameObject);
            }
        } 
        elapsedTime = 0f;
        _mazeGrid = null;
    }

    public void ExitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
