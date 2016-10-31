# Publish Demo

1. npm start
2. open localhost:3000/stream
3. start pikaRelay.py
4. start pikaSender.py

High frequency data will be published by pikaSender. 
pikaRelay receives the raw stream and cache them, another thread will publish latest snapshot to node server periodically.
node server receive the snapshot message and push to Web client via WebSocket.
