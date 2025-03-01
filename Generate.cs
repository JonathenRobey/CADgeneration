using OfficeOpenXml.Drawing.Chart;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADgeneration
{
    public static class Generate
    {
        public static string? NewSketch(this ModelDoc2 modelDoc, List<Tuple<double, double, double>> points)
        {
            /*
             * Used to create a 2D sketch with only coordinates
             */
          
            Console.WriteLine("Checking if points lie on the same plane...");

            
            
            // Check if all points lie on a single plane
            if (!ArePointsCoplanar(points, out string planeName))
            {
                Console.WriteLine("Error: Points are not on the same plane.");
                return null;
            }

            Console.WriteLine($"All points lie on the {planeName} plane. Creating sketch...");



            // Select the appropriate plane
            bool isSelected = modelDoc.Extension?.SelectByID2(planeName, "PLANE", 0, 0, 0, false, 0, null, 0) ?? false;

            if (!isSelected)
            {
                Console.WriteLine($"Error: Could not select the {planeName} plane.");
                return null;
            }

            // Get the sketch manager
            SketchManager sketchManager = modelDoc.SketchManager;

            // Start a new sketch
            sketchManager.InsertSketch(true);
            Console.WriteLine($"New sketch created on the {planeName} plane.");

            // Generate a random name for the sketch
            string sketchName = "Sketch_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            

            // Draw lines between each point
            for (int i = 0; i < points.Count - 1; i++)
            {
                var start = points[i];
                var end = points[i + 1];

                // Create Line
                SketchSegment line = sketchManager.CreateLine(start.Item1, start.Item2, start.Item3, end.Item1, end.Item2, end.Item3);
                if (line == null)
                {
                    Console.WriteLine($"Error: Failed to create line from ({start}) to ({end}).");
                    return null;
                }
            }

            // Finish the Sketch
            sketchManager.InsertSketch(true);


            // Get the newly created sketch feature
            Feature swFeat = (Feature)modelDoc.Extension.GetLastFeatureAdded();
            if (swFeat != null)
            {
                // Rename the sketch
                swFeat.Name = sketchName;

                // Verify the name change
                string updatedName = swFeat.Name;
                Console.WriteLine($"Sketch name changed to: {updatedName}");
                return sketchName;
            }
            else
            {
                Console.WriteLine("Failed to retrieve the sketch feature.");
                return null;
            }
        }




        // Extrude the 2D sketch to create a 3D feature

        public static string? ExtrudeSketch(this ModelDoc2 modelDoc, string sketchName, double extrudeDepth)
        {
            // Select the sketch by name
            bool status = modelDoc.Extension.SelectByID2(sketchName, "SKETCH", 0, 0, 0, false, 0, null, 0);
            if (!status)
            {
                throw new Exception($"Sketch '{sketchName}' not found");
            }

            // Determine extrusion direction (positive or negative)
            bool direction = extrudeDepth > 0 ? false : true;
            double distance = Math.Abs(extrudeDepth);

            Feature extrudeFeature = modelDoc.FeatureManager.FeatureExtrusion(
                true,       // Solid feature (not a surface)
                direction, // Flip direction (extrude negative if depth < 0)
                false,      // Direction type (false = normal extrusion)
                0,          // End condition: Blind
                0,          // Second end condition (not needed)
                distance, // Extrusion depth (converted to meters)
                0,          // Depth2 (not needed)
                false,      // No draft outward
                false,      // No draft inward
                false,      // Draft direction 1
                false,      // Draft direction 2
                0,          // Draft angle 1
                0,          // Draft angle 2
                false,      // Offset reverse 1
                false,      // Offset reverse 2
                false,      // Translate surface 1
                false,      // Translate surface 2
                true,       // Merge result (merge with existing body)
                true,       // Use feature scope
                false       // Use auto select
            );

            return null;
        }





        // Add a hole wizard tapped hole to the model

        /*
        public static bool AddHoleWizardTappedHole(this ModelDoc2 modelDoc, double x, double y, double z, string holeStandard, string holeType, string holeSize, double depth)
        {


            // Select the face where the hole will be created
            bool isSelected = modelDoc.Extension.SelectByRay(x, y, z, 0, 0, -1, 0.01, 2, false, 0, 0);
            if (!isSelected)
            {
                Console.WriteLine("Error: Could not select the face for the hole.");
                return false;
            }

            // Get the hole wizard feature data
            HoleWizardData holeWizardData = (HoleWizardData)modelDoc.FeatureManager.CreateDefinition((int)swFeatureNameID_e.swFmHoleWzd);
            holeWizardData.HoleType = (int)swHoleWizardTypes_e.swTapHole;
            holeWizardData.Standard = holeStandard;
            holeWizardData.HoleType = holeType;
            holeWizardData.Size = holeSize;
            holeWizardData.Depth = depth;

            // Create the hole wizard feature
            Feature holeFeature = modelDoc.FeatureManager.CreateFeature(holeWizardData);
            if (holeFeature == null)
            {
                Console.WriteLine("Error: Failed to create the hole wizard tapped hole.");
                return false;
            }

            Console.WriteLine("Hole wizard tapped hole created successfully.");
            return true;
        }

        */















































































        // GENERATE CLASS UTILITIES


        // Function to check if all points are coplanar
        private static bool ArePointsCoplanar(List<Tuple<double, double, double>> points, out string planeName)
        {
            planeName = "Front Plane"; // Default to front plane if not determined

            // If all Z-values are the same, they lie on the XY plane
            bool onXYPlane = points.TrueForAll(p => p.Item3 == points[0].Item3);
            if (onXYPlane)
            {
                planeName = "Front Plane";  // XY Plane
                return true;
            }

            // If all Y-values are the same, they lie on the XZ plane
            bool onXZPlane = points.TrueForAll(p => p.Item2 == points[0].Item2);
            if (onXZPlane)
            {
                planeName = "Top Plane";  // XZ Plane
                return true;
            }

            // If all X-values are the same, they lie on the YZ plane
            bool onYZPlane = points.TrueForAll(p => p.Item1 == points[0].Item1);
            if (onYZPlane)
            {
                planeName = "Right Plane"; // YZ Plane
                return true;
            }

            return false;
        }

        /*
        private static List<Face2> GetFacesAtCoordinate(this ModelDoc2 modelDoc, double x, double y, double z)
        {
            List<Face2> faces = new List<Face2>();
            double[] rayDir = { 0, 0, -1 };
            double rayRadius = 0.01;
            int faceCount = modelDoc.Extension.GetRayIntersections(x, y, z, rayDir[0], rayDir[1], rayDir[2], rayRadius, out object[] faceArray);

            if (faceCount > 0)
            {
                foreach (object faceObj in faceArray)
                {
                    if (faceObj is Face2 face)
                    {
                        faces.Add(face);
                    }
                }
            }

            return faces;
        }

        */




















    }
}
