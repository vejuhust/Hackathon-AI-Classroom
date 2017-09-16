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

        [HttpPost]
        public bool Post([FromUri]MapRequest model)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.Client) && !string.IsNullOrWhiteSpace(model.Emotion))
            {
                StatusCollection.Save(model.Client, model.Emotion);

                RequestCounter += 1;
                LastUpdateTime = DateTime.UtcNow;

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<ReportHub>();
                hubContext.Clients.All.renderReport(StatusCollection.GetConclusion());

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
