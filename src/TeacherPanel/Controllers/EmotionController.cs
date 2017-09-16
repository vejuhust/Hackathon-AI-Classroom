namespace TeacherPanel.Controllers
{
    using Microsoft.AspNet.SignalR;
    using System;
    using System.Linq;
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

        private static void ProcessEmotionRequest(string client, string emotion)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ReportHub>();

            var emotionStatus = Enum.GetNames(typeof(EmotionStatus)).Any(x => x.ToLowerInvariant() == emotion.ToLowerInvariant())
              ? (EmotionStatus)Enum.Parse(typeof(EmotionStatus), emotion, true)
              : EmotionStatus.None;

            hubContext.Clients.All.printLog($"{DateTime.UtcNow:O} '{client}' post: {emotion} - {emotionStatus}");

            if (emotionStatus == EmotionStatus.None || emotionStatus == EmotionStatus.Freezed)
            {
                return;
            }

            hubContext.Clients.All.addMessage($"{client} - {emotionStatus}");

            hubContext.Clients.All.updateFocusIndex((client + emotion).Length);

            if (emotionStatus == EmotionStatus.HandUp)
            {
                hubContext.Clients.All.updateHandupCount(StatusCollection.CountHandUp(client));
            }
        }
    }
}
