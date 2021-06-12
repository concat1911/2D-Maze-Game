// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace ML.MazeGame{
  public class GameManager : MonoBehaviour
  {
    private static GameManager instance;
    public static  GameManager Instance{get{return instance;}}

    [Header("Properties")]
    [SerializeField] bool isGameOver;
    [SerializeField] bool isOverTime;
    [SerializeField] int levelIndex = 1;
    [SerializeField] int starLeft;
    [SerializeField] int hintLeft;
    [SerializeField] float totalTime;
    [SerializeField] Vector2Int endPos;

    [Header("Refs")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject destinationObj;
    [SerializeField] Transform mazeGridParent;
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] PathFinding pathFinding;
    [SerializeField] Image timerBar;
    [SerializeField] Text hintLeftText;
    [SerializeField] TextMeshProUGUI levelIndexText;
    [SerializeField] GameObject inputObj;

    //private
    MazeCell[,] cells;
    float timer;

    #region Mono
    private void Awake() {
      if(instance == null){
        instance = this;
      }else{
        Destroy(gameObject);
      }
    }

    private void Start() {
      mazeGenerator.Generate();
      InitialSetup();
    }

    private void Update() {
      if(isOverTime) return;
      timer += Time.deltaTime;

      timerBar.fillAmount = 1 - (timer / totalTime);

      if(timer >= (totalTime / 2) && starLeft == 3)  starLeft--;
      if(timer >= (totalTime / 3) + (totalTime / 2) && starLeft == 2)  starLeft--;

      if(timer >= totalTime){
        isOverTime = true;
        starLeft = 0;
      }
    }
    #endregion

    #region Functions
    void InitialSetup(){
      starLeft = 3;
      isGameOver = false;
      isOverTime = false;
      timer = 0f;

      levelIndexText.text = "No. " + levelIndex.ToString();

      GetAllNode();

      //Random endposition
      Vector2Int randomEndPos = new Vector2Int(Random.Range(5, 10), Random.Range(0, 12));
      pathFinding.SetEndPosition(randomEndPos);
      destinationObj.transform.position = cells[randomEndPos.x, randomEndPos.y].gameObject.transform.position;
      player.transform.position = cells[0, 12].gameObject.transform.position;
      player.SetActive(true);
    }

    public void GetAllNode()
    {
      Vector2Int gridSize = pathFinding.GridSize;
      MazeCell[] allCells = mazeGridParent.gameObject.GetComponentsInChildren<MazeCell>();
      cells = new MazeCell[gridSize.x, gridSize.y];

      for (int i = 0; i < allCells.Length; i++)
      {
        MazeCell cell = allCells[i];
        cells[cell.position.x, cell.position.y] = cell;
      }
    }

    public void GetHint(){
      if(hintLeft <= 0) return;
      hintLeft--;
      hintLeftText.text = hintLeft.ToString();

      MazeCell curCell = player.GetComponent<PlayerMovement>().CurrentCell;
      pathFinding.SetStartPosition(new Vector2Int(curCell.position.x, curCell.position.y));

      List<Vector2Int> path = pathFinding.FindPath(cells);
      int length = path.Count;
      Vector2[] pathInWorldPos = new Vector2[length];
      
      for (int i = 0; i < length; i++)
      {
        pathInWorldPos[length - i - 1] = cells[path[i].x, path[i].y].transform.position;
      }

      PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
      if(playerMovement){
        playerMovement.SetPath(pathInWorldPos);
      }else{
        Debug.LogError("Player doesn't have PlayerMovement script");
      }
    }

    public void AutoMove(){
      PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
      if(playerMovement){
        if(playerMovement.PathLength <= 0) return;
        playerMovement.canControl = false;
      }else{
        Debug.LogError("Player doesn't have PlayerMovement script");
      }
    }

    public void GameOver(){
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
  }
}
