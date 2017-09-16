
namespace TeacherPanel.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using TeacherPanel.Models;

    [Route("api/Emotion")]
    public class EmotionController : ApiController
    {
        private static int RequestCounter = 0;
        private static DateTime LastUpdateTime = DateTime.UtcNow;

        [HttpGet]
        public ReduceResult Get()
        {
            return new ReduceResult(RequestCounter, GetWording(), LastUpdateTime);
        }

        [HttpPost]
        public bool Post([FromUri]MapRequest model)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.Client) && !string.IsNullOrWhiteSpace(model.Emotion))
            {
                StatusCollection.Save(model.Client, model.Emotion);

                RequestCounter += 1;
                LastUpdateTime = DateTime.UtcNow;

                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetWording()
        {
            var list = StatusCollection.Load();
            return string.Join(";", list.Select(x => x.Client + "=" + x.Emotion + "=" + x.UpdateTime.ToString("mm:ss")).ToArray());
        }

        [HttpDelete]
        public void Delete()
        {
            StatusCollection.Reset();
        }
    }
}
