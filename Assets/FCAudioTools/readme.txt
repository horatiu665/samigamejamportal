----------------------------------------------
       Future Cartographer Audio Tools
 Copyright (C) 2015-2017 Future Cartographer
               Version 2.2.1
      http://www.futurecartographer.com/
        support@futurecartographer.com
----------------------------------------------

Hi there, thank you for using FC Audio Tools!

Documentation can be found here: http://www.futurecartographer.com/fcaudiotools-documents

If you have any questions, suggestions, comments or feature requests, please
drop by the FC Audio Tools forum, found here: http://forum.unity3d.com/threads/fc-audio-tools.342001/

If you like this package, please give it a review/rating in the AssetStore
as it will help me to improve the tools.


-----------
Wave Editor
-----------
Wave Editor provides the raw level editing of audio files to you.
You can edit such as cut/copy/paste.

If from your sound assets want to cut the silence part, you can use this.
If the volume of the sound assets who you want to use is not match, you can use this.
If you want to embed a loop point in your assets, you can use this.

To try it out.
1) Show WaveEditor tab. (Choose menu Window -> FCAudioTools -> Wave Editor)
2) Drop file to edit from project tab to the Wave Editor window.
3) Use the WaveEditor to the editing of the waveform.
4) Choose to save (overwrite or new file).


--------------
Level Adjuster
--------------
Are you troubled by the sound effects and music volume is incorrect?
Sound experts in your team, might have a different work.
Do not worry even in such a case. To drop the file you want to edit to Level Adjuster window.
And, while listening to each sound, moving the volume bar.
The sound is immediately able to preview.
I believe that it is difficult to adjust to listen?
The tool adds RMS-based analysis capabilities.

To try it out.
1) Show WaveEditor tab. (Choose menu Window -> FCAudioTools -> Level Adjuster)
2) Drop files to adjust from project tab to the Level Adjuster window.
3) Change sound level by the slider or using analyze.
4) Choose to save (all overwrite).


------------
Why tool"s"?
------------
I have plans to offer you the other tools in the future.
All of these will be included to the package of FC Audio Tools.
It will work on Windows, Mac OS X and Linux.

However, the FC Audio Tools is not a tool for the only sound professionals.
Developer beginners and hobbyist is, is to provide a package that can be safely processing the audio.
So, if there is that you are in trouble in the sound, please tell me whether it to me.


------------
Contact info
------------
For bugs, feature requests and a friendly chat, you can send an email to:
support@futurecartographer.com
http://www.futurecartographer.com/aboutme

I will always answer as soon as possible but please keep in mind that I am just one guy with a normal day job.


---------------------
Unity's version issue
---------------------

5.2.0 - 5.3.2
The crash problem of the editor by playback and script compile was fixed in Unity5.3.2p4.
FC Audio Tools does not recommend the use between Unity5.2.0 and 5.3.2.
Please use Unity5.3.3 or later. 

5.1.x
You will see that the undo histories remains after close the window.
This is because undoClear has a problem on 5.1.x.
You do not encount this problem on 5.2.0 or later.


---------------
Asset Uninstall
---------------
If you want to uninstall the FCAudioTools, please delete the 'Assets/FCAudioTools' directory.


---------------
Version History
---------------

2.2.1

General
- FIX: GUI skins appearance in Unity5.6
- FIX: Compile error of sample scripts in Unity5.6
- FIX: Console warning message in Unity5.5 and Unity5.6


2.2.0

General
- FIX: Console warning message in Unity5.5

Wave Editor
- IMP: When click on the marker label, the name change dialog is displayed.
- IMP: Change mouse cursor on SampleLoop, Marker's label and message.


2.1.0
General
- CHG: All settings move to Preferences menu. (Choose menu Edit -> Preferences -> FC Audio Tools)
- ADD: Open facebook page.

Wave Editor
- NEW: Support editing undo/redo

Level Adjuster
- FIX: Shared audioclip between Level Adjuser and Wave Editor


2.0.1
General
- IMP: Edited clip was memory release. Improved script compile time after the window closed.

Level Adjuster
- FIX: When lost focus does not transition to continue playback.


2.0.0
General
- NEW: Level Adjuster Tool.
- NEW: New skin.
- ADD: Accept a load of broken wav file.
- ADD: Send to FCAudioTools to context menu.
- IMP: Reduced memory footprint when AudioClip loading.
- IMP: Reduced CPU usage. (re-draw and timing optimization)
- FIX: LoopMusicDemo scene objects/scripts.
- FIX: If all of the marker texts is empty, an broke wav-save.

Wave Editor
- NEW: Editing of multiple files.
- ADD: Channel mute for preview.
- FIX: Export AnimationClip-float parameter is 0 reserved.
- CHG: Disable auto layout of selection status box.
- CHG: Drop file to edit.

Level Adjuster
- NEW: First release.


1.2.1
General
- ADD: Event callback demo that is synchronized with the AnimationClip and AudioClip.
- FIX: Remove an empty name label from wav file. (output file size reduction.)
- FIX: Size calculation of the wav file that contains many markers.
- FIX: If all of the marker texts is empty, an wav-open exception occurs.
- CHG: Sub dialogs close at reload editor assemblies.

Wave Editor
- ADD: Export markers to AnimationClip. (File > Export > Markers > AnimatonClip)
- ADD: Export markers to Json. (File > Export > Markers > Json)
- ADD: Import markers from Json. (File > Import > Markers > Json)
- ADD: Snap the Selection and cursor to the marker position (Disable temporary by the shift key)
- FIX: Optimized markers rendering.
- FIX: Toolbar buttons status at preview.


1.2.0
General
- NEW: Audio Mixer Demo(Link to Audio Mixer files).
- ADD: Create/AudioClip/(mono - 8ch) to context menu.
- CHG: Change Demo sub directories in FCAudioTools.

Wave Editor
- NEW: Link to Audio Mixer.
- CHG: Preview doesn't stop by the lost focus and preview halt by script compile.


1.1.0
General
- FIX: Editing data broken at script re-compile.

Wave Editor
- NEW: Selection per channel.
- ADD: Cut/Copy/Paste between channels.
- ADD: Support to processing per channel.
- ADD: Selection triming.
- FIX: Sometimes no display cue markers.
- FIX: When the script reload, waveform zoom level is reset.
- FIX: Marker display after the movement.
- FIX: Selection drag to outside the WaveEditor window.
- CHG: Processing the selection channels to work.(Cut/Copy/Paste/Playback sound/All Process)
- DEL: Removed menu "Paste Special/Overwrite". (Replaced new Paste)


1.0.2
General
- NEW: Add Create/AudioClip to context menu.
- FIX: Does not work in the FCAudioTools directories changed.
- FIX: Editing data broken at script re-compile.
- FIX: Static dependence of Assembly-CSharp dll file.
- CHG: FCAudioToolsPlayPreview gameobject is hide now.

Wave Editor
- ADD: Open files from audio clips object picker. Choose to File > Open.
- FIX: Unusual return of edits by the scripts reload.
- FIX: Audio Clip files path tracking. (move,rename,delete and etc)


1.0.1
Wave Editor
- ADD: Cut with removes selected waveform and places it on the clipboard.
- ADD: Del key to call the Cut.
- FIX: Cut/Copy with no selection.
- FIX: Apperance  of line width on Intel GPU and AMD GPU.
- FIX: When selecting the Process menu, waveform view position of change.
- FIX: Playback crash when the Sample Loop indicates the outside waveform. 
- FIX: Crash when waveform size is 0.
- FIX: Console error log of when you close the dialog. (only Mac OS X Editor)


1.0.0
- NEW: First release with WaveEditor.


