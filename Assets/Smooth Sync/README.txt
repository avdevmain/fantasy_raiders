
## PUN and PUN2
## Step 1 - Drag and Drop

1. Put the SmoothSync script onto any networked object that you want to be smoother. 
2. Drag the object's Smooth Sync component to the Photon View's Observed Components. 
3. It is now smoothy synced across the network. 
4. This applies to children as well, each child should have their own Photon View and Smooth Sync component.

## Step 2 - Tweak to Your Needs

Now that it is on your networked object:

1. Read the tooltips corresponding to the variables to tweak the smoothness to your game's
specific needs. More detailed comments are in the code comments for the variables. 
2. Reduce your bandwidth by only sending position, rotation, and velocity variables that you need.


# How it Works

Unlike the NetworkTransform script provided by Unity, the SmoothSync script stores a list of network States to interpolate 
between. This allows for a more accurate and smoother representation of synced objects.

# Methods you may need

Check out SmoothSyncExamplePlayerController.cs and SmoothSyncExample.unity for implementations and further explanation of these:

SmoothSync.teleport() - Used to teleport your objects for situations like respawning so that they don't interpolate.
SmoothSync.forceStateSendNextFixedUpdate() - Useful if you have a low send rate but you want to manually send a transform
in between the send rate. 
SmoothSync.clearBuffer() - You will need to manually call this on all of the object's instances if you change the object's network owner.
Alternatively, check Smooth Authority Changes (UNet only) to use an extra byte and handle it automatically and smoothly.
*When using Mirror you must call AssignAuthorityCallback() on the server after changing authority.*
SmoothSync.validateStateMethod - You may set up a validation method to check incoming States to see if cheating may be happening. 


Don't hesitate to contact us with any problems, questions, or comments.
With Love,
Noble Whale Studios