using System;
using System.Collections.Generic;
using Elasticsearch.Net;
using MySelfLog.Adapter;
using Nest;

namespace MySelfLog.DiaryCache
{
    public class DiaryCacheService : IDiaryCache
    {
        private readonly Uri _link;
        private IDictionary<string, string> _cache;
        public DiaryCacheService(Uri link)
        {
            _link = link;
        }
        public IDictionary<string, string> GetDiaries()
        {
            if (_cache != null) return _cache;
            var client = GetClient();
            var searchResult = client.Search<DiaryEvent>(
                s => s
                    .Index("diary-events")
                    .AllTypes()
                    .From(0)
                    .Size(1000)
                    .Query(q => q.MatchAll())); // Think about using the scroll feature to avoid fix max val
            _cache = new Dictionary<string, string>();
            foreach (var doc in searchResult.Documents)
                _cache.Add(doc.Id, doc.DiaryName);
            return _cache;
        }

        private ElasticClient GetClient()
        {
            var settings = new ConnectionSettings(_link);
            return new ElasticClient(settings);
        }
    }
}
