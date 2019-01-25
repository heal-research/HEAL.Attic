for %%i in ("*.proto") do (
  echo Processing %%i
  protoc-3.5.0 --proto_path="." "%%i" --csharp_out="."
)