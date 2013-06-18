# Roar Unity SDK: Angrybots demo

This demo repository uses the well known Unity AngryBots demo, which has been fully integrated with **Roar Unity SDK**.

It *also* allows to play in a multiplayer mode using **[Photon Cloud](http://cloud.exitgames.com/)** and it is now possible to invite friends, accept invitations, delete friends and purchasing items.

This AngryBots version takes full advantage of both Photon and Roar SDKs.

## Demo

This demo is literally just that, a demo. It's rough, unfinished, and designed as a *starting point* for you to get a handle on how to integrate the Roar SDK into your game. Couple things to try using the SDK widgets:

- Create a **new player** account
- Buy a new laser sight from the **shop**
- Consume the `super speed` from your **inventory**

There's a ton more, so take it for a spin here:

**[http://roarengine.com/unity/demo](http://roarengine.com/unity/demo/)**


## Project Setup

First grab a copy of the [Roar SDK package](http://github.com/roarengine/sdk-unity/Roar.unityPackage).

- Create a fresh project and **import** the Roar SDK package
- Add the following scenes to your build settings:
    - `Scenes/0_Preloader`
    - `Scenes/1_MainMenu`
    - `Scenes/2_AngryBotsMP`
    - `Scenes/3_Endscene`
- Click **Build**
    - Note: You can repeat this step to Build several different versions. In particular, on OS X it will enable you to run two or more instances of the game at the same time
- **Photon server:** Setup **PUN** (see the PUN wizard or the Photon provided [Readme](https://github.com/unityosgt/source/blob/master/Assets/Photon%20Unity%20Networking/readme.txt)). You can either host your own server or use the free Photon Cloud trial
- **Run** the `Preloader` or `MainMenu` scenes

## Multiplayer mode

You can play two players at the same time on one machine using two separate instances of the game. On OS X you will need two (or more) different builds to do this.

After starting the game you will be presented with two widgets:

1. **Roar SDK login** - create a new user or log in with a previously created player
2. **Photon multiplayer mode widget**
    - On the first instance of the game click `GO` to create a new room.
    - On the second instance click `JOIN`.

At this moment you should be able to see two players in each instance of the game. Once you login using Roar's Widget, each of the game characters will be labelled correctly with their login name.

You can now enjoy the multi-player game and use the Roar's widgets to invite friends, check invites, accept invitations, delete friend and purchase and activate items.
