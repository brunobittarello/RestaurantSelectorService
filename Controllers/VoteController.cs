using Microsoft.AspNetCore.Mvc;
using RestaurantSelectorService.DTOs;
using RestaurantSelectorService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestaurantSelectorService.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        static IList<int> votersId = new List<int>();
        static IDictionary<int, int> restaurantVotes = new Dictionary<int, int>();
        static IList<int> restaurantWeek = new List<int>();

        static List<Restaurant> restaurants = new List<Restaurant>();
        static List<User> users = new List<User>();

        public const string MSG_RESULT_NO_WINNERS = "Sem vencedor, ainda não houve votos!";
        public const string MSG_RESULT_WINNER_IS = "O restaurante vencedor do dia é: ";

        public const string MSG_VOTE_ALREADY = "Você já votou hoje";
        public const string MSG_VOTE_REST_ALREADY_WEAK = "Restaurante já escolhido essa semana. Por favor, escolha outro!";
        public const string MSG_VOTE_OK = "Voto computado";

        public const string MSG_USER_ID_INVALID = "Usuário invalido!";
        public const string MSG_RESTAURANT_ID_INVALID = "Restaurante invalido";

        [HttpGet("result")]
        public string Result()
        {
            var restaurant = GetMostVoted();
            if (restaurant == null) return MSG_RESULT_NO_WINNERS;

            ResetDay(restaurant.Id);
            return MSG_RESULT_WINNER_IS + restaurant.Name;
        }

        [HttpPost]
        public ActionResult<string> Vote([FromBody] VoteDTO vote)
        {
            if (votersId.Contains(vote.UserId))
                return BadRequest(MSG_VOTE_ALREADY);

            if (!users.Exists(u => u.Id == vote.UserId))
                return BadRequest(MSG_USER_ID_INVALID);

            if (!restaurants.Exists(r => r.Id == vote.RestaurantId))
                return BadRequest(MSG_RESTAURANT_ID_INVALID);

            if (restaurantWeek.Contains(vote.RestaurantId))
                return BadRequest(MSG_VOTE_REST_ALREADY_WEAK);

            votersId.Add(vote.UserId);
            if (restaurantVotes.ContainsKey(vote.RestaurantId))
                restaurantVotes[vote.RestaurantId]++;
            else
                restaurantVotes.Add(vote.RestaurantId, 1);
            return Ok(MSG_VOTE_OK);
        }

        [HttpGet("reset")]
        public string Reset()
        {
            ResetDatabase();
            return "Banco de dados renovado";
        }

        internal static void ResetDatabase()
        {
            votersId.Clear();
            restaurantVotes.Clear();
            restaurantWeek.Clear();

            users.Clear();
            for (int i = 1; i <= 50; i++)
                users.Add(new User() {Id = i, Name = "User." + i});

            restaurants.Clear();
            for (int i = 1; i <= 10; i++)
                restaurants.Add(new Restaurant() { Id = i, Name = "Restaurant." + i });
        }

        private Restaurant GetMostVoted()
        {
            if (votersId.Count == 0)
                return null;

            var highVotes = 0;
            var mostVoted = new List<int>();

            foreach (var votes in restaurantVotes)
            {
                if (votes.Value < highVotes)
                    continue;

                if (votes.Value > highVotes)
                {
                    highVotes = votes.Value;
                    mostVoted.Clear();
                }
                mostVoted.Add(votes.Key);
            }

            if (mostVoted.Count == 1)
                return GetRestaurant(mostVoted[0]);

            return GetRestaurant(mostVoted[(new Random()).Next(0, mostVoted.Count)]);

        }

        private Restaurant GetRestaurant(int id)
        {
            return restaurants.Find(r => r.Id == id);
        }

        private void ResetDay(int id)
        {
            if (restaurantWeek.Count == 6)
                restaurantWeek.RemoveAt(0);
            restaurantWeek.Add(id);
            votersId.Clear();
            restaurantVotes.Clear();
        }
    }
}
