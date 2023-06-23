[Init fastlane
	fastlane init
]

Install HomeBrew
	/usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"

Install fastlane
	/usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"

Install bundle
	sudo bundle install
	sudo bundle update

Install badge
	brew install gdbm
	brew link gdbm
	brew install librsvg
	brew install imagemagick
	sudo gem install badge

Run file build:
	[chmod +x BuildXcode.sh]
	bash BuildXcode.sh

More at:
https://docs.fastlane.tools/getting-started/ios/setup/



