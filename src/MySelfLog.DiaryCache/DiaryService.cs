using System;
using System.Collections.Generic;
using System.Linq;
using MySelfLog.Adapter;
using Nest;

namespace MySelfLog.DiaryCache
{
    public class DiaryService : IDiaryCache
    {
        private readonly Uri _link;
        
        public DiaryService(Uri link)
        {
            _link = link;
        }
        public IDictionary<string, string> GetDiaries()
        {
            var client = GetClient();
            var searchResult = client.Search<DiaryEvent>(
                s => s
                    .Index("diary-events")
                    .AllTypes()
                    .From(0)
                    .Size(2000)
                    .Query(q => q.MatchAll())); // Think about using the scroll feature to avoid fix max val
            
            return searchResult.Documents.ToDictionary(doc => doc.Id, doc => doc.DiaryName);
        }

        private ElasticClient GetClient()
        {
            var settings = new ConnectionSettings(_link);
            settings.ThrowExceptions();
            var client = new ElasticClient(settings);
            return client;
        }
    }
}
