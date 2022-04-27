using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
public class SolverDriver : MonoBehaviour
{

    [SerializeField]
    private GameObject trifoldPrefab;
    [SerializeField]
    private GameObject ampPrefab;
    [SerializeField]
    private GameObject treePrefab;
    [SerializeField]
    private GameObject podiumPrefab;
    [SerializeField]
    private InputField guessInput;
    [SerializeField]
    private Text screenOutput;

    ARAnchorManager m_AnchorManager;
    ARRaycastManager m_RaycastManager;

    Dictionary<string, GameObject> hintObjects = new Dictionary<string, GameObject>();
    public GameObject resolvedObject { get; private set; } // hold the resolved object
    private ARCloudAnchor _localAnchor;
    private bool resolveLock = true;
    private string objectToResolve;

    // Start is called before the first frame update
    void Awake()
    {   
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        hintObjects.Add("Clubs", trifoldPrefab);
        hintObjects.Add("Hearts", ampPrefab);
        hintObjects.Add("Spades", treePrefab);
        hintObjects.Add("Diamonds", podiumPrefab);

    }

    // Update is called once per frame
    void Update()
    {
        if(_localAnchor && (!resolveLock))
        {
            // Check cloud anchor state
            CloudAnchorState cloudAnchorState = _localAnchor.cloudAnchorState;
            if(cloudAnchorState == CloudAnchorState.Success)
            {
                screenOutput.text = "Success";
                resolvedObject = Instantiate(hintObjects[objectToResolve], _localAnchor.transform.position, _localAnchor.transform.rotation);
                resolvedObject.transform.SetParent(_localAnchor.transform, false); // 2nd param false makes object keep local orientation rather than global
                resolveLock = true;
                _localAnchor = null;
            }
            else if(cloudAnchorState == CloudAnchorState.TaskInProgress)
            {
                // Wait, not done yet
                screenOutput.text = "Resolving in progress";
            }
            else
            {
                screenOutput.text = "Resolve failed";
            }
        }
    }

    public void setObjectToResolve(string objectName)
    {
        objectToResolve = objectName;
    }
    public void Resolve(string objectId)
    {
        
        screenOutput.text = "Resolving";
        _localAnchor = m_AnchorManager.ResolveCloudAnchorId(objectId);
        resolveLock = false;
    }

    public void CheckGuess()
    {
        if(guessInput.text == "Final project")
        {
            screenOutput.color = Color.green;
            screenOutput.text = "Correct!";
        }
        else
        {
            screenOutput.color = Color.red;
            screenOutput.text = "Incorrect. Try again!";
        }
    }
}
