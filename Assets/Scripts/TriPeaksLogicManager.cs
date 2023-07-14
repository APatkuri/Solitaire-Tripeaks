using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class TriPeaksLogicManager : MonoBehaviour
{

    public Sprite[] cardfaces;
    public Sprite cardback;
    public Sprite wildcardsprite;
    public Sprite undosprite;


    public GameObject cardgameobject;
    public GameObject wildcardgameobject;
    public GameObject plusfivegameobject;
    public GameObject[] playingarea;
    public GameObject talonarea;
    public GameObject pilearea;
    public GameObject wildcardarea;
    public GameObject undoarea;
    public GameObject gameoverscreen;
    public GameObject exitgamescreen;
    private GameObject playingarearoot;
    public GameObject wincoingameobject;
    public GameObject coin;
    public GameObject mousegameobject;
    public GameObject cheesegameobject;


    public int wildcardcount;
    public int undocardcount;
    public int plusfivecardcount;


    public static string[] suits = new string[] {"C", "D", "H", "S"};
    public static string[] values = new string[] {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};
    public int TotalCardNumber;


    public List<string> deck;
    public List<string> playingareacards;
    public List<string> piledeck;
    public List<string> activecards;
    public List<string> Movesavailable;
    public List<string> additionalcards;


    public float pilezoffset;
    public Text Movestext;
    public Text GameOvertext;
    public bool isgameover = false;
    public bool movesavilablecheck;
    public bool levelcheck;
    public bool achievementcheck;
    public bool wingamerewardcheck;
    public bool deckcheck;

    
    public UILogicManager uilogic;
    public Animator wrongmoveanimation;
    public int wrongclicksnumber = 0;
    public int maxrownum;

    // Start is called before the first frame update
    


    public LeverScript lever;
    public GameObject extra;

    public GameObject[] objectsToDestroy;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){

        objectsToDestroy = GameObject.FindGameObjectsWithTag("DontDestroyOnLoad");
        lever = FindObjectOfType<LeverScript>();

        piledeck.Clear();
        activecards.Clear();
        playingareacards.Clear();
        Movesavailable.Clear();
        movesavilablecheck = false;
        ClearPileArea();

        isgameover = false;
        gameoverscreen.SetActive(false);
        exitgamescreen.SetActive(false);
        levelcheck = false;
        achievementcheck = false;
        wingamerewardcheck= false;
        deckcheck = false;

        talonarea.GetComponent<SpriteRenderer>().sprite = cardback;
        wildcardarea.GetComponent<SpriteRenderer>().sprite = wildcardsprite;
        undoarea.GetComponent<SpriteRenderer>().sprite = undosprite;
        uilogic = FindObjectOfType<UILogicManager>();

        wildcardcount = lever.WildCardCount;
        undocardcount = lever.UndoCardCount;
        plusfivecardcount = lever.PlusFiveCardCount;

        pilezoffset = 0.03f;
        
        TotalCardNumber = GameObject.FindGameObjectsWithTag("PlayingArea").Length;
        playingarea = new GameObject[TotalCardNumber];
        playingarearoot = GameObject.FindGameObjectWithTag("PlayingAreaRoot");


        for (int i = 0; i < playingarearoot.transform.childCount; i++) {
            playingarea[i] = playingarearoot.transform.GetChild(i).gameObject;
            playingarea[i].name = i.ToString();
        }
        
        if(lever.hiditemsdict.Count != 0 || lever.mousecheese.Count != 0){
            extra = Instantiate(new GameObject(), playingarearoot.transform);
            extra.name = "extra";
        }
        
        PlayCards();
        StartCoroutine(PlayingAreaHiddenCards());
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(GameOver());
        StartCoroutine(MovePossible());
        StartCoroutine(PlayingAreaHiddenCards());
    }

    public IEnumerator CardAnimation(Transform startpos, Vector3 endpos, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = startpos.position;
        Quaternion startingRot = startpos.rotation;
        Quaternion endRotation = startpos.rotation * Quaternion.Euler(0f, 0f, Mathf.Sign(-startingPos.x + endpos.x) * 180f);

        float totalRotation = 4f * 360f;

        Vector3 controlPoint = startingPos + (endpos - startingPos) / 2f + Vector3.up * 10f;

        float durationPlusDelta = duration + Time.deltaTime;

        while (elapsedTime <= durationPlusDelta)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            Vector3 position = Mathf.Pow(1f - t, 2f) * startingPos + 2f * (1f - t) * t * controlPoint + Mathf.Pow(t, 2f) * endpos;

            Quaternion rotation;
            if (t < 0.1f || t > 0.9f) // Threshold for using LerpUnclamped
            {
                rotation = Quaternion.LerpUnclamped(startingRot, endRotation, t);
            }
            else
            {
                rotation = Quaternion.Lerp(startingRot, endRotation, Mathf.Lerp(0f, totalRotation, t / 1f) % 360f / 360f);
            }

            startpos.position = position;
            startpos.rotation = rotation;
            yield return null;
        }

        startpos.rotation = Quaternion.Euler(0f, 0f, 0f);
        startpos.position = endpos;
    }

    public IEnumerator TalonAnimation(Transform startpos, Vector3 endpos, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = startpos.position;

        while (elapsedTime < duration){
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            Vector3 position = Vector3.Lerp(startingPos, endpos, t);
            startpos.position = position;
            yield return null;
        }
        startpos.position = endpos;
    }

    public IEnumerator DealCardsAnimation(Transform startpos, Vector3 endpos, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = startpos.position;
        // Quaternion startingRot = startpos.rotation * Quaternion.Euler(Mathf.Sign(-startingPos.x + endpos.x) * 90f, Mathf.Sign(-startingPos.x + endpos.x) * 90f, 0f);
        // Quaternion endRotation = startpos.rotation * Quaternion.Euler(0f, 0f, 0f);

        while (elapsedTime < duration){
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            Vector3 position = Vector3.Lerp(startingPos, endpos, t);
            // Quaternion rotation;
            // rotation = Quaternion.LerpUnclamped(startingRot, endRotation, t);

            startpos.position = position;
            // startpos.rotation = rotation;
            yield return null;
        }
        
        // startpos.rotation = endRotation;
        startpos.position = endpos;
    }

    public void PlayCards(){
        deck = GenerateDeck();
        Shuffle(deck);
        
        PlayingAreaCards();
        StartCoroutine(DealCards());
        StartCoroutine(MovePossible());
    }

    public static List<string> GenerateDeck(){

        List<string> newDeck = new List<string>();
        foreach(string s in suits)
        {
            foreach(string v in values)
            {
                newDeck.Add(s+v);
            }
        }

        return newDeck;
    }

    void Shuffle<T>(List<T> list)  
    {  
        System.Random random = new System.Random();
        int n = list.Count;  
        while (n > 1) 
        {  
            int k = random.Next(n);
            n--;  
            T temp = list[k];  
            list[k] = list[n];  
            list[n] = temp;  
        }  
    }

    void PlayingAreaCards(){

        // playingarea = new GameObject[TotalCardNumber];
        for(int i=0; i<TotalCardNumber; i++){ 
            // playingarea[i] = GameObject.FindGameObjectsWithTag("PlayingArea")[i];
            playingareacards.Add(deck.Last<string>());
            deck.RemoveAt(deck.Count - 1);

        }
        
        maxrownum = playingarea[TotalCardNumber-1].GetComponent<PlayingAreaScript>().rownumber;
    }

    IEnumerator DealCards(){

        float zoffset = 0.03f;
        float hidzoffset = 0.045f;
        float mousezoffset = 0.045f;
        float cheesezoffset = 0.015f;
        int i = 0;
        int prevrownum = 1;
        int currrownum;
        foreach(string c in playingareacards){

            yield return new WaitForSeconds(0.01f);

            currrownum = playingarea[i].GetComponent<PlayingAreaScript>().rownumber;

            if(currrownum > prevrownum){
                prevrownum = currrownum;
                // yield return new WaitForSeconds(0.2f);
                zoffset += 0.03f;
                hidzoffset += 0.03f;
                mousezoffset += 0.03f;
                cheesezoffset += 0.03f;
            }

            if(lever.hiditemsdict.ContainsKey(i)){
                GameObject hiditems = Instantiate(wildcardgameobject, new Vector3(0, 6, 0), playingarea[i].transform.rotation * Quaternion.Euler(0, 0, -30), extra.transform);
                hiditems.tag = "Untagged";
                hiditems.name = "hiditems" + i.ToString();
                // extra.transform.localScale = new Vector3(2, 2, 2);
                StartCoroutine(DealCardsAnimation(hiditems.transform, new Vector3(playingarea[i].transform.position.x, playingarea[i].transform.position.y, playingarea[i].transform.position.z-hidzoffset), 0.4f));
            }

            if(lever.mousecheese.ContainsKey(i)){
                if(((TotalCardNumber-1) > i) && ((TotalCardNumber-1) > lever.mousecheese[i])){
                    GameObject mouse = Instantiate(mousegameobject, new Vector3(0, 6, 0), playingarea[i].transform.rotation, extra.transform);
                    mouse.name = "mouse" + i.ToString();
                    mouse.transform.localScale = new Vector3(2, 2, 2);
                    StartCoroutine(DealCardsAnimation(mouse.transform, new Vector3(playingarea[i].transform.position.x, playingarea[i].transform.position.y, playingarea[i].transform.position.z-mousezoffset), 0.4f));
                }
            }

            if(lever.mousecheese.ContainsValue(i)){
                int localkeys = lever.mousecheese.FirstOrDefault(x => x.Value == i).Key;
                if(((TotalCardNumber-1) > i) && ((TotalCardNumber-1) > localkeys)){
                    GameObject cheese = Instantiate(cheesegameobject, new Vector3(0, 6, 0), playingarea[i].transform.rotation, extra.transform);
                    cheese.name = "cheese" + i.ToString();
                    cheese.transform.localScale = new Vector3(2, 2, 2);
                    StartCoroutine(DealCardsAnimation(cheese.transform, new Vector3(playingarea[i].transform.position.x, playingarea[i].transform.position.y, playingarea[i].transform.position.z-cheesezoffset), 0.4f));
                }
            }


            GameObject newcard = Instantiate(cardgameobject, new Vector3(0, 6, 0), playingarea[i].transform.rotation, playingarea[i].transform);
            StartCoroutine(DealCardsAnimation(newcard.transform, new Vector3(playingarea[i].transform.position.x, playingarea[i].transform.position.y, playingarea[i].transform.position.z - zoffset), 0.4f));
            newcard.name = c;
            newcard.GetComponent<Selectable>().cardinitlocation = i.ToString(); 
            newcard.GetComponent<Selectable>().isPlayingAreaCard = true;
            newcard.GetComponent<Selectable>().isSpecialIncreamentCard = false;

            if(lever.specialincrcardpos.Contains(i)){
                newcard.GetComponent<Selectable>().isSpecialIncreamentCard = true;
            }

            if(playingarea[i].GetComponent<PlayingAreaScript>().children.Count == 0){
                Animator flipcard = newcard.GetComponent<Animator>();
                flipcard.Play("FlipCard");
                yield return new WaitForSeconds(0.1f);
                newcard.GetComponent<Selectable>().FaceUp = true;
            }
            i++;
            
        }

        // InitFaceUps();

        GameObject pilenewcard = Instantiate(cardgameobject, new Vector3(pilearea.transform.position.x , pilearea.transform.position.y, pilearea.transform.position.z - pilezoffset), Quaternion.identity, pilearea.transform);
        pilenewcard.name = deck.Last<string>();
        pilenewcard.GetComponent<Selectable>().FaceUp = true;
        piledeck.Add(deck.Last<string>());
        deck.RemoveAt(deck.Count -1);
        pilezoffset += 0.03f;

        if(!deckcheck){
            List<string> index;
            index = SelectRandomElements(deck, lever.InitialTalonCardCount);
            deck.Clear();
            deck = index;
            if(lever.AddWildCardInStack){
                foreach(int k in lever.WildcardPositionInStack){
                    lever.AddWildCard(k);
                }
            }
            deckcheck = true;
        }

        TalonCardsGraphic(1, 0);
    }

    public void TalonCardsGraphic(int y, int o){

        GameObject talonparent = talonarea.transform.parent.gameObject;
        if(deck.Count > 0){
            if(deck.Last<string>() != "Wild"){
                talonarea.GetComponent<SpriteRenderer>().sprite = cardback;
            }
            else{
                talonarea.GetComponent<SpriteRenderer>().sprite = wildcardsprite;
            }
        }
        float xoffset = 0.15f;

        if(y==1){
            if((deck.Count-2) >= 0){

                for (int j = deck.Count - 2; j >= 0; j--){
                    GameObject x = Instantiate(talonarea, new Vector3(talonarea.transform.position.x -xoffset, talonarea.transform.position.y, talonarea.transform.position.z +xoffset), Quaternion.identity, talonparent.transform);
                    x.GetComponent<SpriteRenderer>().sprite = cardback;
                    x.name = "Talon" + (deck.Count-2-j).ToString();
                    xoffset += 0.15f;
                }

            }

        }

        if(y==2){

            if((deck.Count-2) >= 0){
                Destroy(GameObject.Find("Talon" + (deck.Count-2).ToString()));
                xoffset -= 0.15f;
            }

        }

        if(y==3){

            if((deck.Count-1) >= 0){
                GameObject k;
                if(deck.Count-2 >=0){
                    k = GameObject.Find("Talon" + (deck.Count-2).ToString());
                }
                else{
                    k = talonarea;
                }
                
                GameObject x = Instantiate(k, new Vector3(k.transform.position.x -xoffset, k.transform.position.y, k.transform.position.z +xoffset), Quaternion.identity, talonparent.transform);
                x.GetComponent<SpriteRenderer>().sprite = cardback;
                x.name = "Talon" + (deck.Count-1).ToString();
                xoffset += 0.15f;
            }

        }

        if(y==4){
            if((deck.Count-1) >= 0){
                GameObject z;
                if(deck.Count-2 >=0){
                    z = Enumerable.Range(0, int.MaxValue).Select(i => GameObject.Find("Talon" + i)).TakeWhile(obj => obj != null).Last();
                }
                else{
                    z = talonarea;
                }

                if(o >= 1){

                    for (int j = 1; j <=o; j++){
                        GameObject x = Instantiate(z, new Vector3(z.transform.position.x -xoffset, z.transform.position.y, z.transform.position.z +xoffset), Quaternion.identity, talonparent.transform);
                        x.GetComponent<SpriteRenderer>().sprite = cardback;
                        x.name = "Talon" + (deck.Count-2+j).ToString();
                        xoffset += 0.15f;
                    }

                }

            }

        }

    }

    List<T> SelectRandomElements<T>(List<T> originalList, int count)
    {
        List<T> selectedElements = new List<T>();

        while (selectedElements.Count < count && originalList.Count > 0)
        {
            int randomIndex = Random.Range(0, originalList.Count);
            T element = originalList[randomIndex];
            selectedElements.Add(element);
            originalList.RemoveAt(randomIndex);
        }

        return selectedElements;
    }

    public void DealCardstoTalon(){

        wrongclicksnumber = 0;
        
        if(deck.Count > 0){
            talonarea.GetComponent<SpriteRenderer>().sprite = cardback;
            TalonCardsGraphic(2, 0);

            uilogic.MoveCount(1);
            if(Movesavailable[Movesavailable.Count - 1] != "0"){
                if(Movesavailable[Movesavailable.Count - 1] != "Wild"){
                    Animator hintcard = GameObject.Find(Movesavailable[Movesavailable.Count - 1]).GetComponent<Animator>();
                    hintcard.Play("HintCard");
                }
            }
            movesavilablecheck = false;

            foreach(int u in lever.specialincrcardpos){
                if(u < TotalCardNumber-1){
                    GameObject h = playingarea[u].transform.GetChild(0).gameObject;
                    int oldindex = playingareacards.IndexOf(h.name);
                    string modcard = h.GetComponent<UpdateSprite>().IncreamentCardLogic(1);
                    playingareacards[oldindex] = modcard;
                }
            }

            if(deck.Last<string>() == "Wild"){
                uilogic.StreakLogic(2, "W");
                GameObject newcard = Instantiate(wildcardgameobject, talonarea.transform.position, Quaternion.identity, pilearea.transform);
                Animator flipcard = newcard.GetComponent<Animator>();
                flipcard.Play("FlipCard");
                StartCoroutine(TalonAnimation(newcard.transform, new Vector3(pilearea.transform.position.x , pilearea.transform.position.y, pilearea.transform.position.z - pilezoffset), 0.2f));

                newcard.name = deck.Last<string>();
                newcard.GetComponent<Selectable>().isDeckCardSpecial = true;
                piledeck.Add(deck.Last<string>());
                deck.RemoveAt(deck.Count -1);
                pilezoffset += 0.03f; 
            }

            else{
                uilogic.StreakLogic(0, "T");
                GameObject newcard = Instantiate(cardgameobject, talonarea.transform.position, Quaternion.identity, pilearea.transform);
                Animator flipcard = newcard.GetComponent<Animator>();
                flipcard.Play("FlipCard");
                StartCoroutine(TalonAnimation(newcard.transform, new Vector3(pilearea.transform.position.x , pilearea.transform.position.y, pilearea.transform.position.z - pilezoffset), 0.2f));
                
                newcard.name = deck.Last<string>();
                newcard.GetComponent<Selectable>().FaceUp = true;
                piledeck.Add(deck.Last<string>());
                deck.RemoveAt(deck.Count -1);
                pilezoffset += 0.03f;
            }
        }

        if (deck.Count == 0){

            if(plusfivecardcount > 0){
                GameObject newcard = Instantiate(plusfivegameobject, talonarea.transform.position, Quaternion.identity, talonarea.transform);
            }
            talonarea.GetComponent<SpriteRenderer>().sprite = null;
        }

    }

    public void ClearPileArea(){

        while(pilearea.transform.childCount > 0){
            DestroyImmediate(pilearea.transform.GetChild(0).gameObject);
        }
    }

    public void PlayingAreaLogic(GameObject selected){

        int currpilecardvalue = GameObject.Find(piledeck[piledeck.Count - 1]).GetComponent<Selectable>().cardvalue; 
        string cardloc = selected.GetComponent<Selectable>().cardinitlocation;
        string currcardsuit = selected.GetComponent<Selectable>().cardsuit;

        if((selected.GetComponent<Selectable>().cardvalue == currpilecardvalue+1 || selected.GetComponent<Selectable>().cardvalue == currpilecardvalue-1) || (selected.GetComponent<Selectable>().cardvalue==13 && currpilecardvalue==1) || (selected.GetComponent<Selectable>().cardvalue==1&& currpilecardvalue==13)){
            // selected.transform.position = new Vector3(pilearea.transform.position.x , pilearea.transform.position.y, pilearea.transform.position.z - pilezoffset);
            MouseChessLogic(int.Parse(selected.transform.parent.name));
            
            if(!lever.specialincrcardpos.Contains(int.Parse(selected.transform.parent.name))){

                foreach(int u in lever.specialincrcardpos){
                    if(u < TotalCardNumber-1){
                        GameObject h = playingarea[u].transform.GetChild(0).gameObject;
                        int oldindex = playingareacards.IndexOf(h.name);
                        string modcard = h.GetComponent<UpdateSprite>().IncreamentCardLogic(1);
                        playingareacards[oldindex] = modcard;
                    }
                }
            }
            else{
                lever.specialincrcardpos.Remove(int.Parse(selected.transform.parent.name));
            }

            for(int i=0; i<TotalCardNumber -1; i++){

                GameObject b = playingarea[i];
                if(b.GetComponent<PlayingAreaScript>().children.Contains(int.Parse(cardloc))){
                    b.GetComponent<PlayingAreaScript>().childrenduplicate.Remove(int.Parse(cardloc));
                } 
            }


            if(currcardsuit == "S" || currcardsuit == "C"){
                // AchievementManager.black_cards++;
                int black_cards = PlayerPrefs.GetInt("black_cards", 0);
                PlayerPrefs.SetInt("black_cards", black_cards+1);
            }


            selected.transform.rotation = Quaternion.identity;
            GameObject wincoin = Instantiate(wincoingameobject, selected.transform.position, Quaternion.identity);
            Animator wincoinanimator = wincoin.GetComponent<Animator>();
            wincoinanimator.Play("WinCoin");
            Destroy(wincoin, 0.5f);

            StartCoroutine(CardAnimation(selected.transform, new Vector3(pilearea.transform.position.x , pilearea.transform.position.y, pilearea.transform.position.z - pilezoffset), 0.8f));
            pilezoffset += 0.03f;
            selected.GetComponent<Selectable>().isPlayingAreaCard = false;
            selected.transform.parent = pilearea.transform;
            playingareacards.Remove(selected.name);
            piledeck.Add(selected.name);
            

            movesavilablecheck = false;
            uilogic.MoveCount(1);
            uilogic.StreakLogic(1, selected.GetComponent<Selectable>().cardsuit);
            uilogic.GameCoinsLogic(1);
            wrongclicksnumber = 0;
            StartCoroutine(PlayingAreaHiddenCards());
        }

        if(currpilecardvalue == 20){
            // selected.transform.position = new Vector3(pilearea.transform.position.x , pilearea.transform.position.y, pilearea.transform.position.z - pilezoffset);
            MouseChessLogic(int.Parse(selected.transform.parent.name));

            if(!lever.specialincrcardpos.Contains(int.Parse(selected.transform.parent.name))){

                foreach(int u in lever.specialincrcardpos){
                    if(u < TotalCardNumber-1){
                        GameObject h = playingarea[u].transform.GetChild(0).gameObject;
                        int oldindex = playingareacards.IndexOf(h.name);
                        string modcard = h.GetComponent<UpdateSprite>().IncreamentCardLogic(1);
                        playingareacards[oldindex] = modcard;
                    }

                }
            }
            else{
                lever.specialincrcardpos.Remove(int.Parse(selected.transform.parent.name));
            }

            for(int i=0; i<TotalCardNumber -1; i++){

                GameObject b = playingarea[i];
                if(b.GetComponent<PlayingAreaScript>().children.Contains(int.Parse(cardloc))){
                    b.GetComponent<PlayingAreaScript>().childrenduplicate.Remove(int.Parse(cardloc));
                } 
            }

            if(currcardsuit == "S" || currcardsuit == "C"){
                // AchievementManager.black_cards++;
                int black_cards = PlayerPrefs.GetInt("black_cards", 0);
                PlayerPrefs.SetInt("black_cards", black_cards+1);
            }

            selected.transform.rotation = Quaternion.identity;
            GameObject wincoin = Instantiate(wincoingameobject, selected.transform.position, Quaternion.identity);
            Animator wincoinanimator = wincoin.GetComponent<Animator>();
            wincoinanimator.Play("WinCoin");
            Destroy(wincoin, 0.5f);

            StartCoroutine(CardAnimation(selected.transform, new Vector3(pilearea.transform.position.x , pilearea.transform.position.y, pilearea.transform.position.z - pilezoffset), 0.8f));
            pilezoffset += 0.03f;
            selected.GetComponent<Selectable>().isPlayingAreaCard = false;
            selected.transform.parent = pilearea.transform;
            playingareacards.Remove(selected.name);
            piledeck.Add(selected.name);

            movesavilablecheck = false;
            uilogic.MoveCount(1);
            uilogic.StreakLogic(1, selected.GetComponent<Selectable>().cardsuit);
            uilogic.GameCoinsLogic(1);
            wrongclicksnumber = 0;
            StartCoroutine(PlayingAreaHiddenCards());
        }

        else{
            wrongmoveanimation = selected.GetComponent<Animator>();
            wrongmoveanimation.Play("WrongCardClick");
            wrongclicksnumber++;
        }
    }

    public void MouseChessLogic(int obj){


        if(lever.mousecheese.ContainsValue(obj)){
            GameObject a = GameObject.Find("cheese" + obj.ToString());
            foreach (KeyValuePair<int, int> pair in lever.mousecheese){
                if(pair.Value == obj){
                    GameObject b = GameObject.Find("mouse" + pair.Key);
                    StartCoroutine(uilogic.CoinsAnimation(a.transform, b.transform.position, 1));
                    Destroy(a, 1.1f);
                    Destroy(b, 1.1f);
                    break;
                }
            }
        }

    }

    public void WildCardLogic(){

        wrongclicksnumber = 0;

        if(wildcardcount > 0 && uilogic.wildcardavailable){

            foreach(int u in lever.specialincrcardpos){
                if(u < TotalCardNumber-1){
                    GameObject h = playingarea[u].transform.GetChild(0).gameObject;
                    int oldindex = playingareacards.IndexOf(h.name);
                    string modcard = h.GetComponent<UpdateSprite>().IncreamentCardLogic(1);
                    playingareacards[oldindex] = modcard;
                }
            }

            uilogic.MoveCount(1);
            uilogic.StreakLogic(2, "W");
            uilogic.GameCoinsLogic(2);
            movesavilablecheck = false;

            GameObject newcard = Instantiate(wildcardgameobject, wildcardarea.transform.position, Quaternion.identity, pilearea.transform);
            StartCoroutine(CardAnimation(newcard.transform, new Vector3(pilearea.transform.position.x , pilearea.transform.position.y, pilearea.transform.position.z - pilezoffset), 0.7f));
            newcard.name = "Wild";
            piledeck.Add("Wild");
            pilezoffset += 0.03f;
            wildcardcount--;
        }

        if(wildcardcount == 0){
            wildcardarea.GetComponent<SpriteRenderer>().sprite = null;
        }
        
    }

    public IEnumerator PlusFiveCardLogic(GameObject plusfivecard){

        if(plusfivecardcount > 0 && uilogic.plusfivecardavailable){

            DestroyImmediate(plusfivecard);
            plusfivecardcount--;
            uilogic.GameCoinsLogic(4);
            additionalcards = GenerateDeck();
            Shuffle(additionalcards);

            for(int i=0; i<5; i++){

                int index = Random.Range(0, additionalcards.Count -1);
                deck.Add(additionalcards[i]);

                GameObject newcard = Instantiate(cardgameobject, new Vector3(0, 6, 0), Quaternion.identity, talonarea.transform);
                newcard.transform.localScale = new Vector3((float)0.3, (float)0.3, (float)0.3);
                StartCoroutine(DealCardsAnimation(newcard.transform, talonarea.transform.position, 0.5f));
                Destroy(newcard, 0.6f);
                yield return new WaitForSeconds(0.1f);
            }       


            if(deck.Count > 0){
                talonarea.GetComponent<SpriteRenderer>().sprite = cardback;
                yield return new WaitForSeconds(0.4f);
                TalonCardsGraphic(1, 0);
            }

        }

        if(plusfivecardcount == 0){
            talonarea.GetComponent<SpriteRenderer>().sprite = null;
        }

    }

    IEnumerator PlayingAreaHiddenCards(){
        
        yield return new WaitForSeconds(4);

        if(playingareacards.Count >0){

            for(int i=0; i<TotalCardNumber-1; i++){

                GameObject a = playingarea[i];

                if(a.GetComponent<PlayingAreaScript>().childrenduplicate.Count == 0){
                    if(a.transform.childCount>0){
                        playingarea[i].GetComponentInChildren<Selectable>().FaceUp = true;

                        if((lever.hiditemsdict.ContainsKey(i))){
                            StartCoroutine(HiddenObjects(lever.hiditemsdict[i], playingarea[i]));
                            GameObject z = GameObject.Find("hiditems" + i.ToString());
                            Destroy(z);
                            lever.hiditemsdict.Remove(i);
                        }
                    }
                }
                
                if(a.GetComponent<PlayingAreaScript>().childrenduplicate.Count > 0){
                    playingarea[i].GetComponentInChildren<Selectable>().FaceUp = false;

                    if(lever.specialincrcardpos.Contains(i)){
                        playingarea[i].GetComponentInChildren<Selectable>().isSpecialIncreamentCard = true;
                    }

                    if(!lever.specialincrcardpos.Contains(i)){
                        playingarea[i].GetComponentInChildren<Selectable>().isSpecialIncreamentCard = false;
                    }
                }
            }
        }
    }

    public IEnumerator HiddenObjects(string x, GameObject y){
        if(x == "GrandAlbum"){
            Debug.Log("GrandAlbum");
        }

        if(x == "Gems"){
            
            int gems = PlayerPrefs.GetInt("gems", 500);
            gems = gems + lever.streakrewardgems;
            PlayerPrefs.SetInt("gems", gems);
            GameObject d = Instantiate(coin, y.transform.position, Quaternion.identity);
            StartCoroutine(uilogic.CoinsAnimation(d.transform, uilogic.coinfinishposition.transform.position, 1));
            Destroy(d, 1.1f);

        }

        if(x == "PlusXCard"){

            for(int i=0; i<lever.hiddenobjplusX; i++){
                GameObject c = Instantiate(cardgameobject, y.transform.position, Quaternion.identity, uilogic.talonfinalposition.transform);
                c.transform.localScale = new Vector3((float)0.3, (float)0.3, (float)0.3);
                StartCoroutine(uilogic.CoinsAnimation(c.transform, uilogic.talonfinalposition.transform.position, 1));
                Destroy(c, 1.1f);
                List<string> cards = TriPeaksLogicManager.GenerateDeck();
                int j = Random.Range(0, 51);
                TalonCardsGraphic(4, 1);
                deck.Add(cards[j]);
                yield return new WaitForSeconds(0.1f);
            }

        }

        if(x == "WildCard"){
            
            GameObject c = Instantiate(wildcardgameobject, y.transform.position, Quaternion.identity, uilogic.talonfinalposition.transform);
            c.transform.localScale = new Vector3((float)0.3, (float)0.3, (float)0.3);
            StartCoroutine(uilogic.CoinsAnimation(c.transform, uilogic.talonfinalposition.transform.position, 1));
            Destroy(c, 1.1f);
            TalonCardsGraphic(4, 1);
            deck.Add("Wild");

        }
    }

    public void RestartGame(){
        isgameover = false;
        gameoverscreen.SetActive(false);
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BacktoHome(){
        
        foreach(GameObject g in objectsToDestroy){
            Destroy(g);
        }
        SceneManager.LoadScene("HomeScene");
    }

    public IEnumerator GameOver(){

        yield return new WaitForSeconds(1f);

        if(playingareacards.Count == 0){
            GameOvertext.text = "YOU WIN!";
            gameoverscreen.SetActive(true);
            isgameover = true;
            RemainingCardsRewardLogic();
            if(!achievementcheck){
                int wins = PlayerPrefs.GetInt("wins", 0);
                PlayerPrefs.SetInt("wins", wins+1);
                achievementcheck = true;
            }

            int currlevel = PlayerPrefs.GetInt("currlevel", 1);
            int highestlevel = PlayerPrefs.GetInt("highestlevel", 1);
            if(((currlevel+1) > highestlevel) && !levelcheck){
                PlayerPrefs.SetInt("highestlevel", currlevel+1);
                levelcheck = true;
            }

            if(currlevel%2 == 0){
                gameoverscreen.SetActive(false);
                foreach(GameObject g in objectsToDestroy){
                    Destroy(g);
                }
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene("MiniGame");
            }
        }


        if(deck.Count == 0){
            exitgamescreen.SetActive(true);
        }

        if(deck.Count > 0){
            exitgamescreen.SetActive(false);
        }
        
    }

    public IEnumerator MovePossible(){

        yield return new WaitForSeconds(2f);
        activecards.Clear();
        Movestext.text = "";

        if(piledeck.Count > 0){

            int currpilecardvalue = GameObject.Find(piledeck[piledeck.Count - 1]).GetComponent<Selectable>().cardvalue;

            for(int i=0; i<playingareacards.Count; i++){
                if(GameObject.Find(playingareacards[i]).GetComponent<Selectable>().FaceUp){
                    activecards.Add(playingareacards[i]);
                }
            }

            if(currpilecardvalue == 20){
                Movestext.text = "WildCard";

                if(!movesavilablecheck){ 
                    Movesavailable.Add("Wild");
                    movesavilablecheck = true;        
                }
            }

            if(currpilecardvalue != 20){

                for(int i=0; i<activecards.Count; i++){
                    if((GameObject.Find(activecards[i]).GetComponent<Selectable>().cardvalue == currpilecardvalue+1 || GameObject.Find(activecards[i]).GetComponent<Selectable>().cardvalue == currpilecardvalue-1) || (GameObject.Find(activecards[i]).GetComponent<Selectable>().cardvalue==13 && currpilecardvalue==1) || (GameObject.Find(activecards[i]).GetComponent<Selectable>().cardvalue==1&& currpilecardvalue==13)){
                        
                        if(!movesavilablecheck){
                            Movesavailable.Add(activecards[i]);
                            movesavilablecheck = true;
                        }

                        Movestext.text = Movestext.text + " " +  activecards[i] + ":" + currpilecardvalue;
                        Animator hintcard = GameObject.Find(activecards[i]).GetComponent<Animator>();
                        if(wrongclicksnumber >2){
                            hintcard.Play("HintCard");
                            wrongclicksnumber = 0;
                        }
                    }
                }

                if(string.IsNullOrWhiteSpace(Movestext.text)){
                    if(!movesavilablecheck){
                        Movesavailable.Add("0");
                        movesavilablecheck = true;
                    }
                }
            }
        }
    }

    public void UndoLogic(){

        if(undocardcount > 0 && uilogic.undocardavailable){

            if(piledeck.Count > 1){
                // Debug.Log(piledeck.Last<string>());
                
                if(piledeck.Last<string>() == "Wild" && !GameObject.Find("PileCards").transform.GetChild(piledeck.Count - 1).GetComponent<Selectable>().isDeckCardSpecial){
                    wildcardcount++;
                    uilogic.MoveCount(-1);
                    uilogic.StreakLogic(2, "U");
                    uilogic.GameCoinsLogic(3);
                    Movesavailable.RemoveAt(Movesavailable.Count -1);

                    GameObject topone = GameObject.Find("PileCards").transform.GetChild(piledeck.Count -1).gameObject;
                    piledeck.RemoveAt(piledeck.Count -1);
                    Destroy(topone);

                    if(wildcardcount > 0){
                            wildcardarea.GetComponent<SpriteRenderer>().sprite = wildcardsprite;
                    }

                    foreach(int u in lever.specialincrcardpos){
                        if(u < TotalCardNumber-1){
                            GameObject h = playingarea[u].transform.GetChild(0).gameObject;
                            int oldindex = playingareacards.IndexOf(h.name);
                            string modcard = h.GetComponent<UpdateSprite>().IncreamentCardLogic(-1);
                            playingareacards[oldindex] = modcard;
                        }
                    }

                }

                else if(piledeck.Last<string>() == "Wild" && GameObject.Find("PileCards").transform.GetChild(piledeck.Count - 1).GetComponent<Selectable>().isDeckCardSpecial){
                    uilogic.MoveCount(-1);
                    uilogic.StreakLogic(2, "U");
                    uilogic.GameCoinsLogic(3);
                    TalonCardsGraphic(3, 0);
                    Movesavailable.RemoveAt(Movesavailable.Count -1);

                    GameObject topone = GameObject.Find("PileCards").transform.GetChild(piledeck.Count -1).gameObject;
                    deck.Add(piledeck.Last<string>());
                    piledeck.RemoveAt(piledeck.Count -1);
                    Destroy(topone);

                    if(deck.Count > 0){
                            talonarea.GetComponent<SpriteRenderer>().sprite = cardback;
                            while(GameObject.Find("TalonCards").transform.childCount > 0){
                                DestroyImmediate(GameObject.Find("TalonCards").transform.GetChild(0).gameObject);
                            }
                    }

                    foreach(int u in lever.specialincrcardpos){
                        if(u < TotalCardNumber-1){
                            GameObject h = playingarea[u].transform.GetChild(0).gameObject;
                            int oldindex = playingareacards.IndexOf(h.name);
                            string modcard = h.GetComponent<UpdateSprite>().IncreamentCardLogic(-1);
                            playingareacards[oldindex] = modcard;
                        }
                    }
                }

                else{
                    // if(string.IsNullOrWhiteSpace(GameObject.Find(piledeck[piledeck.Count - 1]).GetComponent<Selectable>().cardinitlocation))
                    if(string.IsNullOrWhiteSpace(GameObject.Find("PileCards").transform.GetChild(piledeck.Count - 1).GetComponent<Selectable>().cardinitlocation))
                    {
                        // Debug.Log(piledeck[piledeck.Count - 1]);
                        uilogic.MoveCount(-1);
                        uilogic.StreakLogic(-2, "U");
                        uilogic.GameCoinsLogic(3);
                        TalonCardsGraphic(3, 0);
                        Movesavailable.RemoveAt(Movesavailable.Count -1);

                        GameObject topone = GameObject.Find("PileCards").transform.GetChild(piledeck.Count -1).gameObject;
                        deck.Add(piledeck.Last<string>());
                        piledeck.RemoveAt(piledeck.Count -1);
                        Destroy(topone);

                        if(deck.Count > 0){
                                talonarea.GetComponent<SpriteRenderer>().sprite = cardback;
                                while(GameObject.Find("TalonCards").transform.childCount > 0){
                                    DestroyImmediate(GameObject.Find("TalonCards").transform.GetChild(0).gameObject);
                                }
                        }

                        foreach(int u in lever.specialincrcardpos){
                            if(u < TotalCardNumber-1){
                                GameObject h = playingarea[u].transform.GetChild(0).gameObject;
                                int oldindex = playingareacards.IndexOf(h.name);
                                string modcard = h.GetComponent<UpdateSprite>().IncreamentCardLogic(-1);
                                playingareacards[oldindex] = modcard;
                            }
                        }
                    }

                    // else if(!string.IsNullOrWhiteSpace(GameObject.Find(piledeck[piledeck.Count - 1]).GetComponent<Selectable>().cardinitlocation))
                    else if(!string.IsNullOrWhiteSpace(GameObject.Find("PileCards").transform.GetChild(piledeck.Count - 1).GetComponent<Selectable>().cardinitlocation))
                    {

                        uilogic.MoveCount(-1);
                        uilogic.StreakLogic(-3, "U");
                        uilogic.GameCoinsLogic(3);
                        Movesavailable.RemoveAt(Movesavailable.Count -1);

                        pilezoffset += 0.03f;
                        string location = GameObject.Find("PileCards").transform.GetChild(piledeck.Count - 1).GetComponent<Selectable>().cardinitlocation;

                        for(int i=0; i<TotalCardNumber -1; i++){
                            GameObject b = playingarea[i];
                            if(b.GetComponent<PlayingAreaScript>().children.Contains(int.Parse(location))){
                                b.GetComponent<PlayingAreaScript>().childrenduplicate.Add(int.Parse(location));
                            } 
                        }

                        int loc = int.Parse(location);

                        if(lever.mousecheese.ContainsValue(loc)){
                            GameObject cheese = Instantiate(cheesegameobject, new Vector3(playingarea[loc].transform.position.x, playingarea[loc].transform.position.y, playingarea[loc].transform.position.z - pilezoffset + 0.0001f), playingarea[loc].transform.rotation, extra.transform);
                            cheese.name = "cheese" + location.ToString();
                            cheese.transform.localScale = new Vector3(2, 2, 2);

                            foreach (KeyValuePair<int, int> pair in lever.mousecheese){
                                if(pair.Value == loc){
                                    GameObject b = Instantiate(mousegameobject, new Vector3(playingarea[pair.Key].transform.position.x, playingarea[pair.Key].transform.position.y, playingarea[pair.Key].transform.position.z-0.6f), playingarea[pair.Key].transform.rotation, extra.transform);
                                    b.name = "mouse" + pair.Key;    
                                    b.transform.localScale = new Vector3(2, 2, 2);
                                    break;
                                }
                            }
                        }

                        GameObject topone = GameObject.Find("PileCards").transform.GetChild(piledeck.Count -1).gameObject;
                        topone.transform.position = new Vector3(playingarea[int.Parse(location)].transform.position.x , playingarea[int.Parse(location)].transform.position.y, playingarea[int.Parse(location)].transform.position.z - pilezoffset);
                        topone.transform.rotation = playingarea[int.Parse(location)].transform.rotation;
                        topone.transform.parent = playingarea[int.Parse(location)].transform;
                        playingareacards.Add(topone.name);
                        piledeck.RemoveAt(piledeck.Count -1);
                        topone.GetComponent<Selectable>().isPlayingAreaCard = true;

                        if(topone.GetComponent<Selectable>().isSpecialIncreamentCard){
                            lever.specialincrcardpos.Add(loc);
                        }
                    }
                }
                undocardcount--;


            }

        }
        
        if(undocardcount == 0){
            undoarea.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    public void RemainingCardsRewardLogic(){
        if(deck.Count > 0 && !wingamerewardcheck){

            int coins = PlayerPrefs.GetInt("totalcoins", 10000);
            int specialindex = 0;
            for(int i=0; i<deck.Count; i++){

                if(deck[i] != "Wild"){

                    if(i==0){
                        coins = coins + lever.initregularcardsleftreward;
                    }
                    if(i>1){
                        coins = coins + lever.regularcardsleftrewardincrement;
                    }

                }

                else{

                    if(specialindex==0){
                        coins = coins + lever.initspecialcardsleftreward;
                        specialindex++;
                    }
                    if(specialindex>1){
                        coins = coins + lever.specialcardsleftrewardincrement;
                        specialindex++;
                    }

                }
                
            }
            wingamerewardcheck = true;
            PlayerPrefs.SetInt("totalcoins", coins);
        }
    }


    public int valuecard(string card){

        int cardvalue;
        string cardnumber = "";

            for(int i=1; i< card.Length; i++){

                cardnumber = cardnumber + card[i];
            }

            if(cardnumber == "A"){
                cardvalue = 1;
                return cardvalue;
            }

            else if(cardnumber == "2"){
                cardvalue = 2;
                return cardvalue;
            }
            
            else if(cardnumber == "3"){
                cardvalue = 3;
                return cardvalue;
            }

            else if(cardnumber == "4"){
                cardvalue = 4;
                return cardvalue;
            }

            else if(cardnumber == "5"){
                cardvalue = 5;
                return cardvalue;
            }

            else if(cardnumber == "6"){
                cardvalue = 6;
                return cardvalue;
            }

            else if(cardnumber == "7"){
                cardvalue = 7;
                return cardvalue;
            }

            else if(cardnumber == "8"){
                cardvalue = 8;
                return cardvalue;
            }

            else if(cardnumber == "9"){
                cardvalue = 9;
                return cardvalue;
            }

            else if(cardnumber == "10"){
                cardvalue = 10;
                return cardvalue;
            }

            else if(cardnumber == "J"){
                cardvalue = 11;
                return cardvalue;
            }

            else if(cardnumber == "Q"){
                cardvalue = 12;
                return cardvalue;
            }

            else if(cardnumber == "K"){
                cardvalue = 13;
                return cardvalue;
            }
                
            return 0;
    }

    public bool cardsplayable(int a, int b){

        if( (a == b+1) || (a == b-1) || (a == 13 && b == 1) || (a == 1 && b == 13)){
            return true;
        }

        else{
            return false;
        }
    }
}
