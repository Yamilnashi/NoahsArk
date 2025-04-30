using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using NoahsArk.Controls;
using NoahsArk.Entities;
using NoahsArk.Entities.Enemies;
using NoahsArk.Entities.GameObjects;
using NoahsArk.Levels;
using NoahsArk.Levels.Maps;

namespace NoahsArk.Utilities
{
    public class TileMapReader
    {
        #region Fields
        private string _filePath;
        private int _mapWidth = 0;
        private int _mapHeight = 0;
        private int _tileWidth = 0;
        private int _tileHeight = 0;
        private List<TileSet> _tileSets = new List<TileSet>();
        private List<ILayer> _mapLayers = new List<ILayer>();
        private int _currentLayerId = 0;
        private int _currentObjectGroupId = 0;
        private int _currentObjectId = 0;
        private string _currentNodeType;
        private List<Rectangle> _obstacles = new List<Rectangle>();
        private List<DoorTransition> _doors = new List<DoorTransition>();
        private List<EnemySpawner> _enemySpawners = new List<EnemySpawner>();
        private Dictionary<int, (string, float)> _mapLayerNameDict = new Dictionary<int, (string, float)>();
        private Dictionary<int, long[,]> _mapLayerDataDict = new Dictionary<int, long[,]>();
        private Dictionary<int, Dictionary<string, object>> _mapLayerPropertiesDict = new Dictionary<int, Dictionary<string, object>>();
        private Dictionary<int, string> _objectGroupNameDict = new Dictionary<int, string>();
        private Dictionary<(int, int), Rectangle> _objectRectangleDict = new Dictionary<(int, int), Rectangle>();
        private Dictionary<int, Dictionary<string, object>> _objectGroupPropertiesDict = new Dictionary<int, Dictionary<string, object>>();
        private Dictionary<(int, int), Dictionary<string, object>> _objectPropertiesDict = new Dictionary<(int, int), Dictionary<string, object>>();
        private readonly XmlReaderSettings _xmlReaderSettings = new XmlReaderSettings() { IgnoreWhitespace = true };
        #endregion

        #region Constructor
        public TileMapReader(string filePath)
        {
            _filePath = filePath;
        }
        #endregion

        #region Methods
        public void ProcessTileMap()
        {
            ReadTileMap();
            AddMapLayers();
            AddObjects();            
        }
        public TileMap CreateTileMap()
        {
            return new TileMap(_mapWidth,
                                    _mapHeight,
                                    _tileWidth,
                                    _tileHeight,
                                    _mapLayers,
                                    _tileSets,
                                    _obstacles,
                                    _doors,
                                    _enemySpawners);
        }
        #endregion

