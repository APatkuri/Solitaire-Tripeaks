using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static List<Achievement> achievements;

    public Button home;
    public static int wins;
    public static int black_cards;
    public Text winstext;
    public Text blackcardstext;

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

        InitializeAchievements();
        CheckAchievementCompletion();
    }

    public void Awake(){
        // PlayerPrefs.DeleteKey("wins");   
        // PlayerPrefs.DeleteKey("black_cards");
    }

    public bool AchievementUnlocked(string achievementName)
    {
        bool result = false;

        if (achievements == null)
            return false;

        Achievement[] achievementsArray = achievements.ToArray();
        Achievement a = Array.Find(achievementsArray, ach => achievementName == ach.title);

        if (a == null)
            return false;

        result = a.achieved;

        return result;
    }

    private void Start()
    {
        // InitializeAchievements();
        // int wins = PlayerPrefs.GetInt("wins", 0);
        // int black_cards = PlayerPrefs.GetInt("black_cards", 0);
    }

    private void InitializeAchievements()
    {
        // if (achievements != null)
        //     return;

        int wins = PlayerPrefs.GetInt("wins", 0);
        int black_cards = PlayerPrefs.GetInt("black_cards", 0);

        achievements = new List<Achievement>();
        achievements.Add(new Achievement("Win Streak", "You have Completed 10 Wins!", (object o) => wins == 10, winstext, wins, 10));
        achievements.Add(new Achievement("Color Fever", "You have Collected 100 Black Cards!", (object o) => black_cards == 100, blackcardstext, black_cards, 100));
    }

    private void Update()
    {
        CheckAchievementCompletion();
    }

    private void CheckAchievementCompletion()
    {
        if (achievements == null)
            return;

        foreach (var achievement in achievements)
        {
            achievement.UpdateCompletion();
        }
    }

    public void GoHome(){
        SceneManager.LoadScene("HomeScene");
    }
}

public class Achievement
{
    public Achievement(string title, string description, Predicate<object> requirement, Text achievementtext, int completion, int total)
    {
        this.title = title;
        this.description = description;
        this.requirement = requirement;
        this.achievementtext = achievementtext;
        this.completion = completion;
        this.total = total;
    }

    public string title;
    public string description;
    public Predicate<object> requirement;
    public Text achievementtext;
    public int completion;
    public int total;

    public bool achieved;

    public void UpdateCompletion()
    {
        if (achieved)
            return;

        if (RequirementsMet())
        {
            Debug.Log($"{title}: {description}");
            achievementtext.text = $"{description}";
            achieved = true;
        }

        if(completion < total){
            achievementtext.text = completion.ToString() + "/" + total.ToString();
        }

        if(completion > total){
            achievementtext.text = "COMPLETED";
        }
    }

    public bool RequirementsMet()
    {
        return requirement.Invoke(null);
    }
}