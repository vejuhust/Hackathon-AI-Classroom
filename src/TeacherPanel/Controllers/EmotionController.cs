namespace TeacherPanel.Controllers
{
    using Microsoft.AspNet.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using TeacherPanel.Hubs;
    using TeacherPanel.Models;

    [Route("api/Emotion")]
    public class EmotionController : ApiController
    {
        private static int RequestCounter = 0;
        private static DateTime LastUpdateTime = DateTime.UtcNow;
        private static Random RandomGenerator = new Random();

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

            hubContext.Clients.All.addMessage(GenerateMessage(client, emotionStatus), GetMessageType(emotionStatus));
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

        private static string GetMessageType(EmotionStatus emotion)
        {
            var type = string.Empty;

            switch (emotion)
            {
                case EmotionStatus.Absence:
                case EmotionStatus.Sleeping:
                    type = "error";
                    break;
                case EmotionStatus.Wandering:
                case EmotionStatus.Streching:
                    type = "warning";
                    break;
                case EmotionStatus.HandUp:
                case EmotionStatus.Nodding:
                    type = "success";
                    break;
                case EmotionStatus.Thinking:
                case EmotionStatus.Shaking:
                case EmotionStatus.Freezed:
                default:
                    type = "info";
                    break;
            }

            return type;
        }

        private static string GenerateMessage(string client, EmotionStatus emotion)
        {
            var name = "王洋洋同学";

            var template = string.Empty;

            switch (emotion)
            {
                case EmotionStatus.Absence:
                    template = RandomChoice(new string[] { "{0}已经离开", "{0}已离席", "{0}似乎不见" });
                    break;
                case EmotionStatus.Sleeping:
                    template = RandomChoice(new string[] { "{0}似乎睡着了的样子", "{0}正在酣睡中ZZzzzzZZZzzz..." });
                    break;
                case EmotionStatus.Wandering:
                    template = RandomChoice(new string[] { "{0}已经开始神游 yooooooo~~~" });
                    break;
                case EmotionStatus.Streching:
                    template = RandomChoice(new string[] { "{0}伸了个懒腰并打了个哈气", "{0}可能要准备睡了", "{0}觉得困困困了～～～唉～～～" });
                    break;
                case EmotionStatus.HandUp:
                    template = RandomChoice(new string[] { "{0}举起了小手👋", "{0}举起了小手🙋‍", "{0}举手要求回答问题啦！" });
                    break;
                case EmotionStatus.Nodding:
                    template = RandomChoice(new string[] { "{0}听的很投入", "{0}听懂了耶✌️" });
                    break;
                case EmotionStatus.Thinking:
                    template = RandomChoice(new string[] { "{0}正在认真思考", "{0}陷入了思索～" });
                    break;
                case EmotionStatus.Shaking:
                    template = RandomChoice(new string[] { "{0}似乎没有听懂", "{0}可能需要您阐述的再详细一些" });
                    break;
                case EmotionStatus.Freezed:
                case EmotionStatus.None:
                default:
                    template = "{0}正在听讲～";
                    break;
            }

            return string.Format(template, name);
        }

        private static T RandomChoice<T>(IEnumerable<T> list)
        {
            var index = RandomGenerator.Next(list.Count());
            return list.ElementAtOrDefault(index);
        }

        private static int ComputeFocusIndex(EmotionStatus emotion)
        {
            int delta = RandomGenerator.Next(1, 10);

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
