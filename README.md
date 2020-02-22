# InterlinguaSpellchecker
MS Word Addin to Spellcheck Interlingua Language
=========================================================

Interlingua language spelling checker Addin for Microsoft Word 2010-2013 using NHunspell (https://sourceforge.net/projects/nhunspell/).

How to Contribute and Build
==============
With the Open Source Visual Studio Community Edition 2015, you can now contribute and add to the existing features of Interlingua Spell Checker for MS Word.

To start contributing, please install the following tools:

1. Visual Studio Community 2015: Download and Install Visual Studio Community 2015 from the below link. https://www.visualstudio.com/products/visual-studio-community-vs

2. Visual Studio 2015 Installer Project Extension: Download and Install the Visual Studio 2015 Installer Project Extension from the below link. https://visualstudiogallery.msdn.microsoft.com/f1cc3f3e-c300-40a7-8797-c509fb8933b9

3. Office Developer Tools: From the below link, download and install the Office Developer Tools by clicking on “2 Get Office Developer Tools” button. https://www.visualstudio.com/en-us/features/office-tools-vs.aspx

Then once these are installed, download the source by cloning in Desktop or downloading as a ZIP.

4. Once you have the source go to “InterlinguaSpellCheck\bin\Debug” directory and replace the “ia.aff” and “ia.dic” files. Please make sure you do not change the file names

5. Double-Click “InterlinguaSpellCheck.sln” file to open the project in Visual Studio 2015.

6. We now would need to rebuild each project in the solution explorer.

7. To rebuild a project, right-click on “InterlinguaSpellCheck” and click Rebuild. Repeat the steps for “InterlinguaSpellCheckSetup” and “InterlinguaSpellCheckSetup64”.

8. Finally, for 32-bit office, go to “InterlinguaSpellCheckSetup\Release” folder to get the setup files. And for 64-bit office, go to “InterlinguaSpellCheckSetup64\Release” folder to get new setup files.

License
=======
GNU GENERAL PUBLIC LICENSE v2

Special Thanks
==============
Thanks to Bohdan Šmilauer (b.smilauer@post.cz) for financing the initial addin development, as well as to Kunal Desai for his work on this project: https://www.freelancer.com/u/firmuscode.html (kunal@firmuscode.com)
