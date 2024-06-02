using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Moq;
using VideoMetaService.Models;
using VideoMetaService.Services;
using Xunit;

namespace VideoMetaServiceTest
{
    public class VideoMetadataServiceTests
    {
        private readonly Mock<IMongoCollection<VideoMetadata>> _mockCollection;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly VideoMetadataService _service;

        public VideoMetadataServiceTests()
        {
            _mockCollection = new Mock<IMongoCollection<VideoMetadata>>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockDatabase.Setup(db => db.GetCollection<VideoMetadata>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                         .Returns(_mockCollection.Object);

            _service = new VideoMetadataService(_mockDatabase.Object);
        }

        /// <summary>
        /// Function creates two video's and adds them to the mock database. 
        /// Then it runs the function made to retrieve all videos from the database.
        /// 
        /// In the assert it checks if the count of the returned video list equals the count of the list used to add videos.
        /// </summary>
        /// <returns>Should Return True</returns>
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllVideos()
        {
            // Arrange
            var video1 = new VideoMetadata
            {
                Id = Guid.NewGuid(),
                Title = "Test Title 1",
                Description = "Test Description 1"
            };

            var video2 = new VideoMetadata
            {
                Id = Guid.NewGuid(),
                Title = "Test Title 2",
                Description = "Test Description 2"
            };

            var videos = new List<VideoMetadata> { video1, video2 };

            _mockCollection.Setup(collection => collection.FindAsync(It.IsAny<FilterDefinition<VideoMetadata>>(), It.IsAny<FindOptions<VideoMetadata, VideoMetadata>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(MockCursor(videos));

            await _service.CreateAsync(video1);
            await _service.CreateAsync(video2);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(videos.Count, result.Count);
        }

        /// <summary>
        /// Function first creates a videometa object. Then it uses the generated Id to retrieve the video in var result
        /// Then it checks if the video matches the values set previously.
        /// </summary>
        /// <returns>Should Return True</returns>
        [Fact]
        public async Task GetAsync_ShouldReturnVideoById()
        {
            // Arrange
            var videoId = Guid.NewGuid();
            var video = new VideoMetadata { Id = videoId, Title = "Test Title", Description = "Test Description" };
            var videos = new List<VideoMetadata> { video };

            _mockCollection.Setup(collection => collection.FindAsync(It.IsAny<FilterDefinition<VideoMetadata>>(), It.IsAny<FindOptions<VideoMetadata, VideoMetadata>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(MockCursor(videos));

            await _service.CreateAsync(video);

            // Act
            var result = await _service.GetAsync(videoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(videoId, result.Id);
            Assert.Equal(video.Title, result.Title);
            Assert.Equal(video.Description, result.Description);
        }

        /// <summary>
        /// Function creates a variable and adds it to the database. Then in the assert if verifies that the var video is actually inside of the mockCollection.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateAsync_ShouldCallInsertOneAsync()
        {
            // Arrange
            var video = new VideoMetadata { Id = Guid.NewGuid(), Title = "Test Title", Description = "Test Description" };

            // Act
            await _service.CreateAsync(video);

            // Assert
            _mockCollection.Verify(collection => collection.InsertOneAsync(video, null, default), Times.Once);
        }

        /// <summary>
        /// This function first adds a video to the database. 
        /// Then it changes the title and description values of the video.
        /// In the end it asserts that the update did in fact change the value. 
        /// </summary>
        /// <returns>Should return true for all Asserts</returns>
        [Fact]
        public async Task UpdateAsync_ShouldCallReplaceOneAsync()
        {
            // Arrange
            var videoId = Guid.NewGuid();
            var video = new VideoMetadata { Id = videoId, Title = "Test Title", Description = "Test Description" };
            var videoSecond = new VideoMetadata { Id = videoId, Title = "Changed Title", Description = "Changed Description" };

            // Mocking the initial creation of the video
            _mockCollection.Setup(collection => collection.InsertOneAsync(video, null, default)).Returns(Task.CompletedTask);
            await _service.CreateAsync(video);

            // Mocking the update
            _mockCollection.Setup(collection => collection.ReplaceOneAsync(It.IsAny<FilterDefinition<VideoMetadata>>(), videoSecond, It.IsAny<ReplaceOptions>(), default))
                           .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, videoId));

            // Mocking the retrieval after the update
            var updatedVideos = new List<VideoMetadata> { videoSecond };
            _mockCollection.Setup(collection => collection.FindAsync(It.IsAny<FilterDefinition<VideoMetadata>>(), It.IsAny<FindOptions<VideoMetadata, VideoMetadata>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(MockCursor(updatedVideos));

            // Act
            await _service.UpdateAsync(videoId, videoSecond);

            // Assert
            var videoTest = await _service.GetAsync(videoId);
            Assert.NotNull(videoTest);
            Assert.Equal(videoSecond.Id, videoTest.Id);
            Assert.Equal(videoSecond.Title, videoTest.Title);
            Assert.Equal(videoSecond.Description, videoTest.Description);
            Assert.NotEqual(video.Title, videoTest.Title);
        }

        [Fact]
        public async Task RemoveAsync_ShouldCallDeleteOneAsync()
        {
            // Arrange
            var videoId = Guid.NewGuid();

            // Act
            await _service.RemoveAsync(videoId);

            // Assert
            _mockCollection.Verify(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<VideoMetadata>>(), default), Times.Once);
        }

        private IAsyncCursor<VideoMetadata> MockCursor(IEnumerable<VideoMetadata> items)
        {
            var mockCursor = new Mock<IAsyncCursor<VideoMetadata>>();
            mockCursor.Setup(_ => _.Current).Returns(items);
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));
            return mockCursor.Object;
        }
    }
}
