// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using TouchControlsKit;

namespace ML.MazeGame
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class PlayerMovement : MonoBehaviour
  {
    [Header("Properties")]
    [SerializeField] float speed;
    [SerializeField] public bool canControl;
    [SerializeField] MazeCell curCell;
    public MazeCell CurrentCell{get{return curCell;}}
    public void SetCell(MazeCell nextCell) {curCell = nextCell;}
    Rigidbody2D rigidbody;
    Vector2 movement;
    Vector2[] path;
    int curPathIndex;

    [SerializeField] TCKJoystick joystick;

    public void SetPath(Vector2[] n_path){
      canControl = true;
      curPathIndex = 0;
      path = n_path;
    }
    public int PathLength {get{return path != null ? path.Length : 0;}}

    #region Mono
    private void Awake()
    {
      rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
      if(canControl){
        // movement.x = Input.GetAxisRaw("Horizontal");
        // movement.y = Input.GetAxisRaw("Vertical");
        movement.x = joystick.axisX.value;
        movement.y = joystick.axisY.value;
      }
    }

    private void FixedUpdate()
    {
      if(canControl){
        MoveTo(movement);
        return;
      }

      if(path.Length <= 0) return;
      if(Vector2.Distance(transform.position, path[curPathIndex]) <= 0.2f){
        curPathIndex++;
      }

      if(curPathIndex == path.Length - 1){
        curPathIndex = 0;
        canControl = true;
      }

      Vector2 direction = path[curPathIndex] - new Vector2(transform.position.x, transform.position.y);
      MoveTo(direction);
    }

    public void MoveTo(Vector2 direction){
      direction.Normalize();

      rigidbody.velocity = direction * speed * Time.deltaTime;

      if(direction != Vector2.zero){
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rigidbody.rotation = angle;
      }
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if(other.CompareTag("Destination")){
        if(GameManager.Instance){
          GameManager.Instance.GameOver();
        }
      }
    }
    #endregion
  }
}
