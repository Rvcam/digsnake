using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyUtil;
using UnityEngine.Experimental.Rendering.Universal;

public class SavePoint : MonoBehaviour
{
    private enum SavePointStates
    {
        OffScreen,
        OnScreen,
        FarEnough,
        Judging,
        ReadyToActivate,
        Activated
    }

    private GameSceneController gameSceneController;

    [SerializeField]
    private Transform cameraTransform=null;
    private Vector3 targetPosition;

    [SerializeField]
    private float errorMargin=10f;

    [Range(0f, 4f)]
    public float futureSpeed;
    public Directions futureDirection;
    
    private SavePointStates state=SavePointStates.OffScreen;

    public event Action<SavePoint> savePointActivated;
    public event Action arrivedAtSavePoint;
    private float outerLightsOriginalIntensity;
    private Light2D[] outerLights;
    private Light2D innerLight;

    [SerializeField]
    private int savedLength;
    [SerializeField]
    private SavePoint[] childrenSPs=null;
    [SerializeField]
    private bool isChild=false;

    private void Awake()
    {
        if (savedLength == 0)
        {
            savedLength = 1;
        }
    }

    void Start()
    {
        if (errorMargin<Mathf.Epsilon)
        {
            Debug.LogError("erro margin for save point not set");
        }
        gameSceneController = FindObjectOfType<GameSceneController>();
        targetPosition = cameraTransform.position; // it will be adjusted later at adjustTarget
        targetPosition.z = transform.position.z;

        innerLight = transform.GetChild(14).GetComponent<Light2D>();
        outerLights = new Light2D[7];
        for (int i = 0; i < 7; i++)
        {
            outerLights[i] = transform.GetChild(i).GetComponent<Light2D>();
        }
        outerLightsOriginalIntensity = outerLights[0].intensity;
        if (state != SavePointStates.Activated)
        {
            for (int i = 0; i < outerLights.Length; i++)
            {
                outerLights[i].intensity = 0;
            }
            innerLight.color = new Color(0.2f, 0.2f, 0.2f);
        }

        //when a joint save point activates, all should activate as well
        foreach (SavePoint childSP in childrenSPs)
        {
            foreach (SavePoint otherChildSP in childrenSPs)
            {
                if (childSP!=otherChildSP)
                {
                    otherChildSP.savePointActivated += childSP.Activate;
                }
            }
            savePointActivated += childSP.Activate;
            childSP.savePointActivated += Activate;
        }
    }
     
    public void adjustTarget()
    {
       switch (gameSceneController.getRoomDirection())
        {
            case Directions.Up:
                targetPosition.x = transform.position.x;
                targetPosition.y = transform.position.y - gameSceneController.getBounds().height / 2 - getSize().y / 2; ;
                break;
            case Directions.Down:
                targetPosition.x = transform.position.x;
                targetPosition.y = transform.position.y + gameSceneController.getBounds().height / 2 + getSize().y / 2; ;
                break;
            case Directions.Left:
                targetPosition.y = transform.position.y;
                targetPosition.x = transform.position.x + gameSceneController.getBounds().width / 2 + getSize().x / 2;
                break;
            case Directions.Right:
                targetPosition.y = transform.position.y;
                targetPosition.x = transform.position.x - gameSceneController.getBounds().width / 2 - getSize().x / 2;
                break;
        }
    }

    void Update()
    {
        if (state == SavePointStates.OnScreen && (transform.position - targetPosition).magnitude >  errorMargin)
        {
            state = SavePointStates.FarEnough;
        }
        if (!isChild && state == SavePointStates.FarEnough && (transform.position - targetPosition).magnitude <  errorMargin)
        {
            state = SavePointStates.Judging;
            foreach (SavePoint childSP in childrenSPs)
            {
                childSP.setJudging();
            }
            arrivedAtSavePoint();
        }
        if (state==SavePointStates.Judging && gameSceneController.roomSpeed==0 && gameSceneController.isCollectingWell())
        {
            innerLight.color = new Color(1f, 1f, 1f);
            state = SavePointStates.ReadyToActivate;
        }
    }

    private void OnCollisionEnter2D (Collision2D collision)
    {
        Tagger tagger = collision.gameObject.GetComponent<Tagger>();
        if (tagger != null && tagger.containsCustomTag("player"))
        {
            if (state == SavePointStates.ReadyToActivate)
            {
                savedLength = collision.gameObject.GetComponent<PlayerController>().getLength();
                Activate(null);
            }
        }
        else if (tagger!=null && tagger.containsCustomTag("outer screen bounds"))
        {
            if (state==SavePointStates.OffScreen)
            {
                state = SavePointStates.OnScreen;
                adjustTarget();
            }
        }
    }

    public Vector3 getSize()
    {
        return GetComponent<PolygonCollider2D>().bounds.size;
    }

    public int getSavedLength()
    {
        return savedLength;
    }
    public void setSavedLength(int newLength)
    {
        savedLength = newLength;
    }
    protected void Activate(SavePoint activator)
    {
        state = SavePointStates.Activated;
        if (outerLights != null)
        {
            for (int i = 0; i < outerLights.Length; i++)
            {
                outerLights[i].intensity = outerLightsOriginalIntensity;
            }
            innerLight.color = new Color(1f, 1f, 1f);
        }
        if (activator == null)
        {
            savePointActivated(this);
       }
    }

    public void externalActivate()
    {
        Activate(null);
    }

    protected void setJudging()
    {
        state = SavePointStates.Judging;
    }
}
