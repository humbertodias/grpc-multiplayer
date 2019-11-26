OS := linux
#OS := macosx
ARCH := x64
PROTOC_PATH := tools/$(OS)_$(ARCH)
CLIENT_DIR := client-unity/Assets/Scripts/Grpc 
CLIENT_ASSETS_DIR := client-unity/Assets/
SERVER_DIR := server/gen 

# https://packages.grpc.io/archive/2019/11/a30f2f95017bf0f53acf2a89056252eb3a2cbbab-b42eea7c-f904-45bf-aaef-5a1c7959c12c/index.xml

get-client-tools:
	wget https://packages.grpc.io/archive/2019/11/a30f2f95017bf0f53acf2a89056252eb3a2cbbab-b42eea7c-f904-45bf-aaef-5a1c7959c12c/csharp/Grpc.Tools.2.26.0-dev201911221120.nupkg \
	-O grpc-tools.zip && \
	unzip -o grpc-tools.zip "$(PROTOC_PATH)/*" && \
	rm -f grpc-tools.zip

get-client-unity:
	wget https://packages.grpc.io/archive/2019/11/a30f2f95017bf0f53acf2a89056252eb3a2cbbab-b42eea7c-f904-45bf-aaef-5a1c7959c12c/csharp/grpc_unity_package.2.26.0-dev.zip \
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

clean:
	rm -rf tools
	rm -rf $(CLIENT_ASSETS_DIR)/Plugins