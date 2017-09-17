using System;
using System.Collections.Generic;

namespace TeacherPanel.Models
{
    public class ReduceResult
    {
        public int Count { get; set; }

        public IEnumerable<StatusItem> Status { get; set; }

        public string Time { get; set; }

        public ReduceResult(int count, IEnumerable<StatusItem> status, DateTime time)
        {
            this.Count = count;
            this.Status = status;
            this.Time = time.ToString("O");
        }
    }
}