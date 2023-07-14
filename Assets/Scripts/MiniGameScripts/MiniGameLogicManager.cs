using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MiniGameLogicManager : MonoBehaviour
{

    public Sprite[] minigamecardfaces;
    public GameObject minicardgameobject;
    public GameObject miniplayingarearoot;
    public GameObject[] miniplayingarea;
    public GameObject coinswinning;
    public GameObject coinfinishposition;
    public static string[] cardvalues = new string[] {"0", "50", "100", "1500"};
    public List<string> playcardslist;
    public int totalminicards;
    public bool isgameover;
    public Text coinstext;

    public GameObject Home;
    public Button Homebutton;
    public int clickcount;
    public int skullcount;

    // Start is called before the first frame update
    void Start()
    {
        isgameover = false;
        clickcount = 0;
        skullcount = Random.Range(3, 7);
        CardSetup();
        Home.SetActive(false);
        Homebutton.onClick.AddListener(BacktoHome);
    }

    // Update is called once per frame
    void Update()
    {
        CoinsHome();
    }

    public void CoinsHome(){
        int coins = PlayerPrefs.GetInt("totalcoins", 10000);
        coinstext.text = coins.ToString();
    }

    public IEnumerator DealCardsAnimation(Transform startpos, Vector3 endpos, float duration)
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

    public IEnumerator RevealCardsAnimation(Transform startpos, Vector3 endpos, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = startpos.position;
        Quaternion startingRot = startpos.rotation * Quaternion.Euler(0f, 0f, 0f);
        Quaternion endRotation = startpos.rotation * Quaternion.Euler(Mathf.Sign(-startingPos.x + endpos.x) * 90f, 0f, Mathf.Sign(-startingPos.x + endpos.x) * 90f);

        while (elapsedTime < duration){
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            Vector3 position = Vector3.Lerp(startingPos, endpos, t);
            Quaternion rotation;
            rotation = Quaternion.LerpUnclamped(startingRot, endRotation, t);

            startpos.position = position;
            startpos.rotation = rotation;
            yield return null;
        }
        
        startpos.rotation = endRotation;
        startpos.position = endpos;
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

    public void CardSetup(){

        playcardslist.Clear();
        totalminicards = GameObject.FindGameObjectsWithTag("PlayingArea").Length;
        miniplayingarea = new GameObject[totalminicards];
        miniplayingarearoot = GameObject.FindGameObjectWithTag("PlayingAreaRoot");
        int j = Random.Range(0, totalminicards -1);

        for (int i=0; i < miniplayingarearoot.transform.childCount; i++) {
            miniplayingarea[i] = miniplayingarearoot.transform.GetChild(i).gameObject;

            if(i == j){
                playcardslist.Add(cardvalues[0]);
            }
            else{
                playcardslist.Add(cardvalues[Random.Range(1,3)]);
            }
        }

        StartCoroutine(DealMiniCards());

    }

    public IEnumerator DealMiniCards(){
        
        int k=0;
        foreach(string c in playcardslist){
            yield return new WaitForSeconds(0.01f);

            GameObject a = Instantiate(minicardgameobject, new Vector3(0, 0, 0), Quaternion.identity, miniplayingarea[k].transform);
            StartCoroutine(DealCardsAnimation(a.transform, new Vector3(miniplayingarea[k].transform.position.x, miniplayingarea[k].transform.position.y, miniplayingarea[k].transform.position.z - (float)0.1), 0.4f));
            // a.name = c;
            a.GetComponent<MiniGameCardScript>().FaceUp = false; 
            k++;
        }

    }

    public void CardPlayLogic(GameObject a){

        clickcount++;

        if(clickcount == skullcount){
            a.name = "0";
        }
        else{
            a.name = cardvalues[Random.Range(1,3)];
        }
        
        Animator cardanim = a.GetComponent<Animator>();
        a.GetComponent<MiniGameCardScript>().FaceUp = true;

        if(a.name == "0"){
            cardanim.Play("SkullCard"); 
            isgameover = true;
            StartCoroutine(RevealAllCards());
            // StartCoroutine(CoinsWinning());
        }

        else{
            cardanim.Play("FlipCard");
            
        }
    }

    public IEnumerator RevealAllCards(){
        yield return new WaitForSeconds(2);
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Card")){
            if(g.GetComponent<MiniGameCardScript>().FaceUp == false){

                StartCoroutine(RevealCardsAnimation(g.transform, new Vector3(0, -6, 0), 0.8f));
                g.GetComponent<MiniGameCardScript>().FaceUp = true;
            }

            else if(g.GetComponent<MiniGameCardScript>().cardvalue != 0){
                GameObject b = Instantiate(coinswinning, g.transform.position, Quaternion.identity);
                b.transform.localScale = new Vector3((float)0.2, (float)0.2, (float)0.2);
                StartCoroutine(CoinsAnimation(b.transform, coinfinishposition.transform.position, 0.8f));
                Destroy(b, 0.85f);

                int coins = PlayerPrefs.GetInt("totalcoins", 10000);
                coins = coins + g.GetComponent<MiniGameCardScript>().cardvalue;
                PlayerPrefs.SetInt("totalcoins", coins);
            }
        }

        Home.SetActive(true);
        
    }

    public void BacktoHome(){
        SceneManager.LoadScene("HomeScene");
    }
}
