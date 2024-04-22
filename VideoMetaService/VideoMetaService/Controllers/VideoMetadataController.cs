using Microsoft.AspNetCore.Mvc;
using VideoMetaService.Interfaces;
using VideoMetaService.Models;

namespace VideoMetaService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoMetadataController : ControllerBase
    {
        private readonly IVideoMetadataService _videoMetadataService;

        public VideoMetadataController(IVideoMetadataService videoMetadataService)
        {
            _videoMetadataService = videoMetadataService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _videoMetadataService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var video = await _videoMetadataService.GetAsync(id);
            if (video == null)
            {
                return NotFound();
            }
            return Ok(video);
        }

        [HttpPost]
        public async Task<IActionResult> Create(VideoMetadata videoMetadata)
        {
            await _videoMetadataService.CreateAsync(videoMetadata);
            return CreatedAtAction(nameof(Get), new { id = videoMetadata.Id }, videoMetadata);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, VideoMetadata videoMetadata)
        {
            var video = await _videoMetadataService.GetAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            await _videoMetadataService.UpdateAsync(id, videoMetadata);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var video = await _videoMetadataService.GetAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            await _videoMetadataService.RemoveAsync(id);
            return NoContent();
        }
    }
}
