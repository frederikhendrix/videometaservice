using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VideoMetaService.Models
{
    public class VideoMetadata
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Cast { get; set; }
        public List<string> Crew { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Genres { get; set; }
        public int Rating { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Director { get; set; }
        public int ViewCount { get; set; }
    }
}
