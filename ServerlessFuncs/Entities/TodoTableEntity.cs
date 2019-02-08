using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace ServerlessFuncs
{
    public class TodoTableEntity : TableEntity
    {
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
    }
}