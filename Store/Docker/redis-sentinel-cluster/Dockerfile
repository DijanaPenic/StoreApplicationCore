FROM redis:6

# The quorum is the number of Sentinels that need to agree about the fact the master is not reachable, in order to really mark the master as failing, and eventually start a failover procedure if possible.
ENV SENTINEL_QUORUM 2
ENV SENTINEL_DOWN_AFTER 1000
ENV SENTINEL_FAILOVER 1000

# Create configuration directory
# -p is short for --parents - it creates the entire directory tree up to the given directory.
RUN mkdir -p /redis 
WORKDIR /redis

COPY sentinel.conf .
COPY entrypoint.sh /usr/local/bin/

# chown - change owner (redis user and group redis:redis)
# chmod + x - allow executable permissions
RUN chown redis:redis ./* && \
    chmod +x /usr/local/bin/entrypoint.sh
 
EXPOSE 26379
 
ENTRYPOINT ["entrypoint.sh"]