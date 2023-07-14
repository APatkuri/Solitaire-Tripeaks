using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateSprite : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite cardFace;
    public Sprite cardBack;
    public TriPeaksLogicManager logic;
    public SpriteRenderer spriteRenderer;
    public Selectable selectable;
    public int cardfacelistnumber;
    // Start is called before the first frame update

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){

        // spriteRenderer = GetComponent<SpriteRenderer>();
        // selectable = GetComponent<Selectable>();

        // SpriteUpdate();
    }

    void Start()
    {
        List<string> deck = TriPeaksLogicManager.GenerateDeck();
        logic = FindAnyObjectByType<TriPeaksLogicManager>();

        cardfacelistnumber = 0;
        foreach(string c in deck){
            if(this.name == c){
                cardFace = logic.cardfaces[cardfacelistnumber];
                break;
            }

            cardfacelistnumber++;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        SpriteUpdate();

        IncreamentCardColor();
    }

    public void IncreamentCardColor(){
        if(selectable.isSpecialIncreamentCard && selectable.isPlayingAreaCard && selectable.FaceUp){
            Color testcolor;
            ColorUtility.TryParseHtmlString("#FD94FF", out testcolor);
            spriteRenderer.color = testcolor;
        }
        else{
            Color testcolor;
            ColorUtility.TryParseHtmlString("#FFFFFF", out testcolor);
            spriteRenderer.color = testcolor;
        }
    }

    public void SpriteUpdate(){

        if(selectable.FaceUp){
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
    }

    public string IncreamentCardLogic(int z){

        if(selectable.isSpecialIncreamentCard && selectable.FaceUp){

            if(cardfacelistnumber>=0 && cardfacelistnumber <= 12){
                cardfacelistnumber+= z;
                if(cardfacelistnumber == 13 && z==1){
                    cardfacelistnumber = 0;
                }
                if(cardfacelistnumber == -1 && z==-1){
                    cardfacelistnumber = 13;
                }
                cardFace = logic.cardfaces[cardfacelistnumber];

                if(selectable.cardvalue == 13 && z==1){
                    selectable.cardvalue = 1;
                }
                else if(selectable.cardvalue == 1 && z==-1){
                    selectable.cardvalue = 13;
                }
                else{
                    selectable.cardvalue+= z;
                }

                EditName(selectable.cardvalue);
                this.name = selectable.cardsuit + selectable.cardvalue;
            }

            else if(cardfacelistnumber>=13 && cardfacelistnumber <= 25){
                cardfacelistnumber+= z;
                if(cardfacelistnumber == 26 && z==1){
                    cardfacelistnumber = 13;
                }
                if(cardfacelistnumber == 12 && z==-1){
                    cardfacelistnumber = 25;
                }
                cardFace = logic.cardfaces[cardfacelistnumber];

                if(selectable.cardvalue == 13 && z==1){
                    selectable.cardvalue = 1;
                }
                else if(selectable.cardvalue == 1 && z==-1){
                    selectable.cardvalue = 13;
                }
                else{
                    selectable.cardvalue+= z;
                }
                
                EditName(selectable.cardvalue);
                this.name = selectable.cardsuit + selectable.cardvalue;
            }

            else if(cardfacelistnumber>=26 && cardfacelistnumber <= 38){
                cardfacelistnumber+= z;
                if(cardfacelistnumber == 39 && z==1){
                    cardfacelistnumber = 26;
                }
                if(cardfacelistnumber == 25 && z==-1){
                    cardfacelistnumber = 38;
                }
                cardFace = logic.cardfaces[cardfacelistnumber];

                if(selectable.cardvalue == 13 && z==1){
                    selectable.cardvalue = 1;
                }
                else if(selectable.cardvalue == 1 && z==-1){
                    selectable.cardvalue = 13;
                }
                else{
                    selectable.cardvalue+= z;
                }

                EditName(selectable.cardvalue);
                this.name = selectable.cardsuit + selectable.cardvalue;
            }

            else if(cardfacelistnumber>=39 && cardfacelistnumber <= 51){
                cardfacelistnumber+= z;
                if(cardfacelistnumber == 52 && z==1){
                    cardfacelistnumber = 39;
                }
                if(cardfacelistnumber == 38 && z==-1){
                    cardfacelistnumber = 51;
                }
                cardFace = logic.cardfaces[cardfacelistnumber];

                if(selectable.cardvalue == 13 && z==1){
                    selectable.cardvalue = 1;
                }
                else if(selectable.cardvalue == 1 && z==-1){
                    selectable.cardvalue = 13;
                }
                else{
                    selectable.cardvalue+= z;
                }
                
                EditName(selectable.cardvalue);
                this.name = selectable.cardsuit + selectable.cardvalue;
            }
        }

        return this.name;
    }

    public void EditName(int cardvalue){

            if(cardvalue == 1){
                selectable.cardnumber = "A";
            }

            else if(cardvalue == 2){
                selectable.cardnumber = "2";
            }
            
            else if(cardvalue == 3){
                selectable.cardnumber = "3";
            }

            else if(cardvalue == 4){
                selectable.cardnumber = "4";
            }

            else if(cardvalue == 5){
                selectable.cardnumber = "5";
            }

            else if(cardvalue == 6){
                selectable.cardnumber = "6";
            }

            else if(cardvalue == 7){
                selectable.cardnumber = "7";
            }

            else if(cardvalue == 8){
                selectable.cardnumber = "8";
            }

            else if(cardvalue == 9){
                selectable.cardnumber = "9";
            }

            else if(cardvalue == 10){
                selectable.cardnumber = "10";
            }

            else if(cardvalue == 11){
                selectable.cardnumber = "J";
            }

            else if(cardvalue == 12){
                selectable.cardnumber = "Q";
            }

            else if(cardvalue == 13){
                selectable.cardnumber = "K";
            }

    }

}
