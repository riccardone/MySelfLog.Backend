FROM mono:latest

ADD src/MySelfLog.Host/bin/Release .
CMD [ "mono",  "MySelfLog.Host.exe" ]