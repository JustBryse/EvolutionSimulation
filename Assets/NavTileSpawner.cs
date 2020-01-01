using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavTileSpawner : MonoBehaviour {

    public NavTile navTile;

	// Use this for initialization
	void Awake ()
    {
        spawnNavTiles();
	}

    private void spawnNavTiles()
    {
        GameObject environment = GameObject.Find("Environment");
        for (float x = 0.5f + -environment.transform.lossyScale.x/2; x < environment.transform.lossyScale.x/2; x++)
        {
            for (float z = 0.5f + -environment.transform.lossyScale.z / 2; z < environment.transform.lossyScale.z/2; z++)
            {
                Vector3 spawnPos = new Vector3(x, 1, z);
                Instantiate(navTile.gameObject, spawnPos, Quaternion.identity);
            }
        }
    }
}
