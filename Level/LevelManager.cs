using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //what we use to wait inside routines
    WaitForSeconds oneSec;
    //starting positions of the characters
    public Transform[] spawnPositions;

    CharacterManager charM;
    LevelUI levelUI;

    //number of games a player need to win
    public int maxTurns = 2; 
    int currentTurn = 1;

    public bool countdown; 
    public int maxTurnsTimer = 30;
    int currentTimer; 
    float internalTimer; 

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("level_manager_start");
        charM = CharacterManager.GetInstance();
        levelUI = LevelUI.GetInstance();

        oneSec = new WaitForSeconds(1);

        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        levelUI.AnnouncerTextLine2.gameObject.SetActive(false);

        StartCoroutine("StartGame");
    }

    void FixedUpdate()
    {
        //used to handle players' orientation
        if (charM.players[0].playerStates.transform.position.x < 
            charM.players[1].playerStates.transform.position.x){
                charM.players[0].playerStates.lookRight = true; 
                charM.players[1].playerStates.lookRight = false;
            }
        else{
                charM.players[0].playerStates.lookRight = false; 
                charM.players[1].playerStates.lookRight = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (countdown){
            HandleTurnTimer(); //control the timer here
        }
    }

    void HandleTurnTimer()
    {
        levelUI.LevelTimer.text = currentTimer.ToString();
        internalTimer += Time.deltaTime;

        if (internalTimer > 1){ //when the countdown is still up, reduce it
            currentTimer--; 
            internalTimer = 0; 
        }else if (internalTimer <= 0){ //when the countdown is over, end the turn
            EndTurnFunction(true);
            countdown = false; 
        }
    }

    IEnumerator StartGame() 
    {
        //let's create the players
        yield return CreatePlayers();

        //then we can initialize the turn
        yield return InitTurn();
        
    }

    IEnumerator CreatePlayers()
    {
        for (int i = 0; i < charM.players.Count; i++){
            GameObject go = Instantiate(
                charM.players[i].playerPrefab, 
                spawnPositions[i].position, 
                Quaternion.identity
            ) as GameObject;

            charM.players[i].playerStates = go.GetComponent<StateManager>();
            Debug.Log("create_players_1");
            charM.players[i].playerStates.healthSlider = levelUI.healthSliders[i];
            Debug.Log("create_players_2");
        }

        yield return new WaitForEndOfFrame();
    }

    IEnumerator InitTurn()
    {
        //disable the announcer texts first 
        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        levelUI.AnnouncerTextLine2.gameObject.SetActive(false);

        //reset the timer
        currentTimer = maxTurnsTimer;
        countdown = false;

        //initialize the players
        yield return InitPlayers();
        //start subroutine that enable to control each player
        yield return EnableControl();
    }

    IEnumerator InitPlayers()
    {
        //the only thing we have to do is reset the health bar
        for (int i = 0; i < charM.players.Count; i++){
            charM.players[i].playerStates.health = 100; 
            charM.players[i].playerStates.handleAnim.anim.Play("Locomotion");
            charM.players[i].playerStates.transform.position = spawnPositions[i].position;
        }

        yield return new WaitForEndOfFrame();
    }

    IEnumerator EnableControl()
    {
        //start with the announcer text
        levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
        levelUI.AnnouncerTextLine1.text = "Turn " + currentTurn; 
        levelUI.AnnouncerTextLine1.color = Color.white;
        yield return oneSec;
        yield return oneSec;
        Debug.Log("hey");
        //change the UI text and color every second that passes 
        //like a countedown for the match to start
        levelUI.AnnouncerTextLine1.text = "3"; 
        levelUI.AnnouncerTextLine1.color = Color.green;
        yield return oneSec; 
        levelUI.AnnouncerTextLine1.text = "2";
        levelUI.AnnouncerTextLine1.color = Color.yellow;
        yield return oneSec;
        levelUI.AnnouncerTextLine1.text = "1";
        levelUI.AnnouncerTextLine1.color = Color.red; 
        yield return oneSec; 
        levelUI.AnnouncerTextLine1.text = "FIGHT"; 
        levelUI.AnnouncerTextLine1.color = Color.red;

        for (int i = 0; i < charM.players.Count; i++){
            //for users players, enable the input handler for example
            if (charM.players[i].playerType == PlayerBase.PlayerType.User){
                InputHandler ih = charM.players[i].playerStates.gameObject.GetComponent<InputHandler>();
                ih.playerInput = charM.players[i].inputId;
                ih.enabled = true; 
            }
        } 

        //after a second, disable the announcer text
        yield return oneSec; 
        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        countdown = true;
    }

    void DisableControl()
    {
        for (int i = 0; i < charM.players.Count; i++){
            charM.players[i].playerStates.ResetStateInputs();

            if (charM.players[i].playerType == PlayerBase.PlayerType.User){
                charM.players[i].playerStates.GetComponent<InputHandler>().enabled = false; 
            }
        }
    }

    public void EndTurnFunction(bool timeOut = false) //to end the turn
    {
        countdown = false; 
        //reset the timer text
        levelUI.LevelTimer.text = maxTurnsTimer.ToString();

        if (timeOut){
            levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
            levelUI.AnnouncerTextLine1.text = "Time Out!";
            levelUI.AnnouncerTextLine1.color = Color.cyan; 
        }
        else{
            levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
            levelUI.AnnouncerTextLine1.text = "K.O.";
            levelUI.AnnouncerTextLine1.color = Color.red; 
        }

        //yield return oneSec;  
        //yield return oneSec;

        DisableControl();

        StartCoroutine("EndTurn"); 
    }

    IEnumerator EndTurn()
    {
        yield return oneSec; 
        yield return oneSec; 
        yield return oneSec; 

        PlayerBase vPlayer = FindWinningPlayer();

        if (vPlayer == null){
            levelUI.AnnouncerTextLine1.text = "Draw";
            levelUI.AnnouncerTextLine1.color = Color.blue;
        }else{
            levelUI.AnnouncerTextLine1.text = vPlayer.playerId + "Wins!"; 
            levelUI.AnnouncerTextLine1.color = Color.red; 
        }

        yield return oneSec; 

        if (vPlayer != null){
            if (vPlayer.playerStates.health == 100){
                levelUI.AnnouncerTextLine2.gameObject.SetActive(true);
                levelUI.AnnouncerTextLine2.text = "Flawless Victory!";
            }
        }

        yield return oneSec; 
        currentTurn++; 

        bool matchOver = isMatchOver();

        if (!matchOver){
            StartCoroutine("InitTurn");
        }else{
            for (int i = 0; i < charM.players.Count; i++){
                charM.players[i].score = 0; 
                charM.players[i].hasCharacter = false; 
            }
            SceneManager.LoadSceneAsync("select");
        }
    }

    bool isMatchOver()
    {
        for (int i = 0; i < charM.players.Count; i++){
            if (charM.players[i].score >= maxTurns){
                return true;
            }
        }

        return false;
    }

    PlayerBase FindWinningPlayer()
    {
        PlayerBase retVal = null; 
        StateManager targetPlayer = null; 

        if (charM.players[0].playerStates.health != charM.players[1].playerStates.health){
            if (charM.players[0].playerStates.health < charM.players[1].playerStates.health){
                charM.players[1].score++;
                targetPlayer = charM.players[1].playerStates; 
                levelUI.AddWinIndicator(1);
            }else{
                charM.players[0].score++;
                targetPlayer = charM.players[0].playerStates;
                levelUI.AddWinIndicator(0);
            }

            retVal = charM.returnPlayerFromStates(targetPlayer);
        }
        return retVal;
    }

    public static LevelManager instance; 
    public static LevelManager GetInstance()
    {
        return instance; 
    }

    void awake()
    {
        instance = this; 
    }
}