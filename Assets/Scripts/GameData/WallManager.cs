using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public class WallManager : MonoBehaviour
    {
        [SerializeField] private GameObject wall_prefab;
        [SerializeField] private Transform wall_parent;
        [SerializeField] private float scroll_speed = 10.0f;
        [SerializeField] private float gap = 0.0f; // Gap between walls
        [SerializeField] private int starting_walls = 3;
        [SerializeField] private List<GameObject> walls = new List<GameObject>();

        private float wall_height = 0.0f;

        private void Awake()
        {
            // Get the height of the wall based on the sprite renderer's size
            wall_height = wall_prefab.GetComponentInChildren<SpriteRenderer>().bounds.size.y;

            // Create and position starting walls
            for (var i = 0; i < starting_walls; i++)
            {
                var wall = Instantiate(wall_prefab, wall_parent);
                wall.transform.position = new Vector3(0, i * (wall_height + gap), 0.0f); // Proper positioning
                walls.Add(wall);
            }
        }

        private void Update()
        {
            foreach (var wall in walls)
            {
                // Move each wall downward
                wall.transform.Translate(Vector2.down * (scroll_speed * Time.deltaTime));

                // Check if the wall is below the screen (off the bottom)
                if (wall.transform.position.y <= -wall_height)
                {
                    // Get the highest wall's Y position
                    var highest_wall_y = GetHighestWallY();

                    // Reposition the current wall just above the highest wall
                    wall.transform.position = new Vector3(wall.transform.position.x, highest_wall_y + wall_height + gap, 0.0f);
                }
            }
        }

        private float GetHighestWallY()
        {
            // Start by assuming the first wall has the highest Y position
            float highest_y = float.MinValue;

            // Compare each wall's Y position to find the highest one
            foreach (var wall in walls)
            {
                if (wall.transform.position.y > highest_y)
                {
                    highest_y = wall.transform.position.y;
                }
            }

            return highest_y;
        }
    }
}
