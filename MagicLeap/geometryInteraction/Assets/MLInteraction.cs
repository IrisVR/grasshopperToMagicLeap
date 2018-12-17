using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

public class MLInteraction : MonoBehaviour {

    public GameObject rightHand;
    public GameObject leftHand;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (MLHands.IsStarted)  trackHands();
    }


    void trackHands()
    {
        Vector3 rightHandPosition;
        Vector3 leftHandPosition;
#if UNITY_EDITOR
        rightHandPosition = rightHand.transform.GetChild(0).position;
        leftHandPosition = leftHand.transform.GetChild(0).position;
#endif
        rightHandPosition = MLHands.Right.Center;
        leftHandPosition = MLHands.Left.Center;

        Debug.Log("right hand position is " + rightHandPosition.ToString());

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject child = this.gameObject.transform.GetChild(i).gameObject;

            MeshCollider m = child.GetComponent<MeshCollider>();

            float dist1 = Vector3.Distance(m.bounds.center, rightHandPosition);
            float dist2 = Vector3.Distance(m.bounds.center, leftHandPosition);

            float dist = 0.0f;
            if (dist1 < dist2) dist = dist1;
            else dist = dist2;

            if (dist < 0.25f)
            {
                if (child.transform.localPosition.y > -0.25f)
                {
                    //GameObject.Find("Distance").GetComponent<Text>().text = "distance is " + dist;
                    //if(child.transform.localPosition.y - (0.25f - dist) > -0.005)

                    child.transform.localPosition = new Vector3(0.0f, (0.25f - dist), 0.0f);

                }
            }
            else
            {
                if (child.transform.localPosition.y != 0.0f)
                    child.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            }

            //    Debug.Log("Center is " + m.bounds.center);
            //    Debug.Log("Right hand is " + rightHandPosition);
            //    
            //    //Debug.Log(child.transform.localPosition);
        }
    }
}
