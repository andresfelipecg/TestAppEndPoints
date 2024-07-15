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
            var studyGroup = new StudyGroup(
                studyGroupId: 1,
                name: "Math Study Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: new List<User>() // Agrega usuarios si es necesario
            );

            var json = JsonConvert.SerializeObject(studyGroup);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("", content);
            response.EnsureSuccessStatusCode();

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
            var studyGroups = JsonConvert.DeserializeObject<List<StudyGroup>>(responseContent);

            // Assert
            Assert.IsNotNull(studyGroups);
            Assert.IsTrue(studyGroups.Count > 0);
        }

        [Test]
        public async Task TestJoinStudyGroup()
        {
            // Arrange
            int studyGroupId = 1;
            int userId = 5;

            // Act
            var response = await _client.PostAsync($"/{studyGroupId}/join?userId={userId}", null);

            response.EnsureSuccessStatusCode();

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [Test]
        public async Task TestLeaveStudyGroup()
        {
            // Arrange
            int studyGroupId = 1;
            int userId = 1;

            // Act
            var response = await _client.PostAsync($"/{studyGroupId}/leave?userId={userId}", null);
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [Test]
        public async Task TestCreateStudyGroupWithInvalidName()
        {
            // Arrange
            var studyGroup = new StudyGroup(
                studyGroupId: 1,
                name: "TooLongStudyGroupNameThatExceedsThirtyCharacters",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: new List<User>()
            );

            var json = JsonConvert.SerializeObject(studyGroup);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("", content);

            // Assert
            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task TestCreateStudyGroupWithInvalidSubject()
        {
            // Arrange
            var studyGroup = new StudyGroup(
                studyGroupId: 1,
                name: "Invalid Subject Group",
                subject: (Subject)999, // Invalid subject
                createDate: DateTime.Now,
                users: new List<User>()
            );

            var json = JsonConvert.SerializeObject(studyGroup);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("", content);

            // Assert
            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task TestCreateDuplicateStudyGroupForSubject()
        {
            // Arrange
            var studyGroup1 = new StudyGroup(
                studyGroupId: 1,
                name: "Math Study Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: new List<User>()
            );

            var studyGroup2 = new StudyGroup(
                studyGroupId: 2,
                name: "Another Math Study Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: new List<User>()
            );

            var json1 = JsonConvert.SerializeObject(studyGroup1);
            var content1 = new StringContent(json1, Encoding.UTF8, "application/json");

            var json2 = JsonConvert.SerializeObject(studyGroup2);
            var content2 = new StringContent(json2, Encoding.UTF8, "application/json");

            // Act
            var response1 = await _client.PostAsync("", content1);
            response1.EnsureSuccessStatusCode();

            var response2 = await _client.PostAsync("", content2);

            // Assert
            Assert.IsFalse(response2.IsSuccessStatusCode);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response2.StatusCode);
        }
    }
}