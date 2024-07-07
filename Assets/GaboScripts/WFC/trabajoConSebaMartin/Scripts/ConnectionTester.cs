using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SebaTrabajo
{
    public class ConnectionTester : MonoBehaviour
    {
        List<GameObject> tiles = new List<GameObject>();

        int spawnCount = -7;

        const int step = 4;

        bool initialized = false;

        public void SetTiles(List<GameObject> tileGO)
        {
            foreach (GameObject go in tileGO)
            {
                tiles.Add(go);
            }
            initialized = true;
        }

        private void Start()
        {
            StartCoroutine(Spawn());
        }

        IEnumerator Spawn()
        {
            yield return new WaitUntil(() => initialized);
            yield return null;

            DataContainer data = FindObjectOfType<DataContainer>();
            foreach (GameObject go in tiles)
            {
                //Instantiate(go, new Vector3(spawnCount, 3), Quaternion.identity);
                var topN = go.GetComponent<SebaTile>().GetDefaultValidTopNeighbours();
                var bottomN = go.GetComponent<SebaTile>().GetDefaultValidBottomNeighbours();
                var rightN = go.GetComponent<SebaTile>().GetDefaultValidRightNeighbours();
                var leftN = go.GetComponent<SebaTile>().GetDefaultValidLeftNeighbours();
                int max = go.GetComponent<SebaTile>().maxNeighbours;
                for (int i = 0; i < max; i++)
                {
                    spawnCount += step;
                    Instantiate(go, new Vector3(spawnCount, 0), Quaternion.identity);
                    Instantiate(data.GetTile(topN[i % topN.Count]), new Vector3(spawnCount, 1), Quaternion.identity);
                    Instantiate(data.GetTile(bottomN[i % bottomN.Count]), new Vector3(spawnCount, -1), Quaternion.identity);
                    Instantiate(data.GetTile(rightN[i % rightN.Count]), new Vector3(spawnCount + 1, 0), Quaternion.identity);
                    Instantiate(data.GetTile(leftN[i % leftN.Count]), new Vector3(spawnCount - 1, 0), Quaternion.identity);
                }
            }
            /*
            Tile water = data.GetTile(TileType.Lake).GetComponent<Tile>();
            Debug.Log("Water valid top neighbours");
            foreach(TileType t in water.GetDefaultValidTopNeighbours())
            {
                Debug.Log(data.GetTile(t).name);
            }
            */
        }
    }
}