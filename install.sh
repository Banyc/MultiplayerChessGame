#!/bin/bash

mkdir $HOME/opt/chess-game-server
unzip -o MultiplayerChessGame.Server.linux-arm.zip -d $HOME/opt/chess-game-server
chmod +x $HOME/opt/chess-game-server/start.sh
chmod +x $HOME/opt/chess-game-server/MultiplayerChessGame.Server
cd $HOME/opt/chess-game-server
docker-compose build
