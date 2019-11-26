import anharu.Multiplayer;
import anharu.UserGrpc;
import io.grpc.stub.StreamObserver;

import java.util.*;

public class UserService extends UserGrpc.UserImplBase {

    private Map<String, String> users = new HashMap<>();

    @Override
    public void create(Multiplayer.CreateUserRequest request, StreamObserver<Multiplayer.CreateUserResponse> responseObserver) {
        String uuid = users.getOrDefault(request.getName(), UUID.randomUUID().toString());
        users.putIfAbsent(request.getName(), uuid);
        Multiplayer.CreateUserResponse response = Multiplayer.CreateUserResponse.newBuilder().setId(uuid).build();
        responseObserver.onNext(response);
        responseObserver.onCompleted();
    }

    public Map<String, String> getUsers() {
        return users;
    }

}
