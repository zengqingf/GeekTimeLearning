# encoding=utf8
import os

# os.environ['FLASK_PYPI_PROXY_CONFIG']='/data1/pypi/server.conf' #配置文件路径
os.environ['PYPI_PROXY_BASE_FOLDER_PATH'] = r'packages'  #包的存放地址
# os.environ['PYPI_PROXY_LOGGING_PATH'] = r'server.log' #日志文件
os.environ['PYPI_PROXY_PYPI_URL'] = 'https://pypi.tuna.tsinghua.edu.cn/' #pypi源地址，这里用douban的
os.environ['PYPI_PROXY_LOGGING_LEVEL'] = 'WARN' #日志等级
from flask_pypi_proxy.views import app

app.run(host='0.0.0.0', port=8080, debug=False)  #监听地址和端口

if __name__ == '__main__':
    pass