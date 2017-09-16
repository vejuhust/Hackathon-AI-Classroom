namespace TeacherPanel.Controllers
{
    using Microsoft.AspNet.SignalR;
    using System;
    using System.Web.Http;
    using TeacherPanel.Hubs;
    using TeacherPanel.Models;

    [Route("api/Emotion")]
    public class EmotionController : ApiController
    {
        private static int RequestCounter = 0;
        private static DateTime LastUpdateTime = DateTime.UtcNow;

        [HttpGet]
        public ReduceResult Get()
        {
            return new ReduceResult(RequestCounter, StatusCollection.GetConclusion(), LastUpdateTime);
        }

        private static void ProcessEmotionRequest(string client, string emotion)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ReportHub>();
            hubContext.Clients.All.addMessage($"{client} - {emotion}");

            hubContext.Clients.All.updateFocusIndex((client + emotion).Length);

            if (emotion == "handup")
            {
                hubContext.Clients.All.updateHandupCount(RequestCounter);
            }
        }

        [HttpPost]
        public bool Post([FromBody]MapRequest model)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.Client) && !string.IsNullOrWhiteSpace(model.Emotion))
            {
                StatusCollection.Save(model.Client, model.Emotion);
                ProcessEmotionRequest(model.Client, model.Emotion);

                RequestCounter += 1;
                LastUpdateTime = DateTime.UtcNow;
                
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpDelete]
        public void Delete()
        {
            StatusCollection.Reset();
        }
    }
}
