using System.Net.Http.Json;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
namespace TestAPI;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task TestAddAndGetPoint()
    {
        // Arrange
        var client = new HttpClient { BaseAddress = new Uri("http://localhost:5032") };
        var newPhoto = new { UId = 1 };

        // Act
        var postResponse = await client.PostAsJsonAsync("/api/photos", newPhoto);
        postResponse.EnsureSuccessStatusCode();

        var getResponse = await client.GetAsync($"/api/photos/{newPhoto.UId}");
        getResponse.EnsureSuccessStatusCode();

        var photo = await getResponse.Content.ReadAsAsync<Photo>();

        // Assert
        Assert.AreEqual(newPhoto.UId, photo.UId);
    }
}