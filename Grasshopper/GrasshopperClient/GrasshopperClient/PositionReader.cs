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

            var child = client.Child("User");
            var observable = child.AsObservable<MLPoint>();
            var subscription = observable
                .Subscribe(f => setPositions(f.Object, f.Key, DA));

        }

        private void setPositions(MLPoint value, string positionToUpdate, IGH_DataAccess DA)
        {
            //expire solution
            try
            {
                Grasshopper.Instances.ActiveCanvas.Invoke(new MethodInvoker(delegate { ExpireSolution(true); }));
                
            }
            catch (Exception)
            { }


            //switched y & z to match Rhino's axis system
            if(positionToUpdate == "CameraPosition")
            {
               Cpt = new Point3d((double)(value.x),(double)(value.z),(double)(value.y));
            }

            else if (positionToUpdate == "LeftHandPosition")
            {
                Lpt = new Point3d((double)(value.x), (double)(value.z), (double)(value.y));
            }
            else Rpt = new Point3d((double)(value.x), (double)(value.z), (double)(value.y));



            //DA.SetData(0, Cpt);
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
