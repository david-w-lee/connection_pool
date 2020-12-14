# Connection Pool



## Summary

* This project is about creating your own DB Connection Pool for MySql Database.



## How to Set up Environment

* Do `dotnet run` in the main folder to start the API server.
* You need MySql Database. Run sampledb.sql to create DB and table.
* Change config in UserController.cs to match your setting.
* You need JMeter for testing. Use connection_pool_text.jmx.



## PoolTypes

* There are two Pool Types:
  * Cached: This mode caches your connection after use if connection pool has less than specified pool size. It only creates a new connection when connection is not available in the queue.
  * Fixed: This mode fills up the connection pool with the specified amount of connections.
  * You can set Config.PoolSize to 0 to not use any connection pool.

