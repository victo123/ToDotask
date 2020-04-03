using System;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Expiry { get; set; }
        public int Complete { get; set; }
        public bool IsComplete { get; set; }
    }
}