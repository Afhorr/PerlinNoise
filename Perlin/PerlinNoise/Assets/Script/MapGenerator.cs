using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class MapGenerator : MonoBehaviour {

	public enum GenerationType
    {
        RANDOM,PERLINNOIS
    }
    public GenerationType generationType;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public Tilemap tilemap;
    public bool autoUpdate;
    public TerrainType[] regions;
    public TerrainTypeMineral[] minerals; 
    public TerrainTypeCaverne[] cavernes;
    public void GenerateMap()
    {
        if (generationType == GenerationType.PERLINNOIS)
        {
            generateMapWithNoise();
        }
        else if (generationType == GenerationType.RANDOM)
        {
            generateMapWithRandom();
        }
    }


    public void generateMapWithNoise()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseMapMineral = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed+2, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseMapGissement = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed+1, noiseScale, octaves, persistance, lacunarity, offset);


        TileBase[] customTileMap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float rndNoiseMapGissement = noiseMapGissement[x, y];
                if(rndNoiseMapGissement > 0.7 )
                {
                    float rndGissement = noiseMapMineral[x, y];
                    customTileMap[y * mapWidth + x] = FindTileFromMinaral(rndGissement);
                }
                else
                {
                    float rnd = noiseMap[x, y];
                    customTileMap[y * mapWidth + x] = FindTileFromRegion(rnd);
                }

            }
        }
        setTilMap(customTileMap);

    }
    public void generateMapWithRandom()
    {
        TileBase[] customTileMap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float rnd = UnityEngine.Random.Range(0f, 1f);
                customTileMap[y * mapWidth + x] = FindTileFromRegion(rnd);
            }
        }
        setTilMap(customTileMap);
    }

    private void setTilMap(TileBase[] customTileMap)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), customTileMap[y * mapWidth + x]);
            }
        }
    }

    private TileBase FindTileFromRegion(float rnd)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (rnd <= regions[i].heigth)
            {
                return regions[i].tile;
            }
        }
        for (int i = 0; i < minerals.Length; i++)
        {
            if (regions[i].Name == "Coal" || regions[i].Name == "Gold" || regions[i].Name == "RedStone")
            {
                return minerals[i].tile;
            }
        }
        return regions[0].tile;
    }
    private TileBase FindTileFromMinaral(float rnd)
    {

        for (int i = 0; i < minerals.Length; i++)
        {
            if (rnd <= minerals[i].heigth)
            {
                return minerals[i].tile;
            }
        }
        return minerals[0].tile;
    }
    private TileBase FindTileFromCaverne(float rnd)
    {

        for (int i = 0; i < cavernes.Length; i++)
        {
            if (rnd <= cavernes[i].heigth)
            {
                return cavernes[i].tile;
            }
        }
        return cavernes[0].tile;
    }
    private void OnValidate()
    {
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (lacunarity <1)
        {
            lacunarity = 1;
        }
        if (octaves < 1)
        {
            octaves = 1;
        }
    }
}


[System.Serializable]
public struct TerrainType
{
    public string Name;
    public float heigth;
    public TileBase tile;    
}
[System.Serializable]
public struct TerrainTypeMineral
{
    public string Name;
    public float heigth;
    public TileBase tile;
}

[System.Serializable]
public struct TerrainTypeCaverne
{
    public string Name;
    public float heigth;
    public TileBase tile;
}

