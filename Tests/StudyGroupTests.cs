using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestApp.Tests
{
    [TestFixture]
    public class StudyGroupTests
    {
        private HttpClient _client;
        private const string BaseUrl = "https://localhost:7140/api/StudyGroup"; // Update with your API base URL

        [SetUp]
        public void Setup()
        {
            // Configurar HttpClient para las pruebas de la API
            _client = new HttpClient();
            _client.BaseAddress = new Uri(BaseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
        }

        [Test]
        public async Task TestCreateStudyGroup()
        {
            // Arrange
            var studyGroup = new
            {
                studyGroupId = 3,
                name = "New study group with ID 3",
                createDate = DateTime.Now.ToString("o"),
                subjectId = 3,
                userId = 3,
                subject = new
                {
                    subjectId = 0,
                    name = "string",
                    description = "string"
                }
            };

            var json = JsonConvert.SerializeObject(studyGroup);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("", content);

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [Test]
        public async Task TestGetStudyGroups()
        {
            // Arrange 
            var response = await _client.GetAsync("");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var studyGroups = JsonConvert.DeserializeObject<List<StudyGroupDto>>(responseContent);

            // Assert
            Assert.IsNotNull(studyGroups);
            Assert.IsTrue(studyGroups.Count > 0);
        }

        [Test]
        public async Task TestJoinStudyGroup()
        {
            // Arrange
            int studyGroupId = 3;
            int userId = 4;
            string url = $"{BaseUrl}/{studyGroupId}/join?userId={userId}";
            Console.WriteLine($"Joining StudyGroup URL: {url}");

            // Act
            var response = await _client.PostAsync(url, null);  // Usar la URL completa aquí

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to join study group. Status code: {response.StatusCode}");
        }

        [Test]
        public async Task TestLeaveStudyGroup()
        {
            // Arrange
            int studyGroupId = 3;
            int userId = 4;
            string url = $"{BaseUrl}/{studyGroupId}/leave?userId={userId}";
            Console.WriteLine($"Leaving StudyGroup URL: {url}");

            // Act
            var response = await _client.PostAsync(url, null);

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to leave study group. Status code: {response.StatusCode}");
        }

        [Test]
        public async Task TestCreateStudyGroupWithInvalidName()
        { 
            // Arrange
            var studyGroup = new 
            {
                studyGroupId = 1,
                name = "TooLongStudyGroupNameThatExceedsThirtyCharacters",
                subjectId = 3,
                createDate = DateTime.Now.ToString("o"),
                users = new List<User>()  
            };


            var json = JsonConvert.SerializeObject(studyGroup);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("", content);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.IsFalse(response.IsSuccessStatusCode, $"Unexpectedly succeeded in creating study group. Response: {responseContent}");
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task TestCreateStudyGroupWithInvalidSubject()
        {
            // Arrange
            var studyGroup = new
            {
                studyGroupId = 1,
                name = "Invalid Subject Group",
                subjectId = 999, // Invalid subject
                createDate = DateTime.Now.ToString("o"),
                users = new List<User>()
            };

            var json = JsonConvert.SerializeObject(studyGroup);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("", content);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.IsFalse(response.IsSuccessStatusCode, $"Unexpectedly succeeded in creating study group. Response: {responseContent}");
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task TestSearchStudyGroupsBySubject()
        {
            // Arrange
            string subject = "Math";
            string url = $"{BaseUrl}/search?subject={subject}";
            Console.WriteLine($"Searching StudyGroup URL: {url}");

            // Act
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var studyGroups = JsonConvert.DeserializeObject<List<StudyGroupDto>>(responseContent);

            // Assert
            Assert.IsNotNull(studyGroups, "No study groups were returned.");
            Assert.IsTrue(studyGroups.Count > 0, "No study groups found for the subject 'Math'.");

        }

        [Test]
        public async Task TestCreateDuplicateStudyGroupForSubject()
        {
            // Arrange
            var studyGroup1 = new
            {
                studyGroupId = 3,
                name = "Math Study Group",
                subjectId = 3,
                createDate = DateTime.Now.ToString("o"),
                userId = 3,
                subject = new
                {
                    subjectId = 0,
                    name = "string",
                    description = "string"
                }

            };

            var studyGroup2 = new
            {
                studyGroupId = 4,
                name = "Another Math Study Group",
                subjectId = 3,
                createDate = DateTime.Now.ToString("o"),
                userId = 3,
                subject = new
                {
                    subjectId = 0,
                    name = "string",
                    description = "string"
                }
            };

            var json1 = JsonConvert.SerializeObject(studyGroup1);
            var content1 = new StringContent(json1, Encoding.UTF8, "application/json");

            var json2 = JsonConvert.SerializeObject(studyGroup2);
            var content2 = new StringContent(json2, Encoding.UTF8, "application/json");

            // Act
            var response1 = await _client.PostAsync("", content1);
            var responseContent1 = await response1.Content.ReadAsStringAsync();
            Console.WriteLine($"Response 1: {responseContent1}");
            response1.EnsureSuccessStatusCode();

            var response2 = await _client.PostAsync("", content2);
            var responseContent2 = await response2.Content.ReadAsStringAsync();
            Console.WriteLine($"Response 2: {responseContent2}");

            // Assert
            Assert.IsFalse(response2.IsSuccessStatusCode, $"Unexpectedly succeeded in creating duplicate study group. Response: {responseContent2}");
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response2.StatusCode, "Expected BadRequest status for duplicate study group creation.");
        }

        private class StudyGroupDto
        {
            public int StudyGroupId { get; set; }
            public string Name { get; set; }
            public DateTime CreateDate { get; set; }
            public int SubjectId { get; set; }
            public int UserId { get; set; }
            public SubjectDto Subject { get; set; }
        }

        private class SubjectDto
        {
            public int SubjectId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        private class User
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
        }
    }
}