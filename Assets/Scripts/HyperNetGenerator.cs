using System;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using HtmlAgilityPack;

class HyperNetGenerator : MonoBehaviour
{
    public string url = "https://github.com/";
    public GameObject[] tilePrefab;

    public int maxHeight;
    public int maxWidth;

    System.Random rand = new System.Random();

    void Start() 
    {
        maxHeight = 500;
        maxWidth = 500;

        string html = new WebClient().DownloadString(url);
        List<string> urls = ExtractUrls(html);

        // Generate the 2D tile map
        TileType[,] map = GenerateMap(url);
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
    TileType[,] GenerateMap(string url)
    {
        // Load the HTML from the specified URL
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);

        // Create a random number generator
        rand = new System.Random(url.GetHashCode());

        // Create a queue to hold the nodes that need to be processed
        Queue<HtmlNode> nodes = new Queue<HtmlNode>();
        nodes.Enqueue(doc.DocumentNode);

        // Create an empty 2D map
        TileType[,] map = new TileType[500, 500];
        // Initialize the map with Floor
        for (int i = 0; i < 500; i++)
        {
            for (int j = 0; j < 500; j++)
            {
                map[i, j] = TileType.NONE;
            }
        }

        // Create a list to store the positions of the rooms on the map
        List<Tuple<int, int>> rooms = new List<Tuple<int, int>>();

        // Process the nodes in the queue
        while (nodes.Count > 0)
        {
            HtmlNode currentNode = nodes.Dequeue();

            // Add the children of the current node to the queue
            foreach (HtmlNode child in currentNode.ChildNodes)
            {
                nodes.Enqueue(child);
            }

            // Get the position of the current node on the map
            int x = rand.Next(500);
            int y = rand.Next(500);

            // Visualize the current node on the map
            if (currentNode.Name == "a")
            {
                // Create a room for the <a> element
                map[x, y] = TileType.ROOM;
                // Add the position of the room to the list
                rooms.Add(new Tuple<int, int>(x, y));
            }
            else
            {
                // Create a floor tile for other elements
                map[x, y] = TileType.FLOOR;
            }
        }
        // Create walls around the rooms
        foreach (var room in rooms)
        {
            for (int i = room.Item1 - 1; i <= room.Item1 + 1; i++)
            {
                for (int j = room.Item2 - 1; j <= room.Item2 + 1; j++)
                {
                    if (i >= 0 && i < 500 && j >= 0 && j < 500 && map[i, j] == TileType.FLOOR)
                    {
                        map[i, j] = TileType.WALL;
                    }
                }
            }
        }
        // Connect the rooms with pathways
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            // use Bresenham's line algorithm to create a path between the two rooms 
            int x1 = rooms[i].Item1;
            int y1 = rooms[i].Item2;
            int x2 = rooms[i + 1].Item1;
            int y2 = rooms[i + 1].Item2;
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;
            while (true)
            {
                int e2 = err * 2;
                if (x1 == x2 && y1 == y2) break;
                if (e2 > -dy) { err -= dy; x1 += sx; }
                if (e2 < dx) { err += dx; y1 += sy; }
                if (map[x1, y1] == TileType.WALL) map[x1, y1] = TileType.FLOOR;
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