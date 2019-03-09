# FastDFSNetCore

## Nuget Packages

| Package | NuGet Stable | Downloads |
| ------- | ------------ | --------- | 
| [FastDFSNetCore](https://www.nuget.org/packages/FastDFSNetCore/) | [![FastDFSNetCore](https://img.shields.io/nuget/v/FastDFSNetCore.svg)](https://www.nuget.org/packages/FastDFSNetCore/) | [![StackExchange.Redis](https://img.shields.io/nuget/dt/FastDFSNetCore.svg)](https://www.nuget.org/packages/FastDFSNetCore/) |

## Basic Usage
see https://github.com/caozhiyuan/FastDFSNetCore/blob/master/src/FastDFS.Test/Program.cs

## Ngx_fastdfs_module


``` nginx
server {
          listen       80;
 
          location / {
            root   /path/to/fdfsstore/fdfs_storage/data;
          }
 
          location /group1/M00 {
            root /path/to/fdfsstore/fdfs_storage/data;
            ngx_fastdfs_module;                   
         }  
          
         location ~* /group1/M00/(.*?)_([0-9]+)X([0-9]+)\.(jpeg|jpg|gif|png)$ {
           root /path/to/fdfsstore/fdfs_storage/data;       
           set $h $2;
           set $w $3;
           rewrite /group1/M00/(.*?)_([0-9]+)X([0-9]+)\.(jpeg|jpg|gif|png)$ /$1.$4 break;
           image_filter_buffer 10M;
           image_filter resize $h $w;                 
        }        
}
```
