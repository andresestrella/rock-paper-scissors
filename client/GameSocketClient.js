import * as signalR from "@microsoft/signalr";

export function configure() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5023/gameHub")
        .configureLogging(signalR.LogLevel.Critical)
        .build();
    // .withUrl("https://localhost:7106/gameHub", signalR.HttpTransportType.WebSockets ) //signalR client only works with HTTP
    // .withAutomaticReconnect()
    // .withAutomaticReconnect([0, 2000, 10000, 30000]) yields the default behavior

    connection.onreconnecting((error) => {
        console.log(
            'Connection lost due to error "' + error + '". Reconnecting...',
        );
    });

    connection.onreconnected((connectionId) => {
        console.log(
            'Connection reestablished. Connected with connectionId "' +
            connectionId +
            '".',
        );
    });

    connection.onclose((error) => {
        console.log("Goodbye human...");
        if (error) {
            console.log(
                'Connection closed due to error "' +
                error +
                '". Try refreshing this page to restart the connection.',
            );
            process.exit(1);
        }
        process.exit(0);
    });

    connection.on("ReceiveMessage", (user, message) => {
        console.log("msg by " + user + ": " + message);
    });

    return {
        connection,
        startConnection: async function(entryPoint) {
            try {
                await connection.start();
                entryPoint();
            } catch (err) {
                console.log(
                    "There was an error trying to connect to the server: " + err,
                );
                setTimeout(() => this.startConnection(), 5000);
            }
        },
        fetchStats: function(userName) {
            connection
                .invoke("FetchStats", userName)
                .catch((err) => console.error(err.toString()));
        },
        identifyUser: function(userName) {
            connection
                .invoke("IdentifyUser", userName)
                .catch((err) => console.error(err.toString()));
        },
        playMove: function(userName, move) {
            // try {
            //     await connection.invoke("PlayMove", userName, move);
            // } catch (err) {
            //     console.error(err);
            // }
            connection
                .invoke("PlayMove", userName, move)
                .catch((err) => console.error(err.toString()));
        },
    };
}

export function sendMessageToServer(connection, message) {
    connection
        .invoke("SendMessage", userName, message)
        .catch((err) => console.error(err.toString()));
}

export function identifyUser(connection, userName) {
    connection
        .invoke("IdentifyUser", userName)
        .catch((err) => console.error(err.toString()));
}

export async function sendAnotherMessageToServer(connection, message) {
    try {
        await connection.invoke("SendMessage", userName, message);
    } catch (err) {
        console.error(err);
    }
}
