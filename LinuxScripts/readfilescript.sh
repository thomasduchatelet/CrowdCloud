#!/bin/bash
while :
do
    grep -A 10000 -P '(?<=Station MAC)' monitorfiles-01.csv > newfile.csv
    sed -i '$d' newfile.csv
    length=`wc -l < newfile.csv`
    while [ $length  -gt 1 ]
    do
        station=$(tail -1 newfile.csv | awk -F, '{print $1}')
        power=$(tail -1 newfile.csv | awk -F, '{print $4}')
        echo $station
        echo $power
        sed -i '$d' newfile.csv
        length=`wc -l < newfile.csv`
        curl --data "$station&$power" http://<ip:port>/api/wifipowermonitor
    done
    sleep 5
    rm newfile.csv
done