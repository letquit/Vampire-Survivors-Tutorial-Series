using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropRateManager : MonoBehaviour
{
    public List<Drops> drops;

    private void OnDestroy()
    {
        float randomNumber = Random.Range(0f, 100f);
        List<Drops> possiableDrops = new List<Drops>();

        foreach (Drops rate in drops)
        {
            if (randomNumber <= rate.dropRate)
            {
                possiableDrops.Add(rate);
            }
        }

        if (possiableDrops.Count > 0)
        {
            Drops drops = possiableDrops[Random.Range(0, possiableDrops.Count)];
            Instantiate(drops.itemPrefab, transform.position, Quaternion.identity);
        }
    }

    [Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }
}
