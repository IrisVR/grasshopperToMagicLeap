using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.Rendering;


public class GeometryInteraction : MonoBehaviour {

    Firebase.DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
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
     

    }

    private void InitializeFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl("https://helloar-cc880.firebaseio.com/");
        if (app.Options.DatabaseUrl != null)
            app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
        
        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
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
    

}

            

