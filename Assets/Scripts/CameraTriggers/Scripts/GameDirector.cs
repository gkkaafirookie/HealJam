using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : Singleton<GameDirector>
{
    public CustomList<string, PlayerSpawn> SpawnPoints;

    public string currentPoint;

    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPlayer()
    {
        var sp = SpawnPoints.Get(currentPoint);
        sp.SetSpawn(player);
    }
}
