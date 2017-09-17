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
            return new ReduceResult(RequestCounter, StatusCollection.Load(), LastUpdateTime);
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

            hubContext.Clients.All.addMessage(GenerateMessage(client, emotionStatus));
            hubContext.Clients.All.updateFocusIndex(ComputeFocusIndex(emotionStatus));

            if (emotionStatus == EmotionStatus.HandUp)
            {
                hubContext.Clients.All.raiseHand();
                hubContext.Clients.All.updateHandupCount(StatusCollection.CountHandUp(client));
            }
            else
            {
                hubContext.Clients.All.putHandDown();
            }
        }

        private static string GenerateMessage(string client, EmotionStatus emotion)
        {
            return $"{client} - {emotion}";
        }

        private static int ComputeFocusIndex(EmotionStatus emotion)
        {
            var rnd = new Random();
            int delta = rnd.Next(1, 10);

            var index = 0;
            switch (emotion)
            {
                case EmotionStatus.Sleeping:
                    index = 10;
                    break;
                case EmotionStatus.Streching:
                    index = 20 + delta >> 1;
                    break;
                case EmotionStatus.Wandering:
                    index = 30 + delta;
                    break;
                case EmotionStatus.Thinking:
                    index = 70 + delta;
                    break;
                case EmotionStatus.HandUp:
                    index = 80 + delta;
                    break;
                case EmotionStatus.Shaking:
                    index = 85 + delta;
                    break;
                case EmotionStatus.Nodding:
                    index = 90 + delta;
                    break;
                case EmotionStatus.Freezed:
                case EmotionStatus.None:
                    index = 50;
                    break;
            }

            return index;
        }
    }
}
