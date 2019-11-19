using UnityEngine;
using System.Collections.Generic;

public enum PlayerType
{
    NeuralNetwork,
    ScriptedAI,
    Count
}

public class PrefabController : MonoBehaviour
{
    public enum Prefab
    {
        Terrain,
        Reward,
        Obstacle,
        PlayerAI,
        Wall,
        ParticleSystem,
        Count
    }
    static Dictionary<Prefab, GameObject> prefabs;
    static Dictionary<PlayerType, Material> materials;

    static public void Initialize()
    {
        prefabs = new Dictionary<Prefab, GameObject>((int)Prefab.Count);
        materials = new Dictionary<PlayerType, Material>((int)PlayerType.Count);
        LoadContent();
    }

    static void LoadContent()
    {
        prefabs.Add(Prefab.Terrain, Resources.Load<GameObject>("Prefabs/RegionPrefab"));
        prefabs.Add(Prefab.Reward, Resources.Load<GameObject>("Prefabs/YellowBallPrefab"));
        prefabs.Add(Prefab.Obstacle, Resources.Load<GameObject>("Prefabs/ObstaclePrefab"));
        prefabs.Add(Prefab.PlayerAI, Resources.Load<GameObject>("Prefabs/UnitPrefab"));
        prefabs.Add(Prefab.Wall, Resources.Load<GameObject>("Prefabs/WallPrefab"));
        prefabs.Add(Prefab.ParticleSystem, Resources.Load<GameObject>("Prefabs/ParticleSystem"));
        materials.Add(PlayerType.NeuralNetwork, Resources.Load<Material>("Materials/NNAI"));
        materials.Add(PlayerType.ScriptedAI, Resources.Load<Material>("Materials/ScriptedAI"));
    }

    static public GameObject CreateGameObject(Prefab prefab)
    {
        GameObject clone = null;
        switch (prefab)
        {
            case Prefab.Terrain:
                clone = GenerateTerrain();
                break;
            case Prefab.Reward:
                clone = GenerateReward();
                break;
            case Prefab.Obstacle:
                clone = GenerateObstacle();
                break;
            case Prefab.PlayerAI:
                clone = GeneratePlayerAI();
                break;
            case Prefab.Wall:
                clone = GenerateWall();
                break;
            case Prefab.ParticleSystem:
                clone = GenerateParticles();
                break;
            default:
                break;
        }
        return clone;
    }

    static  GameObject GenerateTerrain()
    {
        GameObject clone = Instantiate(prefabs[Prefab.Terrain], prefabs[Prefab.Terrain].transform.position, prefabs[Prefab.Terrain].transform.rotation) as GameObject;
        return clone;
    }

    static GameObject GenerateReward()
    {
        GameObject clone = Instantiate(prefabs[Prefab.Reward], prefabs[Prefab.Reward].transform.position, prefabs[Prefab.Reward].transform.rotation) as GameObject;
        return clone;
    }

    static GameObject GenerateObstacle()
    {
        GameObject clone = Instantiate(prefabs[Prefab.Obstacle], prefabs[Prefab.Obstacle].transform.position, prefabs[Prefab.Obstacle].transform.rotation) as GameObject;
        return clone;
    }

    static GameObject GeneratePlayerAI()
    {
        GameObject clone = Instantiate(prefabs[Prefab.PlayerAI], prefabs[Prefab.PlayerAI].transform.position, prefabs[Prefab.PlayerAI].transform.rotation) as GameObject;
        return clone;
    }

    static GameObject GenerateWall()
    {
        GameObject clone = Instantiate(prefabs[Prefab.Wall], prefabs[Prefab.Wall].transform.position, prefabs[Prefab.Wall].transform.rotation) as GameObject;
        return clone;
    }

    static GameObject GenerateParticles()
    {
        GameObject clone = Instantiate(prefabs[Prefab.ParticleSystem], prefabs[Prefab.ParticleSystem].transform.position, prefabs[Prefab.ParticleSystem].transform.rotation) as GameObject;
        return clone;
    }

    static public PlayerMode GeneratePlayerType(PlayerType unitType)
    {
        PlayerMode unit = null;
        switch (unitType)
        {
            case PlayerType.NeuralNetwork:
                unit = GeneratePlayerNN();
                break;
            case PlayerType.ScriptedAI:
                unit = GenerateTrainedAI();
                break;
            default:
                break;
        }
        return unit;
    }

    static PlayerMode GeneratePlayerNN()
    {
        GameObject playerClone = GeneratePlayerAI();
        playerClone.AddComponent<NeuralNetworkPlayer>();
        MeshRenderer[] meshes = playerClone.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].sharedMaterial = materials[PlayerType.NeuralNetwork];
        }

        return playerClone.GetComponent<NeuralNetworkPlayer>();
    }

    static PlayerMode GenerateTrainedAI()
    {
        GameObject unitClone = GeneratePlayerAI();
        unitClone.AddComponent<TrainedAI>();
        MeshRenderer[] meshes = unitClone.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].sharedMaterial = materials[PlayerType.ScriptedAI];
        }
        return unitClone.GetComponent<TrainedAI>();
    }

    static public Vector2 GetParticleSystemSize()
    {
        return new Vector2(prefabs[Prefab.ParticleSystem].GetComponent<ParticleSystem>().shape.scale.x, prefabs[Prefab.ParticleSystem].GetComponent<ParticleSystem>().shape.scale.z);
    }

    static public Vector2 GetSize(Prefab prefab)
    {
        return GetObjectSize(prefabs[prefab]);
    }

    static Vector2 GetObjectSize(GameObject theObject)
    {
        return new Vector2(theObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * theObject.transform.localScale.x, theObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.z * theObject.transform.localScale.z);
    }
}
