# YMTEditor [WIP]
## Used to edit Codewalker exported *.ymt.xml (Peds clothes) files

**How it works:**
1. Make sure you have latest Codewalker downloaded (You can get it at codewalker discord in channel #releases)
2. Find your .ymt file in Codewalker RPF Explorer, right-click it and select "Export XML..."
3. Open exported file in YMTEditor
4. Edit it
    - You can add/remove new drawables to your current components 
    - You can add/remove textures to your drawables
    - **Renaming files is up to you! - if you have "000 001 002" drawables, and you remove 001, you have to manually change .ydd and .ytd file names, same goes with "a b c" textures**
    - Don't touch propMask/numAlternatives/clothData if you don't know what these do :
5. Save edited file (currently only overriding works)
6. Open Codewalker RPF Explorer, enable "Edit mode", right-click in any folder and select "Import XML..."

**Known bugs:**
  - **No props editing AT ALL - saving .ymt with props will delete all props**
  - Components drop-down is not implemented - you can't add/remove components currently
  - clothData drop-down is not implemented - .ymt will be saved with "false" value

_My coding skills are not greatest but it works 😛_
