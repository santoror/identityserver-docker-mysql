# IdentityServer, ASP.NET Identity, Mysql, nginx on docker
## Configuration
Check out the project
Create a file named ```proxy.conf``` inside your host machine (mine is in ```c:\tmp\proxy.conf```)
Paste the following content
```
# HTTP 1.1 support
proxy_http_version 1.1;
proxy_buffering off;
proxy_set_header Host $http_host;
proxy_set_header Upgrade $http_upgrade;
proxy_set_header Connection $proxy_connection;
proxy_set_header X-Real-IP $remote_addr;
proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
proxy_set_header X-Forwarded-Proto $proxy_x_forwarded_proto;
proxy_set_header X-Forwarded-Ssl $proxy_x_forwarded_ssl;
proxy_set_header X-Forwarded-Port $proxy_x_forwarded_port;
proxy_buffers         8 16k;  # Buffer pool = 8 buffers of 16k
proxy_buffer_size     16k;    # 16k of buffers from pool used for headers
# Mitigate httpoxy attack (see README for details)
proxy_set_header Proxy "";
```
Edit ```.env``` file
```
ASPNETCORE_CONNECTION_STRING=Server=mysql-db;Uid=root;Pwd=identity!Password;Database=identity
ASPNETCORE_ENVIRONMENT=Development
NUGET_SOURCE=https://api.nuget.org/v3/index.json

MYSQL_ROOT_PASSWORD=identity!Password
MYSQL_ROOT_HOST=%

IDENTITY_VIRTUAL_HOST=identityserver.local
API_VIRTUAL_HOST=api.local
MVC_VIRTUAL_HOST=mvc.local
PROXY_CONF=c:\tmp\proxy.conf
```
Add to your ```host``` (```C:\Windows\System32\drivers\etc``` on **Windows**, ```/etc/hosts``` on **Linux**) files the following entries:
```
127.0.0.1 			identityserver.local
127.0.0.1 			mvc.local
127.0.0.1 			api.local
```
Run with command
```
docker-compose up
```