
all: package

package : clean
	# ---- copy the augmented/new angrybots demo files to the zip
	mkdir dist/zip
	zip -r dist/zip/roario-angrybots.zip Assets/Demo
	zip -r dist/zip/roario-angrybots.zip Assets/Scripts/Fx/LaserScope.js
	zip -r dist/zip/roario-angrybots.zip Assets/Scripts/Misc/DemoControl.cs	
	#
	# ---- Package complete!
	# ---- Unity package available at dist/zip/RoarIO.unityPackage
	#
clean:
	# ---- clear out any existing package generation files
	rm -rf dist/package/angybots-roario.zip
