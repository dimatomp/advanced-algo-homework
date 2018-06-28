with import <nixos-18.03> {};
stdenv.mkDerivation rec {
    name = "advanced-algo";
    env = buildEnv { name = "advanced-algo-env"; paths = buildInputs; };
    buildInputs = [ vscode dotnet-sdk mono54 ];
}
