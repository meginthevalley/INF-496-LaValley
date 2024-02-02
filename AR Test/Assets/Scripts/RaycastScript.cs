using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class RaycastScript : MonoBehaviour
{
    ARRaycastManager ar_Manager;
    Camera cam;
    //call the cube from the game
    public GameObject spawnItem;
    //create a list
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    //create a physics raycast hit
    RaycastHit p_hit;
    GameObject spawnedObject;
    int score;
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        ar_Manager = GetComponent<ARRaycastManager>();
        //get the component in children (can also do this with parents)
        cam= GetComponentInChildren<Camera>();
        score = 0;
        scoreText.text = "Score: " + score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //Get touch input from player
        if(Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            //Get position of that touch
            Vector3 position;
            if(Input.touchCount > 0)
            {
            position = Input.GetTouch(0).position;
            }
            else
            {
                position = Input.mousePosition;
            }
            //Create a Ray, starting at position, facing in camera direction
            Ray r = cam.ScreenPointToRay(position);
            //cast the physics ray
            if(Physics.Raycast(r, out p_hit))
            {
                if(p_hit.transform.gameObject.CompareTag("cube"))
                {
                    //Increase score
                    score += 1;
                    scoreText.text = "Score: " + score.ToString();
            }
            }
            //Cast the AR ray (use the empty list you creates above)
            if(ar_Manager.Raycast(r, hits))
            {
                //get the pose from the hit object (hits[0] because you are using the first item in the list)
                Pose hitPose = hits[0].pose;
                //Spawn an object at that position or move object if the object already exists in the scene
                if(spawnedObject == null)
                {
                    spawnedObject = Instantiate(spawnItem,hitPose.position, hitPose.rotation);
                }
                else
                {
                    spawnedObject.transform.position = hitPose.position;
                }
            }
        }
    }
}
