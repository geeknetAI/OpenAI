using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using static AssistantsAPI.Controllers.AssistantController;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AssistantsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssistantController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async void Post([FromBody] string value)
        {
            var apiKey = "";
            var allAssitantsAPIURL = "https://api.openai.com/v1/assistants"; // Returns list of all assistants
            var threadAPIUrl = "https://api.openai.com/v1/threads"; // Returns list of all assistants
            var runAPIUrl = "https://api.openai.com/v1/threads/runs";

            var assistantVersion = "assistants=v1";
            Assistant assistant = null;
            
            // Step 1 : Create assistant object
            
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Add("OpenAI-Beta", $"{assistantVersion}");
                HttpResponseMessage response = await client.GetAsync(allAssitantsAPIURL);
                string responseContent = await response.Content.ReadAsStringAsync();
                var assistantList = JsonConvert.DeserializeObject<AssistantList>(responseContent);

                if (response.IsSuccessStatusCode)
                {
                    assistant = assistantList.Data[0];
                }
            }

            // Step 2 : Create a new thread object and attach message
            
            var requestData = new
            {
                messages = new[]
               {
                    new { role = "user", content = "How does AI work? Explain it in simple terms." }
                }
            };
            string jsonPayload = JsonConvert.SerializeObject(requestData);
            string threadId = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Add("OpenAI-Beta", $"{assistantVersion}");
                HttpResponseMessage response = await client.PostAsync(threadAPIUrl,
                                                                new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var threadList = JsonConvert.DeserializeObject<Thread>(responseBody);

                if (response.IsSuccessStatusCode)
                {
                    threadId = threadList.Id;
                }

                Console.WriteLine(responseBody);
            }

            // Step 3 : Create a new Run object

            string runId = string.Empty;
            // Create the JSON payload
            var payload = new
            {
                //thread_id = threadId,
                assistant_id = assistant.Id
            };


            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Add("OpenAI-Beta", $"{assistantVersion}");
                string jsonPayload1 = JsonConvert.SerializeObject(payload);

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/threads/" + threadId + "/runs",
                                                                   new StringContent(jsonPayload1, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                string runResponse = await response.Content.ReadAsStringAsync();
                var runList = JsonConvert.DeserializeObject<Run>(runResponse);

                if (response.IsSuccessStatusCode)
                {
                    runId = runList.Id;
                }

            }

            // Step 4 Retrieve Run object and check the run status

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Add("OpenAI-Beta", $"{assistantVersion}");
                HttpResponseMessage response = await client.GetAsync("https://api.openai.com/v1/threads/" + threadId + "/runs/" + runId );
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    
                    // STEP 5 : Display Assistant Response
                    
                    using (HttpClient clientMessage = new())
                    {
                        clientMessage.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                        clientMessage.DefaultRequestHeaders.Add("OpenAI-Beta", $"{assistantVersion}");
                        HttpResponseMessage responseMessage = await clientMessage.GetAsync("https://api.openai.com/v1/threads/" + threadId + "/messages");
                        responseMessage.EnsureSuccessStatusCode();
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            string runResponse = await responseMessage.Content.ReadAsStringAsync();
                            RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(runResponse);
                            foreach (var data in rootObject.Data)
                            {
                                Console.WriteLine($"ID: {data.Id}, Role: {data.Role}");
                                foreach (var content in data.Content)
                                {
                                    // Print response from the API
                                    Console.WriteLine($"Content Type: {content.Type}, Value: {content.Text.Value}");                                   

                                }
                            }
                        }
                    }
                }
            }
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public class Assistant
        {
            public string Id { get; set; }            
        }
        public class AssistantList
        {            
            public List<Assistant> Data { get; set; }         
        }

        public class Thread
        {
            public string Id { get; set; }
        }
        public class ThreadList
        {
            public List<Thread> Data { get; set; }
        }
        public class Run
        {
            public string Id { get; set; }
        }
        public class RootObject
        {
            public string Object { get; set; }
            public List<Data> Data { get; set; }
            public string FirstId { get; set; }
            public string LastId { get; set; }
            public bool HasMore { get; set; }
        }

        public class Data
        {
            public string Id { get; set; }            
            public string ThreadId { get; set; }
            public string Role { get; set; }
            public List<Content> Content { get; set; }
            public string AssistantId { get; set; }
            public string RunId { get; set; }            
        }

        public class Content
        {
            public string Type { get; set; }
            public Text Text { get; set; }
        }

        public class Text
        {
            public string Value { get; set; }
            public List<object> Annotations { get; set; }
        }

        public class Metadata
        {
        }
    }

    internal class Message
    {
    }
}
