# YMTEditor
## Used to edit *.ymt OR *.ymt.xml - (Peds clothes) files
### Using [Codewalker.Core](https://github.com/dexyfex/CodeWalker) by @dexyfex
### Download YMTEditor [here](https://github.com/grzybeek/YMTEditor/releases) ###
### [Tutorial](https://forum.cfx.re/t/how-to-stream-clothes-and-props-as-addons-for-mp-freemode-models/3345474) ###
### [Second tutorial](https://forum.cfx.re/t/how-to-create-addon-heels-or-hide-hair-with-addon-hat/4209989) ###

#### How it works: ####
* __New *.YMT__
	- Click File > New
	- Add Components/Props by your choice
	- Edit it
	- Save it to *.YMT (or *.YMT.XML and import with Codewalker RPF Explorer)

* __Import/Export *.YMT.XML__
    - Make sure you have latest Codewalker downloaded (You can get it at codewalker discord in channel #releases)
    - Find your .ymt file in Codewalker RPF Explorer, enable "Edit mode", right-click it and select "Export XML..."
    - Open exported file in YMTEditor
    - Edit it
        - You can add/remove new drawables to your current components and props
        - You can add/remove textures to your drawables
        - **Renaming files is up to you! - if you have "000 001 002" drawables, and you remove 001, you have to manually change .ydd and .ytd file names, same goes with "a b c" textures**
        - Don't touch propMask/numAlternatives/clothData and component/prop properties if you don't know what these do
    - Save edited file to XML (or YMT and you can skip next step)
    - Open Codewalker RPF Explorer, enable "Edit mode", right-click in any folder and select "Import XML..."

* __Import/Export *.YMT__
    - Find your .ymt file in Codewalker RPF Explorer, copy it to any folder
    - Open .ymt file in YMTEditor
    - Edit it
        - You can add/remove new drawables to your current components and props
        - You can add/remove textures to your drawables
        - **Renaming files is up to you! - if you have "000 001 002" drawables, and you remove 001, you have to manually change .ydd and .ytd file names, same goes with "a b c" textures**
        - Don't touch propMask/numAlternatives/clothData and component/prop properties if you don't know what these do
    - Save edited file to YMT

**Known bugs:**
  - clothData drop-down is not implemented - .ymt will be saved with "false" value

_My coding skills are not greatest but it works :P_
