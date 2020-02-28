FROM ubuntu:latest
ARG DEBIAN_FRONTEND=noninteractive

RUN \
    apt-get update -y && \
    apt-get install -y curl wget tar git python build-essential maven openjdk-11-jdk make g++ && \
    curl -sL https://deb.nodesource.com/setup_12.x | bash - && \
    apt-get install -y nodejs && \
    rm -rf /var/lib/apt/lists/* && \
    mkdir -p /app

ENV JAVA_HOME=/usr/lib/jvm/java-11-openjdk-amd64
ENV LD_LIBRARY_PATH=/usr/lib/jvm/java-11-openjdk-amd64/lib/server

WORKDIR /app
COPY . .
RUN \
    find . -type f -exec chmod 644 {} \; && \
    find . -type d -exec chmod 755 {} \; && \
    mvn clean package && \
    apt-get purge -y maven openjdk-11-jdk make g++ && \
    apt-get install openjdk-11-jre && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app/target
CMD [ "node", "index.js" ]