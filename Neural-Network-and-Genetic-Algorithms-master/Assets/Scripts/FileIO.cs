using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class FileIO
{
    public static void WriteJson<T>(string path, ref T classData)
    {
        string fullPath = Application.persistentDataPath + "/" + path;
        string data = JsonUtility.ToJson(classData);
        File.WriteAllText(fullPath, data);
    }

    public static T ReadJson<T>(string path)
    {
        string fullPath = Application.persistentDataPath + "/" + path;
        T classData = default(T);

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            classData = (T)JsonUtility.FromJson<T>(data);
        }
        else
        {
            Debug.Log("FILE DOES NOT EXIST!!!");
        }

        return classData;
    }


    [System.Serializable]
    public class GenomeData
    {
        public GenomeData(List<float> w)
        {
            weights = w;
        }

        public List<float> weights;
    }

    [System.Serializable]
    public class EvolutionData
    {
        public EvolutionData(List<ImprovementData> dt, int genomesAlive, int timesLegendUsed)
        {
            this.genomesAlive = genomesAlive;
            dataTracked = dt;
            timesHeroEvoked = timesLegendUsed;
        }

        public int genomesAlive;
        public int timesHeroEvoked;
        public List<ImprovementData> dataTracked;
    }

    [System.Serializable]
    public class ImprovementData
    {
        public ImprovementData(float f, int g, int gsi)
        {
            fitness = f;
            generation = g;
            generationsSinceImprovement = gsi;
        }

        public int      generation;
        public int      generationsSinceImprovement;
        public float    fitness;
    }

    [System.Serializable]
    public class GameData
    {
        public GameData(List<AI_Data> dt, string w, int nnap, int aiap, int nntp, int aitp)
        {
            dataTracked = dt;
            winner = w;
            nnAvgScore = nnap;
            aiAvgScore = aiap;
            nnTotalScore = nntp;
            aiTotalScore = aitp;
        }

        public string winner;
        public int nnAvgScore;
        public int aiAvgScore;
        public int nnTotalScore;
        public int aiTotalScore;
        public List<AI_Data>    dataTracked;
    }

    [System.Serializable]
    public class AI_Data
    {
        public AI_Data(string t, int p)
        {
            type = t;
            points = p;
        }

        public string type;
        public int points;
    }
}
