FROM nginx:latest

RUN apt-get update && apt-get install -y build-essential libssl-dev git curl

WORKDIR /usr/src
RUN git clone https://github.com/Refinitiv/nginx-sticky-module-ng.git
WORKDIR /usr/src/nginx-sticky-module-ng
RUN git checkout master

WORKDIR /usr/src
RUN curl -O http://nginx.org/download/nginx-1.20.1.tar.gz
RUN tar -xvzf nginx-1.20.1.tar.gz

RUN apt-get install -y libpcre3 libpcre3-dev zlib1g-dev

WORKDIR /usr/src/nginx-1.20.1
RUN ./configure --add-dynamic-module=../nginx-sticky-module-ng
RUN make modules
RUN cp objs/ngx_http_sticky_module.so /etc/nginx/modules

WORKDIR /usr/src
RUN rm -rf nginx-sticky-module-ng
RUN rm -rf nginx-1.20.1
RUN apt-get clean
