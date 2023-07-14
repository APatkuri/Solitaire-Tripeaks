using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Selectable : MonoBehaviour
{
    // Start is called before the first frame update
    public bool FaceUp = false;
    public bool isPlayingAreaCard = false;
    public bool isDeckCardSpecial = false;
    public bool isSpecialIncreamentCard = false;
    public string cardinitlocation = "";
    public string cardsuit;
    public string cardnumber;
    public int cardvalue;

     private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        FaceUp = false;
        isSpecialIncreamentCard = false;
    }

    void Start()
    {
        ValueAssign(transform);
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void ValueAssign(Transform x){

        if(x.gameObject.CompareTag("Card")){

            cardsuit = x.name[0].ToString();
            for(int i=1; i<x.name.Length; i++){
                
                char c = x.name[i];
                cardnumber = cardnumber + c.ToString();
            }

            if(cardnumber == "A"){
                cardvalue = 1;
            }

            else if(cardnumber == "2"){
                cardvalue = 2;
            }
            
            else if(cardnumber == "3"){
                cardvalue = 3;
            }

            else if(cardnumber == "4"){
                cardvalue = 4;
            }

            else if(cardnumber == "5"){
                cardvalue = 5;
            }

            else if(cardnumber == "6"){
                cardvalue = 6;
            }

            else if(cardnumber == "7"){
                cardvalue = 7;
            }

            else if(cardnumber == "8"){
                cardvalue = 8;
            }

            else if(cardnumber == "9"){
                cardvalue = 9;
            }

            else if(cardnumber == "10"){
                cardvalue = 10;
            }

            else if(cardnumber == "J"){
                cardvalue = 11;
            }

            else if(cardnumber == "Q"){
                cardvalue = 12;
            }

            else if(cardnumber == "K"){
                cardvalue = 13;
            }
        }

        if(x.gameObject.CompareTag("WildCard")){
            cardnumber = "W";
            cardvalue = 20;
        }

    }
}
