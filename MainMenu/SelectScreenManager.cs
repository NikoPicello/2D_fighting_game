using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectScreenManager : MonoBehaviour
{
    public int numberOfPlayers = 1;
    public PlayerInterfaces user, ai; 
    public PortraitInfo[] portraits; 
    public int maxX; 

    public GameObject portraitCanvas; 
    public bool loadLevel; 

    CharacterManager charManager;

    #region Singleton
    public static SelectScreenManager instance; 
    public static SelectScreenManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this; 
    }
    #endregion

    #region Start
    void Start()
    {
        charManager = CharacterManager.GetInstance();

        portraits = portraitCanvas.GetComponentsInChildren<PortraitInfo>();

        loadLevel = false;
        user.selector.SetActive(true);
    }
    #endregion

    #region Update
    void Update()
    {
        if (!loadLevel && !charManager.players[0].hasCharacter){
            user.playerBase = charManager.players[0];
            HandleSelectorPosition(user);
            HandleCharacterPreview(user);
            HandleSelectScreenInput(user);
        }else{
            charManager.players[0].hasCharacter = true;
        }

        if (charManager.players[0].hasCharacter){
            Debug.Log("loading_level");
            StartCoroutine("LoadLevel");
            loadLevel = true; 
        }
    }
    #endregion

    void HandleSelectorPosition(PlayerInterfaces pl)
    {
        // pl.selector.SetActive(true); //enable the selector

        pl.activePortrait = portraits[pl.activeX]; //find the active portrait

        //place the selector over its position
        Vector2 selectorPosition = pl.activePortrait.transform.localPosition;
        selectorPosition = selectorPosition + new Vector2(portraitCanvas.transform.localPosition.x,
            portraitCanvas.transform.localPosition.y);
        pl.selector.transform.localPosition = selectorPosition;
    }

    void HandleSelectScreenInput(PlayerInterfaces pl)
    {
        if (!loadLevel){
            if (Input.GetKeyUp(KeyCode.D))
                pl.activeX = (pl.activeX < maxX - 1) ? pl.activeX + 1 : 0;
            
            if (Input.GetKeyUp(KeyCode.A))
                pl.activeX = (pl.activeX > 0) ? pl.activeX - 1 : maxX - 1;

            if (Input.GetKeyUp(KeyCode.Return)){
                pl.createdCharacter.GetComponentInChildren<Animator>().Play("Kick");

                pl.playerBase.playerPrefab =
                    charManager.returnCharacterWithID(pl.activePortrait.characterId).prefab;
                pl.playerBase.hasCharacter = true; 
            }
        }
    }

    void HandleCharacterPreview(PlayerInterfaces pl)
    {
        if (pl.previewPortrait != pl.activePortrait){
            if (pl.createdCharacter != null) Destroy(pl.createdCharacter);

            GameObject go = Instantiate(
                charManager.returnCharacterWithID(pl.activePortrait.characterId).prefab,
                pl.charVisPos.position,
                Quaternion.identity
            ) as GameObject; 

            pl.createdCharacter = go; 
            pl.previewPortrait = pl.activePortrait; 

            if (!string.Equals(pl.playerBase.playerId, charManager.players[0].playerId)){
                pl.createdCharacter.GetComponent<StateManager>().lookRight = false; 
            }
            
        }
    }

    IEnumerator LoadLevel()
    {
        for (int i = 0; i < charManager.players.Count; i++){
            if (charManager.players[i].playerType == PlayerBase.PlayerType.Ai){
                if (charManager.players[i].playerPrefab == null){
                    int ranValue = Random.Range(0, portraits.Length);
                    while (ranValue == user.activeX) ranValue = Random.Range(0, portraits.Length);

                    charManager.players[i].playerPrefab = 
                        charManager.returnCharacterWithID(portraits[ranValue].characterId).prefab;
                    charManager.players[i].hasCharacter = true; 

                    Debug.Log(portraits[ranValue].characterId);
                }
            }
        }

        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync("level", LoadSceneMode.Single);
    }

    [System.Serializable]
    public class PlayerInterfaces
    {
        public PortraitInfo activePortrait; //current active portrait for player 1
        public PortraitInfo previewPortrait; 
        public GameObject selector; //select indicator for player 1
        public Transform charVisPos; //visual position of the character
        public GameObject createdCharacter; 

        public int activeX; 
        // public int activeY; 

        // to smooth action
        // public bool hitInputOnce; 
        // public float timerToReset; 

        public PlayerBase playerBase; 
    }
}
