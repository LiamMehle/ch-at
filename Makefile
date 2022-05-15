all: run

# build is a proxy file for the build as it could have none or a .exe extension

run: build
	dotnet run

build: 
	dotnet build

