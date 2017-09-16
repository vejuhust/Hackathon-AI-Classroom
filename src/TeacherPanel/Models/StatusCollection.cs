using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeacherPanel.Models
{
    public static class StatusCollection
    {
        private static Dictionary<string, StatusItem> _storage = new Dictionary<string, StatusItem>();

        public static bool Save(string client, string emotion)
        {
            if (string.IsNullOrWhiteSpace(client) || string.IsNullOrWhiteSpace(emotion))
            {
                return false;
            }

            _storage[client] = new StatusItem(client, emotion);
            return true;
        }

        public static List<StatusItem> Load(int limit = 10)
        {
            return _storage.ToList().OrderByDescending(x => x.Value.UpdateTime).Select(x => x.Value).Take(limit).ToList();
        }

        public static void Reset()
        {
            _storage = new Dictionary<string, StatusItem>();
        }

        public static string GetConclusion()
        {
            var list = StatusCollection.Load();
            return string.Join(";", list.Select(x => x.Client + "=" + x.Emotion + "=" + x.UpdateTime.ToString("mm:ss")).ToArray());
        }
    }

    public class StatusItem
    {
        public string Client { get; set; }

        public string Emotion { get; set; }

        public DateTime UpdateTime { get; set; }

        public StatusItem(string client, string emotion)
        {
            this.Client = client;
            this.Emotion = emotion;
            this.UpdateTime = DateTime.UtcNow;
        }
    }
}