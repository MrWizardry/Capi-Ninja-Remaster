using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
   public string checkpointID;

   private void OnTriggerEnter2D(Collider2D collision)
   {
       if (collision.CompareTag("Player"))
       {
           PlayerPrefs.SetFloat("cp_x", transform.position.x);
           PlayerPrefs.SetFloat("cp_y", transform.position.y);
           PlayerPrefs.SetFloat("cp_z", transform.position.z);

           PlayerPrefs.SetInt("SavedScene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

           PlayerPrefs.Save();
           Debug.Log($"Checkpoint: {checkpointID}");
       }
   }
}
