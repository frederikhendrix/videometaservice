using VideoMetaService.Models;

namespace VideoMetaService.Interfaces
{
    public interface IVideoMetadataService
    {
        Task<List<VideoMetadata>> GetAllAsync();
        Task<VideoMetadata> GetAsync(Guid id);
        Task CreateAsync(VideoMetadata videoMetadata);
        Task UpdateAsync(Guid id, VideoMetadata videoMetadataIn);
        Task RemoveAsync(Guid id);
    }
}
