using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.EventSystems;

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
    char state = 'i';
    private float scoreFull;
    public TextMeshProUGUI scoreFullText;
    private float scoreLove;
    public TextMeshProUGUI scoreLoveText;
    private float timeInState = 0;
    public float scoreFullRate = 0.25f;
    public float scoreLoveRate = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        ar_Manager = GetComponent<ARRaycastManager>();
        //get the component in children (can also do this with parents)
        cam= GetComponentInChildren<Camera>();
        scoreFull = 100;
        scoreFullText.text = "Full: " + scoreFull.ToString();
        scoreLove = 100;
        scoreLoveText.text = "Love: " + scoreLove.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        scoreFull -= Time.deltaTime * scoreFullRate;
        scoreLove -= Time.deltaTime * scoreLoveRate;
        if(state == 'f' || state == 'p')
        {
            timeInState += Time.deltaTime;
            if(timeInState >= 5)
            {
                state = 'i';
            }
        }
        //Get touch input from player
        if(Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                print("UI Touch Detected");
                //return breaks out of the input function
                return;
            }
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
                    //Increase score if in feed state
                    if(state == 'f')
                    {
                        scoreFull += 1;
                        scoreFull = Mathf.Clamp(scoreFull, 0, 100);
                        scoreFullText.text = "Full: " + Mathf.Round(scoreFull).ToString();
                    }
                    else if(state == 'p')
                    {
                        scoreLove += 1;
                        scoreLove = Mathf.Clamp(scoreLove, 0, 100);
                        scoreLoveText.text = "Love: " + Mathf.Round(scoreLove).ToString();
                    }
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
    public void EnterFeedState()
    {
        timeInState = 0;
        state = 'f';
    }
    public void EnterPetState()
    {
        timeInState = 0;
        state = 'p';
    }
}
