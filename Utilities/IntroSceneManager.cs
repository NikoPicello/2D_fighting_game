using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    public GameObject startText; 
    bool loadLevel; 
    // bool init; 

    //public int activeElement = 1; 
    // public GameObject menuObj; 
    // public ButtonRef[] menuOptions; 

    // Start is called before the first frame update
    void Start()
    {
        // we disable the game object
        startText.SetActive(true);
        loadLevel = false; 
    }

    // Update is called once per frame
    // it flickers the Press Start text
    void Update()
    {
        if (!loadLevel){
            if (Input.GetKeyUp(KeyCode.Return)){
                loadLevel = true;
                startText.SetActive(false);
                Debug.Log("load"); 
                StartCoroutine("LoadLevel");
            }
        }
        /*if (init){
            if (Input.GetKeyUp(KeyCode.Return)){
                init = false; 
                startText.SetActive(false);
                menuObj.SetActive(true); //closes the text and opens the menu
            }
        }else{
            if(!loadingLevel)
            { 
                menuOptions[activeElement].selected = true; // selected option
                if (Input.GetKeyUp(KeyCode.W)){
                    menuOptions[activeElement].selected = false; 
                    if (activeElement > 0){
                        activeElement--; 
                    }else{
                        activeElement = menuOptions.Length - 1; 
                    }
                }
                if (Input.GetKeyUp(KeyCode.S)){
                    menuOptions[activeElement].selected = false; 
                    if (activeElement < menuOptions.Length - 1){
                        activeElement++; 
                    }else{
                        activeElement = 0;
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.Return)){
                //load the level
                Debug.Log("load");
                loadingLevel = true; 
                StartCoroutine("LoadLevel");
                menuOptions[activeElement].transform.localScale *= 1.2f;
            }
        }*/
    }

    /*void HandleSelectedOption()
    {
        switch(activeElement){
            case 0: 
                CharacterManager.GetInstance().numberOfUsers = 1; 
                break;
            case 1: 
                CharacterManager.GetInstance().numberOfUsers = 2;
                CharacterManager.GetInstance().players[1].playerType = PlayerBase.PlayerType.user; 
                break;
        }
    }*/

    IEnumerator LoadLevel()
    {
        CharacterManager.GetInstance().numberOfUsers = 1;
        yield return new WaitForSeconds(0.6f);
        // startText.SetActive(false);
        // yield return new WaitForSeconds(1.5f);
        SceneManager.LoadSceneAsync("select", LoadSceneMode.Single); // if you use LoadScene instead, the game freeze until the scene is totally loaded
    }
}
