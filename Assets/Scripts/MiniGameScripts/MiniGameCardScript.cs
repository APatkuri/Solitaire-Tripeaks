using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameCardScript : MonoBehaviour
{

    public Sprite cardFace;
    public Sprite cardBack;
    public bool FaceUp;
    public int cardvalue;
    public MiniGameLogicManager logic;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        FaceUp = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        logic = FindAnyObjectByType<MiniGameLogicManager>();
   
    }

    // Update is called once per frame
    void Update()
    {
        AssignCardDetails();
        SpriteUpdate();
    }

    public void AssignCardDetails(){

        int i = 0;
        foreach(string c in MiniGameLogicManager.cardvalues){
            if(this.name == c){
                cardFace = logic.minigamecardfaces[i];
                cardvalue = int.Parse(c);
                break;
            }
            i++;
        }
    }

    public void SpriteUpdate(){

        if(FaceUp){
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
    }
}
