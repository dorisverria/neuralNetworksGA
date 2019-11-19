using UnityEngine;
using System.Collections.Generic;

public class TerrainGeneration : MonoBehaviour
{
    Transform trnsfrm;
    ParticleSystem particleSyst;
    List<Reward> rewards;
    Vector2 size;
    [SerializeField]
    List<GameObject> _obstacles;
    struct GenerationInfo
    {
        public GenerationInfo(Vector2 p, Vector2 s)
        {
            position = p;
            size = s;
        }
        public Vector2 position;
        public Vector2 size;
    }

    public void Initialize(Vector2 size)
    {
        trnsfrm = transform;
        rewards = new List<Reward>(10);       
        particleSyst = GetComponentInChildren<ParticleSystem>();
        particleSyst.enableEmission = false;
        this.size = size;
    }
   
    public void Setup (Vector2 size)
    {
        Initialize(size);
        particleSyst.enableEmission = true;
        List<GenerationInfo> positionsOccupied = new List<GenerationInfo>();
        GenerateObstacles(ref positionsOccupied);
        GenerateRewards(ref positionsOccupied);
    }

    void GenerateRewards(ref List<GenerationInfo> positionsOccupied)
    {
        Vector2 size = PrefabController.GetSize(PrefabController.Prefab.Reward) * 0.5f;
        Vector2 failedToPosition = new Vector3(-1, -1);
        int rewardCount = 0;
        int yellowBallsToCreate = Random.Range(0, 10);
        while (rewardCount != yellowBallsToCreate)
        {
            Vector2 position = GetAvailabePosition(size, ref positionsOccupied);
            if (position != failedToPosition)
            {
                positionsOccupied.Add(new GenerationInfo(position, size));
                GameObject clone = PrefabController.CreateGameObject(PrefabController.Prefab.Reward);
                clone.transform.position = new Vector3(position.x, clone.transform.position.y, position.y);
                clone.transform.parent = trnsfrm;
                Reward reward = clone.GetComponent<Reward>();
                reward.Initialize();
                rewards.Add(reward);
                rewardCount++;
            }
            else
            {
                break;
            }
        }
    }

    void GenerateObstacles(ref List<GenerationInfo> positionsOccupied)
    {
        int obstacleCount = 0;
        int obstaclesToCreate = Random.Range(1, GameManager.maxObstacles + 1);
        ActivateWall(ref positionsOccupied);
        Vector2 size = PrefabController.GetSize(PrefabController.Prefab.Wall) * 0.5f;
        Vector2 failedToPosition = new Vector2(-1, -1);
        while (obstacleCount < obstaclesToCreate)
        {
            Vector2 position = GetAvailabePosition(size, ref positionsOccupied);
            if (position != failedToPosition)
            {
                positionsOccupied.Add(new GenerationInfo(position, size));
                GameObject clone = PrefabController.CreateGameObject(PrefabController.Prefab.Wall);
                Transform cloneTransform = clone.transform;
                cloneTransform.position = new Vector3(position.x, cloneTransform.position.y, position.y);
                cloneTransform.parent = trnsfrm;
                _obstacles.Add(clone);
                obstacleCount++;
            }
            else
            {
                break;
            }
        }
    }

    void ActivateWall(ref List<GenerationInfo> positionsOccupied)
    {
        float chance = Random.Range(0.0f, 1.1f);
        if (chance < 0.4f)
        {
            ActivateWallAtIndex(ref positionsOccupied, 0);
        }
        else if (chance < 0.8f)
        {
            ActivateWallAtIndex(ref positionsOccupied, 1);
        }
        else
        {
            ActivateWallAtIndex(ref positionsOccupied, 2);
        }
    }

    void ActivateWallAtIndex(ref List<GenerationInfo> positionsOccupied, int index)
    {
        _obstacles[index].SetActive(true);
        Transform cloneTransform = _obstacles[index].transform;
        Vector2 position = new Vector2(cloneTransform.position.x, cloneTransform.position.z);
        Vector2 scaleSize = new Vector2(cloneTransform.localScale.x, cloneTransform.localScale.z);
        positionsOccupied.Add(new GenerationInfo(position, scaleSize));
    }
   
    Vector2 GetAvailabePosition(Vector2 othersize, ref List<GenerationInfo> positionsOccupied)
    {
        GenerationInfo data = new GenerationInfo(GetRandomPosition(othersize), othersize);
        int guard = 0;
        while (ShouldRespawn(data, ref positionsOccupied))
        {
            data.position = GetRandomPosition(othersize);
            if (guard == 1000)
            {
                data.position = new Vector2(-1, -1);
                break;
            }
            else
            {
                guard++;
            }
        }
        return data.position;
    }

    Vector2 GetRandomPosition(Vector2 otherSize)
    {
        Vector2 position = new Vector2(trnsfrm.position.x + Random.Range(-size.x + otherSize.x, size.x - otherSize.x), trnsfrm.position.z + Random.Range(-size.y + otherSize.y, size.y - otherSize.y));
        return position;
    }

    bool ShouldRespawn(GenerationInfo other, ref List<GenerationInfo> positionsOccupied)
    {
        bool shouldRespawn = false;

        for (int i = 0; i < positionsOccupied.Count; i++)
        {
            float diameter = other.size.magnitude + positionsOccupied[i].size.magnitude;
            if (Vector2.Distance(other.position, positionsOccupied[i].position) < diameter)
            {
                shouldRespawn = true;
                break;
            }
        }
        return shouldRespawn;
    }

    public Vector3 Position
    {
        get { return trnsfrm.position; }
    }
}
