using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text;

namespace FaceRecognitionHttp
{
    public static class FaceRecognitionHttp
    {
        // get a subscription key on https://azure.microsoft.com/en-us/services/cognitive-services/face/
        // Add your Face subscription key to your environment variables.
        private static string subscriptionKey = "<face api subscriptionkey>";
        // Add your Face endpoint to your environment variables.
        private static string faceEndpoint = "<face-api-endpoint>";

        private static string _powerbiUrl = "<face recognition dataset>";
        private static IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });
        [FunctionName("FaceRecognitionHttp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            File.WriteAllBytes("temp.jpg", Convert.FromBase64String(requestBody));
            faceClient.Endpoint = faceEndpoint;
            // The list of Face attributes to return.
            IList<FaceAttributeType?> faceAttributes =
                new FaceAttributeType?[]
                {
                    FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Emotion
                };

            // Call the Face API.
            using (Stream myBlob = File.OpenRead("temp.jpg"))
            {

                try
            {
                // The second argument specifies to return the faceId, while
                // the third argument specifies not to return face landmarks.
                IList<DetectedFace> faceList = await faceClient.Face.DetectWithStreamAsync(myBlob, true, false, faceAttributes);

                log.LogInformation(faceList.ToString());
                foreach (var face in faceList)
                {
                    JObject PowerBIRow =
                    new JObject(
                        new JProperty("faceid", face.FaceId.ToString()),
                        new JProperty("age", (int)face.FaceAttributes.Age),
                        new JProperty("gender", face.FaceAttributes.Gender.ToString()),
                        new JProperty("anger", (double)face.FaceAttributes.Emotion.Anger),
                        new JProperty("contempt", (double)face.FaceAttributes.Emotion.Contempt),
                        new JProperty("disgust", (double)face.FaceAttributes.Emotion.Disgust),
                        new JProperty("fear", (double)face.FaceAttributes.Emotion.Fear),
                        new JProperty("happiness", (double)face.FaceAttributes.Emotion.Happiness),
                        new JProperty("neutral", (double)face.FaceAttributes.Emotion.Neutral),
                        new JProperty("sadness", (double)face.FaceAttributes.Emotion.Sadness),
                        new JProperty("surprise", (double)face.FaceAttributes.Emotion.Surprise),
                        new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"))
                    );

                    //Generate content for http post request
                    var content = new StringContent(PowerBIRow.ToString(), Encoding.UTF8, "application/json");

                    //Define http client to push the row
                    HttpClient powerbiclient = new HttpClient();
                    HttpResponseMessage pushrow = await powerbiclient.PostAsync(_powerbiUrl, content);
                }
                //Define Json body for Power BI


            }
            // Catch and display Face API errors.
            catch (APIErrorException f)
            {
                log.LogError(f.Message);
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
                log.LogError(e.Message, "Error");
            }
            }

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