        #region Private
        private void ReadTileMap()
        {
            using (XmlReader reader = XmlReader.Create(_filePath, _xmlReaderSettings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "map":
                                _currentNodeType = reader.Name;
                                HandleMap(reader);
                                break;
                            case "tileset":
                                _currentNodeType = reader.Name;
                                HandleTileSet(reader);
                                break;
                            case "layer":
                                _currentNodeType = reader.Name;
                                HandleLayer(reader);
                                break;
                            case "data":
                                _currentNodeType = reader.Name;
                                HandleData(reader);
                                break;
                            case "objectgroup":
                                _currentNodeType = reader.Name;
                                HandleObjectGroup(reader);
                                break;
                            case "object":
                                _currentNodeType = reader.Name;
                                HandleObject(reader);
                                break;
                            case "properties":
                                HandleProperties(reader);
                                break;
                            case "property":
                                HandleProperty(reader);
                                break;
                            default:
                                Write($"Found element: {reader.Name}");
                                break;
                        }
                    }
                }
            }
        }
        private void HandleMap(XmlReader reader)
        {
            Write($"Map properties: Version: {reader.GetAttribute("version")}, " +
                  $"RenderOrder: {reader.GetAttribute("renderorder")}");
            _mapWidth = int.Parse(reader.GetAttribute("width"));
            _mapHeight = int.Parse(reader.GetAttribute("height"));
            _tileWidth = int.Parse(reader.GetAttribute("tilewidth"));
            _tileHeight = int.Parse(reader.GetAttribute("tileheight"));
        }
        private void HandleTileSet(XmlReader reader)
        {
            string source = reader.GetAttribute("source");
            int firstGid = int.Parse(reader.GetAttribute("firstgid"));
            TileSet tileSet = new TileSet(source, firstGid, _tileWidth, _tileHeight);
            _tileSets.Add(tileSet);
        }
        private void HandleLayer(XmlReader reader)
        {
            int layerId = int.Parse(reader.GetAttribute("id"));
            string layerName = reader.GetAttribute("name");
            float opacity = 1f;
            if (reader.GetAttribute("opacity") != null)
            {
                opacity = float.Parse(reader.GetAttribute("opacity"));
            }
            Write($"Layer: {layerName}");
            _mapLayerNameDict[layerId] = (layerName, opacity);
            _mapLayerPropertiesDict[layerId] = new Dictionary<string, object>();
            _currentLayerId = layerId;            
        }
        private void HandleData(XmlReader reader)
        {
            string encoding = reader.GetAttribute("encoding");
            if (encoding.Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                string csv = reader.ReadElementContentAsString().Trim();
                long[,] data = ParseCsvData(csv, _mapWidth, _mapHeight);
                _mapLayerDataDict[_currentLayerId] = data;
                Write($"CsvData (First 50 chars): {csv.Substring(0, Math.Min(50, data.Length))}");
            }
        }
        private void HandleObjectGroup(XmlReader reader)
        {
            int objectGroupId = int.Parse(reader.GetAttribute("id"));
            string groupName = reader.GetAttribute("name");
            Write($"Object Group: {groupName}");
            _objectGroupNameDict[objectGroupId] = groupName;
            _objectGroupPropertiesDict[objectGroupId] = new Dictionary<string, object>();
            _currentObjectGroupId = objectGroupId;
        }
        private void HandleObject(XmlReader reader)
        {
            int objectId = int.Parse(reader.GetAttribute("id"));
            string name = reader.GetAttribute("name") ?? "Unamed";
            float x = float.Parse(reader.GetAttribute("x"));
            float y = float.Parse(reader.GetAttribute("y"));
            float width = float.Parse(reader.GetAttribute("width"));
            float height = float.Parse(reader.GetAttribute("height"));
            //todo: if we have other types of objects like circles, polygons, detect those here
            _objectRectangleDict[(_currentObjectGroupId, objectId)] = new Rectangle(
                    (int)x,
                    (int)y, 
                    (int)width,
                    (int)height
                );
            _objectPropertiesDict[(_currentObjectGroupId, objectId)] = new Dictionary<string, object>();
            _currentObjectId = objectId;
        }
        private void HandleProperties(XmlReader reader)
        {
            Write($"Properties:");
        }
        private void HandleProperty(XmlReader reader)
        {
            string name = reader.GetAttribute("name");
            string type = reader.GetAttribute("type");
            string valueString = reader.GetAttribute("value");
            object value = null;
            switch (type)
            {
                case "bool":
                    value = bool.Parse(valueString);
                    break;
                case "int":
                    value = int.Parse(valueString);
                    break;
                default:
                    value = valueString;
                    break;
            }

            if (_currentNodeType.Equals("objectgroup", StringComparison.OrdinalIgnoreCase))
            {
                _objectGroupPropertiesDict[_currentObjectGroupId].Add(name, value);
            }
            else if (_currentNodeType.Equals("object"))
            {
                _objectPropertiesDict[(_currentObjectGroupId, _currentObjectId)].Add(name, value);
            }
            else if (_currentNodeType.Equals("layer", StringComparison.OrdinalIgnoreCase))
            {
                _mapLayerPropertiesDict[_currentLayerId].Add(name, value);
            }
            Write($"    Property: {name} = {valueString}");
        }
        private long[,] ParseCsvData(string csvData, int width, int height)
        {
            long[,] tileData = new long[height, width];
            string[] rows = csvData.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            for (int y = 0; y < height; y++)
            {
                string[] tiles = rows[y].Split(',');
                for (int x = 0; x < width; x++)
                {
                    tileData[y, x] = long.Parse(tiles[x]);
                }
            }
            return tileData;
        }
        private void AddMapLayers()
        {
            for (int i = 0; i < _mapLayerNameDict.Keys.Count; i++)
            {
                int layerId = _mapLayerNameDict.Keys.ElementAt(i);
                (string layerName, float opacity) = _mapLayerNameDict[layerId];
                long[,] data = _mapLayerDataDict[layerId];
                Dictionary<string, object> props = new Dictionary<string, object>();
                if (_mapLayerPropertiesDict.TryGetValue(layerId, out var properties))
                {
                    props = properties;
                }

                ILayer mapLayer = new MapLayer(layerName, data, opacity, props);
                _mapLayers.Add(mapLayer);
            }
        }
        private void AddObjects()
        {
            for (int i = 0; i < _objectGroupNameDict.Keys.Count; i++)
            {
                int objectGroupId = _objectGroupNameDict.Keys.ElementAt(i);
                string objectGroupName = _objectGroupNameDict[objectGroupId];

                for (int j = 0; j < _objectRectangleDict.Keys.Count; j++)
                {
                    (int groupId, int objectId) obj = _objectRectangleDict.Keys.ElementAt(j);
                    Rectangle rectangle = new Rectangle();
                    if (_objectRectangleDict.TryGetValue((objectGroupId, obj.objectId), out var rect))
                    {
                        rectangle = rect;
                        if (objectGroupName.Equals("obstacles", StringComparison.OrdinalIgnoreCase))
                        {
                            _obstacles.Add(rectangle);
                        }
                        else if (objectGroupName.Equals("signs", StringComparison.OrdinalIgnoreCase))
                        {
                            // add signs
                        }
                        else if (objectGroupName.Equals("enemy spawner", StringComparison.OrdinalIgnoreCase))
                        {
                            // add enemy spawners
                            Dictionary<string, object> properties = _objectPropertiesDict[(objectGroupId, obj.objectId)];
                            AddEnemySpawner(properties, rectangle);
                        }
                        else if (objectGroupName.Equals("interactables", StringComparison.OrdinalIgnoreCase))
                        {
                            // add interactables
                            Dictionary<string, object> properties = _objectPropertiesDict[(objectGroupId, obj.objectId)];
                            if (properties.TryGetValue("type", out var foundType))
                            {
                                string typeString = foundType.ToString();
                                EInteractableType type = (EInteractableType)Enum.Parse(typeof(EInteractableType), typeString, true);
                                AddInteractable(properties, type, rectangle);
                            }
                        }
                    } 
                }
            }
        }
        private void AddInteractable(Dictionary<string, object> properties, EInteractableType type, Rectangle rectangle)
        {
            if (type == EInteractableType.door)
            {
                AddDoor(properties, rectangle);
            }
        }
        private void AddDoor(Dictionary<string, object> properties, Rectangle rectangle)
        {
            string targetMap = properties["targetMap"].ToString();
            EMapCode targetMapCode = (EMapCode)Enum.Parse(typeof(EMapCode), targetMap, true);
            int spawnX = int.Parse(properties["spawnX"].ToString());
            int spawnY = int.Parse(properties["spawnY"].ToString());
            string directionValue = properties["direction"].ToString();
            EDirection direction = (EDirection)Enum.Parse(typeof(EDirection), directionValue, true);
            DoorTransition door = new DoorTransition(rectangle, targetMapCode, new Vector2(spawnX, spawnY), direction);
            _doors.Add(door);
        }
        private void AddEnemySpawner(Dictionary<string, object> properties, Rectangle rectangle)
        {
            object foundEnemyTypeCode = properties["enemyTypeCode"];
            object foundMaxSpawnCount = properties["maxSpawnCount"];
            object foundRarity = properties["rarity"];
            string enemyTypeValue = foundEnemyTypeCode.ToString();
            string rarityValue = foundRarity.ToString();
            int maxSpawnCount = int.Parse(foundMaxSpawnCount.ToString());
            EEnemyType enemyType = (EEnemyType)Enum.Parse(typeof(EEnemyType), enemyTypeValue, true);
            ERarity rarity = (ERarity)Enum.Parse(typeof(ERarity), rarityValue, true);
            Vector2 enemySpawnerPosition = new Vector2(rectangle.X, rectangle.Y);
            EnemySpawner enemySpawner = new EnemySpawner(enemySpawnerPosition, rectangle.Width, rectangle.Height, enemyType, rarity, maxSpawnCount);
            _enemySpawners.Add(enemySpawner);
        }
        private static void Write(string text)
        {
            Debug.WriteLine(text);
        }
        #endregion
    }
}
