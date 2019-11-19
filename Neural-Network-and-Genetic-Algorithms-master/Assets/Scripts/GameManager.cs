using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    static public GameMode gameMode;
    static public float playerSpeed = 30.0f;
    static public int maxObstacles = 1;
    public enum GameMode
    {
        AI_vs_AI,
        NeuralNetworkTraining,
    }
    enum TerrainPosition
    {
        Bot,
        BotMid,
        Mid,
        TopMid,
        Top,

        Count
    }
    enum Status
    {
        Starting,
        Playing,
        GameOver
    }
    Dictionary<TerrainPosition, TerrainGeneration> terrains;    
    List<PlayerMode> players;
    List<ParticleSystem> particles;
    CameraFollow cam;
    EvolutionController evolutionManager;
    Vector2 terrainSize;
    Vector2 halfRegionSize;
    float time;
    float startTime;
    int currentObjective;
    int currentTerrain;
    int previousTerrain;
    int tests;
    Status status;
    void Awake()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1;
        players = new List<PlayerMode>(11);
        terrains = new Dictionary<TerrainPosition, TerrainGeneration>();
        particles = new List<ParticleSystem>(11);
        cam = Camera.main.GetComponent<CameraFollow>();
        evolutionManager = GetComponent<EvolutionController>();
        tests = 0;
    }

    void Start()
    {
        PrefabController.Initialize();
        GenerateTerrain();
        GeneratePlayers();
        SetupCamera();
    }

    void GeneratePlayers()
    {
        GameObject players = new GameObject();
        players.name = "UnitContainer";
        Vector3 initialPosition = new Vector3(6.5f, 0.0f, 5.0f);
        Vector3 initialEulerAngle = new Vector3(0, 270, 0);
        int agentSize = 11;
        List<NeuralNetworkPlayer> nnAgents = new List<NeuralNetworkPlayer>(agentSize);

        switch (GameManager.gameMode)
        {
            case GameManager.GameMode.AI_vs_AI:
                GenerateScriptedAIPlayers(ref nnAgents, ref players, initialPosition, initialEulerAngle, agentSize);
                break;

            case GameManager.GameMode.NeuralNetworkTraining:
                GeneratePlayersForNeuralNetworkTraining(ref nnAgents, ref players, initialPosition, initialEulerAngle, agentSize);
                break;

            default:
                break;
        }

        IgnoreCollisionBetweenUnits();
        evolutionManager.Initialize(nnAgents);
    }

    void GenerateScriptedAIPlayers(ref List<NeuralNetworkPlayer> agents, ref GameObject unitContainer, Vector3 initialPosition, Vector3 initialEulerAngle, int agentSize)
    {
        for (int i = 0; i < agentSize; i++)
        {
            players.Add(SetUnit(PlayerType.ScriptedAI, ref unitContainer, initialPosition, initialEulerAngle));
            initialPosition.x += 4.0f;
        }
    }

    void GeneratePlayersForNeuralNetworkTraining(ref List<NeuralNetworkPlayer> agents, ref GameObject unitContainer, Vector3 initialPosition, Vector3 initialEulerAngle, int agentSize)
    {
        for (int i = 0; i < agentSize; i++)
        {
            players.Add(SetUnit(PlayerType.NeuralNetwork, ref unitContainer, initialPosition, initialEulerAngle));
            initialPosition.x += 4.0f;
            agents.Add(players[i].GetComponent<NeuralNetworkPlayer>());
        }
    }

    void IgnoreCollisionBetweenUnits()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].name = "Player" + i;

            for (int j =  i + 1; j < players.Count; j++)
            {
                Physics.IgnoreCollision(players[i].collider, players[j].collider);
            }
        }
    }

    PlayerMode SetUnit(PlayerType type, ref GameObject container, Vector3 spawnPosition, Vector3 spawnEulerAngle)
    {
        PlayerMode unit = PrefabController.GeneratePlayerType(type);
        Vector2 unitSize = PrefabController.GetSize(PrefabController.Prefab.PlayerAI);
        unit.transform.position = spawnPosition;
        unit.transform.eulerAngles = spawnEulerAngle;
        unit.transform.parent = container.transform;
        unit.Initialize(type);

        return unit;
    }

    void GenerateTerrain()
    {
        RestartGame();
        terrainSize = PrefabController.GetSize(PrefabController.Prefab.Terrain);
        halfRegionSize = terrainSize * 0.5f;
        TerrainGeneration terrain = PrefabController.CreateGameObject(PrefabController.Prefab.Terrain).GetComponent<TerrainGeneration>();
        terrain.Initialize(halfRegionSize);
        terrains.Add(TerrainPosition.Bot, terrain);
        float z = terrains[TerrainPosition.Bot].Position.z + terrainSize.y;
        int extraTerrains = (int)TerrainPosition.Count;

        for (int i = 1; i < extraTerrains; i++)
        {
            GameObject terrainClone = PrefabController.CreateGameObject(PrefabController.Prefab.Terrain);
            Vector3 position = terrainClone.transform.position;
            position.z = z;
            terrainClone.transform.position = position;
            terrain = terrainClone.GetComponent<TerrainGeneration>();
            terrain.Initialize(halfRegionSize);
            terrains.Add((TerrainPosition) i, terrain);
            z += terrainSize.y;
        }
    }

    void RestartGame()
    {
        time = 0;
        currentTerrain = 0;
        previousTerrain = 0;
        currentObjective = 0;
        startTime = 2.0f;
        status = Status.Starting;
        GameManager.playerSpeed = 30.0f;
        GameManager.maxObstacles = 1;
    }
    
    void SetupCamera()
    {
        cam.Initialize(players[0].transform);
    }

    void Update()
    {
        switch (status)
        {
            case Status.Starting:
                UpdateStarting();
                break;
            case Status.Playing:
                UpdatePlaying();
                break;
            case Status.GameOver:
                UpdateGameOver();
                break;
        }
    }

    void UpdateStarting()
    {
            if (startTime > 0)
            {
                startTime -= 3 * Time.deltaTime;
                if (startTime < 0)
                {
                    startTime = 0;
                }
            }
            else
            {
                status = Status.Playing;
            }
        IfQuitGame();
    }

    void UpdatePlaying()
    {
        cam.ManualUpdate();
        UpdateDifficulty();
        UpdateTerrains();
        UpdatePlayers();
        UpdateParticleSystems();
        UpdateEvolutionManager();
        IfQuitGame();
    }

    void UpdateGameOver()
    {
      SceneController.LoadTargetScene();
      IfQuitGame();
    }

    void UpdateDifficulty()
    {
        if (time < 60.0f)
        {
            time += Time.deltaTime;
            float level = time / 60.0f;

            if (level > 1)
            {
                level = 1;
            }

            float newSpeed = 60.0f * level;
            if (newSpeed > 30.0f)
            {
                GameManager.playerSpeed = newSpeed;
            }

            int newMaxObstacles = (int)(2 * level);

            if (newMaxObstacles > 1)
            {
                GameManager.maxObstacles = newMaxObstacles;
            }
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    void UpdateEvolutionManager()
    {
        if (GameManager.gameMode == GameManager.GameMode.NeuralNetworkTraining)
        {
            evolutionManager.ManualUpdate();
            if (evolutionManager.newGeneration)
            {
                ResetGame();
                evolutionManager.newGeneration = false;
            }
        }
    }

    void UpdateTerrains()
    {
        Vector3 cameraPosition = cam.Position;
        currentTerrain = (int)(cameraPosition.z / terrainSize.y);
        if (currentTerrain > previousTerrain)
        {
            Destroy(terrains[TerrainPosition.Bot].gameObject);
            int size = terrains.Count - 1;

            for (int i = 0; i < size; i++)
            {
                terrains[(TerrainPosition)i] = terrains[(TerrainPosition)i + 1];
            }

            GameObject regionClone = PrefabController.CreateGameObject(PrefabController.Prefab.Terrain);
            Vector3 position = regionClone.transform.position;
            position.z = terrains[(TerrainPosition)((int)TerrainPosition.Top-1)].Position.z + terrainSize.y;
            regionClone.transform.position = position;
            TerrainGeneration terrain = regionClone.GetComponent<TerrainGeneration>();
            terrain.Setup(halfRegionSize);
            terrains[TerrainPosition.Top] = terrain;
        }

        previousTerrain = currentTerrain;
    }

    void UpdatePlayers()
    {
        int topUnit = 0;
        int index = 0;
        bool allDead = true;

        for (int i = 0; i < players.Count; i++)
        {
            UpdatePlayer(players[i], ref topUnit, ref index, ref allDead, i);
        }

        TrackBestPlayer(index, topUnit);
        cam.SetObjective(players[currentObjective].transform);

        if (allDead)
        {
            SetupGameOver();
        }
    }

    private static int ScoreCompare(PlayerMode x, PlayerMode y)
    {
        return y.Score.CompareTo(x.Score);
    }

    void UpdatePlayer(PlayerMode unit, ref int topUnit, ref int index, ref bool allDead, int i)
    {
        if (unit.alive)
        {
            allDead = false;
            unit.ManualUpdate();
            int zPosition = (int)unit.position.z;

            if (zPosition > topUnit)
            {
                topUnit = zPosition;
                index = i;
            }
        }
        else
        {
            if (unit.hit)
            {
                unit.hit = false;
            }
        }
    }

    void TrackBestPlayer(int index, float topUnit)
    {
        if (!players[currentObjective].alive)
        {
            if (currentObjective != index)
            {
                if (players[currentObjective].alive)
                {
                    if (players[currentObjective].position.z < topUnit + 0.5f)
                    {
                        currentObjective = index;
                    }
                }
                else
                {
                    currentObjective = index;
                }
            }
        }
    }

    void SetupGameOver()
    {
        status = Status.GameOver;
        players.Sort(ScoreCompare);
        
        if (GameManager.gameMode == GameManager.GameMode.AI_vs_AI)
        {
            SaveAI_Data();
            ResetGame();
            tests++;
        }
    }

    void ResetGame()
    {
        for (int i = 0; i < terrains.Count; i++)
        {
            Destroy(terrains[(TerrainPosition)i].gameObject);
        }
        terrains.Clear();
        GenerateTerrain();

        for (int i = 0; i < players.Count; i++)
        {
            players[i].Reset();
        }

        cam.RepositionToObjective(players[0].transform);
    }

    void SaveAI_Data()
    {
        List<FileIO.AI_Data> aiData = new List<FileIO.AI_Data>(players.Count);
        int nnAvgScore = 0;
        int aiAvgScore = 0;
        int nnTotalScore = 0;
        int aiTotalScore = 0;

        for (int i = 0; i < players.Count; i++)
        {
            aiData.Add(new FileIO.AI_Data(players[i].type.ToString(), players[i].Score));

            if (players[i].type == PlayerType.NeuralNetwork)
            {
                nnTotalScore += players[i].Score;
            }
            else
            {
                aiTotalScore += players[i].Score;
            }
        }

        int halfPopulation = (int)(players.Count * 0.5f);
        nnAvgScore = nnTotalScore / halfPopulation;
        aiAvgScore = aiTotalScore / halfPopulation;
        FileIO.GameData gameData = new FileIO.GameData(aiData, players[0].type.ToString(), nnAvgScore, aiAvgScore, nnTotalScore, aiTotalScore);
        string path = "GameDataTest" + tests;
        FileIO.WriteJson(path + ".json", ref gameData);
    }

    void UpdateParticleSystems()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            if (!particles[i].IsAlive())
            {
                Destroy(particles[i].gameObject);
                particles.RemoveAt(i);
                i--;
            }
        }
    }

    public void IfQuitGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1;
            SceneController.targetScene = SceneController.Scene.Menu;
            SceneController.LoadTargetScene();
        }
    }

    public void MainMenu()
    {
        ChangeToGameScene(SceneController.Scene.Menu);
    }

    void ChangeToGameScene(SceneController.Scene sceneToGoTo)
    {
        SceneController.targetScene = sceneToGoTo;
    }
}
