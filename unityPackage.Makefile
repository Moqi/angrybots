
all: package

package : clean
	# ---- create the empty unity project
	/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -createProject dist/package/roario-angrybots -logFile dist/package/1-create-project.log
	#
	# ---- copy the augmented/new angrybots demo files to the project
	cp -v -r Assets/Demo dist/package/roario-angrybots/Assets/Demo
	mkdir -p dist/package/roario-angrybots/Assets/Scripts/Fx/
	cp Assets/Scripts/Fx/LaserScope.js dist/package/roario-angrybots/Assets/Scripts/Fx/
	mkdir -p dist/package/roario-angrybots/Assets/Scripts/Misc/
	cp Assets/Scripts/Misc/DemoControl.cs dist/package/roario-angrybots/Assets/Scripts/Misc/	
	#
	# ---- export the Assets from the project
	/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath $(CURDIR)/dist/package/roario-angrybots/ -exportPackage Assets AngryBots-RoarIO.unityPackage -logFile dist/logs/export-assets.log
	# put the unity package in an easy to find location
	mv AngryBots-RoarIO.unityPackage dist/AngryBots-RoarIO.unityPackage
	#
	# ---- Package complete!
	# ---- Unity package available at dist/RoarIO.unityPackage
	#
clean:
	# ---- clear out any existing package generation files
	rm -rf dist/package/*
