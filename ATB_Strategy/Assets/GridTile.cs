using UnityEngine;
using System.Collections.Generic;

public class GridTile : MonoBehaviour
{
    public GameObject floor;
    public List<GameObject> objects = new List<GameObject>();

    public int positionX;
    public int positionZ;
}
