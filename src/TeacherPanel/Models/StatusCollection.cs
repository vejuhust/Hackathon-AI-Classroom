using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeacherPanel.Models
{
    public static class StatusCollection
    {
        private static Dictionary<string, StatusItem> _storage = new Dictionary<string, StatusItem>();
        private static Dictionary<string, int> _counter = new Dictionary<string, int>();

        public static bool Save(string client, string emotion)
        {
            if (string.IsNullOrWhiteSpace(client) || string.IsNullOrWhiteSpace(emotion))
            {
                return false;
            }

            _storage[client] = new StatusItem(client, emotion);
            return true;
        }

        public static int CountHandUp(string client)
        {
            if (string.IsNullOrWhiteSpace(client))
            {
                return 0;
            }

            if (_counter.ContainsKey(client))
            {
                _counter[client] += 1;
            }
            else
            {
                _counter[client] = 1;
            }

            return _counter[client];
        }

        public static List<StatusItem> Load(int limit = 10)
        {
            return _storage.ToList().OrderByDescending(x => x.Value.UpdateTime).Select(x => x.Value).Take(limit).ToList();
        }

        public static void Reset()
        {
            _storage = new Dictionary<string, StatusItem>();
            _counter = new Dictionary<string, int>();
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