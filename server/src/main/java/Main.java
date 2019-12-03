import io.grpc.Server;
import io.grpc.ServerBuilder;
import java.io.IOException;

public class Main {
    public static void main(String [] args) throws InterruptedException, IOException {
        Server server =ServerBuilder.forPort(57601)
                .addService( new UserService() )
                .addService( new MultiplayerService() )
                .build();
        System.out.println("Starting server...");
        server.start();
        System.out.println("Server started !");
        server.awaitTermination();

    }
}