FROM ubuntu:latest

WORKDIR /root

COPY ./concorde ./concorde

#ENTRYPOINT ["tail", "-f", "/dev/null"]
CMD ./concorde -x -o $OUTPUT_FILE $INPUT_FILE 