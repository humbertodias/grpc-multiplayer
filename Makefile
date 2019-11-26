PROTOC_ARCH := macosx_x64
PROTOC_PATH := tools/$(PROTOC_ARCH)
CLIENT_DIR := client/gen 
SERVER_DIR := server/gen 

get-client-tools:
	wget https://packages.grpc.io/archive/2019/11/a30f2f95017bf0f53acf2a89056252eb3a2cbbab-b42eea7c-f904-45bf-aaef-5a1c7959c12c/csharp/Grpc.Tools.2.26.0-dev201911221120.nupkg \
	-O grpc-tools.zip && \
	unzip -o grpc-tools.zip "$(PROTOC_PATH)/*" && \
	rm -f grpc-tools.zip

get-server-tools:
	wget https://repo1.maven.org/maven2/io/grpc/protoc-gen-grpc-java/1.25.0/protoc-gen-grpc-java-1.25.0-osx-x86_64.exe \
	-O $(PROTOC_PATH)/grpc_java_plugin

protoc-chmod:
	find $(PROTOC_PATH) -type f -exec chmod +x {} \;

grpc-tools:	get-client-tools	get-server-tools	protoc-chmod

init:
	mkdir -p $(CLIENT_DIR) $(SERVER_DIR)

build-client: init
	$(PROTOC_PATH)/protoc proto/*.proto \
	-I proto \
	--csharp_out=$(CLIENT_DIR) \
	--grpc_out=$(CLIENT_DIR) \
	--plugin=protoc-gen-grpc=$(PROTOC_PATH)/grpc_csharp_plugin

build-server: init
	$(PROTOC_PATH)/protoc proto/*.proto \
	-I proto \
	--java_out=$(SERVER_DIR) \
	--grpc_out=$(SERVER_DIR) \
	--plugin=protoc-gen-grpc=$(PROTOC_PATH)/grpc_java_plugin

build:	build-client	build-server

clean:
	rm -rf tools