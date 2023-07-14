using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class LeverScript : MonoBehaviour
{
    public int InitialTalonCardCount; //Number of cards at the start of the game in talon
    public bool AddWildCardInStack; //To add a wildcard in talon or nor
    public List<int> WildcardPositionInStack; //The position from first where you want to add
    public int WildCardCount;
    public int UndoCardCount;
    public int PlusFiveCardCount;

    //PlusFiveCards are Randomcurrently

    public int MatchEntryFee;
    public int matchfeerange;
    public int MatchIncrFee;
    public int FreeWildCardsCount;
    public int InitialPlusFiveCost;
    public int InitialWildCardCost;
    public int InitialUndoCardCost;
    public int PlusFiveIncrCost;
    public int WildCardIncrCost;
    public int UndoCardIncrCost;

    public int streaklength;//ignore this
    public List<int> streaklengths;
    public int streakindex;//ignore this
    public List<int> mixstreakrewardlist;  // 0 is coins, 1 is wildcard, 2 is mixed, 3 is coins2 or gems;
    public List<int> colorstreakrewardlist;  // 0 is coins, 1 is wildcard, 2 is mixed, 3 is coins2 or gems;
    public int streakrewardcoin;
    public int streakrewardwildcount;
    public int streakrewardplusX;
    public int streakrewardgems;

    public List<int> hiddenitemsposition;
    public List<string> hiddenitemslist;
    public Dictionary<int, string> hiditemsdict = new Dictionary<int, string>();
    public int hiddenobjplusX;
    public List<int> mousepos;
    public List<int> cheesepos;
    public Dictionary<int, int> mousecheese = new Dictionary<int, int>();
    public List<int> specialincrcardpos;


    public int playingcardstartingreward; //coins you recieve on playing a correct card for the first time
    public int playingcardincrreward; //coins you recieve on playing a correct card in streak
    public int initregularcardsleftreward;
    public int regularcardsleftrewardincrement;
    public int initspecialcardsleftreward;
    public int specialcardsleftrewardincrement;

    public int freeentryticket;
    public List<int> freelevel;


    //For Theme Checking Add images to folder Resources/TestingImages
    public GameObject HomeBackground;
    public GameObject LevelBackground;
    public GameObject TriPeakLogic;
    public TriPeaksLogicManager logic1;

    void Start()
    {
        logic1 = TriPeakLogic.GetComponent<TriPeaksLogicManager>();

        //Parent PlayingArea should have a tag "PlayingAreaRoot"
        //Children should have tag "PlayingArea" and define its children
        //Also mention its rownumber

        hiditemsdict.Clear();
        mousecheese.Clear();
        AddHiddenObj();
        streakindex = 0;
        // AssignSprites();
    }

    void Update()
    {
        StreakLengthManager();
    }

    public void AddWildCard(int WildcardPositionInStack){
        FindObjectOfType<TriPeaksLogicManager>().deck.Insert(-WildcardPositionInStack + FindObjectOfType<TriPeaksLogicManager>().deck.Count,"Wild");
    }

    public void AddHiddenObj(){
        if(hiddenitemsposition.Count == hiddenitemslist.Count){
            for(int i=0; i<hiddenitemsposition.Count; i++){
                hiditemsdict.Add(hiddenitemsposition[i], hiddenitemslist[i]);
            }
        }

        if(mousepos.Count == cheesepos.Count){
            for(int i=0; i<mousepos.Count; i++){
                mousecheese.Add(mousepos[i], cheesepos[i]);
            }
        }
    }

    public void StreakLengthManager(){
        if(streaklengths.Count > 0){

            if(streakindex >= streaklengths.Count){
                streakindex =0;
            }

            streaklength = streaklengths[streakindex];
        }
    }

    public void AssignSprites(){

        string[] filePaths = Directory.GetFiles("Assets/Resources/TestingImages", "*.jpg", SearchOption.TopDirectoryOnly)
                            .Union(Directory.GetFiles("Assets/Resources/TestingImages", "*.png", SearchOption.TopDirectoryOnly))
                            .ToArray();

        foreach (string filePath in filePaths)
        {
            byte[] imageData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);    

            string fileName = Path.GetFileNameWithoutExtension(filePath);

            if(fileName == "homebg"){
                HomeBackground.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
        }


        Sprite[] sprites = Resources.LoadAll<Sprite>("TestingImages");

        if (sprites != null && sprites.Length > 0)
        {

            var numericSprites = sprites.Where(s => int.TryParse(s.name, out _)).OrderBy(s => int.Parse(s.name));
            logic1.cardfaces = new Sprite[52];

            foreach(Sprite t in numericSprites){
                
                logic1.cardfaces[int.Parse(t.name)] = t;
            }

            foreach(Sprite s in sprites){
                
                if(s.name == "levelbg"){
                    SpriteRenderer spriteRenderer = LevelBackground.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = s;
                }

                if(s.name == "cardback"){
                    logic1.cardback = s;
                }

                if(s.name == "wildcard"){               
                    logic1.wildcardsprite = s;
                }

                if(s.name == "undo"){               
                    logic1.undosprite = s;
                }
            }
        }
        else
        {
            Debug.LogError("No sprites found. Make sure the folder path is correct and contains sprites.");
        }
    }
}
