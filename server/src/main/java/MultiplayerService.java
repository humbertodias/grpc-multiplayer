import anharu.MultiplayGrpc.MultiplayImplBase;
import anharu.Multiplayer;
import io.grpc.stub.StreamObserver;

import java.io.Console;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.TimeUnit;
import java.util.LinkedHashSet;
import java.util.*;
import java.util.concurrent.*;

public class MultiplayerService extends MultiplayImplBase {

    // private UserService userService;

    // private Map<String,Multiplayer.UserPosition> userPositions = new HashMap();

    // public MultiplayerService(UserService userService){
    //     this.userService = userService;
    // }

    @Override
    public void getUsers(Multiplayer.GetUsersRequest request, StreamObserver<Multiplayer.GetUsersResponse> responseObserver) {
        System.out.println("GetUsers:");
        // userPositions.entrySet().stream().forEach( userPositionEntry -> {
        //     Multiplayer.UserPosition user = userPositionEntry.getValue();
        //     Multiplayer.UserPosition userPosition = Multiplayer.UserPosition.newBuilder().setX(user.getX()).setY(user.getY()).setZ(user.getZ()).setId(user.getId()).build();
        //     responseObserver.onNext(Multiplayer.GetUsersResponse.newBuilder().addUsers(userPosition).build());
        // });
        Multiplayer.UserPosition userPosition = Multiplayer.UserPosition.newBuilder().setId("XX").setX(1f).build();
        responseObserver.onNext(Multiplayer.GetUsersResponse.newBuilder().addUsers(userPosition).build());
        responseObserver.onCompleted();
    }


        //an hashset to store all the streams which the server uses to communicate with each client

    LinkedHashSet<StreamObserver<Multiplayer.SetPositionResponse>> observers = new LinkedHashSet<>();

    @Override
    public  StreamObserver<Multiplayer.SetPositionRequest> setPosition(final StreamObserver<Multiplayer.SetPositionResponse> responseObserver) {
        String id = UUID.randomUUID().toString();
        System.out.printf("%s - %s\n", responseObserver, id);
       // observers.add(responseObserver);
        
        return new StreamObserver<Multiplayer.SetPositionRequest>() {

            //receiving a message from a specific client
            public void onNext(Multiplayer.SetPositionRequest request) {
                //for(StreamObserver<Multiplayer.SetPositionResponse> o : observers){
                    System.out.println("onNext:" + request);
                    responseObserver.onNext(Multiplayer.SetPositionResponse.newBuilder().setId(id).setStatus("ok").build());
                //}
            }

            //if there is an error (client abruptly disconnect) we remove the client.
            public void onError(Throwable t) {
                System.out.println("onError:" + t);
               // observers.remove(responseObserver);
                responseObserver.onError(t);
            }

            //if the client explicitly terminated, we remove it from the hashset.
            public void onCompleted() {
                System.out.println("onCompleted:" + responseObserver);
               // observers.remove(responseObserver);
                responseObserver.onCompleted();
            }
            
        };
    }

    LinkedHashSet<StreamObserver<Multiplayer.ConnectPositionResponse>> observers2 = new LinkedHashSet<>();
    List<Multiplayer.UserPosition> users = new ArrayList<>();
    @Override
    public synchronized StreamObserver<Multiplayer.ConnectPositionRequest> connectPosition(StreamObserver<Multiplayer.ConnectPositionResponse> responseObserver) {
        String id = UUID.randomUUID().toString();
        System.out.printf("%s - %s\n", responseObserver, id);
        observers2.add(responseObserver);

        users.add( Multiplayer.UserPosition.newBuilder().setId(users.size() + "").setX(1).setY(2).setZ(3).build() );

        return new StreamObserver<Multiplayer.ConnectPositionRequest>() {

            //receiving a message from a specific client
            public void onNext(Multiplayer.ConnectPositionRequest request) {
                
                observers2.forEach( o-> {
                    System.out.println("onNext:" + request);
                    Multiplayer.ConnectPositionResponse.Builder builder = Multiplayer.ConnectPositionResponse.newBuilder();

                    users.forEach( u -> {
                        builder.addUsers(u);     
                    });

                    o.onNext(builder.build());
                } );
            }

            //if there is an error (client abruptly disconnect) we remove the client.
            public void onError(Throwable t) {
                System.out.println("onError:" + t);
                observers2.remove(responseObserver);
                responseObserver.onError(t);
            }

            //if the client explicitly terminated, we remove it from the hashset.
            public void onCompleted() {
                System.out.println("onCompleted:" + responseObserver);
                observers2.remove(responseObserver);
            }
            
        };
    }

}
