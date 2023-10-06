import readline from "readline";
import * as GameSocketClient from "./GameSocketClient.js";

let userName = "";
let userId = null;
const matchPrompts = [
    "Cmon! Let's play again!",
    "You pesky humans are so predictable.",
    "I'll play with you, but only because I'm programmed to.",
    "I'll go with Rock this time for sure.",
    "I'm feeling lucky, I'll go with Paper.",
    "You won't expect Scissors this time, will you?",
    "If I win this one you'll have to hire Andres, if I lose you can hire whoever programmed me.",
];

function getRandomMatchPrompt() {
    return matchPrompts[Math.floor(Math.random() * matchPrompts.length)];
}

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout,
});

function greetPlayer() {
    console.log(
        "\nHello human, I am RPS3000 and it looks like you have connected \
to my Rock-Paper-Scissors WebSocket service.\n\
Let's play some RPS, but first, I don't recognize you... Please identify yourself with the ID the humans \
you refer to as 'parents' gave you at birth \n",
    );
    console.log("Input your name or 'q' to leave: ");
    promptForName();
}

function promptForName() {
    rl.question("> ", (input) => {
        if (input === "q") {
            gameClient.connection.stop();
        } else if (input === "") {
            console.log("You must input a name");
            promptForName();
        } else {
            userName = input;
            gameClient.identifyUser(userName);
        }
    });
}

function promptForMove() {
    console.log("\n" + getRandomMatchPrompt());
    console.log("1. Rock\n2. Paper\n3. Scissors \nq  Back to menu\n");
    rl.question("> ", (input) => {
        if (input === "1") {
            // console.log("You chose Rock");
            gameClient.playMove(userId, "Rock");
        } else if (input === "2") {
            // console.log("You chose Paper");
            gameClient.playMove(userId, "Paper");
        } else if (input === "3") {
            // console.log("You chose Scissors");
            gameClient.playMove(userId, "Scissors");
        } else if (input === "q") {
            console.log("\nYeah I got bored anyways...\n");
            startGame();
        } else {
            console.log("Invalid input");
            promptForMove();
        }
    });
}

function startGame() {
    console.log("1. Play\n2. Stats\nq  Quit\n");
    console.log("What do you wish to do?");
    const promptMenu = () => {
        rl.question("> ", (input) => {
            if (input === "1") {
                promptForMove();
            } else if (input === "2") {
                console.log("You chose to view stats");
                gameClient.fetchStats(userId);
            } else if (input === "q") {
                gameClient.connection.stop();
            } else {
                console.log("Invalid input");
                promptMenu();
            }
        });
    };
    promptMenu();
}

const gameClient = GameSocketClient.configure();
gameClient.connection.on("StartGame", (uid, message) => {
    userId = uid;
    console.log("\n" + message);
    startGame();
});

gameClient.connection.on("ReceiveResult", (message) => {
    console.log(message);
    promptForMove();
});

gameClient.connection.on("ReceiveStats", (message) => {
    console.log(message);
    rl.question("\nPress Enter to return to the main menu\n> ", () => {
        startGame();
    });
});

gameClient.startConnection(greetPlayer);
