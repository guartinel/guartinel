FROM debian:jessie
# MAINTAINER gua

# Set timezone
RUN echo "UTC" > /etc/timezone && \
    dpkg-reconfigure --frontend noninteractive tzdata

# Create a default user
#RUN useradd automation --shell /bin/bash --create-home

RUN apt-get -yqq update && \
    apt-get -yqq install libssl-dev libunwind8 icu-devtools && \
    rm -rf /var/lib/apt/lists/*

ADD . /app
CMD ["/app/Guartinel.Service.WebsiteChecker"]
