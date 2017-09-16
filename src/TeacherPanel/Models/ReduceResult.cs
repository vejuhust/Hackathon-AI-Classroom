using System;

namespace TeacherPanel.Models
{
    public class ReduceResult
    {
        public int Count { get; set; }

        public string Wording { get; set; }

        public string Time { get; set; }

        public ReduceResult(int count, string wording, DateTime time)
        {
            this.Count = count;
            this.Wording = wording;
            this.Time = time.ToString("O");
        }
    }
}