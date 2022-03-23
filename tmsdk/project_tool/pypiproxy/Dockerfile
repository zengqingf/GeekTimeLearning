FROM python:3.9-alpine

RUN echo -e 'https://mirrors.aliyun.com/alpine/v3.14/main\nhttps://mirrors.aliyun.com/alpine/v3.14/community' > /etc/apk/repositories

RUN rm -rf /var/cache/apk/*  && \
    apk add libmagic
RUN mkdir -p /pypi/packages
COPY . /pypi
RUN pip install --index-url http://mirrors.aliyun.com/pypi/simple/ --trusted-host mirrors.aliyun.com -r /pypi/requirements.txt

VOLUME [ "/pypi/packages" ]
WORKDIR /pypi
EXPOSE 8080
CMD [ "python","run.py" ]
