using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http.Headers;
using System.IO;
using System;
using Microsoft.ProjectOxford.Face;
using Newtonsoft.Json;
using System.Text;
using System.Dynamic;

namespace DtsFaceApi
{
    public static class RecognizeFace
    {
        private static string personGroupId = System.Environment.GetEnvironmentVariable("FaceApiPersonGroup", EnvironmentVariableTarget.Process);
        private static string faceApiKey = System.Environment.GetEnvironmentVariable("FaceApiKey", EnvironmentVariableTarget.Process);
        private static string faceApiRoot = System.Environment.GetEnvironmentVariable("FaceApiRootUrl", EnvironmentVariableTarget.Process);
        private static Guid johnDoePersonId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        [FunctionName("RecognizeFace")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            var echoIds = req.GetQueryNameValuePairs().Where(kvp => kvp.Key.Equals("echoId", StringComparison.OrdinalIgnoreCase));
            if (echoIds.Any())
            {
                HttpResponseMessage resp = await GetResponse(req, null, johnDoePersonId, echoIds.First().Value);
                return resp;
            }

            var stream = await req.Content.ReadAsStreamAsync();
            if (stream.Length != 0)
            {
                using (var ms = new MemoryStream())
                using (var faceService = new FaceServiceClient(faceApiKey, faceApiRoot))
                {
                    var faces = await faceService.DetectAsync(stream);
                    if (faces.Any())
                    {
                        var identified = await faceService.IdentifyAsync(personGroupId, faces.Select(f => f.FaceId).ToArray(), 5);
                        var acquaintance = identified.Where(i => i.Candidates.Any()).OrderByDescending(i => i.Candidates.Select(c => c.Confidence)).FirstOrDefault();
                        var face = acquaintance?.Candidates?.OrderByDescending(c => c.Confidence)?.FirstOrDefault();

                        if (face != null)
                        {
                            HttpResponseMessage resp = await GetResponse(req, faceService, face.PersonId);
                            return resp;
                        }
                    }
                }
            }

            return req.CreateResponse(HttpStatusCode.NoContent);
        }

        private static async Task<HttpResponseMessage> GetResponse(HttpRequestMessage req, FaceServiceClient faceService, Guid personId, string echoId = null)
        {
            var resp = req.CreateResponse(HttpStatusCode.OK);
            dynamic respConent = new ExpandoObject();
            
            if(echoId != null)
            {
                respConent.PersonId = echoId;
            }
            else
            {
                var person = await faceService.GetPersonAsync(personGroupId, personId);
                respConent.PersonId = person?.Name;
            }
            resp.Content = new StringContent(JsonConvert.SerializeObject(respConent), Encoding.UTF8, "application/json");

            return resp;
        }
    }
}
