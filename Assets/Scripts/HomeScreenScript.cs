using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScreenScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] gamepart;
    public Text coinstext;
    public Text buttonText;

    public Button harvestbutton;
    public Button achievementsbutton;
    public Button minigamebutton;
    public float delay = 600f;
    private float lastClickTime;
    public int Entryfee;

    public Sprite BlackDot;
    public Sprite RedDot;

    public LeverScript lever;

    void Awake(){

        lever = FindObjectOfType<LeverScript>();
        // PlayerPrefs.SetInt("totalcoins", 10000);
        Entryfee = lever.MatchEntryFee;
        PlayerPrefs.SetInt("highestlevel", 10);
        // PlayerPrefs.DeleteKey("highestlevel");
    }

    void Start()
    {
        harvestbutton.onClick.AddListener(ButtonClicked);
        achievementsbutton.onClick.AddListener(LevelChange);
        minigamebutton.onClick.AddListener(MiniGame);

        lastClickTime = PlayerPrefs.GetFloat("lastClickTime", -delay);
        StartCoroutine(UpdateButtonText());

        // harvestbutton.onClick.AddListener(ResetTimer);

        foreach(var o in gamepart){

            if (o != null)
            {
                if (o.GetComponent<DontDestroyOnLoadTag>() == null)
                {
                    DontDestroyOnLoad(o);
                    o.AddComponent<DontDestroyOnLoadTag>();
                }
                else
                {
                    Destroy(o);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CoinsHome();
        LevelActive();
        GameQuitCoinsLogic();
    }

    public void CoinsHome(){
        int coins = PlayerPrefs.GetInt("totalcoins", 10000);
        coinstext.text = coins.ToString();
    }

    public void LevelSceneChange(int x){
        string a = "Level" + x.ToString();
        int highestlevel = PlayerPrefs.GetInt("highestlevel", 1);


        int coins = PlayerPrefs.GetInt("totalcoins", 10000);

        if( (lever.freeentryticket>0 && x <= highestlevel) || (lever.freelevel.Contains(x))){
            PlayerPrefs.SetInt("currlevel", x);
            PlayerPrefs.SetInt("totalcoins", coins);

            foreach(GameObject obj in gamepart){
                obj.SetActive(true);
            }
            SceneManager.LoadScene(a);
        }

        else if(((coins - (Entryfee + ExtraFee(x))) > 0) && (x <= highestlevel)){

            PlayerPrefs.SetInt("currlevel", x);

            coins = coins - (Entryfee + ExtraFee(x));
            PlayerPrefs.SetInt("totalcoins", coins);

            foreach(GameObject obj in gamepart){
                obj.SetActive(true);
            }
            SceneManager.LoadScene(a);
        }
    }

    public int ExtraFee(int y){
        return ((int)(Mathf.Floor((y/lever.matchfeerange)) * lever.MatchIncrFee));
    }

    public void LevelChange(){

        foreach(GameObject obj in gamepart){
                Destroy(obj);
        }
        SceneManager.LoadScene("Achievements");
    }

    public void MiniGame(){

        foreach(GameObject obj in gamepart){
                Destroy(obj);
        }
        SceneManager.LoadScene("MiniGame");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if(pauseStatus){
            PlayerPrefs.SetFloat("lastClickTime", lastClickTime);
            PlayerPrefs.Save();
        }
    }

    void ButtonClicked()
    {
        if (Time.unscaledTime - lastClickTime >= delay)
        {
            Debug.Log("Button clicked after 10 minutes");
            lastClickTime = Time.unscaledTime;
            buttonText.text = "Collect Coins";
            int coins = PlayerPrefs.GetInt("totalcoins", 10000);
            coins += 1000;
            PlayerPrefs.SetInt("totalcoins", coins);
        }
        else
        {
            float remainingTime = delay - (Time.unscaledTime - lastClickTime);
            Debug.Log("Button is inactive. Remaining time: " + remainingTime.ToString("F0") + " seconds");
            buttonText.text = remainingTime.ToString("F0") + "s";
        }
    }

    IEnumerator UpdateButtonText()
    {
        while (true)
        {
            if (Time.unscaledTime - lastClickTime >= delay){
                buttonText.text = "Collect Coins";
            }

            if (Time.unscaledTime - lastClickTime < delay)
            {
                float remainingTime = delay - (Time.unscaledTime - lastClickTime);
                buttonText.text = remainingTime.ToString("F0") + "s";
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void ResetTimer()
    {
        lastClickTime = -delay;
        buttonText.text = "Collect Coins";
    }

    public void LevelActive(){
        
        foreach(GameObject x in GameObject.FindGameObjectsWithTag("LevelButton")){
            int a = PlayerPrefs.GetInt("highestlevel", 1);
            if(int.Parse(x.name) > a){
                x.GetComponent<Image>().sprite = RedDot;
                x.GetComponent<Button>().interactable = false;
            }
            else{
                x.GetComponent<Image>().sprite = BlackDot;
                x.GetComponent<Button>().interactable = true;
            }
        }
    }

    public void GameQuitCoinsLogic(){

        if(PlayerPrefs.GetInt("matchcoinsgained", 0) != 0){

            int coinsgained = PlayerPrefs.GetInt("matchcoinsgained");
            int coins = PlayerPrefs.GetInt("totalcoins", 10000);
            if((coins - coinsgained) > Entryfee){
                PlayerPrefs.SetInt("totalcoins", coins - coinsgained);
                PlayerPrefs.SetInt("matchcoinsgained", 0);
            }
            else{
                PlayerPrefs.SetInt("totalcoins", Entryfee);
                PlayerPrefs.SetInt("matchcoinsgained", 0);
            }
        }
    }
}
