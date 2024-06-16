using MongoDB.Driver;
using VideoMetaService.Interfaces;
using VideoMetaService.Models;

namespace VideoMetaService.Services
{
    public class VideoMetadataService : IVideoMetadataService
    {
        private readonly IMongoCollection<VideoMetadata> _videos;

        public VideoMetadataService(IMongoDatabase database)
        {
            _videos = database.GetCollection<VideoMetadata>("VideoMetadata");
        }

        public async Task<List<VideoMetadata>> GetAllAsync()
        {
            return await _videos.Find(video => true).ToListAsync();
        }

        public async Task<VideoMetadata> GetAsync(Guid id)
        {
            return await _videos.Find<VideoMetadata>(video => video.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(VideoMetadata videoMetadata)
        {
            await _videos.InsertOneAsync(videoMetadata); 
        }

        public async Task UpdateAsync(Guid id, VideoMetadata videoMetadataIn)
        {
            await _videos.ReplaceOneAsync(video => video.Id == id, videoMetadataIn);
        }

        public async Task RemoveAsync(Guid id)
        {
            await _videos.DeleteOneAsync(video => video.Id == id);
        }
    }
}
