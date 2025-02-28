using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Runtime.InteropServices;
using CADgeneration;

namespace SolidWorksAutomation
{
    class CADgeneration
    {
        static void Main(string[] args)
        {

            // Parameters
            SldWorks swApp = null;
            ModelDoc2 modelDoc = null;
            bool started = false;


            // Start a new instance of SolidWorks

            try
            {
                Console.WriteLine("Starting SolidWorks...");

                /*
                 * How to specify a version if needed
                 * SolidWorks 2024	"SldWorks.Application.32"
                 * SolidWorks 2023	"SldWorks.Application.31"
                 * SolidWorks 2022	"SldWorks.Application.30"
                 * SolidWorks 2021	"SldWorks.Application.29"
                 * SolidWorks 2020	"SldWorks.Application.28"
                 * SolidWorks 2019	"SldWorks.Application.27"
                 * SolidWorks 2018	"SldWorks.Application.26" 
                 */


                swApp = Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application")) as SldWorks;

                // Check if SolidWorks opened successfully
                if (swApp != null)
                {
                    swApp.Visible = true; // Make SolidWorks visible for debugging and testing
                    started = true;
                    Console.WriteLine("SolidWorks started successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to start SolidWorks.");
                }
            }
            catch (COMException ex)
            {
                Console.WriteLine("Error: Unable to launch SolidWorks. Ensure it is installed correctly.");
                Console.WriteLine($"Exception Message: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }



            //             Main code block to generate CAD
            //******************************************

            if (started)
            {
                // Create the new document using template
                string templatePath = swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
                modelDoc = swApp.NewDocument(templatePath, 0, 0, 0) as ModelDoc2;


                var boxPoints = new List<Tuple<double, double, double>>
                {
                    Tuple.Create(0.0, 0.0, 0.0),      // Bottom-left corner
                    Tuple.Create(0.1, 0.0, 0.0),    // Bottom-right corner
                    Tuple.Create(0.1, 0.05, 0.0),   // Top-right corner
                    Tuple.Create(0.0, 0.05, 0.0),     // Top-left corner
                    Tuple.Create(0.0, 0.0, 0.0)       // Closing back to the first point
                };


                // Create 2D Sketch
                string sketch1 = modelDoc.NewSketch(boxPoints);

                // Extrude Sketch
                double depth = 0.05;
                modelDoc.ExtrudeSketch(sketch1, depth);


            }



            // Need to add code to save the model and close the SW application






            // Wait for user input before closing
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
