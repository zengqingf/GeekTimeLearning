source ~/.bash_profile

local_ip=`/sbin/ifconfig -a|grep inet|grep -v 127.0.0.1|grep -v inet6|awk '{print $2}'|tr -d "addr:"`

# rethinkdb --bind all --cache-size 8192 --http-port 8090

# brew services stop rethinkdb

# brew services start rethinkdb

#stf local --public-ip ${local_ip} --allow-remote --no-cleanup

#stf local --public-ip 192.168.2.112  --no-cleanup


# support ios devices

stf_bin=/Users/tenmove/_ExtendTools/STF/stf/bin/stf
webdriver_proj=/Users/tenmove/_ExtendTools/STF/_svn/STF/WebDriverAgent/

if [ ! -x ${stf_bin} ]; then
  chmod +x ${stf_bin}
fi

echo ${local_ip}

${stf_bin} local --public-ip ${local_ip} --allow-remote --no-cleanup --wda-path ${webdriver_proj} --wda-port 8100
