using UnityEngine;

namespace ML.MazeGame
{
  public class LevelManager : MonoBehaviour
  {
    private static LevelManager instance;
    public static LevelManager Instance{get{return instance;}}

    private void Awake() {
      if(instance == null){
        instance = this;
      }else{
        Destroy(this.gameObject);
      }
    }

    [Header("Properties")]
    [SerializeField] Sprite lockedSprite;
    public Sprite GetLockedSprite(){return lockedSprite;}
    [SerializeField] Sprite unlockedSprite;
    public Sprite GetUnLockedSprite(){return unlockedSprite;}
  }
}
