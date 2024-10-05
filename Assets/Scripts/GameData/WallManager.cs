using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public class WallManager : MonoBehaviour
    {
        [SerializeField] private GameObject wall_prefab;
        [SerializeField] private Transform wall_parent;
        [SerializeField] private float scroll_speed = 10.0f;
        [SerializeField] private float gap = 0.0f;
        [SerializeField] private int starting_walls = 3;
        [SerializeField] private List<GameObject> walls = new List<GameObject>();

        private float wall_height = 0.0f;


        private void Awake()
        {
            wall_height = wall_prefab.GetComponentInChildren<SpriteRenderer>().bounds.size.y;

            //Create extra walls above to have some leigh way before recycling old walls
            for (var i = 0; i < starting_walls; i++)
            {
                var wall = Instantiate(wall_prefab, wall_parent);
                wall.transform.position = new Vector3(0, i * wall_height * gap, 0.0f);
                walls.Add(wall);
            }
        }

        private void Update()
        {
            foreach (var wall in walls)
            {
                wall.transform.Translate(Vector2.down * (scroll_speed * Time.deltaTime));
                if (wall.transform.position.y <= -wall_height)
                {
                    var highest_wall_y = 
                }
            }
        }



    }
}
