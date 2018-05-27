cd $PSScriptRoot
$nuget_packages = $HOME+'/.nuget/packages'
$grpc_tools = $nuget_packages+'/grpc.tools/1.12.0/tools/windows_x64'
$protobuf_tools = $nuget_packages+'/google.protobuf.tools\3.5.1\tools'
.$grpc_tools/protoc.exe -I $protobuf_tools -I $PSScriptRoot --csharp_out $PSScriptRoot EventStream.proto --grpc_out $PSScriptRoot --plugin=protoc-gen-grpc=$grpc_tools\grpc_csharp_plugin.exe