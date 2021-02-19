#!/bin/bash
iw phy phy0 interface add mon0 type monitor
ifconfig mon0 up
rm *.csv
airodump-ng mon0 -w monitorfiles --write-interval 5 -o csv