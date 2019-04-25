using System.Collections;
using HoloToolkitExtensions.Utilities;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using UnityEngine;

public class MainScript : MonoBehaviour {
    private float current_foot_loc_Y;
    private float current_foot_loc_X;
    private float current_foor_loc_height;
    private Vector2 directionVector;
    // Use this for initialization

    public GameObject gameObjectPrefab;
    public GameObject parentObject;

    // only used for testing_
    private int tempCounter_;

    private AudioSource ac;

    //floor finder
    public float maxDistance = 3.0f;
    public float minHeight = 1.0f;
    private Vector3? _foundPosition = null;
    private float _delayMoment;
    public Text LabelText;
    private int max_retry = 1;

    public int currentStep = 0;
    private int maxStep = 75;

    private int directionFoot = -1;

    private float distanceClear = 0.25f;

    private ArrayList footprints;

    void Start () {
        print(StaticClass.straightOrCurve);
        print(StaticClass.haveObstacle);
        print(StaticClass.obstacleIntensity);

        _delayMoment = Time.time + 2;
        Reset();
        current_foot_loc_Y = 0f;
        current_foot_loc_X = 0.1f;
        directionVector = new Vector2(0.0f, 1.0f);
        tempCounter_ = maxStep;
        footprints = new ArrayList();
        ac = GetComponent<AudioSource>();
    }

    public int stepIncrement() {
        int temp = currentStep;
        if (currentStep < (maxStep - 1)) {
            currentStep += 1;
        }
        return temp;
    }

    public void Reset()
    {
        if (max_retry > 0)
        {
            _delayMoment = Time.time + 2;
            _foundPosition = null;
            if (LabelText != null)
                LabelText.enabled = true;
        }
        else {

            _delayMoment = Time.time + 2;
            current_foor_loc_height = 1.70f;
            LabelText.text = "Floor not found";
            _foundPosition = new Vector3();
        }
        max_retry -= 1;
    }

    // Update is called once per frame
    void Update () {

        if (_foundPosition == null && Time.time > _delayMoment)
        {
            //Debug.Log(GazeManager.Instance.Stabilizer);

            //var lier = GazeManager.Instance.Stabilizer;
            _foundPosition = LookingDirectionHelpers.GetPositionOnSpatialMap(maxDistance, GazeManager.Instance.Stabilizer);
            if (_foundPosition != null)
            {
                if (GazeManager.Instance.Stabilizer.StablePosition.y - _foundPosition.Value.y <= minHeight)
                {
                    Reset();
                    Debug.Log("reseted");
                }
                else
                {
                    Debug.Log("found");
                    current_foor_loc_height = _foundPosition.Value.y;
                    LabelText.enabled = false;
                }
            }
            else {
                Reset();
                Debug.Log("found null");
            }
        }

        if (_foundPosition != null && tempCounter_ > 0) {
            update_foot_location();
            add_foot_object_to_scene();
            tempCounter_ -= 1;
            ((GameObject)footprints[currentStep]).GetComponent<Renderer>().material.color = Color.blue;
            ((GameObject)footprints[currentStep + 1]).GetComponent<Renderer>().material.color = Color.red;
        }

        if (_foundPosition != null && Time.time > _delayMoment)
        {
            LabelText.enabled = false;
            removeIfNearby();
        }
    }

    void add_foot_object_to_scene() {
        // add a cue to scene
        //spawn object
        var newObj = Instantiate(gameObjectPrefab);
        newObj.transform.parent = parentObject.transform;
        newObj.transform.localPosition = new Vector3(current_foot_loc_X, -current_foor_loc_height, current_foot_loc_Y);
        newObj.transform.localScale = new Vector3(directionFoot*0.05f, 0.05f, 1f);
        newObj.transform.Rotate(90, 0, 0);
        footprints.Add(newObj);
    }

    private void turn_by(float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = directionVector.x;
        float ty = directionVector.y;
        directionVector.x = (cos * tx) - (sin * ty);
        directionVector.y = (sin * tx) + (cos * ty);
    }

    private void removeIfNearby() {
        Vector3 difference = ((GameObject)footprints[currentStep]).transform.position - gameObject.transform.position;
        var distanceInX = Mathf.Abs(difference.x);
        var distanceInY = Mathf.Abs(difference.y);
        var distanceInZ = Mathf.Abs(difference.z);
        var dist = Mathf.Sqrt(Mathf.Pow(distanceInX, 2) + Mathf.Pow(distanceInZ, 2));
        //print("X: " + distanceInX + "Y:" + distanceInY + "Z: " + distanceInZ + "Dist:" + dist);
        if (dist < distanceClear) {
            ac.Play(0);
            int previousStep = stepIncrement();
            if (previousStep < currentStep) {
                Destroy((GameObject)footprints[previousStep]);
                ((GameObject)footprints[currentStep]).GetComponent<Renderer>().material.color = Color.blue;
                ((GameObject)footprints[currentStep+1]).GetComponent<Renderer>().material.color = Color.red;

            }
        }

    }

    private void update_foot_location() {
        // need update
        current_foot_loc_X = -current_foot_loc_X;
        current_foot_loc_Y += 0.35f;
        directionFoot = -directionFoot;
    }
}
