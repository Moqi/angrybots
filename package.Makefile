
all: package

package : clean
	#
	# ---- export the Assets from the project
	# TODO: update project path!!!
	/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath /Users/fma/git/angrybots -exportPackage Assets AngryBots-RoarIO.unityPackage -logFile dist/logs/export-assets.log
	# put the unity package in an easy to find location
	mv AngryBots-RoarIO.unityPackage dist/AngryBots-RoarIO.unityPackage
	#
	# ---- Package complete!
	# ---- Unity package available at dist/RoarIO.unityPackage
	#
clean:
	# ---- clear out any existing package generation files
	rm -rf dist/package/*
