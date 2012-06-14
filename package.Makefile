
all: package

package : clean
	# ---- copy the augmented/new angrybots demo files to the zip
	mkdir -p dist/zip
	cd Assets;zip -r ../dist/zip/roarsdk-angrybots.zip Demo
	cd Assets;zip -r ../dist/zip/roarsdk-angrybots.zip Scripts/Fx/LaserScope.*
	cd Assets;zip -r ../dist/zip/roarsdk-angrybots.zip Scripts/Misc/DemoControl.*
	#
	# ---- Unity AngryBots roar sdk asset overlay zip available at: dist/zip/roarsdk-angrybots.zip
	#
clean:
	# ---- clear out any existing zip
	rm -rf dist/zip
