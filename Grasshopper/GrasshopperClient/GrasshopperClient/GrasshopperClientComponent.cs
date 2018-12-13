using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Firebase.Database;
using System.Reactive.Linq;
using Grasshopper.Kernel.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ARGrasshopperClient
{
    public class ARGrasshopper : GH_Component
    {

        private static FirebaseClient firebase;
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ARGrasshopper()
          : base("ARGrasshopperClient", "GC",
              "Description",
              "AR", "Stream")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //pManager.AddNumberParameter("Model", "M", "Model to load", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run?", "R", "Run app?", GH_ParamAccess.item);
            pManager.AddMeshParameter("Mesh", "M", "Mesh to view in AR", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //double number = -1;
            //DA.GetData(0, ref number);
            //Run(number);

            bool shouldRun = false;
            DA.GetData(0, ref shouldRun);
            if (shouldRun)
            {
                GH_Mesh mesh = null;

                if (!DA.GetData(1, ref mesh))
                {
                    return;
                }

                //supporting triangles only
                Mesh m = mesh.Value;

                JObject meshjson = new JObject(
                    new JProperty("vertices",
                        new JArray(
                            from v in m.Vertices
                            select new JObject(
                                    new JProperty("x", v.X),
                                    new JProperty("y",v.Z),
                                    new JProperty("z", v.Y)
                            )
                        )
                    ),
                    new JProperty("normals",
                        new JArray(
                            from n in m.Normals
                            select new JObject(
                                new JProperty("x", n.X),
                                new JProperty("y", n.Z),
                                new JProperty("z", n.Y)
                            )
                        )
                    ),
                    new JProperty("colors",
                        new JArray(
                            from c in m.VertexColors
                            select new JObject(
                                new JProperty("r", c.R),
                                new JProperty("g", c.G),
                                new JProperty("b", c.B),
                                new JProperty("a", c.A)
                            )
                        )
                    ),
                    new JProperty("faces",
                        new JArray(
                            from f in m.Faces
                            select new JObject(
                                new JProperty("a", f.C),
                                new JProperty("b", f.B),
                                new JProperty("c", f.A)
                            )
                        )
                    )
                );

                Run(meshjson);
            }
            

        }

        private static async Task Run(JObject n)
        {
            if (firebase == null) firebase = new FirebaseClient("https://helloar-cc880.firebaseio.com/");

            await firebase
                .Child("Mesh")
                .PutAsync(n.ToString());
        }

        private static async Task Run(double n)
        {
            if(firebase==null) firebase = new FirebaseClient("https://helloar-cc880.firebaseio.com/");

            await firebase
                .Child("ModelToLoad")
                .PutAsync(n.ToString());
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
            get { return new Guid("436cd515-0c25-4594-8002-bcd66794aca8"); }
        }
    }
}
