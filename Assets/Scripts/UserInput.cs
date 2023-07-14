using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UserInput : MonoBehaviour
{
    public TriPeaksLogicManager logic;
    public UILogicManager uilogic;

    void Start()
    {
        logic = FindObjectOfType<TriPeaksLogicManager>();
        uilogic = FindObjectOfType<UILogicManager>();
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
                    if(hit.collider.CompareTag("Talon")){
                        logic.DealCardstoTalon();
                    }
                    
                    else if(hit.collider.CompareTag("Card")){
                        Card(hit.collider.gameObject);
                    }

                    else if(hit.collider.CompareTag("WildCard")){
                        logic.WildCardLogic();
                    }

                    else if(hit.collider.CompareTag("Undo")){
                        logic.UndoLogic();
                    }

                    else if(hit.collider.CompareTag("+5Card")){
                        StartCoroutine(logic.PlusFiveCardLogic(hit.collider.gameObject));
                    }
                }
            }
        }
    }

    void Card(GameObject selected){

        if(selected.GetComponent<Selectable>().FaceUp && selected.GetComponent<Selectable>().isPlayingAreaCard){
            logic.PlayingAreaLogic(selected);
        }

        else if(selected.GetComponent<Selectable>().FaceUp && !selected.GetComponent<Selectable>().isPlayingAreaCard){
            Debug.Log("Pile Card" + " " + selected.name);
        }

        else if(!selected.GetComponent<Selectable>().FaceUp)
        {
            Debug.Log("Invalid Click");
        }
        
    }
}