using UnityEngine;
using UnityEngine.UI;

namespace ML.MazeGame
{
  public class LevelButtonGenerate : MonoBehaviour
  {
    [Header("Properties")]
    [SerializeField] int numOfLevel;

    [Header("Refs")]
    [SerializeField] Transform contentObj;
    [SerializeField] GameObject buttonPrefab;
    [Tooltip("Empty RectTransform, which contain Horizontal Layout")]
    [SerializeField] GameObject rowPrefab;
    [Tooltip("Dummy obj, just for filling the blank spot in last row")]
    [SerializeField] GameObject dummyRect;
    [SerializeField] Sprite lockedSprite;
    [SerializeField] Sprite unlockedSprite;
    [SerializeField] LevelSelect[] allLevels;

    ///<summary>
    ///Clear all child inside content of scrollview
    ///</summary>
    public bool ClearAllLevel()
    {
      allLevels = new LevelSelect[numOfLevel];
      if (contentObj != null)
      {
        while (contentObj.childCount > 0)
        {
          DestroyImmediate(contentObj.GetChild(0).gameObject);
        }
        return true;
      }
      else
      {
        Debug.LogError("Content transform not found.");
        return false;
      }
    }

    ///<summary>
    ///Generate levels base on numOfLevel
    ///</summary>
    public void GenerateLevel()
    {
      if (!ClearAllLevel()) return;

      int rowCount = Mathf.CeilToInt((float)numOfLevel / 4.0f);
      int btnIndex = 1;

      for (int i = 1; i <= rowCount; i++)
      {

        GameObject newRow = Instantiate(rowPrefab);
        newRow.transform.SetParent(contentObj, false);
        newRow.transform.SetAsFirstSibling();
        if(i%2==0) newRow.GetComponent<HorizontalLayoutGroup>().reverseArrangement = true;

        for (int j = 1; j <= 4; j++)
        {
          if(btnIndex > numOfLevel){
            GameObject dummR = Instantiate(dummyRect);
            dummR.transform.SetParent(newRow.transform, false);
            continue;
          }

          GameObject newLevel = Instantiate(buttonPrefab);
          newLevel.transform.SetParent(newRow.transform, false);

          if(i%2 == 0){
            if(j != 1) SetRightLine(newLevel, btnIndex);
          }else{
            if(j != 4) SetRightLine(newLevel, btnIndex);
          }

          //Set index to button
          LevelSelect levelSelect = newLevel.GetComponent<LevelSelect>();
          if (levelSelect)
          {
            if (btnIndex == 1)
            {
              newLevel.transform.Find("tutorial").gameObject.SetActive(true);
              newLevel.transform.Find("index").gameObject.SetActive(false);
              levelSelect.UnlockWithSprite(unlockedSprite);
            }
            levelSelect.SetButtonIndex((uint)btnIndex);
            newLevel.transform.gameObject.name = "LevelSelect_" + btnIndex;

            //cache this button
            allLevels[btnIndex-1] = levelSelect;

            //Set vertical line
            if (btnIndex % 4 == 0){
              newLevel.transform.Find("line_vertical").gameObject.SetActive(true);
            }

            btnIndex++;
          }
          else
          {
            Debug.LogError("LevelSelect script not found in prefab. Fixing by adding another one.");
            continue;
          }
        }
      }
    }

    public void RandomStageUnlock(){
      int randomIndex = Random.Range(1, allLevels.Length);
      Debug.Log(randomIndex);

      for (int i = 0; i < randomIndex; i++)
      {
        allLevels[i].UnlockWithSprite(unlockedSprite);
        allLevels[i].UnLockWithStar(Random.Range(1, 4));
      } 
    }

    public void ResetStages(){

    }

    void SetRightLine(GameObject obj, int btnIndex)
    {
      // if (btnIndex == numOfLevel) return;
      if (btnIndex % 2 == 0)
      {
        obj.transform.Find("line_up").gameObject.SetActive(true);
      }
      else
      {
        obj.transform.Find("line_down").gameObject.SetActive(true);
      }
    }
  }
}
