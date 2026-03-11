# Masters-MR-Quest-Capstone



This is my Masters of Engineering capstone project for CS at University of Cincinnati.

This project is focused around the concept of having multiple Mixed Reality (MR) headsets active in the same room at the same time. This however is not a straightforward solution as MR, especially multiuser MR is not a common thing as of writing. Additionally, most MR systems/APIs are out of date, depreciated, or poorly documented. This project has required extensive reverse engineering and research in order to find suitable information in order to build this project. Below is all of the information, tutorials, and updates I have needed in order to create a working solution.



Due to lack of available headsets capable of room/scene meshing I am utilizing the Quest series. (Specifically this has been tested on the Quest Pro and Quest 3)



\# SHIT DOCKS

https://developers.meta.com/horizon/downloads/package/meta-xr-core-sdk/





\# Setups

Start:                   https://developers.meta.com/horizon/documentation/unity/unity-tutorial-hello-vr/

Passthrough:             https://developers.meta.com/horizon/documentation/unity/unity-mr-utility-kit-gs/

Basic Scene setup from:  https://www.youtube.com/watch?v=3sVgwPxR4TE

P2P:                     https://codedimmersions.gitbook.io/mirrorvr/manual/getting-started

&nbsp;                        https://purrnet.gitbook.io/docs/getting-started/getting-started

    (p2p for meta is depreciated as of 2023)







\# Fix the simulator

Under Preferences go to Meta XR Simulator and select version 78 from the available versions. All higher versions crash.

