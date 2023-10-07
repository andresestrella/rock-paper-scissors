# rock-paper-scissors
.net signalR websocket server + nodejs cli client  

# how to run 
- run a postgres instance locally and change the 'ConnectionString' value in the `appsettings.json` according to your DB instance  
- create and run DB migration: `dotnet ef migrations add InitialCreate` -> `dotnet ef database update` (make sure to have dotnet ef installed: `dotnet tool install -g dotnet-ef`)  
- run the app: `dotnet run` or run in Visual Studio.  
- to run the client install packages `npm install` and run `node Game.js`  

# design
## server
I chose to build a server that handles websocket over regular HTTP because it is a much more adequate protocol for games.  
If we for example wanted to implement player vs player to the game instead of just playing against the bot. It would be much harder to implement over regular HTTP and the solution would be extremely inefficient since it would require doing long polling as opposed to websockets which enable real time communication between server and clients.  


## client
I wanted to build a client to test out the server's functionalities and could play the game.  
Since I didn't have to build UI I made one that runs on the command line.  

## business logic
- the bot's move and the result of each match must be generated in the server instead of the client so users can't cheat by changing their client's code.  

- currently the server chooses rock paper or scissors randomly with equal 1/3 chance for each.  
One of the assignment requirements suggested exploiting that humans are bad at generating random numbers   
One first upgrade I can think of is adding some logic that uses all of the player's previous matches stored in the database and assume that as the probability of his next move. For example, if in the past the user has played 50 rocks, 25, papers and 25 scissors the server would have the most chance at winning if it plays paper 50% of the time, rock 25% and scissors 25% of the time.  
This solution however doesn't exploit if the player plays continuos patterns like playing rock, then paper, and finally scissors continuously in that order. In this suggested  solution the server would play each option 33.3% of the time, when a human that recognizes the pattern would win every time.  
This caveat of patterns I believe turns the problem into a much more complex one that could be solved with some advanced statistics calculation or AI.  


## Libraries used
for the server: SignalR for websockets and Entity Framework for ORM functionalities.  
for the client: SignalR client node package. 

# demo
https://github.com/andresestrella/rock-paper-scissors/assets/57234183/755f9fe2-852e-4afc-b1cf-15b2b12eea0a


