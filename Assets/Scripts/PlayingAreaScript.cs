using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingAreaScript : MonoBehaviour
{
    public int rownumber;
    public List<int> children;
    public List<int> childrenduplicate;
    public LeverScript lever;
    
    // Start is called before the first frame update

    void Start()
    {  
        lever = FindObjectOfType<LeverScript>(); 

        if(lever.mousecheese.ContainsKey(int.Parse(transform.name))){
                children.Add(lever.mousecheese[int.Parse(transform.name)]);
        } 

        childrenduplicate = new List<int>(children);
        // GetComponent<SpriteRenderer>().sortingOrder = rownumber; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
