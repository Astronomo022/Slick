using UnityEngine;
using Unity.Cinemachine;

public class ChangeCameraZone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject gameobject,bound1,bound2,bound3,bound4;
    private Collider2D b1,b2,b3,b4;

    public CinemachineCamera vcam;
    public SoundManager sm;
    public AudioClip music;
    private CinemachineConfiner2D camerabounds;
    private CinemachineBrain cine;

    
    void Start()
    {
        // initializes all the variables and gives me all of the things and dodads
        camerabounds = gameobject.GetComponent<CinemachineConfiner2D>();
        b1 = bound1.GetComponent<PolygonCollider2D>();
        b2 = bound2.GetComponent<PolygonCollider2D>();
        b3 = bound3.GetComponent<PolygonCollider2D>();
        b4 = bound4.GetComponent<PolygonCollider2D>();

        Debug.Log("Confiner Exists: " + (camerabounds != null));

        
    }

    void OnTriggerEnter2D(Collider2D col)

    {
        // Supposedly, this is supposed to update the camera after disabling and reenabling it with the confiner field 
        if (col.gameObject.tag == "Player" && col.IsTouching(b1))
        {
            camerabounds.enabled = false;
            camerabounds.BoundingShape2D = b1;
            camerabounds.enabled = true;

            vcam.PreviousStateIsValid = false; // Resets the camera's internal state to force it to update immediately
        }
        else if (col.gameObject.tag == "Player" && col.IsTouching(b2))
        {
            camerabounds.enabled = false;
            camerabounds.BoundingShape2D = b2;
            camerabounds.enabled = true;

            vcam.PreviousStateIsValid = false; // Resets the camera's internal state to force it to update immediately
        }
        else if (col.gameObject.tag == "Player" && col.IsTouching(b3))
        {
            camerabounds.enabled = false;
            camerabounds.BoundingShape2D = b3;
            camerabounds.enabled = true;

            vcam.PreviousStateIsValid = false; // Resets the camera's internal state to force it to update immediately
        }
        else if (col.gameObject.tag == "Player" && col.IsTouching(b4))
        {
            camerabounds.enabled = false;
            camerabounds.BoundingShape2D = b4;
            camerabounds.enabled = true;

            vcam.PreviousStateIsValid = false; // Resets the camera's internal state to force it to update immediately
        }
    }

}
