using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MiniGameUserInput : MonoBehaviour
{
    public MiniGameLogicManager logic;

    void Start()
    {
        logic = FindObjectOfType<MiniGameLogicManager>();
    }

    void Update()
    {
        GetTouchInput();
    }

    void GetTouchInput(){

        if(Input.touchCount > 0){

            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began){
                Ray touchPosition = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, -10));
                RaycastHit2D hit = Physics2D.GetRayIntersection(touchPosition);

                if(hit && !logic.isgameover)
                {
                    
                    if(hit.collider.CompareTag("Card")){
                        Card(hit.collider.gameObject);
                    }
                }
            }
        }
    }

    void Card(GameObject selected){

        if(!selected.GetComponent<MiniGameCardScript>().FaceUp){
            logic.CardPlayLogic(selected);
        }

        else if(selected.GetComponent<MiniGameCardScript>().FaceUp){
            Debug.Log("Already FaceUp");
        }
    }
}