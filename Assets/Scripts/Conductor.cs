using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Conductor : MonoBehaviour
{
    public Dictionary<string, int> scenes = new Dictionary<string, int>()
    {
        { "MainMenu", 0},
        { "Authors", 1},
        { "Settings" , 2},
        { "Begin" , 3},
        { "Lobby" , 4},
        { "Artefact" , 5},
        { "LevelCatacombs" , 6},
        { "LevelSnakeTemple" , 7},
        { "LevelLightTemple" , 8},
        { "LevelMagicForest" , 9},
    };

    public void findScene(string sceneName)
    {
        try
        {
            int idScene = Int32.Parse(sceneName);
            showScene(idScene);
        }
        catch (Exception)
        {
            if (scenes.ContainsKey(sceneName))
                showScene(scenes[sceneName]);
            else
            {
                var button = EventSystem.current.currentSelectedGameObject;
                Debug.LogError($"У кнопки {button.name} указано неверное название сцены для перехода");
            }
        }
    }

    public void showScene(int idScene = 0)
    {
        SceneManager.LoadScene(idScene);
    }

    public void quitGame()
    {
        Application.Quit();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

enum Scenes
{
    MainMenu,
    Authors,
    Settings,
    Begin,
    Lobby,
    Artefact,
    LevelCatacombs,
    LevelSnakeTemple,
    LevelLightTemple,
    LevelMagicForest
}