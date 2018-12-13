using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Firebase.Database;
using Firebase.Database.Query;
using System.Reactive.Linq;
using Grasshopper.Kernel.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ARGrasshopperClient
{
    public class PositionReader : GH_Component
    {

        private static FirebaseClient firebase;
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public PositionReader()
          : base("PositionReader", "PR",
              "Description",
              "AR", "Position Reader")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run?", "R", "Run app?", GH_ParamAccess.item);  
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_PointParam("Camera Position", "C", "The position of the user in World");
            pManager.Register_PointParam("Left Hand Position", "LH", "The position of the left hand in World");
            pManager.Register_PointParam("Right Hand Position", "RH", "The position of the right hand in World");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            bool shouldRun = false;
            DA.GetData(0, ref shouldRun);
            if (shouldRun)
            {
                GH_Point cameraPosition = new GH_Point();
                GH_Point leftHand = new GH_Point();
                GH_Point rightHand = new GH_Point();


                Run(DA).Wait();

            }
            else
            {
                DA.SetData(0, new Point3d());
                DA.SetData(1, new Point3d());
                DA.SetData(2, new Point3d());
            }


        }

        private static async Task Run(IGH_DataAccess DA)
        {
            if (firebase == null) firebase = new FirebaseClient("https://helloar-cc880.firebaseio.com/");
            var camera = await firebase.Child("CameraPosition").OnceAsync<double>();
            var left = await firebase.Child("LeftHandPosition").OnceAsync<double>();
            var right = await firebase.Child("RightHandPosition").OnceAsync<double>();

            double x=0.0;
            double y = 0.0;
            double z = 0.0;
            int count = 0;

            foreach (var c in camera)
            {
                if (count == 0) x = (double)c.Object;
                else if (count ==1) y = (double)c.Object;
                else z = (double)c.Object;
                count++;
            }

            Point3d cpt = new Point3d(x, y, z);
            count = 0;

            foreach (var c in left)
            {
                if (count == 0) x = (double)c.Object;
                else if (count == 1) y = (double)c.Object;
                else z = (double)c.Object;
                count++;
            }

            Point3d lpt = new Point3d(x, y, z);
            count = 0;

            foreach (var c in right)
            {
                if (count == 0) x = (double)c.Object;
                else if (count == 1) y = (double)c.Object;
                else z = (double)c.Object;
                count++;
            }
            Point3d rpt = new Point3d(x, y, z);

            DA.SetData(0, cpt);
            DA.SetData(1, lpt);
            DA.SetData(2, rpt);
        }

        public class MLCoordinate
        {
            ///JObject pointJson = new JObject(new JProperty("x", 0), new JProperty("y", 0), new JProperty("z", 0));
            public string Coordinate { get; set; }
        }

        

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b5330bec-b1a1-45a4-b9ff-4aa687e92e17"); }
        }
    }
}
