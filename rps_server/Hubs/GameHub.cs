using Microsoft.AspNetCore.SignalR;
using rps_server.Entities;
using rps_server.Helpers;
using rps_server.Repository;

namespace rps_server.Hubs
{
    public class GameHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private readonly IMatchRepository _matchRepository;

        public GameHub(IUserRepository userRepository, IMatchRepository matchRepository)
        {
            _userRepository = userRepository;
            _matchRepository = matchRepository;
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
                    //else, create and save new user to db
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
            await Clients
                .Client(Context.ConnectionId)
                .SendAsync("StartGame", responseUser.Id, message);
        }

        public async Task FetchStats(int userId)
        {
            string message = "";
            var matches = _matchRepository.GetAllByUserId(userId);

            //calculate stats
            int totalMatches = matches.Count();
            int totalWins = matches.Where(x => x.Result == Match.MatchResult.Win).Count();
            int totalLosses = matches.Where(x => x.Result == Match.MatchResult.Lose).Count();
            int totalDraws = matches.Where(x => x.Result == Match.MatchResult.Draw).Count();
            DateTime mostRecentMatch = matches.Max(x => x.Date);

            message =
                "You have played "
                + totalMatches
                + " games, won "
                + totalWins
                + " games, and lost "
                + totalLosses
                + " games.\nYour most recent match was on "
                + mostRecentMatch.ToString("MMMM dd, yyyy")
                + ".\n";

            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveStats", message);
        }

        public async Task PlayMove(int userId, string move)
        {
            string computerMove = GenerateRandomChoice();
            string playerMove = move.ToLower();
            Match.MatchResult result = DetermineWinner(playerMove, computerMove);
            string message = "";
            switch (result)
            {
                case Match.MatchResult.Draw:
                    message =
                        "You played " + move + ".\nI played " + computerMove + ".\nIt's a draw!";
                    break;
                case Match.MatchResult.Win:
                    message = "You played " + move + ".\nI played " + computerMove + ".\nYou win!";
                    break;
                case Match.MatchResult.Lose:
                    message = "You played " + move + ".\nI played " + computerMove + ".\nYou lose!";
                    break;
            }

            //update database
            User matchedUser = _userRepository.GetById(userId);
            Match resultMatch = new Match
            {
                Result = result,
                PlayerMove = playerMove,
                ComputerMove = computerMove,
                Date = DateTime.UtcNow,
                UserId = userId,
            };
            matchedUser.Matches.Add(resultMatch);
            _userRepository.Save();

            //send result message to user
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveResult", message);
        }

        private Match.MatchResult DetermineWinner(string playerMove, string computerMove)
        {
            if (computerMove == playerMove)
            {
                //draw
                return Match.MatchResult.Draw;
            }
            else if (
                (computerMove == "scissors" && playerMove == "rock")
                || (computerMove == "paper" && playerMove == "scissors")
                || (computerMove == "rock" && playerMove == "paper")
            )
            {
                //player wins
                return Match.MatchResult.Win;
            }
            else if (
                (computerMove == "rock" && playerMove == "scissors")
                || (computerMove == "paper" && playerMove == "rock")
                || (computerMove == "scissors" && playerMove == "paper")
            )
            {
                //player loses
                return Match.MatchResult.Lose;
            }
            else
            {
                //error
                throw new AppException("Invalid move");
            }
        }

        private static string GenerateRandomChoice()
        {
            // Create a Random object to generate random numbers
            Random random = new Random();

            // Generate a random number between 0 and 2 (inclusive)
            int randomNumber = random.Next(0, 3);

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
