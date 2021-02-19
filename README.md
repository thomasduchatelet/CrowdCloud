
# Use a Raspberry Pi to monitor a crowd using Microsoft Azure Facial recognition, Kali Linux and…

Photo by Jeff Loucks on Unsplash

## **About**

This project was part of the 8th International Smart Cities Intensive Programme 2021 in collaboration with students from Universidad Politécnica de Cartagena, Karelia University Of Applied Sciences, InHolland hogeschool and Hogeschool Gent. The **goal **of this project is to deliver a **proof of concept** for an IoT security solution during a one week hackathon.

<iframe src="https://medium.com/media/5d815a15879211ee33e9d2a4abd8d41e" frameborder=0></iframe>

## The project

In this project, we use a Raspberry Pi to monitor a crowd’s density, emotion, age and gender. We take a picture with the webcam when motion is detected, then we use [Microsoft Azure facial recognition](https://azure.microsoft.com/en-us/services/cognitive-services/face/) to get several attributes from the detected faces in the image. We also scan the surroundings for devices that have their WiFi option on. If you set up two more devices, you could use this data to calculate the location of the nearby devices. This is not included in this project. The gathered data can be visualized in [Microsoft Power BI](https://powerbi.microsoft.com/nl-nl/).

![dataflow](https://cdn-images-1.medium.com/max/2000/1*Zxy2IOWCK2uIuINbe__lLw.png)*dataflow*

**Prerequesites**

* [Set up a Raspberry Pi](https://projects.raspberrypi.org/en/projects/raspberry-pi-setting-up/1) 4 with a [Kali Linux installation](https://www.kali.org/docs/arm/kali-linux-raspberry-pi/)

* A USB Webcam

* A cloned version of the [CrowdCloud Github repository](https://github.com/thomasduchatelet/CrowdCloud)

* [Microsoft Visual Studio](https://visualstudio.microsoft.com/)

**Step 1: Set up motion service**

Install motion service on Rasperry

    $ sudo apt update
    $ sudo apt-get install motion
    $ sudo nano /etc/motion/motion.conf

Adjust the following properties in the configuration file:

* daemon on

* framerate 1

* output_pictures on

Enable the motion daemon

    $ sudo nano /etc/default/motion

Paste the following line in the file

    start_moion_daemon=yes

Start the service

    $ sudo service motion restart
    $ sudo motion

Confirm the motion service is running and outputs pictures when motion is detected

    $ ls /var/lib/motion

**Step 2: Set up HTTP endpoints in Power Bi**

Go to [app.powerbi.com](https://app.powerbi.com/) and create a new dashboard

![Create new dashboard](https://cdn-images-1.medium.com/max/2000/1*ukkszSIbpf-YjQJi1D3KQw.png)*Create new dashboard*

Enter the name and create a new tile

![Add a tile](https://cdn-images-1.medium.com/max/2000/1*XqYU-s4hMPxy2kOGszFH4A.png)*Add a tile*

Select custom streaming data and click next

![Select custom streaming data](https://cdn-images-1.medium.com/max/2000/1*QVvvIKSLPGfv6fBs6Qpe7g.png)*Select custom streaming data*

Add a new dataset

![Add streaming dataset](https://cdn-images-1.medium.com/max/2000/1*UdTVuqF1KBWCXz-zZ8Abkg.png)*Add streaming dataset*

![Select Api](https://cdn-images-1.medium.com/max/2000/1*07-SPtWaVNc5Blsnyr8r4w.png)*Select Api*

Set the dataset columns and turn de hystorical analysis option on

![Face recognition columns](https://cdn-images-1.medium.com/max/2000/1*ASPb_xyGIC01xcWMS0FOhA.png)*Face recognition columns*

Copy the URL and save for later use

![copy Push URL](https://cdn-images-1.medium.com/max/2000/1*FlA1vqPk9Udn5e2QHSm4WA.png)*copy Push URL*

Repeat the steps above and create a new dataset for the wifi monitor output

![WiFi monitor columns](https://cdn-images-1.medium.com/max/2000/1*VN1JyxzVP5-fDgCZILC4RQ.png)*WiFi monitor columns*

**Step 2: Set up HTTP endpoints to recieve the data:**

1. Open the Visual Studio project in [the AzureFunctions folder of the repository.](https://github.com/thomasduchatelet/CrowdCloud/tree/master/AzureFunctions)

1. Fill in the subscriptionkey and the URL of the face recognition dataset in the FaceRecognitionHttp.cs file

1. Fill in the URL of the WiFi monitor dataset in the FaceRecognition.cs file

1. Run the FaceRecognitionHttp project

1. Open another window of Visual studio and run the WiFiPowerMonitor project

**Step 3: Edit the linux scripts**

You can find the scripts in the [LinuxScripts folder of the repository.](https://github.com/thomasduchatelet/CrowdCloud/tree/master/LinuxScripts)

Fill in the ip address of the computer that is running the functions in the imagescript.sh and the readfilescript.sh

**Step 4: Run the scripts**

1. Open 3 different terminals at once to run de scripts simultaneously.

1. Run the monitorscript.sh, imagescript.sh, and readfilescript.sh as root

    $ sudo bash <script name>

**Now you can create new tiles in Power BI and see the magic happen!**

## The team

Our team consisted of 5 members, each with a different profession:

* Thomas Duchatelet (Applied Informatics, Hogent)

* Jose Antonio Hernández Solano (Telematics Engineering, UP de Cartagena)

* Nashwa Madanee (Safety & Security Management, InHolland)

* Rabina Hunnoman (Safety & Security Management, InHolland)

* Joonas Jaskari (Karelia University Of Applied Sciences)
