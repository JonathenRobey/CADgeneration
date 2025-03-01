using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Runtime.InteropServices;
using CADgeneration;
using Newtonsoft.Json;


namespace SolidWorksAutomation
{
    class CADgeneration
    {
        static void Main(string[] args)
        {



            // Load the JSON file
            var specs = LoadJsonFile("C:\\WorkArea\\CADgeneration\\spec.json");
            double length = 0.0;
            double width = 0.0;
            double height = 0.0;

            // Example usage of the parsed JSON data
            foreach (var spec in specs)
            {
                foreach (var kvp in spec)
                {
                    if (kvp.Key == "attributes")
                    {

                        
                        foreach (var kvp2 in kvp1)
                        {
                            if (kvp2.Key == "length")
                            {
                                length += Convert.ToDouble(kvp2.Value);
                            }
                            else if (kvp2.Key == "width")
                            {
                                width += Convert.ToDouble(kvp2.Value);
                            }
                            else if (kvp2.Key == "height")
                            {
                                height += Convert.ToDouble(kvp2.Value);
                            }
                        }
                    }


                }




            }


            double voluem = length * width * height;
            Console.WriteLine($"Volume of the box is {voluem} cubic mm");





            /*
            // Parameters
            SldWorks swApp = null;
            ModelDoc2 modelDoc = null;
            bool started = false;
            */

            // Start a new instance of SolidWorks
            /*
            try
            {
                Console.WriteLine("Starting SolidWorks...");

                
                 * How to specify a version if needed
                 * SolidWorks 2024	"SldWorks.Application.32"
                 * SolidWorks 2023	"SldWorks.Application.31"
                 * SolidWorks 2022	"SldWorks.Application.30"
                 * SolidWorks 2021	"SldWorks.Application.29"
                 * SolidWorks 2020	"SldWorks.Application.28"
                 * SolidWorks 2019	"SldWorks.Application.27"
                 * SolidWorks 2018	"SldWorks.Application.26" 
                 


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





            */
            //      Main code block to generate CAD
            //******************************************
            /*
            if (started)
            {
                // Create the new document using template
                string templatePath = swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
                modelDoc = swApp.NewDocument(templatePath, 0, 0, 0) as ModelDoc2;


                var boxPoints = new List<Tuple<double, double, double>>
                {
                    Tuple.Create(0.0, 0.0, 0.0),      // Bottom-left corner
                    Tuple.Create(550, 0.0, 0.0),    // Bottom-right corner
                    Tuple.Create(550, 800, 0.0),   // Top-right corner
                    Tuple.Create(0, 800, 0.0),     // Top-left corner
                    Tuple.Create(0.0, 0.0, 0.0)       // Closing back to the first point
                };


                // Create 2D Sketch
                string sketch1 = modelDoc.NewSketch(boxPoints);

                // Extrude Sketch
                double depth = 50;
                modelDoc.ExtrudeSketch(sketch1, depth);

            // translate object

            }


            






            // Need to add code to save the model and close the SW application




            */

        }
        private static List<Dictionary<string, object>> LoadJsonFile(string filePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonContent);
                return list ?? new List<Dictionary<string, object>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading JSON file: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }

    }
}
