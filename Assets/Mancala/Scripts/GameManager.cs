/***************************************************************************\
Project:      Mancala
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Niobium
{
    /**
    * The GameManager is responsible for every game aspect, variables and logic.
    * You can change the Game Configuration in the editor to simulate. Don't forget to change the player
    * settings in the PlayerDataManager GameObject
    */
    public class GameManager : MonoBehaviour
    {
        public Turn turn = Turn.NONE;                           // Player 1 or Player 2 turn
        public float stonesSpeed = 0.01f;                       // How fast are the stones are placed

        [Header("Game Configuration")]
        [Tooltip("You can change this in the editor to simulate. Don't forget to change the PlayerDataManager too")]
        private GameConfiguration gameConfiguration;            // The Game Configuration
        public GameConfiguration.GameMode gameMode;             // The Game Configuration Mode

        public GameState state = GameState.INITIAL;             // State machine	
        private List<Slot> slots;                               // The 14 slots
        private Slot lastSlot;                                  // The last slot where the stone landed on
        private float turnDelay;                                // How long the current player is delaying its turn
        private Board board;

        public static GameManager instance;

        // Returns dynamically the player 1 data
        private PlayerData player1Data
        {
            get { return PlayerDataManager.instance.playerData; }
            set { PlayerDataManager.instance.playerData = value; }
        }

        // Returns dynamically the turn time
        private float turnTime
        {
            get { return gameConfiguration.turnTime; }
        }

        public enum Turn
        {
            NONE,
            PLAYER_1,
            PLAYER_2
        }

        // Turn State Machine
        public enum GameState
        {
            INITIAL,
            START,
            TURN_START,
            MOVE_STONES,
            CAPTURE_STONES,
            CHECK_END_CONDITION,
            END
        }

        public enum WinState
        {
            PLAYER_1_WIN,
            PLAYER_2_WIN,
            DRAW
        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            gameMode = player1Data.gameMode;

            Initialize();
            GameInterfaceManager.instance.Initialize();
            StartGame();
        }

        void Update()
        {
            switch (state)
            {
                case GameState.INITIAL:
                    break;
                case GameState.START:
                    state = GameState.TURN_START;
                    StartCoroutine("HandleTimerCR");
                    break;
                case GameState.TURN_START:
                    // Waits for OnSlotClick
                    // If is against an AI, calculates the best movement and plays for this player
                    if (gameMode == GameConfiguration.GameMode.AGAINST_AI && turn == Turn.PLAYER_2)
                    {
                        PlayAI();
                    }
                    break;
                case GameState.MOVE_STONES:
                    // Waits for the coroutine MovingStonesCR
                    break;
                case GameState.CAPTURE_STONES:
                    // Waits for the player to click or the AI plays
                    if (gameMode == GameConfiguration.GameMode.AGAINST_AI && turn == Turn.PLAYER_2)
                    {
                        CaptureStonesAI();
                    }
                    break;
                case GameState.CHECK_END_CONDITION:
                    CheckEndCondition();
                    break;
            }
        }

        // Initializes the board
        public void Initialize()
        {
            gameConfiguration = ConfigurationManager.instance.GetGameConfigurationByType(gameMode);

            LoadBoard();

            GameInterfaceManager.instance.UpdateScore(0, 0);
            InitializeStones();
        }

        public void InitializeStones()
        {
            StartCoroutine(InitializeStonesCR());
        }

        public void StartGame()
        {
            state = GameState.START;
        }

        // Places the pieces with a small delay
        IEnumerator InitializeStonesCR()
        {
            // Starts populating the slots
            for (int i = 0; i < 14; i++)
            {
                Slot slot = GetSlot(i);
                if (!slot.house)
                {
                    for (int s = 0; s < 4; s++)
                    {
                        CreateStone(slot.number);
                        yield return new WaitForSeconds(0.001f);
                    }
                    slot.stones = 4;
                }
            }
            turn = Turn.PLAYER_1;
        }

        // Highlight a slot selection
        public void HighlightSlot(int slotNum, bool show)
        {
            Slot slot = GetSlot(slotNum);
            slot.slotSelect.GetComponent<Renderer>().enabled = show;
        }

        // Loads the board
        public void LoadBoard()
        {
            // This is for testing purposes. If I find a board in the scene, ignore the player preferences
            var boardGo = Instantiate(gameConfiguration.boardPrefab);

            board = boardGo.GetComponent<Board>();

            Slot[] slotsTmp = board.GetComponentsInChildren<Slot>();
            slots = new List<Slot>();

            foreach (Slot slot in slotsTmp)
            {
                slots.Add(slot);
            }

            // Sorts the slot position
            slots.OrderBy(slot => slot.number);
        }

        // Created a random stone object
        public void CreateStone(int slotNum)
        {
            var stonePrefab = gameConfiguration.GetRandomStone();
            GameObject stoneGo = GameObject.Instantiate(stonePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            stoneGo.name = "Stone";

            Transform slotTra = GetSlot(slotNum).transform;

            PlaceStone(slotTra, stoneGo);
        }

        // Places a stone randomly inside a slot
        private void PlaceStone(Transform slotTra, GameObject stone)
        {
            stone.transform.parent = slotTra;

            // Make Dynamic
            SphereCollider sc = slotTra.GetComponent<Collider>() as SphereCollider;
            float posX = UnityEngine.Random.Range(-sc.radius / 2, sc.radius / 2);
            float posY = sc.radius / 2;
            float posZ = UnityEngine.Random.Range(-sc.radius / 2, sc.radius / 2);

            stone.transform.localPosition = new Vector3(posX, posY, posZ);
            stone.transform.rotation = UnityEngine.Random.rotation;

            // Starts dropping fast
            stone.GetComponent<Rigidbody>().velocity = Vector3.down;
        }

        public void OnSlotClick(int slotNum)
        {
            OnSlotClick(slotNum, false);
        }

        // Action when the player clicks a slot
        public void OnSlotClick(int slotNum, bool aiPlay)
        {
            if (state == GameState.TURN_START)
            {
                //checks if the player can select the slot
                if (CanSelectSlot(slotNum, aiPlay))
                {
                    MoveStones(slotNum, aiPlay);
                }
                else
                {
                    // Shakes board
                    ShakeStones(slotNum);
                }
            }
            else if (state == GameState.CAPTURE_STONES)
            {
                if (GetOppositeSlotNum(lastSlot.number) == slotNum)
                {
                    CaptureStones(slotNum, aiPlay);
                }
                else
                {
                    // Shakes board
                    ShakeStones(slotNum);
                }
            }
        }

        private void ShakeStones(int slotNum)
        {
            Slot slot = GetSlot(slotNum);
            Stone[] stones = slot.transform.GetComponentsInChildren<Stone>();

            foreach (Stone stone in stones)
            {
                Vector3 force = UnityEngine.Random.insideUnitSphere * 100;

                stone.GetComponent<Rigidbody>().AddForce(force);
            }
        }

        // Moves the stones to the slot
        public void MoveStones(int slotNum, bool aiPlay)
        {
            state = GameState.MOVE_STONES;
            StartCoroutine(MovingStonesCR(slotNum, aiPlay));

            // Stops timer
            StopCoroutine("HandleTimerCR");
        }

        // Stones movement animation
        IEnumerator MovingStonesCR(int slotNum, bool aiPlay)
        {
            // Waits a while before playing
            if (aiPlay)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
            }

            Slot slot = GetSlot(slotNum);
            int stonesPool = slot.stones;
            slot.stones = 0;
            Stone[] stones = slot.transform.GetComponentsInChildren<Stone>();

            // Place the stones in another place not to be shown
            foreach (Stone stone in stones)
            {
                stone.transform.position = Vector3.one * 150;

            }

            int nextSlotNum = slot.number;

            while (stonesPool > 0)
            {
                GameObject stoneGo = stones[stonesPool - 1].gameObject;

                nextSlotNum = GetNextSlot(nextSlotNum);

                Slot nextSlot = GetSlot(nextSlotNum);

                // Skips the other player house slot
                if (nextSlot.house && ((turn == Turn.PLAYER_1 && !nextSlot.p1Owner) || (turn == Turn.PLAYER_2 && nextSlot.p1Owner)))
                {
                    nextSlotNum = GetNextSlot(nextSlotNum);
                    nextSlot = GetSlot(nextSlotNum);
                }

                nextSlot.stones++;
                stonesPool--;

                PlaceStone(nextSlot.transform, stoneGo);

                yield return new WaitForSeconds(stonesSpeed);
            }

            // Disables highlight
            foreach (Stone stone in stones)
            {
                stone.Highlight(false, true);
            }

            lastSlot = GetSlot(nextSlotNum);

            // If the final stone lands on an empty hole on your side, grabs all stones on the opposite site and places in your mancala
            if (lastSlot.stones == 1 && !lastSlot.house && ((turn == Turn.PLAYER_1 && lastSlot.p1Owner) || (turn == Turn.PLAYER_2 && !lastSlot.p1Owner)))
            {
                // Waits for the other player to grab the stones
                Slot oppositeSlot = GetSlot(GetOppositeSlotNum(lastSlot.number));
                stonesPool = oppositeSlot.stones;

                if (stonesPool > 0)
                {
                    Debug.Log("Got the other player stones");
                    if (aiPlay || turn == Turn.PLAYER_2)
                    {
                        if (aiPlay)
                        {
                            GameInterfaceManager.instance.ShowMessage("The other player got your stones");
                        }
                        else
                        {
                            GameInterfaceManager.instance.ShowMessage("You can get the other player stones");
                        }
                    }
                    else
                    {
                        GameInterfaceManager.instance.ShowMessage("You can get the other player stones");
                    }

                    state = GameState.CAPTURE_STONES;

                    // Highlight the stones of the slot
                    stones = oppositeSlot.transform.GetComponentsInChildren<Stone>();
                    foreach (Stone stone in stones)
                    {
                        stone.Highlight(true, true);
                    }

                    // Restart the timer
                    StartCoroutine("HandleTimerCR");
                    yield break;
                }
            }

            state = GameState.CHECK_END_CONDITION;
        }

        private void CaptureStonesAI()
        {
            OnSlotClick(GetOppositeSlotNum(lastSlot.number), true);

            state = GameState.MOVE_STONES;
        }

        // Grabs the other player stones and places in the house.
        public void CaptureStones(int slotNum, bool aiPlay)
        {
            StartCoroutine(CaptureStonesCR(slotNum, aiPlay));
        }

        private IEnumerator CaptureStonesCR(int slotNum, bool aiPlay)
        {
            // Delays just for the message to show
            if (aiPlay)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
            }

            Slot slot = GetSlot(slotNum);
            int stonesPool = slot.stones;

            Stone[] stones = slot.transform.GetComponentsInChildren<Stone>();
            slot.stones = 0;

            Slot playerHole = turn == Turn.PLAYER_1 ? GetSlot(7) : GetSlot(0);

            // Place the stones in another place so its not shown
            // TODO - Make a cool animation
            foreach (Stone stone in stones)
            {
                stone.transform.position = Vector3.one * 100;
            }

            while (stonesPool > 0)
            {
                playerHole.stones++;

                PlaceStone(playerHole.transform, stones[stonesPool - 1].gameObject);

                stonesPool--;
                yield return new WaitForSeconds(stonesSpeed);
            }

            // Capture single Opposite Stone
            Slot oppositeSlot = GetSlot(GetOppositeSlotNum(slotNum));
            stonesPool = oppositeSlot.stones;
            oppositeSlot.stones = 0;

            Stone[] oppositeStones = oppositeSlot.transform.GetComponentsInChildren<Stone>();

            while (stonesPool > 0)
            {
                playerHole.stones++;

                PlaceStone(playerHole.transform, oppositeStones[stonesPool - 1].gameObject);
                stonesPool--;
                yield return new WaitForSeconds(stonesSpeed);
            }

            foreach (Stone stone in stones)
            {
                stone.Highlight(false, true);
            }

            state = GameState.CHECK_END_CONDITION;
        }

        // Grabs the opposite slot
        private int GetOppositeSlotNum(int slotNum)
        {
            return 14 - slotNum;
        }

        // Grabs the other player slot on the other side
        private int GetOtherPlayerSlotNum(int slotNum)
        {
            return slotNum + 7;
        }

        // Verifies if the player slots are all empty
        private void CheckEndCondition()
        {
            bool slotsP1Empty = true;
            bool slotsP2Empty = true;

            // Player 1 Slots
            for (int i = 1; i < 7; i++)
            {
                if (GetSlot(i).stones > 0)
                {
                    slotsP1Empty = false;
                    break;
                }
            }

            // Player 2 Slots
            for (int i = 8; i < 14; i++)
            {
                if (GetSlot(i).stones > 0)
                {
                    slotsP2Empty = false;
                    break;
                }
            }

            int stonesP1 = GetSlot(7).stones;
            int stonesP2 = GetSlot(0).stones;

            // Updates the score
            GameInterfaceManager.instance.UpdateScore(stonesP1, stonesP2);

            if (slotsP1Empty || slotsP2Empty)
            {
                WinState winState = WinState.DRAW;

                if (stonesP1 == stonesP2)
                {
                    winState = WinState.DRAW;
                }
                else
                {
                    bool winner = stonesP1 > stonesP2;
                    if (winner)
                    {
                        winState = WinState.PLAYER_1_WIN;
                    }
                    else
                    {
                        winState = WinState.PLAYER_2_WIN;
                    }
                }

                GameInterfaceManager.instance.ShowEndGamePanel(winState);
                StopCoroutine("HandleTimerCR");
                state = GameState.END;
            }
            else
            {
                // checks if landed on the player house and starts the next turn
                // Is the players final slot?
                if (lastSlot != null && ((turn == Turn.PLAYER_1 && lastSlot.number == 7) ||
                   (turn == Turn.PLAYER_2 && lastSlot.number == 0)))
                {
                    if (gameMode == GameConfiguration.GameMode.OFFLINE || (gameMode != GameConfiguration.GameMode.OFFLINE && turn == Turn.PLAYER_1))
                    {
                        GameInterfaceManager.instance.ShowMessage("You can play again");
                    }
                }
                else
                {
                    turn = turn == Turn.PLAYER_1 ? Turn.PLAYER_2 : Turn.PLAYER_1;

                    GameInterfaceManager.instance.SetPlayer(turn);
                }
                state = GameState.TURN_START;
                // Starts the turn timer
                StartCoroutine("HandleTimerCR");
            }

            lastSlot = null;
        }

        // Lowers the player timer
        private IEnumerator HandleTimerCR()
        {
            turnDelay = 1f;

            while (turnDelay > 0)
            {
                GameInterfaceManager.instance.UpdateTimer(turn, turnDelay);
                turnDelay -= (Time.deltaTime / turnTime * 0.8f);
                yield return null;
            }

            // If there were no legit play, the game ends and the current player loses
            bool winner = turn == Turn.PLAYER_2 ? true : false;

            WinState win = WinState.DRAW;

            if (winner)
            {
                win = WinState.PLAYER_1_WIN;
            }
            else
            {
                win = WinState.PLAYER_2_WIN;
            }

            GameInterfaceManager.instance.ShowEndGamePanel(win);
        }

        // AI player calculates the best possible movement and plays
        private void PlayAI()
        {
            // The best movements (when you can play again)
            List<int> bestMovements = new List<int>();
            // The good movements (when you can place stones to get the best movements)
            List<int> goodMovements = new List<int>();
            // The possible movements
            List<int> possibleMovements = new List<int>();

            // Cycle between the possible slots
            for (int i = 8; i < 14; i++)
            {
                if (GetSlot(i).stones > 0)
                {
                    possibleMovements.Add(i);
                }
            }

            // Tries to pick the best movements from the possibilities
            foreach (int i in possibleMovements)
            {
                Slot slot = GetSlot(i);

                // Should calculate if the last stone ends on the ending hole
                // 8 + 6 = 14, 9 + 5 = 14, 10 + 4 = 14 and so on...
                if (slot.stones + i == 14)
                {
                    bestMovements.Add(i);
                }

                // Another best movement is to capture the opposing player stones
                // Forecast the ending stone position
                int stonesPool = slot.stones;
                int endSlotNum = slot.number;
                while (stonesPool > 0)
                {
                    endSlotNum = GetNextSlot(endSlotNum);
                    stonesPool--;
                }
                Slot endSlot = GetSlot(endSlotNum);
                if (endSlot.stones == 0 && !endSlot.p1Owner && !endSlot.house)
                {
                    // Now I need to check if the opposing slot if there is at least one stone
                    Slot oppositeSlot = GetSlot(GetOppositeSlotNum(endSlotNum));
                    if (oppositeSlot.stones > 0)
                    {
                        bestMovements.Add(i);
                    }
                }
            }

            // Forecast the good movements
            foreach (int i in possibleMovements)
            {
                // Starts at position 9 since pos 8 is the minimum
                for (int j = 9; j < 14; j++)
                {
                    // Needs to place at least one piece
                    if (GetSlot(j).stones + i == 13)
                    {
                        goodMovements.Add(i);
                        break;
                    }
                }
            }

            int clickSlotNum = 8;
            // After calculating choses the best, good then possible moves.
            if (bestMovements.Count > 0)
            {
                clickSlotNum = bestMovements[UnityEngine.Random.Range(0, bestMovements.Count)];
            }
            else if (goodMovements.Count > 0)
            {
                clickSlotNum = goodMovements[UnityEngine.Random.Range(0, goodMovements.Count)];
            }
            else
            {
                clickSlotNum = possibleMovements[UnityEngine.Random.Range(0, possibleMovements.Count)];
            }

            OnSlotClick(clickSlotNum, true);
        }

        // Grabs the next cycling slot.
        public int GetNextSlot(int slotNum)
        {
            if (slotNum == 13)
                return 0;

            return slotNum + 1;
        }

        // Condition for selecting the slot. The player must be the owner and the slot must have at least one stone
        public bool CanSelectSlot(int slotNum, bool aiPlay)
        {
            Slot slot = GetSlot(slotNum);

            // Cannot select empty slots
            if (slot.stones == 0)
                return false;

            switch (gameMode)
            {
                case GameConfiguration.GameMode.OFFLINE:
                    // can only select my own slots
                    return (turn == Turn.PLAYER_1 && slot.p1Owner) || (turn == Turn.PLAYER_2 && !slot.p1Owner);
                case GameConfiguration.GameMode.AGAINST_AI:
                    // If the AI is playing, can select the slot otherwise can only move my own pieces
                    return (turn == Turn.PLAYER_1 && slot.p1Owner) || aiPlay;
            }


            return false;
        }

        // Restart the game
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Get slot by number
        private Slot GetSlot(int slotNum)
        {
            return slots[slotNum];
        }

    }
}