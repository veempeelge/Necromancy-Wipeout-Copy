using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy2 : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public float Speed;
    GameObject player;
    Transform player1;
    public float health;
    public float enemyAttack = 2;
    public float knockbackDuration = 0.2f;
    Vector3 parameter = new Vector3(1, 0, 1);
    int index;


    public List<GameObject> Players = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        // Find all GameObjects with the tag "Player"
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");
        // Add each found player to the list
        foreach (GameObject player in foundPlayers)
        {
            Players.Add(player);
            // Debug.Log("Player added: " + player.name);
        }

        // Ensure there are players in the list before accessing it
        if (Players.Count > 0)
        {
            // Generate a random index within the range of the Players list
            index = Random.Range(0, Players.Count); // Note: Random.Range(max) is inclusive of 0 and exclusive of max
            player = Players[index];
            player1 = player.transform;

            // Debug.Log("I am following player " + index);
        }
        else
        {
            //  Debug.LogWarning("No players found with the tag 'Player'.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(this.transform.position, player1.position + parameter, Speed * Time.deltaTime);

        if (player != null)
        {
            Vector3 direction = (player1.position - transform.position).normalized;
            transform.position += direction * Speed * Time.deltaTime;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, player1.position + parameter, Speed * Time.deltaTime);
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //       MovementPlayer1 player = collision.gameObject.GetComponent<MovementPlayer1>();
    //        Debug.Log( "Hit " + player);
    //        if (player != null)
    //        {
    //           // Call the PerformAction function on the PlayerScript
    //            player.TakeDamage(enemyAttack);
    //        }
    //        else
    //       {
    //           Debug.LogWarning("PlayerScript component not found on the collided GameObject.");
    //       }
    //    }
    //}

    private void Die()
    {
        // Handle enemy death
        Destroy(gameObject);
    }

}
