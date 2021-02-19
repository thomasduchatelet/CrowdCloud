motion
cd /var/lib/motion
while :
do
    files=(*)
    base64 ${files[0]} > image.txt
    curl -X POST -d @image.txt http://<ip:port>/api/facerecognitionhttp
    rm *.jpg -f
    sleep 1
done