
ps aux | grep 192.168.2.6 | awk -f killautossh.ack

exit
autossh -M 5679 -CqTfnNR 10000:192.168.2.61:8080 root@121.41.17.47
autossh -M 5678 -CqTfnNR 8888:192.168.2.60:29418 root@121.41.17.47
