#!/bin/sh
 
# Replacing text in sentinel.conf file
# /g to replace all the patterns from the nth occurrence of a pattern in a line
sed -i "s/\$SENTINEL_QUORUM/$SENTINEL_QUORUM/g" /redis/sentinel.conf
sed -i "s/\$SENTINEL_DOWN_AFTER/$SENTINEL_DOWN_AFTER/g" /redis/sentinel.conf
sed -i "s/\$SENTINEL_FAILOVER/$SENTINEL_FAILOVER/g" /redis/sentinel.conf

# Run Sentinel 
redis-server ./sentinel.conf --sentinel