import anharu.MultiplayGrpc.MultiplayImplBase;
import anharu.Multiplayer;
import io.grpc.stub.StreamObserver;

import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

public class MultiplayerService extends MultiplayImplBase {

    private UserService userService;

    private Map<String,Multiplayer.UserPosition> userPositions = new HashMap();

    public MultiplayerService(UserService userService){
        this.userService = userService;
    }

    @Override
    public void getUsers(Multiplayer.GetUsersRequest request, StreamObserver<Multiplayer.GetUsersResponse> responseObserver) {
        System.out.println("GetUsers:" + userPositions.entrySet().size());
        userPositions.entrySet().stream().forEach( userPositionEntry -> {
            Multiplayer.UserPosition user = userPositionEntry.getValue();
            Multiplayer.UserPosition userPosition = Multiplayer.UserPosition.newBuilder().setX(user.getX()).setY(user.getY()).setZ(user.getZ()).setId(user.getId()).build();
            responseObserver.onNext(Multiplayer.GetUsersResponse.newBuilder().addUsers(userPosition).build());
        });
        responseObserver.onCompleted();
    }

    @Override
    public StreamObserver<Multiplayer.SetPositionRequest> setPosition(StreamObserver<Multiplayer.SetPositionResponse> responseObserver) {
        return new StreamObserver<Multiplayer.SetPositionRequest>() {
            Multiplayer.UserPosition userPosition;
            @Override
            public void onNext(Multiplayer.SetPositionRequest request) {
                userPosition = Multiplayer.UserPosition.newBuilder().setId(request.getId()).setX(request.getX()).setY(request.getY()).setZ(request.getZ()).build();
                userPositions.put(userPosition.getId(),userPosition);
                Multiplayer.SetPositionResponse setPositionResponse = Multiplayer.SetPositionResponse.newBuilder().setId(request.getId()).setStatus("ok").build();
                responseObserver.onNext(setPositionResponse);
            }

            @Override
            public void onError(Throwable t) {

            }

            @Override
            public void onCompleted() {
                responseObserver.onCompleted();
            }
        };
    }

    @Override
    public StreamObserver<Multiplayer.ConnectPositionRequest> connectPosition(StreamObserver<Multiplayer.ConnectPositionResponse> responseObserver) {
        return new StreamObserver<Multiplayer.ConnectPositionRequest>() {
            @Override
            public void onNext(Multiplayer.ConnectPositionRequest request) {
                System.out.printf("connectPosition.count %s x:%s, y:%s, z:%s - id:%s\n", userPositions.size(), request.getX(), request.getY(), request.getZ(), request.getId());
                Multiplayer.UserPosition userPosition = Multiplayer.UserPosition.newBuilder().setId(request.getId()).setX(request.getX()).setY(request.getY()).setZ(request.getZ()).build();
                userPositions.put(userPosition.getId(),userPosition);
                userPositions.values().forEach( p -> {
                    Multiplayer.ConnectPositionResponse setPositionResponse = Multiplayer.ConnectPositionResponse.newBuilder().addUsers(p).build();
                    responseObserver.onNext(setPositionResponse);
                });
            }

            @Override
            public void onError(Throwable t) {
                responseObserver.onError(t);
            }

            @Override
            public void onCompleted() {
                responseObserver.onCompleted();
            }
        };
    }

}
