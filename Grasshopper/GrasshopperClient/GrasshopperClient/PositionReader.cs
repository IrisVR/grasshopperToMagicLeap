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
using System.Windows.Forms;

namespace ARGrasshopperClient
{
    public class PositionReader : GH_Component
    {

        private static FirebaseClient client;
        private Point3d Cpt = new Point3d();
        private Point3d Lpt = new Point3d();
        private Point3d Rpt = new Point3d();
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
            if(client==null) client = new FirebaseClient("https://helloar-cc880.firebaseio.com/");
            
            bool shouldRun = false;
            DA.GetData(0, ref shouldRun);
            if (shouldRun)
            { 
                Run(DA).Wait();
            }

            DA.SetData(0, Cpt);
            DA.SetData(1, Lpt);
            DA.SetData(2, Rpt);
        }


        private async Task Run(IGH_DataAccess DA)
        {

            var child = client.Child("CameraPosition");
            var observable = child.AsObservable<double>();
            var subscription = observable
                .Subscribe(c => setCamera(c.Object, c.Key, DA));

            var child2 = client.Child("LeftHandPosition");
            var observable2 = child2.AsObservable<double>();
            var subscription2 = observable2
                .Subscribe(l => setLeftHand(l.Object, l.Key, DA));

            var child3 = client.Child("RightHandPosition");
            var observable3 = child3.AsObservable<double>();
            var subscription3 = observable3
                .Subscribe(r => setRightHand(r.Object, r.Key, DA));


            //var camera = await client.Child("CameraPosition").OnceAsync<double>();
            //var left = await client.Child("LeftHandPosition").OnceAsync<double>();
            //var right = await client.Child("RightHandPosition").OnceAsync<double>();

            //double x = 0.0;
            //double y = 0.0;
            //double z = 0.0;
            //int count = 0;

            //foreach (var c in camera)
            //{
            //    if (count == 0) x = (double)c.Object;
            //    else if (count == 1) y = (double)c.Object;
            //    else z = (double)c.Object;
            //    count++;
            //}

            //Point3d cpt = new Point3d(x, y, z);
            //count = 0;

            //foreach (var c in left)
            //{
            //    if (count == 0) x = (double)c.Object;
            //    else if (count == 1) y = (double)c.Object;
            //    else z = (double)c.Object;
            //    count++;
            //}

            //Point3d lpt = new Point3d(x, y, z);
            //count = 0;

            //foreach (var c in right)
            //{
            //    if (count == 0) x = (double)c.Object;
            //    else if (count == 1) y = (double)c.Object;
            //    else z = (double)c.Object;
            //    count++;
            //}
            //Point3d rpt = new Point3d(x, y, z);

            //DA.SetData(0, cpt);
            //DA.SetData(1, lpt);
            //DA.SetData(2, rpt);
        }

        private void setCamera(double value, string coordinate, IGH_DataAccess DA)
        {
            //expire solution
            try
            {
                Grasshopper.Instances.ActiveCanvas.Invoke(new MethodInvoker(delegate { ExpireSolution(true); }));
                
            }
            catch (Exception)
            { }

            if (coordinate == "x") Cpt.X = value;
            else if (coordinate == "y") Cpt.Y = value;
            else Cpt.Z = value;
            
            DA.SetData(0, Cpt);
        }

        private void setLeftHand(double value, string coordinate, IGH_DataAccess DA)
        {
            //expire solution
            try
            {
                Grasshopper.Instances.ActiveCanvas.Invoke(new MethodInvoker(delegate { ExpireSolution(true); }));

            }
            catch (Exception)
            { }

            if (coordinate == "x") Lpt.X = value;
            else if (coordinate == "y") Lpt.Y = value;
            else Lpt.Z = value;

            DA.SetData(1, Lpt);
        }

        private void setRightHand(double value, string coordinate, IGH_DataAccess DA)
        {
            //expire solution
            try
            {
                Grasshopper.Instances.ActiveCanvas.Invoke(new MethodInvoker(delegate { ExpireSolution(true); }));

            }
            catch (Exception)
            { }

            if (coordinate == "x") Rpt.X = value;
            else if (coordinate == "y") Rpt.Y = value;
            else Rpt.Z = value;

            DA.SetData(2, Rpt);
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
        public class MLPoint
        {
            public double x { get; set; }
            public double y { get; set; }
            public double z { get; set; }
        }
    }
}
