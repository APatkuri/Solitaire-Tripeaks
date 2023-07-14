using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class UILogicManager : MonoBehaviour
{

    public TriPeaksLogicManager logic;
    public Text Movecounttext;
    public Text Streaktext;
    public Text timertext;
    public Text coinstext;
    public Text wildcoincost;
    public Text undocoincost;
    public Text plusfivecoincost;

    public int moves = 0;
    public int streak = 0;
    public int initwildcost;
    public int initundocost;
    public int initplusfivecost;
    public int matchcoinsgained;
    public List<int> numstreak;
    public List<string> cardsuits;
    public List<int> blackstreaklist;
    public List<int> redstreaklist;
    public float timeInSeconds = 0f;

    public GameObject UndoCoinCost;
    public GameObject WildCardCoinCost;
    public GameObject PlusFiveCoinCost;
    public GameObject coin;
    public GameObject coinstartposition;
    public GameObject coinfinishposition;
    public GameObject wildcard;
    public GameObject wildcardfinalpostion;
    public GameObject cardgameobject;
    public GameObject talonfinalposition;
    public GameObject streakmask;

    public int initwildcardcount;
    public int initundocardcount;
    public int initplusfivecardcount;
    public int freewildcard;

    public bool wildcardavailable;
    public bool undocardavailable;
    public bool plusfivecardavailable;
    public bool check = true;
    public bool perfectstreakcheck;

    public GameObject[] streakgameobjects;
    public Sprite reddot;
    public Sprite blackdot;
    public LeverScript lever;
    public int colorcurrindex;
    public int mixcurrindex;

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

        lever = FindObjectOfType<LeverScript>();
        moves = 0;
        streak = 0;
        timeInSeconds = 0f;
        initwildcost = lever.InitialWildCardCost;
        initundocost = lever.InitialUndoCardCost;
        initplusfivecost =lever.InitialPlusFiveCost;
        matchcoinsgained = 0;
        colorcurrindex = 0;
        mixcurrindex = 0;
        numstreak.Clear();
        cardsuits.Clear();
        blackstreaklist.Clear();
        redstreaklist.Clear();
        numstreak.Add(0);
        Time.timeScale = 1;
        freewildcard = lever.FreeWildCardsCount;
        PlayerPrefs.DeleteKey("streak");
        PlayerPrefs.DeleteKey("matchcoinsgained");

        logic = FindObjectOfType<TriPeaksLogicManager>();
        Movecounttext.text = "Moves: " + moves;
        Streaktext.text = "Streak: " + streak;
        timertext.text = "00:00";

        int coins = PlayerPrefs.GetInt("totalcoins", 10000);
        coinstext.text = coins.ToString();

        wildcardavailable = true;
        undocardavailable = true;
        plusfivecardavailable = true;
        perfectstreakcheck = true;


    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(!logic.isgameover){
            Timerlogic();
        }

        if(check){
            initwildcardcount = logic.wildcardcount;
            initundocardcount = logic.undocardcount;
            initplusfivecardcount = logic.plusfivecardcount;
            check = false;
        }   

        SpecialCardsAvailable();
        Streakvisual();
        
    }

    void OnApplicationPause(bool pauseStatus){

        if(pauseStatus){

            if(!logic.isgameover){
                PlayerPrefs.SetInt("matchcoinsgained", matchcoinsgained);
                PlayerPrefs.Save();
            }
        }
    }

    public IEnumerator CoinsAnimation(Transform startpos, Vector3 endpos, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = startpos.position;
        Vector3 controlPoint = startingPos + (endpos - startingPos) / 2f + Vector3.down * 5f;

        while (elapsedTime < duration){
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            Vector3 position = (Mathf.Pow(1f - t, 2f) * startingPos + 2f * (1f - t) * t * controlPoint + Mathf.Pow(t, 2f) * endpos);

            startpos.position = position;
            yield return null;
        }
        startpos.position = endpos;
    }

    public void MoveCount(int x){
        moves = moves + x;
        Movecounttext.text = "Moves: " + moves;
    }

    public void Timerlogic(){
        if(Time.timeScale == 0){
            return;
        }
        
            timeInSeconds += Time.unscaledDeltaTime;
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
            timertext.text = timerString;
    }

    public void StreakLogic(int y, string cardsuit){
        if(y == 1){
            streak = streak + 1;
            Streaktext.text = "Streak: " + streak;
            numstreak.Add(streak);
            cardsuits.Add(cardsuit);
            ColorStreakLogic(cardsuit);
            
        }

        if(y == 0){
            streak = 0;
            Streaktext.text = "Streak: " + streak;
            numstreak.Add(0);
            redstreaklist.Add(0);
            blackstreaklist.Add(0);
        }
        
        if(y == 2){
            Streaktext.text = "Streak: " + streak;
        }

        if(y == -2){

            if(numstreak[numstreak.Count -1] == 0){
                numstreak.RemoveAt(numstreak.Count -1);
                streak = numstreak[numstreak.Count -1];
                redstreaklist.RemoveAt(redstreaklist.Count -1);
                blackstreaklist.RemoveAt(blackstreaklist.Count -1);
                Streaktext.text = "Streak: " + streak;
            }
        }

        if(y == -3){

            if(numstreak[numstreak.Count -1] != 0 && numstreak[numstreak.Count -1] != 100){
                numstreak.RemoveAt(numstreak.Count -1);
                cardsuits.RemoveAt(cardsuits.Count -1);
                redstreaklist.RemoveAt(redstreaklist.Count -1);
                blackstreaklist.RemoveAt(blackstreaklist.Count -1);
                streak = numstreak[numstreak.Count -1];
                Streaktext.text = "Streak: " + streak;
            }
        }

    }

    public string ColorCheck(string x){

        if(x == "D" || x == "H"){
            return("R");
        }
        else{
            return("B");
        }

    }

    public void ColorStreakLogic(string cardsuit){

        if(cardsuits.Count == 1){
                if(ColorCheck(cardsuit) == "R"){
                    redstreaklist.Add(1);
                    blackstreaklist.Add(0);
                }

                else{
                    blackstreaklist.Add(1);
                    redstreaklist.Add(0);
                }
            }

            else if((streak > lever.streaklength) && (streak%lever.streaklength == 1)){

                if(ColorCheck(cardsuit) == "R"){
                    redstreaklist.Add(1);
                    blackstreaklist.Add(0);
                }

                else{
                    blackstreaklist.Add(1);
                    redstreaklist.Add(0);
                }
            }

            else{
                
                if(ColorCheck(cardsuits[cardsuits.Count-2]) != ColorCheck(cardsuits[cardsuits.Count -1])){

                    if(ColorCheck(cardsuits[cardsuits.Count -1]) == "R"){
                        blackstreaklist.Add(0);
                        redstreaklist.Add(1);
                    }
                    else{
                        blackstreaklist.Add(1);
                        redstreaklist.Add(0);
                    }
                    
                }

                else{
                    if(ColorCheck(cardsuits[cardsuits.Count -1]) == "R"){
                        int a = redstreaklist[redstreaklist.Count -1];
                        redstreaklist.Add(a+1);
                        blackstreaklist.Add(0);
                    }

                    else{
                        int a = blackstreaklist[blackstreaklist.Count -1];
                        blackstreaklist.Add(a+1);
                        redstreaklist.Add(0);
                    }
                }
            }
    }

    public void GameCoinsLogic(int x){

        if(x == 1){
            int coins = PlayerPrefs.GetInt("totalcoins", 10000);
            if(streak == 1){
                coins += lever.playingcardstartingreward;
                matchcoinsgained += lever.playingcardstartingreward;
            }

            else{
                if(lever.playingcardincrreward - streak > 1){
                    coins += (lever.playingcardstartingreward + (lever.playingcardincrreward - streak));
                    matchcoinsgained += (lever.playingcardstartingreward  + (lever.playingcardincrreward - streak));
                }

                else{
                    coins += (lever.playingcardstartingreward +1);
                    matchcoinsgained += (lever.playingcardstartingreward +1);
                }
            }
            
            PlayerPrefs.SetInt("totalcoins", coins);
            coinstext.text = coins.ToString();
        }

        if(x == 2){
            WildCardCoinCost.SetActive(true);

            if(freewildcard > 0){
                freewildcard--;
                wildcoincost.text = initwildcost.ToString();
            }

            else{
                
                if(logic.wildcardcount <= initwildcardcount){
                
                    int coins = PlayerPrefs.GetInt("totalcoins", 10000);
                    if(coins >= initwildcost){
                        coins = coins - initwildcost;
                        PlayerPrefs.SetInt("totalcoins", coins);
                        initwildcost += lever.WildCardIncrCost;
                        wildcoincost.text = initwildcost.ToString();
                    }
                }
            }
            
        }

        if(x == 3){
            UndoCoinCost.SetActive(true);
            
            if(logic.undocardcount <= initundocardcount){
               
                int coins = PlayerPrefs.GetInt("totalcoins", 10000);
                if(coins >= initundocost){
                    coins = coins - initundocost;
                    PlayerPrefs.SetInt("totalcoins", coins);
                    initundocost += lever.UndoCardIncrCost;
                    undocoincost.text = initundocost.ToString();
                }
            }
            
        }

        if(x == 4){
            
            if(logic.plusfivecardcount <= initplusfivecardcount){
               
                int coins = PlayerPrefs.GetInt("totalcoins", 10000);
                if(coins >= initplusfivecost){
                    coins = coins - initplusfivecost;
                    PlayerPrefs.SetInt("totalcoins", coins);
                    initplusfivecost += lever.PlusFiveIncrCost;
                    plusfivecoincost.text = initplusfivecost.ToString();
                }
            }
            
        }

    }
    
    public void SpecialCardsAvailable(){

        int coins = PlayerPrefs.GetInt("totalcoins", 10000);
        coinstext.text = coins.ToString();

        if(coins < initwildcost){
            wildcoincost.color = Color.red;
            wildcardavailable = false;
        }

        if(coins > initwildcost){
            wildcoincost.color = Color.black;
            wildcardavailable = true;
        }

        if(logic.wildcardcount == initwildcardcount){

            if(lever.InitialWildCardCost > 0){
                initwildcost = lever.InitialWildCardCost;
                wildcoincost.text = initwildcost.ToString();
            }

            else{
                WildCardCoinCost.SetActive(false);
            }
        }

        if(logic.undocardcount == initundocardcount){

            if(lever.InitialUndoCardCost > 0){
                initundocost = lever.InitialUndoCardCost;
                undocoincost.text = initundocost.ToString();
            }
            else{
                UndoCoinCost.SetActive(false);
            }
            
        }

        if(coins < initundocost){
            undocoincost.color = Color.red;
            undocardavailable = false;
        }

        if(coins > initundocost){
            undocoincost.color = Color.black;
            undocardavailable = true;
        }

        if(freewildcard > 0){
            WildCardCoinCost.SetActive(true);
            wildcoincost.text = "FREE";
        }

        if(logic.deck.Count == 0 && logic.plusfivecardcount <= initplusfivecardcount){
            PlusFiveCoinCost.SetActive(true);
        }

        if(logic.deck.Count != 0 || logic.plusfivecardcount == 0){
            PlusFiveCoinCost.SetActive(false);
        }

        if(coins < initplusfivecost){
            plusfivecoincost.color = Color.red;
            plusfivecardavailable = false;
        }

        if(coins > initplusfivecost){
            plusfivecoincost.color = Color.black;
            plusfivecardavailable = true;
        }
    }

    public void Streakvisual(){

        int streakcopy=0;
        if(streak > 0){

            foreach(GameObject g in streakgameobjects){
                g.GetComponent<SpriteRenderer>().sprite = null;
            }

            if(streak%lever.streaklength != 0){
                int streakvalue = PlayerPrefs.GetInt("streak", 0);
                perfectstreakcheck = true;
                if(streak > streakvalue){
                    PlayerPrefs.SetInt("streak", streak);
                }
                streakcopy = streak%lever.streaklength;
            }

            if((streak%lever.streaklength == 0) && perfectstreakcheck){
                streakcopy = lever.streaklength;

                int streakvalue = PlayerPrefs.GetInt("streak", 0);

                if(streak > streakvalue){
                    PlayerPrefs.SetInt("streak", streak);

                    if(redstreaklist[redstreaklist.Count-1] == lever.streaklength || blackstreaklist[blackstreaklist.Count-1] == lever.streaklength){

                        if(lever.colorstreakrewardlist.Count > 0){
                            StartCoroutine(RewardType(lever.colorstreakrewardlist[colorcurrindex]));
                            colorcurrindex++;
                            lever.streakindex++;
                            lever.StreakLengthManager();
                            StreakUIChanger(lever.streaklength);
                            streak = 0;
                            // lever.streakindex++;

                            if(colorcurrindex >= lever.colorstreakrewardlist.Count){
                                colorcurrindex =0;
                            }
                        }
                    }

                    else{
                        if(lever.mixstreakrewardlist.Count > 0){
                            StartCoroutine(RewardType(lever.mixstreakrewardlist[mixcurrindex]));
                            mixcurrindex++;
                            lever.streakindex++;
                            lever.StreakLengthManager();
                            StreakUIChanger(lever.streaklength);
                            streak = 0;
                            // lever.streakindex++;
                            
                            if(mixcurrindex >= lever.mixstreakrewardlist.Count){
                                mixcurrindex =0;
                            }
                        }
                    }
                }
            }

            for(int i=0; i<streakcopy; i++){

                if(cardsuits[cardsuits.Count -streakcopy +i] == "H" || cardsuits[cardsuits.Count - streakcopy +i] == "D"){
                    streakgameobjects[i].GetComponent<SpriteRenderer>().sprite = reddot;
                }

                else{
                    streakgameobjects[i].GetComponent<SpriteRenderer>().sprite = blackdot;
                }
            }
        }

        if(streak == 0){
            perfectstreakcheck = true;
            PlayerPrefs.DeleteKey("streak");
            StreakUIChanger(lever.streaklength);

            foreach(GameObject g in streakgameobjects){
                g.GetComponent<SpriteRenderer>().sprite = null;
            }
        }
    }

    public void StreakUIChanger(int x){

        if(x == 4){
            streakmask.transform.localScale = new Vector3(0.88f, 0.7267858f, 0.7267858f);
            streakmask.transform.localPosition = new Vector3(-798.55f, -713.35f, 2.29388f);
        }
        else if(x == 5){
            streakmask.transform.localScale = new Vector3(1.09f, 0.7267858f, 0.7267858f);
            streakmask.transform.localPosition = new Vector3(-798.3f, -713.35f, 2.29388f);
        }
        else if(x == 6){
            streakmask.transform.localScale = new Vector3(1.4f, 0.7267858f, 0.7267858f);
            streakmask.transform.localPosition = new Vector3(-798f, -713.35f, 2.29388f);;
        }

    }

    public IEnumerator RewardType(int x){

        if(x == 0)
        {
            int coins = PlayerPrefs.GetInt("totalcoins", 10000);
            coins = coins + lever.streakrewardcoin;
            PlayerPrefs.SetInt("totalcoins", coins);
            GameObject a = Instantiate(coin, coinstartposition.transform.position, Quaternion.identity);
            StartCoroutine(CoinsAnimation(a.transform, coinfinishposition.transform.position, 1));
            Destroy(a, 1.1f);
            perfectstreakcheck = false;
        }
        
        if(x==1)
        {
            for(int i=0; i<lever.streakrewardwildcount; i++){
                GameObject c = Instantiate(wildcard, coinstartposition.transform.position, Quaternion.identity, talonfinalposition.transform);
                c.transform.localScale = new Vector3((float)0.3, (float)0.3, (float)0.3);
                StartCoroutine(CoinsAnimation(c.transform, talonfinalposition.transform.position, 1));
                Destroy(c, 1.1f);
                // freewildcard++;
                logic.TalonCardsGraphic(4, 1);
                logic.deck.Add("Wild");
                perfectstreakcheck = false;
                yield return new WaitForSeconds(0.1f);
            }
        }

        if(x==2){
            
            for(int i=0; i<lever.streakrewardplusX; i++){
                GameObject c = Instantiate(cardgameobject, coinstartposition.transform.position, Quaternion.identity, talonfinalposition.transform);
                c.transform.localScale = new Vector3((float)0.3, (float)0.3, (float)0.3);
                StartCoroutine(CoinsAnimation(c.transform, talonfinalposition.transform.position, 1));
                Destroy(c, 1.1f);
                List<string> cards = TriPeaksLogicManager.GenerateDeck();
                int j = Random.Range(0, 51);
                logic.TalonCardsGraphic(4, 1);
                logic.deck.Add(cards[j]);
                // freewildcard++;
                perfectstreakcheck = false;
                yield return new WaitForSeconds(0.1f);
            }
        }

        if(x==3){
            int gems = PlayerPrefs.GetInt("gems", 500);
            gems = gems + lever.streakrewardgems;
            PlayerPrefs.SetInt("gems", gems);
            GameObject d = Instantiate(coin, coinstartposition.transform.position, Quaternion.identity);
            StartCoroutine(CoinsAnimation(d.transform, coinfinishposition.transform.position, 1));
            Destroy(d, 1.1f);
            perfectstreakcheck = false;
        }
    }   
}
