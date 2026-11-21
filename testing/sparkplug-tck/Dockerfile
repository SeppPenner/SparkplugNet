# syntax=docker/dockerfile:1

FROM alpine:3.22 AS tck
WORKDIR /opt/tck
ADD --checksum=sha256:a70b2c2f00d67ac714eadd5ac50f6241e0efa26036d95ca8ec667d491021b86b \
    --chmod=644 https://download.eclipse.org/sparkplug/3.0.0/Eclipse-Sparkplug-TCK-3.0.0.zip ./Eclipse-Sparkplug-TCK-3.0.0.zip
RUN unzip -q Eclipse-Sparkplug-TCK-3.0.0.zip \
    && unzip -q SparkplugTCK/hivemq-extension/sparkplug-tck-3.0.0.zip -d extensions
CMD ["sh", "-ec", "mkdir -p /output/data /output/log; chmod g+w /output/data /output/log; cp Eclipse-Sparkplug-TCK-3.0.0.zip SparkplugTCK/report.py /output/"]

FROM hivemq/hivemq-ce:2026.5 AS broker
COPY --from=tck /opt/tck/extensions/sparkplug-tck/ /opt/hivemq/extensions/sparkplug-tck/
WORKDIR /opt/hivemq/log

FROM node:22-bookworm-slim AS web-console
WORKDIR /opt/tck-webconsole
COPY --from=tck /opt/tck/SparkplugTCK/webconsole/ ./
# The release declares Yarn 3 but ships a Yarn Classic lockfile.
RUN npm pkg delete packageManager && yarn install --frozen-lockfile --non-interactive
# The bundled Webpack 4 build requires the legacy OpenSSL provider.
RUN NODE_OPTIONS=--openssl-legacy-provider yarn build
ENV HOST=0.0.0.0
USER node
CMD ["yarn", "start"]
