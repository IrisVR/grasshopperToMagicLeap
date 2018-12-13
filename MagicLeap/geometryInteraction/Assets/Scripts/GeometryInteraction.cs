using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.Rendering;


public class GeometryInteraction : MonoBehaviour {

    public GameObject rightHand;
    public GameObject leftHand;

    Firebase.DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    DatabaseReference mDatabaseRef;

    // Use this for initialization
    void Start () {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();

            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });

    }
	
	// Update is called once per frame
	void Update () {

        //save camera position to database
        writeNewPosition();

    }

    private void InitializeFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl("https://helloar-cc880.firebaseio.com/");
        if (app.Options.DatabaseUrl != null)
            app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
        
        // Get the root reference location of the database.
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        StartListener();

    }

    private void StartListener()
    {
        //  FirebaseDatabase.DefaultInstance
        //.GetReference("ModelToLoad").ValueChanged += HandleValueChanged;

        FirebaseDatabase.DefaultInstance
      .GetReference("ModelToLoad")
      .GetValueAsync().ContinueWith(task => {
          if (task.IsFaulted)
          {
              // Handle the error...
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              // Do something with snapshot...
              this.GetComponent<Text>().text = snapshot.Value.ToString();
          }
      });

    }
      
        
        void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
            
        string txt = args.Snapshot.Value.ToString();
        this.GetComponent<Text>().text = txt;
    }


    private void writeNewPosition()
    {
        string cameraJson = JsonUtility.ToJson(Camera.main.transform.position);
        mDatabaseRef.Child("CameraPosition").SetRawJsonValueAsync(cameraJson);

        Vector3 leftHandPosition = leftHand.transform.GetChild(0).gameObject.transform.position;
        string leftJson = JsonUtility.ToJson(leftHandPosition);
        mDatabaseRef.Child("LeftHandPosition").SetRawJsonValueAsync(leftJson);

        Vector3 rightHandPosition = rightHand.transform.GetChild(0).gameObject.transform.position;
        string rightJson = JsonUtility.ToJson(rightHandPosition);
        mDatabaseRef.Child("RightHandPosition").SetRawJsonValueAsync(rightJson);
    }

   

}

            

