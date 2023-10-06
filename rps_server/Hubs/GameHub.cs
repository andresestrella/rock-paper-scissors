using Microsoft.AspNetCore.SignalR;
using rps_server.Entities;
using rps_server.Helpers;
using rps_server.Repository;

namespace rps_server.Hubs
{
    public class GameHub : Hub
    {
        private readonly IUserRepository _userRepository;

        public GameHub(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task IdentifyUser(string userName)
        {
            User responseUser = null;
            string message = "";
            if (string.IsNullOrWhiteSpace(userName))
                throw new AppException("Username is required");

            try
            {
                //check if user exists
                responseUser = _userRepository.GetByUserName(userName);
                message =
                    "Ahhh, of course, "
                    + userName
                    + ", how could I have forgotten you?!\nLet's get back to where we were last time.\n";
            }
            catch (KeyNotFoundException e)
            {
                if (e.Message == "User not found")
                {
                    //else, add user to database
                    responseUser = new User { UserName = userName };
                    _userRepository.Create(responseUser);
                    message =
                        "Welcome, "
                        + userName
                        + "!\nLooks like it's your first time here.\nMake yourself at home.\n"
                        + "I don't think I need to explain to you the rules of Rock-Paper-Scissors "
                        + "if you are smart enough to get here in the first place so let's get started!\n";
                }
            }
            //send StartGame signal to user
            /*            await Clients
                            .Client(Context.ConnectionId)
                            .SendAsync("StartGame", message);*/
            /*                .SendAsync("StartGame", responseUser.Id.ToString(), message);*/
            await Clients.Client(Context.ConnectionId).SendAsync("StartGame", responseUser.Id, message);
        }

        public async Task FetchStats(string userName)
        {
            //fetch user stats from databasea
            //return stats to user
            await Clients
                .Client(Context.ConnectionId)
                .SendAsync(
                    "ReceiveStats",
                    "You have played 0 games, won 0 games, and lost 0 games."
                );
        }

        public async Task PlayMove(string userName, string move)
        {
            //create Match object

            //randomly generate computer move
            string computerMove = GenerateRandomChoice();
            string playerMove = move.ToLower();
            Match.MatchResult result;
            string message = "";

            //compare moves and add results to match
            if (computerMove == playerMove)
            { //draw
                result = Match.MatchResult.Draw;
                message = "You played " + move + ".\nI played " + computerMove + ".\nIt's a draw!";
            }
            else if (
                (computerMove == "scissors" && playerMove == "rock")
                || (computerMove == "paper" && playerMove == "scissors")
                || (computerMove == "rock" && playerMove == "paper")
            )
            {
                //player wins
                result = Match.MatchResult.Win;
                message =
                    "You played " + move + ".\nI played " + computerMove + ".\nYou " + "win" + "!";
            }
            else if (
                (computerMove == "rock" && playerMove == "scissors")
                || (computerMove == "paper" && playerMove == "rock")
                || (computerMove == "scissors" && playerMove == "paper")
            )
            {
                //computer wins
                result = Match.MatchResult.Lose;
                message =
                    "You played " + move + ".\nI played " + computerMove + ".\nYou " + "lose" + "!";
            }

            //update database

            //send result signal to user
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveResult", message);
        }

        private static string GenerateRandomChoice()
        {
            // Create a Random object to generate random numbers
            Random random = new Random();

            // Generate a random number between 0 and 2 (inclusive)
            int randomNumber = random.Next(0, 3);

            // Map the random number to 'r', 'p', or 's'
            string randomChoice;
            switch (randomNumber)
            {
                case 0:
                    randomChoice = "rock";
                    break;
                case 1:
                    randomChoice = "paper";
                    break;
                case 2:
                    randomChoice = "scissors";
                    break;
                default:
                    // Handle unexpected values (optional)
                    randomChoice = "rock"; // Default to 'r'
                    break;
            }

            return randomChoice;
        }
    }
}
