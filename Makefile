#OS := linux
OS := macosx
ARCH := x64
PROTOC_PATH := tools/$(OS)_$(ARCH)
CLIENT_DIR := client-unity/Assets/Scripts/Grpc 
CLIENT_ASSETS_DIR := client-unity/Assets/
SERVER_DIR := server/gen 

# https://packages.grpc.io/archive/2019/12/a02d6b9be81cbadb60eed88b3b44498ba27bcba9-edd81ac6-e3d1-461a-a263-2b06ae913c3f/index.xml

get-client-tools:
	wget https://packages.grpc.io/archive/2019/12/a02d6b9be81cbadb60eed88b3b44498ba27bcba9-edd81ac6-e3d1-461a-a263-2b06ae913c3f/csharp/Grpc.Tools.2.26.0-dev201912021138.nupkg \
	-O grpc-tools.zip && \
	unzip -o grpc-tools.zip "$(PROTOC_PATH)/*" && \
	rm -f grpc-tools.zip

get-client-unity:
	wget https://packages.grpc.io/archive/2019/12/a02d6b9be81cbadb60eed88b3b44498ba27bcba9-edd81ac6-e3d1-461a-a263-2b06ae913c3f/csharp/grpc_unity_package.2.26.0-dev.zip \
	-O grpc-unity-package.zip && \
	unzip -o grpc-unity-package.zip -d $(CLIENT_ASSETS_DIR) && \
	rm -f grpc-unity-package.zip

get-server-tools:
	wget https://repo1.maven.org/maven2/io/grpc/protoc-gen-grpc-java/1.25.0/protoc-gen-grpc-java-1.25.0-$(OS)-x86_64.exe \
	-O $(PROTOC_PATH)/grpc_java_plugin

executable:
	find $(PROTOC_PATH) -type f -exec chmod +x {} \;

get-grpc:	get-client-tools	get-client-unity	get-server-tools	executable

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

clean: ## Honey baby
	rm -rf tools $(CLIENT_ASSETS_DIR)/Plugins


help: ## Display this help screen
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'
