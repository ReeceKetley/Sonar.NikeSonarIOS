using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace NikeSonar
{
    public class TaskCell
    {
        public string Title { get; set; }
        public string Status { get; set; }
        public bool Complete { get; set;}
        public TaskCell(string title, string status)
        {
            Title = title;
            Status = status;
        }
    }
}
