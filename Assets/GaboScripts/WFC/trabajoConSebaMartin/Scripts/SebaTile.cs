using SebaTrabajo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SebaTrabajo
{
    public enum TileType
    {
        None,
        Grass,
        Lake_TLconcave,
        Lake_T,
        Lake_TRconcave,
        Lake_L,
        Lake,
        Lake_R,
        Lake_BLconcave,
        Lake_B,
        Lake_BRconcave,
        Lake_TLconvex,
        Lake_TRconvex,
        Lake_BLconvex,
        Lake_BRconvex,
        Street_horizontal,
        Street_TL,
        Street_T,
        Street_TR,
        Street_vertical,
        Street_L,
        Street_cross,
        Street_R,
        Street_BL,
        Street_B,
        Street_BR
    }

    public enum ConnectionType
    {
        None,
        Grass,
        Water,
        Street,
        TopBorder,
        BottomBorder,
        RightBorder,
        LeftBorder
    }

    public class SebaTile : MonoBehaviour
    {
        [SerializeField] TileType type = TileType.None;

        [Space(15)]

        // Tipo de conexión para cada lado (asignado en el inspector)
        [SerializeField] ConnectionType topConn = ConnectionType.None;
        [SerializeField] ConnectionType bottomConn = ConnectionType.None;
        [SerializeField] ConnectionType leftConn = ConnectionType.None;
        [SerializeField] ConnectionType rightConn = ConnectionType.None;

        // Listas de vecinos
        List<TileType> defaultTopValidNeighbours = new List<TileType>();
        List<TileType> defaultBottomValidNeighbours = new List<TileType>();
        List<TileType> defaultRightValidNeighbours = new List<TileType>();
        List<TileType> defaultLeftValidNeighbours = new List<TileType>();

        public TileType GetTileType() => type;

        public ConnectionType GetTopConn() => topConn;
        public ConnectionType GetBottomConn() => bottomConn;
        public ConnectionType GetLeftConn() => leftConn;
        public ConnectionType GetRightConn() => rightConn;

        public List<TileType> GetDefaultValidTopNeighbours() => defaultTopValidNeighbours;
        public List<TileType> GetDefaultValidBottomNeighbours() => defaultBottomValidNeighbours;
        public List<TileType> GetDefaultValidRightNeighbours() => defaultRightValidNeighbours;
        public List<TileType> GetDefaultValidLeftNeighbours() => defaultLeftValidNeighbours;

        public int maxNeighbours = 0;

        public void Initialize()
        {
            Clear();
            GenerateLists();
            Validate();
            maxNeighbours = Mathf.Max(defaultTopValidNeighbours.Count, defaultBottomValidNeighbours.Count, defaultRightValidNeighbours.Count, defaultLeftValidNeighbours.Count);
        }

        // Para prevenir que las listas estén vacías antes de llenarlas (pueden tener elementos por antigua asignación en el inspector)
        void Clear()
        {
            defaultTopValidNeighbours.Clear();
            defaultBottomValidNeighbours.Clear();
            defaultRightValidNeighbours.Clear();
            defaultLeftValidNeighbours.Clear();
        }

        void GenerateLists()
        {
            //Debug.Log("Generando listas de vecinos...");

            // Se obtienen todos GO de los tiles de la lista de prefabs
            foreach (GameObject tileGO in FindObjectOfType<DataContainer>().GetTiles())
            {
                SebaTile tile = tileGO.GetComponent<SebaTile>();
                // Se añade a cada lista de vecinos el tile que concuerde con el tipo de conexión de cada lado
                if (tile.GetBottomConn() == topConn) { defaultTopValidNeighbours.Add(tile.type); }
                if (tile.GetTopConn() == bottomConn) { defaultBottomValidNeighbours.Add(tile.type); }
                if (tile.GetLeftConn() == rightConn) { defaultRightValidNeighbours.Add(tile.type); }
                if (tile.GetRightConn() == leftConn) { defaultLeftValidNeighbours.Add(tile.type); }
            }
        }

        void Validate()
        {
            //Debug.Log("Validando...");

            if (type == TileType.None)
            {
                Debug.LogError($"El tipo del tile {name} no puede ser NONE.");
            }

            if (topConn == ConnectionType.None)
            {
                Debug.LogError($"No se ha especificado el tipo de conexión SUPERIOR de {name}.");
            }

            if (bottomConn == ConnectionType.None)
            {
                Debug.LogError($"No se ha especificado el tipo de conexión INFERIOR de {name}.");
            }

            if (rightConn == ConnectionType.None)
            {
                Debug.LogError($"No se ha especificado el tipo de conexión DERECHA de {name}.");
            }

            if (leftConn == ConnectionType.None)
            {
                Debug.LogError($"No se ha especificado el tipo de conexión IZQUIERDA de {name}.");
            }

            if (defaultTopValidNeighbours.Count == 0)
            {
                Debug.LogError($"No existen vecinos superiores para {name}.");
            }

            if (defaultBottomValidNeighbours.Count == 0)
            {
                Debug.LogError($"No existen vecinos inferiores para {name}.");
            }

            if (defaultRightValidNeighbours.Count == 0)
            {
                Debug.LogError($"No existen vecinos a la derecha para {name}.");
            }

            if (defaultLeftValidNeighbours.Count == 0)
            {
                Debug.LogError($"No existen vecinos a la izquierda para {name}.");
            }

            foreach (TileType t in defaultTopValidNeighbours)
            {
                if (t == TileType.None)
                {
                    Debug.LogWarning($"Uno o más vecinos superiores del tile {name} son de tipo NONE.");
                    break;
                }
            }

            foreach (TileType t in defaultBottomValidNeighbours)
            {
                if (t == TileType.None)
                {
                    Debug.LogWarning($"Uno o más vecinos inferiores del tile {name} son de tipo NONE.");
                    break;
                }
            }

            foreach (TileType t in defaultRightValidNeighbours)
            {
                if (t == TileType.None)
                {
                    Debug.LogWarning($"Uno o más vecinos de la derecha del tile {name} son de tipo NONE.");
                    break;
                }
            }

            foreach (TileType t in defaultLeftValidNeighbours)
            {
                if (t == TileType.None)
                {
                    Debug.LogWarning($"Uno o más vecinos de la izquierda del tile {name} son de tipo NONE.");
                    break;
                }
            }
        }
    }
}