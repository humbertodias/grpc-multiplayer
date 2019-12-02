import io.grpc.Server;
import io.grpc.ServerBuilder;
import io.grpc.netty.NettyServerBuilder;

import java.io.IOException;
import java.net.*;
import io.netty.channel.local.LocalAddress;

public class Main {
    public static void main(String [] args) throws InterruptedException, IOException {
        // NettyServerBuilder builder = NettyServerBuilder.forAddress(LocalAddress.ANY).forPort(57601)
        // .addService( new UserService() )
        // .addService( new MultiplayerService() );
        // Server server = builder.build();
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