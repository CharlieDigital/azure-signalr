import * as SignalR from "@microsoft/signalr";

class SignalRService {
  public async connect(idToken: string) {
    const connection = new SignalR.HubConnectionBuilder()
      .withUrl(`http://localhost:5092/notifications/`, {
        accessTokenFactory: () => idToken,
        headers: {

        }
      })
      .configureLogging(SignalR.LogLevel.Trace)
      .withAutomaticReconnect()
      .build();

    console.log("Connected SignalR");

    connection.on("Send", (msg: string) => {
      console.log(`Received: ${msg}`);
    });

    connection.start();
  }
}

export const signalRService = new SignalRService();
