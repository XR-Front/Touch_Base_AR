using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XRF_InteractionController : MonoBehaviour
{
    //!!rename this "InteractionController" after class

    public Material HighlightMaterial;

    [System.Serializable]
    public enum InteractionType // your custom enumeration
    {
        AnimationController,
        SceneChangeController,
        OnOffController,
    };
    public InteractionType myType = InteractionType.AnimationController;  // this public var should appear as a drop down

    //animation stuff
    public GameObject ObjectWithAnimation;
    private Animator theAnimator;

    //scene load stuff
    public string SceneToLoad;

    //on off stuff
    public int NumberOfThingsToTurnOn = 1;
    public int NumberOfThingsToTurnOff = 1;
    public GameObject[] ClickTurnsTheseON;
    public GameObject[] ClickTurnsTheseOFF;
    private bool OnOffSwitch;

    //Make Camera Child stuff
    public GameObject objectToBecomeParent; 


    private void Start()
    {
        if (myType == InteractionType.OnOffController)
        {
            OnOff(false, true);
        }
        else if (myType == InteractionType.AnimationController)
        {
            theAnimator = ObjectWithAnimation.GetComponent<Animator>();
            if (theAnimator != null)
            {
                string animName = theAnimator.runtimeAnimatorController.animationClips[0].name;
                //Debug.Log("my animation is called: " + animName);
                //play on start but set to false so it stops
                theAnimator.Play(animName, 0, 0);
                theAnimator.enabled = false;
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("i bumped into something called: " + other.name);


        DoTheThing();

        /*
        GameObject theCam = other.GetComponentInChildren<Camera>().gameObject;
        if(theCam!=null)
        {
            DoTheThing(theCam);
        }
        */
    }

    public void DoTheThing()
    {
        //i clicked on this thing with an interaction controller on it
        //Debug.Log("I did the thing");
        if (myType == InteractionType.AnimationController)
        {
            //play or pause animation
            //note, the animator must make a transition to exit if it is not on loop.

            string animName = theAnimator.runtimeAnimatorController.animationClips[0].name;
            Debug.Log("my animation is called: " + animName);


            Debug.Log("my animator state info normalized time: " + theAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime); //< if this is at 1 its done
            Debug.Log("my animator state info is loop true: " + theAnimator.runtimeAnimatorController.animationClips[0].isLooping); //< if this is at 1 its done

            
            

            if(theAnimator.runtimeAnimatorController.animationClips[0].isLooping) //if loop is true
            {
                if (theAnimator.isActiveAndEnabled) //if i am currently on, turn off
                {
                    Debug.Log("my animation was playing and enabled, it will stop now");
                    theAnimator.enabled = false;
                }
                else //if i am currently off, turn on
                {
                    theAnimator.enabled = true;
                }
            }
            else //loop is false
            {
                if (theAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) //if i am done with my animation sequence (at the end)
                {
                    theAnimator.Play(animName, 0, 0);
                    theAnimator.enabled = true;
                }
                else //either i am paused or i am playing and not done
                {
                    if (theAnimator.isActiveAndEnabled) //if i am currently on, turn off
                    {
                        Debug.Log("my animation was playing and enabled, it will stop now");
                        theAnimator.enabled = false;
                    }
                    else //if i am currently off, turn on
                    {
                        theAnimator.enabled = true;
                    }
                }
            }




            /*
            if (theAnimator.GetCurrentAnimatorStateInfo(0).IsName(animName))
            {
                Debug.Log("hey my animation is in the middle of playing or on loop");

                if (theAnimator.isActiveAndEnabled)
                {
                    Debug.Log("my animation was playing and enabled, it will stop now");
                    theAnimator.enabled = false;
                }
                else
                {
                    if( theAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    {
                        theAnimator.Play(animName, 0, 0);
                    }
                    //if at the end, restart
                    //else below

                    Debug.Log("my animation was in the middle of playing playing but not enabled");
                    theAnimator.enabled = true;
                }
            }
            else
            {
                Debug.Log("hey my animation ended, I'm going to start it over");
                //this will be called if the animation has completed only.
                theAnimator.enabled = true;
                theAnimator.Play(animName, 0, 0);
            }
            */

        }
        else if (myType == InteractionType.SceneChangeController)
        {
            //load a scene
            Debug.Log("scene");
            if (SceneToLoad != null && SceneToLoad != "")
            {
                if (Application.CanStreamedLevelBeLoaded(SceneToLoad))
                {
                    SceneManager.LoadScene(SceneToLoad);

                }
                else
                {
                    Debug.LogError("Hey, I couldn't find that scene name. Check if it is spelled correctly in the TriggerInteraction and if it is added to build settings (file > build settings)");
                }
            }
            else
            {
                Debug.LogError("Hey, your scene name is blank, please enter the scene name and ensure that the scene is added to your build settings (file > build settings)");
            }
        }
        else if (myType == InteractionType.OnOffController)
        {
            Debug.Log("on off");

            OnOffSwitch = !OnOffSwitch;
            if (OnOffSwitch)
            {
                OnOff(true, false);
            }
            else
            {
                OnOff(false, true);
            }
        }
    }
    void OnOff(bool bool1, bool bool2)
    {
        foreach (GameObject g in ClickTurnsTheseON)
        {
            if (g != null)
            {
                g.SetActive(bool1);
            }
        }
        foreach (GameObject g in ClickTurnsTheseOFF)
        {
            if (g != null)
            {
                g.SetActive(bool2);
            }
        }
    }
}
