using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SebaTrabajo
{
    public class DataContainer : MonoBehaviour
    {
        [SerializeField] List<GameObject> tilePrefabs = new List<GameObject>();

        List<TileType> tileTypes = new List<TileType>();

        public List<GameObject> GetTiles() => tilePrefabs;
        public List<TileType> GetTileTypes() => tileTypes;

        private void Awake()
        {
            // Acá se inicializan los tiles uno por uno
            foreach (GameObject tile in tilePrefabs)
            {
                tile.GetComponent<SebaTile>().Initialize();
            }
            FindObjectOfType<ConnectionTester>()?.SetTiles(tilePrefabs);

            // Los TyleTypes usados se cargan a la lista tileTypes
            TileType curr;
            foreach (GameObject obj in tilePrefabs)
            {
                curr = obj.GetComponent<SebaTile>().GetTileType();
                // Verifica unicidad de los TileType
                if (!tileTypes.Contains(curr))
                {
                    tileTypes.Add(curr);
                }
                else { Debug.LogError("Se encontró tipos repetidos!"); }
            }
        }

        // Retorno un GameObject para instanciar un tile
        public GameObject GetTile(TileType t)
        {
            foreach (GameObject tile in tilePrefabs)
            {
                if (tile.GetComponent<SebaTile>().GetTileType() == t) return tile;
            }
            return null;
        }


    }
}