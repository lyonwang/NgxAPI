# /etc/systemd/system/ngxapi.service for CentOS7
```
[Unit]
Description=Nginx API for White List
DefaultDependencies=no
 
[Service]
Type=simple
RemainAfterExit=no
ExecStart=/usr/share/dotnet/dotnet run &
WorkingDirectory=/home/pgadminlyon/NgxAPI/NgxAPI
User=root

[Install]
```

# Start service
```bash
sudo systemctl start ngxapi
```

# 取得白名單
## 接口: http://10.10.1.101:5555/api/nginx/[apilive/notify]
## HTTP 方法: GET JSON
## 取得範例: http://10.10.1.101:5555/api/nginx/notify
```
{
    "domain": "notify",
    "ips": [
        {
            "ip": "47.52.164.48",
            "desc": "OneMoneyPay"
        },
        {
            "ip": "211.104.160.6",
            "desc": "HTP"
        },
        {
            "ip": "210.108.146.198",
            "desc": "HTP"
        },
        {
            "ip": "47.75.125.148",
            "desc": "GStar"
        },
        {
            "ip": "120.78.178.248",
            "desc": "FlyPay"
        },
        {
            "ip": "103.107.237.136",
            "desc": "AmmaPay"
        },
        {
            "ip": "47.75.72.2",
            "desc": "NewPay"
        },
        {
            "ip": "103.40.165.14",
            "desc": "WavePay"
        },
        {
            "ip": "210.66.102.250",
            "desc": "WavePay"
        },
        {
            "ip": "112.73.6.202",
            "desc": "LP18890700937"
        }
    ]
}
```
# 設定白名單
## 接口: http://10.10.1.101:5555/api/nginx
## HTTP 方法: POST JSON
## 取得範例: notify
```
{
    "domain": "notify",
    "ips": [
        {
            "ip": "47.52.164.48",
            "desc": "OneMoneyPay"
        },
        {
            "ip": "211.104.160.6",
            "desc": "HTP"
        },
        {
            "ip": "210.108.146.198",
            "desc": "HTP"
        },
        {
            "ip": "47.75.125.148",
            "desc": "GStar"
        },
        {
            "ip": "120.78.178.248",
            "desc": "FlyPay"
        },
        {
            "ip": "103.107.237.136",
            "desc": "AmmaPay"
        },
        {
            "ip": "47.75.72.2",
            "desc": "NewPay"
        },
        {
            "ip": "103.40.165.14",
            "desc": "WavePay"
        },
        {
            "ip": "210.66.102.250",
            "desc": "WavePay"
        },
        {
            "ip": "112.73.6.202",
            "desc": "LP18890700937"
        }
    ]
}
```