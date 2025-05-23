#!/bin/sh

ENV_FILE=/usr/share/nginx/html/env-config.js

echo "window._env_ = {" > $ENV_FILE
echo "  API_URL: \"${API_URL}\"," >> $ENV_FILE
echo "}" >> $ENV_FILE

nginx -g 'daemon off;'
