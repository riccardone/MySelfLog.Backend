using System;
using System.Collections.Generic;
using MySelfLog.Adapter;

namespace MySelfLog.DiaryCache
{
    public class DiaryCacheService : IDiaryCache
    {
        private IDictionary<string, string> _cache;
        private readonly DiaryService _diaryService;
        public DiaryCacheService(Uri link)
        {
            _diaryService = new DiaryService(link);
        }
        public IDictionary<string, string> GetDiaries()
        {
            if (_cache != null) return _cache;
            _cache = _diaryService.GetDiaries();
            return _cache;
        }
    }
}
