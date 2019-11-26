import io.grpc.Server;
import io.grpc.ServerBuilder;

import java.io.IOException;

public class Main {
    public static void main(String [] args) throws InterruptedException, IOException {

        UserService userService = new UserService();

        Server server = ServerBuilder.forPort(57601)
                .addService( userService )
                .addService( new MultiplayerService(userService) )
                .build();
        System.out.println("Starting server...");
        server.start();
        System.out.println("Server started !");
        server.awaitTermination();

    }
}