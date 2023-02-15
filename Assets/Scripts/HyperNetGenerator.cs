using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Collections;

class HyperNetGenerator : MonoBehaviour
{
    public string url = "https://github.com/";
    public GameObject[] tilePrefab;

    public int maxHeight;
    public int maxWidth;

    public SpriteRenderer background;

    System.Random rand = new System.Random();

    IEnumerator Start() 
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
        
        yield return StartCoroutine(DownloadImage("https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=" + url));
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

        var links = doc.DocumentNode.SelectNodes("//a[@href]");

        // Create a random number generator
        rand = new System.Random(url.GetHashCode());

        // Create a queue to hold the nodes that need to be processed
        Queue<HtmlNode> nodes = new Queue<HtmlNode>();
        nodes.Enqueue(doc.DocumentNode);

        // Get the total number of objects in the HTML file
        int numObjects = doc.DocumentNode.SelectNodes("//div").Count;
        // Get the maximum size of the map
        maxWidth = Math.Min(500, Math.Max(100, 50 + (numObjects * 2)));
        maxHeight = Math.Min(500, Math.Max(100, 50 + (numObjects * 2)));
        // Create an empty 2D map
        TileType[,] map = new TileType[maxWidth,maxHeight];
        // Initialize the map with Floor
        for(int i = 0; i < maxWidth; i++)
        {
            for(int j = 0; j < maxHeight; j++)
            {
                map[i,j] = TileType.NONE;
            }
        }
        
        // Create a list to store the positions of the rooms on the map
        List<Tuple<int, int, int, int>> rooms = new List<Tuple<int, int, int, int>>();
        List<Tuple<int, int>> linkTuple = new List<Tuple<int, int>>();

        // Process the nodes
        while (nodes.Count > 0) 
        {
            HtmlNode currentNode = nodes.Dequeue();

            // Add the children of the current node to the queue
            foreach (HtmlNode child in currentNode.ChildNodes)
             {
                nodes.Enqueue(child);
            }

            // Get the position of the current node on the map
            int x = rand.Next(maxWidth);
            int y = rand.Next(maxHeight);

            // Visualize the current node on the map
            if (currentNode.Name == "div") {
                // Get the size of the room based on the length of the content in the node
                int roomWidth = Math.Min(5, Math.Max(2, currentNode.InnerText.Length / 10));
                int roomHeight = Math.Min(5, Math.Max(2, currentNode.InnerText.Length / 10));
                // Create a room for the <a> element
                for (int i = x - roomWidth / 2; i <= x + roomWidth / 2; i++) {
                    for (int j = y - roomHeight / 2; j <= y + roomHeight / 2; j++) {
                        if (i >= 0 && i < maxWidth && j >= 0 && j < maxHeight) 
                        {
                            map[i, j] = TileType.ROOM;
                        }
                    }
                }
                // Add the position of the room to the list
                rooms.Add(new Tuple<int, int, int, int>(x, y, roomWidth, roomHeight));
            } 
            else if(currentNode.Name == "a")
            {
                linkTuple.Add(new Tuple<int, int>(rooms.Count-1
                , 0));
            }
        }
        // Create walls around the rooms
        foreach (var room in rooms) 
        {
            for (int i = room.Item1 - 1; i <= room.Item1 + 1; i++) 
            {
                for (int j = room.Item2 - 1; j <= room.Item2 + 1; j++) 
                {
                    if (i >= 0 && i < maxWidth && j >= 0 && j < maxHeight && map[i, j] == TileType.FLOOR) 
                    {
                        map[i, j] = TileType.WALL;
                    }
                }
            }
        }

        if (links != null)
        {
            foreach (var link in linkTuple)
            {
                // Determine which room the link belongs to
                // Determine which room the link belongs to
                int roomIndex = link.Item1;
                Tuple<int, int, int, int> room = rooms[roomIndex];
                int x = room.Item1 + room.Item3 / 2;
                int y = room.Item2 + room.Item4 / 2;
                //if (map[x, y] == TileType.WALL)
                map[x, y] = TileType.TELEPORTER;
            }
        }
        // Connect the rooms with pathways
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            int x1 = rooms[i].Item1;
            int y1 = rooms[i].Item2;
            int x2 = rooms[i + 1].Item1;
            int y2 = rooms[i + 1].Item2;
            while (x1 != x2 || y1 != y2) 
            {
                if (x1 < x2) x1++;
                else if (x1 > x2) x1--;
                if (y1 < y2) y1++;
                else if (y1 > y2) y1--;
                if (x1 >= 0 && x1 < maxWidth && y1 >= 0 && y1 < maxHeight) 
                {
                    if (map[x1, y1] == TileType.NONE) map[x1, y1] = TileType.FLOOR;
                    // check the tiles to the left and right of the current tile
                    if (x1-1 >= 0 && map[x1-1, y1] == TileType.NONE) map[x1-1, y1] = TileType.FLOOR;
                    if (x1+1 < maxWidth && map[x1+1, y1] == TileType.NONE) map[x1+1, y1] = TileType.FLOOR;
                    // check the tiles above and below the current tile
                    if (y1-1 >= 0 && map[x1, y1-1] == TileType.NONE) map[x1, y1-1] = TileType.FLOOR;
                    if (y1+1 < maxHeight && map[x1, y1+1] == TileType.NONE) map[x1, y1+1] = TileType.FLOOR;
                }
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

    IEnumerator DownloadImage(string MediaUrl)
    {   
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if(request.isNetworkError || request.isHttpError) 
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D rawImage = ((DownloadHandlerTexture) request.downloadHandler).texture;
            rawImage.filterMode = FilterMode.Point;
            rawImage.wrapMode = TextureWrapMode.Repeat;
            rawImage.Apply();
            background.sprite = Sprite.Create(rawImage, new Rect(0.0f, 0.0f, rawImage.width, rawImage.height), new Vector2(0.5f, 0.5f), 16.0f, 4, SpriteMeshType.FullRect);
        }
    } 
}
    

    public class Teleporter 
    {
        public int x, y;
        public string url;
    }

    enum TileType
    {
        NONE, ROOM, WALL, FLOOR, TELEPORTER
    }