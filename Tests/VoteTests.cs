using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using RestaurantSelectorService.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantSelectorService.Tests
{
    public class VoteTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        const string ROUTE_VOTE = "/api/vote";
        const string ROUTE_RESULT = "/api/vote/result";

        public VoteTests()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
               .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task SimpleVote()
        {
            var response = await _client.PostAsync(ROUTE_VOTE, UserStringContent(1, 1));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            
            Assert.Equal(VoteController.MSG_VOTE_OK, responseString);
        }

        [Fact]
        public async Task VotingAgainInSameDay()
        {
            var content = UserStringContent(1, 1);
            var response = await _client.PostAsync(ROUTE_VOTE, content);
            response.EnsureSuccessStatusCode();

            response = await _client.PostAsync(ROUTE_VOTE, content);
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(VoteController.MSG_VOTE_ALREADY, responseString);
        }

        [Fact]
        public async Task MultipleUsersVotingAgainInSameDay()
        {
            var content = UserStringContent(1, 1);
            var response = await _client.PostAsync(ROUTE_VOTE, content);
            response.EnsureSuccessStatusCode();

            content = UserStringContent(2, 2);
            response = await _client.PostAsync(ROUTE_VOTE, content);
            response.EnsureSuccessStatusCode();

            content = UserStringContent(3, 3);
            response = await _client.PostAsync(ROUTE_VOTE, content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(VoteController.MSG_VOTE_OK, responseString);
        }

        [Fact]
        public async Task ResultWithNoVotes()
        {
            var response = await _client.GetAsync(ROUTE_RESULT);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(VoteController.MSG_RESULT_NO_WINNERS, responseString);
        }

        [Fact]
        public async Task ResultWithWinner()
        {
            var restaurantId = 1;
            var content = UserStringContent(1, restaurantId);
            var response = await _client.PostAsync(ROUTE_VOTE, content);
            response.EnsureSuccessStatusCode();

            response = await _client.GetAsync(ROUTE_RESULT);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(VoteController.MSG_RESULT_WINNER_IS + "Restaurant." + restaurantId, responseString);
        }

        [Fact]
        public async Task NotAllowingRestaurantBeVotedAfterWin()
        {
            var content = UserStringContent(1, 1);
            var response = await _client.PostAsync(ROUTE_VOTE, content);
            response.EnsureSuccessStatusCode();

            response = await _client.GetAsync(ROUTE_RESULT);
            response.EnsureSuccessStatusCode();

            response = await _client.PostAsync(ROUTE_VOTE, content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(VoteController.MSG_VOTE_REST_ALREADY_WEAK, responseString);
        }

        [Fact]
        public async Task LetWinnerRestaurantBeVotedAfterWeek()
        {
            HttpContent content;
            HttpResponseMessage response;
            for (int id = 1; id <= 7; id++)
            {
                content = UserStringContent(1, id);
                response = await _client.PostAsync(ROUTE_VOTE, content);
                response.EnsureSuccessStatusCode();

                response = await _client.GetAsync(ROUTE_RESULT);
                response.EnsureSuccessStatusCode();
            }

            content = UserStringContent(1, 1);
            response = await _client.PostAsync(ROUTE_VOTE, content);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(VoteController.MSG_VOTE_OK, responseString);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task InvalidUser()
        {
            var response = await _client.PostAsync(ROUTE_VOTE, UserStringContent(999, 1));
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(VoteController.MSG_USER_ID_INVALID, responseString);
        }

        [Fact]
        public async Task InvalidRestaurant()
        {
            var response = await _client.PostAsync(ROUTE_VOTE, UserStringContent(1, 999));
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(VoteController.MSG_RESTAURANT_ID_INVALID, responseString);
        }

        StringContent UserStringContent(int userId, int restaurantId)
        {
            return new StringContent("{ \"userId\": " + userId + ", \"restaurantId\": " + restaurantId + " }", Encoding.UTF8, "application/json");
        }
    }
}
