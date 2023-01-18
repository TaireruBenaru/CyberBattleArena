using System;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Net;
using System.Linq;
using System.Collections.Generic;

class HyperNetGenerator : MonoBehaviour
{
    public string url = "https://github.com/";
    public GameObject[] tilePrefab;

    public int maxHeight;
    public int maxWidth;

    System.Random rand = new System.Random();

    void Start() 
    {
        string html = new WebClient().DownloadString(url);
        List<string> urls = ExtractUrls(html);

        // Generate the 2D tile map
        TileType[,] map = GenerateMap(urls.Count);
        PrintMap(map);

        // Find teleporter locations and set their destination URLs
        List<Teleporter> teleporters = FindTeleporters(map, urls);
        foreach (Teleporter t in teleporters) {
            Debug.Log("Teleporter at (" + t.x + ", " + t.y + ") leads to " + t.url);
        }
    }

    // Extract all URLs found in the HTML data
    List<string> ExtractUrls(string html) 
    {
        List<string> urls = new List<string>();
        // Use a regular expression to match URLs in the HTML data
        MatchCollection matches = Regex.Matches(html, @"https?://[^\s]+");
        foreach (Match match in matches) {
            urls.Add(match.Value);
        }
        return urls;
    }

    // Generate the 2D tile map
    TileType[,] GenerateMap(int numRooms) 
    {
        // Create a random number generator
        rand = new System.Random(url.GetHashCode());

        maxWidth = Math.Min(500, Math.Max(100, 50 + (numRooms * 2)));
        maxHeight = Math.Min(500, Math.Max(100, 50 + (numRooms * 2)));

        // Create an empty 2D map with a variable size
        TileType[,] map = new TileType[maxWidth,maxHeight];
        // Initialize the map with Floor
        for(int i = 0; i < maxWidth; i++)
        {
            for(int j = 0; j < maxHeight; j++)
            {
                map[i,j] = TileType.FLOOR;
            }
        }
        // Add rooms to the map at random locations
        for (int i = 0; i < numRooms; i++) 
        {
            //Random width and height of the room
            int width = rand.Next(10, maxWidth);
            int height = rand.Next(10, maxHeight);
            //random position of the room
            int x = rand.Next(maxWidth - width);
            int y = rand.Next(maxHeight - height);
            for(int k = x; k < x+width; k++)
            {
                for(int j = y; j < y+height; j++)
                {
                    map[k,j] = TileType.ROOM;
                }
            }
        }
        
        for(int i = 0; i < maxWidth; i++)
        {
            for(int j = 0; j < maxHeight; j++)
            {
                if(map[i,j] == TileType.ROOM)
                {
                    if(i > 0 && map[i-1,j] == TileType.FLOOR) map[i-1,j] = TileType.WALL;
                    if(i < maxWidth-1 && map[i+1,j] == TileType.FLOOR) map[i+1,j] = TileType.WALL;
                    if(j > 0 && map[i,j-1] == TileType.FLOOR) map[i,j-1] = TileType.WALL;
                    if(j < maxHeight-1 && map[i,j+1] == TileType.FLOOR) map[i,j+1] = TileType.WALL;
                }
            }
        }

        // Add pathways to connect the rooms
        for (int i = 0; i < numRooms; i++) 
        {
            int x1 = rand.Next(maxWidth);
            int y1 = rand.Next(maxHeight);
            while(map[x1,y1] != TileType.ROOM)
            {
                x1 = rand.Next(maxWidth);
                y1 = rand.Next(maxHeight);
            }
            int x2 = rand.Next(maxWidth);
            int y2 = rand.Next(maxHeight);
            while(map[x2,y2] != TileType.ROOM)
            {
                x2 = rand.Next(maxWidth);
                y2 = rand.Next(maxHeight);
            }
            // Connect the two rooms with a pathway
            // check if the two rooms are not the same room
            if(x1 != x2 || y1 != y2)
            {
                // use bresenham's line algorithm to create a path between the two rooms 
                int dx = Math.Abs(x2 - x1);
                int dy = Math.Abs(y2 - y1);
                int sx = x1 < x2 ? 1 : -1;
                int sy = y1 < y2 ? 1 : -1;
                int err = dx - dy;
                while(true)
                {
                    int e2 = err * 2;
                    if(x1 == x2 && y1 == y2) break;
                    if(e2 > -dy) { err -= dy; x1 += sx; }
                    if(e2 < dx) { err += dx; y1 += sy; }
                    if(map[x1,y1] != TileType.ROOM) map[x1,y1] = TileType.FLOOR;
                }
                // with a probability of 30% make the path a dead end
                if(rand.Next(10) < 3) map[x1,y1] = TileType.ROOM;
            }
        }
    return map;
}

    // Print the 2D tile map to the console
    void PrintMap(TileType[,] map) 
    {
        for (int i = 0; i < maxWidth; i++) 
        {
            for (int j = 0; j < maxHeight; j++) 
            {
                Instantiate(tilePrefab[(int)map[i, j]], new Vector3(i*2, j*2), Quaternion.identity);
                Debug.Log(map[i, j]);
                //Console.Write(map[i, j] + " ");
            }
        }
    }

    // Find teleporter locations in the map and set their destination URLs
    List<Teleporter> FindTeleporters(TileType[,] map, List<string> urls) 
    {
        List<Teleporter> teleporters = new List<Teleporter>();
        // Iterate through the map and find teleporter locations
        for (int i = 0; i < maxWidth; i++) 
        {
            for (int j = 0; j < maxHeight; j++) 
            {
                if (map[i, j] == TileType.WALL) 
                {
                    // Create a new teleporter at this location
                    Teleporter t = new Teleporter();
                    t.x = i;
                    t.y = j;
                    // Assign a destination URL to the teleporter
                    t.url = urls[rand.Next(urls.Count)];
                    teleporters.Add(t);
                }
            }
        }
        return teleporters;
    }
}
    

    public class Teleporter 
    {
        public int x, y;
        public string url;
    }

    enum TileType
    {
        NONE, ROOM, WALL, FLOOR
    }