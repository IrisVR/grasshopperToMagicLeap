using System;
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
    private GameObject parametricModel;

    Firebase.DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    DatabaseReference mDatabaseRef;

    private bool modelHasChanged = false;
    private DataSnapshot firebaseMesh = null;

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

        parametricModel = GameObject.Find("Cube");
        //parametricModel.AddComponent<MeshFilter>();

    }
	
	// Update is called once per frame
	void Update () {

        //save camera position to database
        writeNewPosition();

        //update the mesh
        if (modelHasChanged)
        {
            modelHasChanged = false;
            updateModel(firebaseMesh);
        }

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
      //.GetReference("ModelToLoad")
      //.GetValueAsync().ContinueWith(task => {
      //    if (task.IsFaulted)
      //    {
      //        // Handle the error...
      //    }
      //    else if (task.IsCompleted)
      //    {
      //        DataSnapshot snapshot = task.Result;
      //        // Do something with snapshot...
      //        this.GetComponent<Text>().text = snapshot.Value.ToString();
      //    }
      //});

        FirebaseDatabase.DefaultInstance
      .GetReference("Mesh")
      .GetValueAsync().ContinueWith(task => {
          if (task.IsFaulted)
          {
              // Handle the error...
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              // Do something with snapshot...
              updateModel(snapshot);
          }
      });

        FirebaseDatabase.DefaultInstance
        .GetReference("Mesh").ValueChanged += HandleValueChanged;

    }
      
        
        void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("mesh changed");
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        //string txt = args.Snapshot.Value.ToString();
        //this.GetComponent<Text>().text = txt;

        modelHasChanged = true;
        firebaseMesh = args.Snapshot;

        
    }


    private void writeNewPosition()
    {
        string cameraJson = JsonUtility.ToJson(Camera.main.transform.position);
        mDatabaseRef.Child("User/CameraPosition").SetRawJsonValueAsync(cameraJson);

        Vector3 leftHandPosition = leftHand.transform.GetChild(0).gameObject.transform.position;
        string leftJson = JsonUtility.ToJson(leftHandPosition);
        mDatabaseRef.Child("User/LeftHandPosition").SetRawJsonValueAsync(leftJson);

        Vector3 rightHandPosition = rightHand.transform.GetChild(0).gameObject.transform.position;
        string rightJson = JsonUtility.ToJson(rightHandPosition);
        mDatabaseRef.Child("User/RightHandPosition").SetRawJsonValueAsync(rightJson);
    }

    private void updateModel(DataSnapshot snapshot)
    {
        Mesh m = parametricModel.GetComponent<MeshFilter>().mesh;
        m.Clear();


        Vector3[] vertices = new Vector3[snapshot.Child("vertices").ChildrenCount];
        Color[] colors = new Color[snapshot.Child("vertices").ChildrenCount];
        Vector3[] normals = new Vector3[snapshot.Child("vertices").ChildrenCount];

        int i = 0;
        while (i < vertices.Length)
        {
            DataSnapshot v = snapshot.Child("vertices").Child(i.ToString());
            vertices[i].x = Convert.ToSingle((v.Child("x").Value));
            vertices[i].y = Convert.ToSingle((v.Child("y").Value));
            vertices[i].z = Convert.ToSingle((v.Child("z").Value));

            DataSnapshot n = snapshot.Child("normals").Child(i.ToString());
            normals[i].x = Convert.ToSingle((n.Child("x").Value));
            normals[i].y = Convert.ToSingle((n.Child("y").Value));
            normals[i].z = Convert.ToSingle((n.Child("z").Value));

            DataSnapshot c = snapshot.Child("colors").Child(i.ToString());
            colors[i] = new Color();
            colors[i].r = float.Parse(c.Child("r").Value.ToString()) / (float)255.0;
            colors[i].g = float.Parse(c.Child("g").Value.ToString()) / (float)255.0;
            colors[i].b = float.Parse(c.Child("b").Value.ToString()) / (float)255.0;

            //Grasshopper serves alpha from 0 to 255
            colors[i].a = float.Parse(c.Child("a").Value.ToString()) / (float)255.0;

            i++;
        }

        int k = 0;

        int[] triangles = new int[snapshot.Child("faces").ChildrenCount * 3];
        while (k < triangles.Length / 3)
        {
            DataSnapshot f = snapshot.Child("faces").Child(k.ToString());
            int A = -1;
            int B = -1;
            int C = -1;
            Int32.TryParse(f.Child("a").Value.ToString(), out A);
            Int32.TryParse(f.Child("b").Value.ToString(), out B);
            Int32.TryParse(f.Child("c").Value.ToString(), out C);
            int j = k * 3;

            // Unity's faces are arranged clockwise and Grasshoppers are counter-clockwise, so we must re-arrange here
            triangles[j] = A;
            triangles[j + 1] = B;
            triangles[j + 2] = C;
            k++;
        }
        m.vertices = vertices;
        m.normals = normals;
        m.colors = colors;
        m.triangles = triangles;
        m.RecalculateNormals();
        m.RecalculateBounds();
        Camera.main.Render();

    }
}

   



            

