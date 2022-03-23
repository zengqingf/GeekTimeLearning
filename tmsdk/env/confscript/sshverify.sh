tmp=`expect -v`
if [ "$?" != "0" ]; then
    echo 不存在expect
    exit 1
fi

if [ ! -f ~/.ssh/id_rsa ];then
    expect -c "spawn ssh-keygen
    expect {
        \"*Enter file*\" { 
            send \"\r\" 
            exp_continue 
        }
        \"*Enter passphrase*\" { 
            send \"\r\" 
            exp_continue 
        }
        \"*Enter same passphrase again*\" { 
            send \"\r\" 
            exp_continue 
        }
    }
    expect eof
    "
fi
if [ ! -f ~/.ssh/id_rsa ];then
    echo 创建key失败
    exit 1
fi


PASSWD=$1
if [ "x$2" == "x" ] 
then
    KEY="id_rsa.pub"
else 
    KEY=$2
fi


for host in `cat allremote`
do
    cd ~/.ssh

    expect -c "set timeout 10
    spawn ssh-copy-id -i ${KEY} $host
    expect {
        \"*yes/no*\" { 
            send \"yes\r\" 
            exp_continue 
        }
        \"*assword:*\" { 
            send \"$PASSWD\r\"
            exp_continue 
        }

    }
    expect eof
    "
done
