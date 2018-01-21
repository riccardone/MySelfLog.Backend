using System;

namespace MySelfLog.DiaryCache
{
    public class DiaryEvent
    {
        public string Id { get; set; }
        public string DiaryName { get; set; }
        public string Source { get; set; }
        public DateTime Applies { get; set; }
    }
}
