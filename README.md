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

                         https://purrnet.gitbook.io/docs/getting-started/getting-started

    (p2p for meta is depreciated as of 2023)







\# Fix the simulator

Under Preferences go to Meta XR Simulator and select version 78 from the available versions. All higher versions crash.





\# Problems

* Headsets save the room information differently, with different wall orders and furniture identities (such as one being called a table vs one being a thing)

  * saved room info has different orientation
* when a headset starts it sets itself as the center of the universe (0,0,0 looking "north")
* rooms are moved based on real world trackers and estimation of location in headset for real world

  * this movement (rotation/translation) is different per headset and different per saved room
* cannot send the entire room info from one person to another

  * they be too big
  * even exported json is too large for complex rooms





\# Methodologies tried

* Trilateration

  * making a circle from 3 points to determine aproximite location
  * Failed because each headset thinks they are at 0,0,0 (so it always returns that)
  * https://math.stackexchange.com/questions/100448/finding-location-of-a-point-on-2d-plane-given-the-distances-to-three-other-know
  * https://math.stackexchange.com/questions/884807/find-x-location-using-3-known-x-y-location-using-trilateration
* Point set registration with SVD

  * Finds rotation and position approximation between two different datasets (can fill in holes)
  * Failed (some rooms are mirrored, and since headsets dont save direction the same we can get flipped or rotated locations)

    * Still not perfect
* Triangle Method

  * Make a known direction with a triangle between floor and 2 walls
  * Failed (mystery bullshit)
* Direct angles

  * Get a known direction to a known wall and then set all local spaces to the same position and rotation
  * Failed (idk why)

