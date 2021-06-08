# YMTEditor [WIP]
## Used to edit *.ymt OR *.ymt.xml - (Peds clothes) files

### How it works: ###
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
  - expressionMods in component and prop section is not editable as i couldn't figure it how to do it (currently all P_HEAD props are saved with "-0.5 0 0 0 0", so if you are adding any head prop that shouldn't hide hair, you have to change that manually in .ymt)
  - Components drop-down is not implemented - you can't add/remove components currently (for example: you can't add jbib if you don't have it in your .ymt already)
  - clothData drop-down is not implemented - .ymt will be saved with "false" value

_My coding skills are not greatest but it works 😛_
