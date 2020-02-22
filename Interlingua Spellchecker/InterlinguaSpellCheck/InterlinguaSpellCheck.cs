using Microsoft.Office.Tools.Ribbon;
using NHunspell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace InterlinguaSpellCheck
{
    public partial class InterlinguaSpellCheck
    {
        //The word application
        Word.Application wordApp;
        List<IgnoreWord> ignoreWords = new List<IgnoreWord>();
        List<IgnoreWord> ignoreAllWords = new List<IgnoreWord>();
        char[] InterlinguaCharacters = new char[] {
            '\u0041', '\u0042', '\u0043', '\u0044','\u0045','\u0046','\u0047','\u0048','\u0049','\u004A','\u004B','\u004C','\u004D','\u004E','\u004F',
            '\u0050', '\u0051', '\u0052', '\u0053','\u0054','\u0055','\u0056','\u0057','\u0058','\u0059','\u005A',
            '\u0061', '\u0062', '\u0063', '\u0064','\u0065','\u0066','\u0067','\u0068','\u0069','\u006A','\u006B','\u006C','\u006D','\u006E','\u006F',
            '\u0070', '\u0071', '\u0072', '\u0073','\u0074','\u0075','\u0076','\u0077','\u0078','\u0079','\u007A'};

        char[] InterlinguaPunctuationsAndControls = new char[] {
            '\u0000', '\u0001', '\u0002', '\u0003','\u0004','\u0005','\u0006','\u0007','\u0008','\u0009','\u000A','\u000B','\u000C','\u000D','\u000E', '\u000F',
            '\u0010', '\u0011', '\u0012', '\u0013','\u0014','\u0015','\u0016','\u0017','\u0018','\u0019','\u001A','\u001B','\u001C','\u001D','\u001E', '\u001F',
            '\u0020', '\u0021', '\u0022', '\u0023','\u0024','\u0025','\u0026','\u0027','\u0028','\u0029','\u002A','\u002B','\u002C','\u002D','\u002E', '\u002F',
            '\u0030', '\u0031', '\u0032', '\u0033','\u0034','\u0035','\u0036','\u0037','\u0038','\u0039','\u003A','\u003B','\u003C','\u003D','\u003E', '\u003F',
            '\u0040',
            '\u005B', '\u005C', '\u005D', '\u005E','\u005F',
            '\u0060',
            '\u007B', '\u007C', '\u007D', '\u007E','\u007F'};

        private void InterlinguaSpellCheck_Load(object sender, RibbonUIEventArgs e)
        {
            //Get the word application object
            wordApp = Globals.ThisAddIn.Application;
        }

        private void btnSpellCheck_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                //Exit if there is no active document
                if (wordApp.ActiveDocument == null)
                    return;

                Word.Range originalSel = null;
                try
                {
                    //Get all the words from the active document
                    Word.Document doc = wordApp.ActiveDocument;
                    originalSel = wordApp.ActiveWindow.Selection.Range;
                    
                    #region Locate Dictionary files
                    //Get the deployment directory
                    System.Reflection.Assembly assemblyInfo = System.Reflection.Assembly.GetExecutingAssembly();

                    //Location is where the assembly is run from 
                    string assemblyLocation = assemblyInfo.Location;

                    //CodeBase is the location of the ClickOnce deployment files
                    Uri uriCodeBase = new Uri(assemblyInfo.CodeBase);
                    string InstallationLocation = Path.GetDirectoryName(uriCodeBase.LocalPath.ToString());

                    //Interlingua dictionaries
                    string affFile = Path.Combine(InstallationLocation, "ia.aff");
                    string dictFile = Path.Combine(InstallationLocation, "ia.dic");
                    #endregion

                    //Loop through all the words to find the first mis-spell
                    using (Hunspell hunspell = new Hunspell(affFile, dictFile))
                    {
                        //Process all Paragraphs in the documents
                        Object oMissing = System.Reflection.Missing.Value;
                        object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdLine; // change a line; 
                        object moveExtend = Microsoft.Office.Interop.Word.WdMovementType.wdExtend;

                        doc.ActiveWindow.Selection.HomeKey(Word.WdUnits.wdStory, ref oMissing);

                        foreach (Word.Paragraph para in doc.Paragraphs)
                        {
                            para.Range.Select();

                            //Set the repeatcheck and stopcheck
                            bool repeatcheck, stopcheck;

                            //Check Interlingua Check
                            do
                            {
                                string selectedText = doc.ActiveWindow.Selection.Text;
                                string[] Interlinguawords = GetInterlinguaWords(selectedText);

                                CheckWords(doc, hunspell, selectedText, Interlinguawords, out stopcheck, out repeatcheck);

                                //Break if user selects to exit
                                if (stopcheck) return;
                            }
                            while (repeatcheck);
                        }

                        var shapes = doc.Shapes;
                        //Finds text within textboxes, then changes them
                        foreach (Microsoft.Office.Interop.Word.Shape shape in shapes)
                        {
                            shape.Select();

                            //Set the repeatcheck and stopcheck
                            bool repeatcheck, stopcheck;

                            //Check Interlingua Check
                            do
                            {
                                //Get the selected text and Interlingua words
                                string selectedText = shape.TextFrame.TextRange.Text;
                                string[] Interlinguawords = GetInterlinguaWords(selectedText);

                                CheckWords(doc, hunspell, selectedText, Interlinguawords, out stopcheck, out repeatcheck, "shape", shape);

                                //Break if user selects to exit
                                if (stopcheck) return;
                            }
                            while (repeatcheck);
                        }


                        MessageBox.Show("Interlingua Spelling Check is complete", "Interlingua Spell Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                finally
                {
                    originalSel.Select();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CheckWords(Word.Document doc, Hunspell hunspell, string selectedText, string[] Interlinguawords, out bool stopcheck, out bool repeatcheck, string objecttype = "", object wordobject = null)
        {
            int startposition = 0;
            Object oMissing = System.Reflection.Missing.Value;
            stopcheck = repeatcheck = false;

            //Check all the Interlingua words from the selected line
            foreach (string Interlinguaword in Interlinguawords)
            {
                DialogResult dialogResult = DialogResult.None;
                frmInterlingua frmInterlingua = null;
                String newInterlinguaWord = String.Empty;

                if (!hunspell.Spell(Interlinguaword))
                {
                    if (!ignoreAllWords.Any(ignoreAllWord => ignoreAllWord.Interlinguaword == Interlinguaword))
                    {
                        if (!ignoreWords.Contains(new IgnoreWord { document = doc.Name, Interlinguaword = Interlinguaword, selectedText = selectedText, startposition = startposition, ignoreAll = false }))
                        {
                            Word.Range start = null;
                            Word.WdColorIndex highlightcolorindex = Word.WdColorIndex.wdNoHighlight;
                            Word.WdUnderline fontunderline = Word.WdUnderline.wdUnderlineNone;
                            Word.WdColor fontcolor = Word.WdColor.wdColorBlack;
                            Word.Range selectionRange = null;

                            //Select the erroneous word on the main document
                            if (String.IsNullOrWhiteSpace(objecttype))
                            {
                                //Set the initial selection
                                start = doc.ActiveWindow.Selection.Range;

                                //Set the search area
                                doc.ActiveWindow.Selection.Start += startposition;
                                Word.Selection searchArea = doc.ActiveWindow.Selection;

                                //Set the find object
                                Word.Find findObject = searchArea.Find;
                                findObject.ClearFormatting();
                                findObject.Text = Interlinguaword;


                                //Find the mis-spelled word
                                findObject.Execute(ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                                //Temp store the current formatting
                                highlightcolorindex = doc.ActiveWindow.Selection.Range.HighlightColorIndex;
                                fontunderline = doc.ActiveWindow.Selection.Range.Font.Underline;
                                fontcolor = doc.ActiveWindow.Selection.Range.Font.UnderlineColor;

                                //Highlight the selection
                                doc.ActiveWindow.Selection.Range.HighlightColorIndex = Word.WdColorIndex.wdYellow;
                                doc.ActiveWindow.Selection.Range.Font.Underline = Word.WdUnderline.wdUnderlineWavy;
                                doc.ActiveWindow.Selection.Range.Font.UnderlineColor = Word.WdColor.wdColorRed;
                                selectionRange = doc.ActiveWindow.Selection.Range;
                                doc.ActiveWindow.Selection.Collapse();
                            }
                            else
                            {
                                if (objecttype == "table")
                                {
                                    start = ((Word.Cell)wordobject).Range;
                                }
                                else if (objecttype == "shape")
                                {
                                    start = ((Word.Shape)wordobject).TextFrame.TextRange;
                                    start.Start += startposition;
                                }

                                //Set the find object
                                Word.Find findObject = start.Find;
                                findObject.ClearFormatting();
                                findObject.Text = Interlinguaword;

                                //Temp store the current formatting
                                highlightcolorindex = start.HighlightColorIndex;
                                fontunderline = start.Font.Underline;
                                fontcolor = start.Font.UnderlineColor;

                                //Find the mis-spelled word
                                findObject.Execute(ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                                //Highlight the selection
                                start.HighlightColorIndex = Word.WdColorIndex.wdYellow;
                                start.Font.Underline = Word.WdUnderline.wdUnderlineWavy;
                                start.Font.UnderlineColor = Word.WdColor.wdColorRed;
                                start.Select();
                            }

                            bool isObject = !String.IsNullOrWhiteSpace(objecttype);
                            frmInterlingua = new frmInterlingua(selectedText, Interlinguaword, startposition, hunspell.Suggest(Interlinguaword), isObject);
                            dialogResult = frmInterlingua.ShowDialog();

                            //Select the line again
                            if (String.IsNullOrWhiteSpace(objecttype))
                            {
                                //Revert the highlights
                                selectionRange.Select();
                                doc.ActiveWindow.Selection.Range.HighlightColorIndex = highlightcolorindex;
                                doc.ActiveWindow.Selection.Range.Font.Underline = fontunderline;
                                doc.ActiveWindow.Selection.Range.Font.UnderlineColor = fontcolor;

                                if (dialogResult != DialogResult.Abort) start.Select();
                            }
                            else
                            {
                                start.HighlightColorIndex = highlightcolorindex;
                                start.Font.Underline = fontunderline;
                                start.Font.UnderlineColor = fontcolor;

                                if (dialogResult != DialogResult.Abort)
                                {
                                    if (objecttype == "table")
                                    {
                                        ((Word.Cell)wordobject).Select();
                                    }
                                    else if (objecttype == "shape")
                                    {
                                        ((Word.Shape)wordobject).Select();
                                    }
                                }
                            }
                        }
                    }
                }

                #region Cancel Button Clicked
                //Return if the user hits Cancel Button
                if (dialogResult == DialogResult.Cancel || dialogResult == DialogResult.Abort)
                {
                    stopcheck = true;
                    repeatcheck = false;
                    return;
                }
                #endregion

                #region Ignore or Ignore All Clicked
                //Ignore the word
                if (dialogResult == DialogResult.Ignore)
                {
                    if (frmInterlingua.ignoreAll)
                    {
                        ignoreAllWords.Add(new IgnoreWord { Interlinguaword = Interlinguaword, ignoreAll = frmInterlingua.ignoreAll });
                    }
                    else
                    {
                        ignoreWords.Add(new IgnoreWord { document = doc.Name, Interlinguaword = Interlinguaword, selectedText = selectedText, startposition = startposition });
                    }
                }
                #endregion

                #region Change or Change All Clicked
                if (dialogResult == DialogResult.Yes)
                {
                    if (String.IsNullOrWhiteSpace(objecttype))
                    {
                        //Set the initial selection
                        Word.Range start = doc.ActiveWindow.Selection.Range;

                        //Set the searcharea
                        if (frmInterlingua.changeAll)
                        {
                            doc.Content.Select();
                        }
                        Word.Selection searchArea = doc.ActiveWindow.Selection;

                        //Set the find object
                        Word.Find findObject = searchArea.Find;
                        findObject.ClearFormatting();
                        findObject.Text = Interlinguaword;
                        findObject.Replacement.ClearFormatting();
                        findObject.Replacement.Text = frmInterlingua.selectedSuggestion;

                        object replaceAll = frmInterlingua.changeAll ? Word.WdReplace.wdReplaceAll : Word.WdReplace.wdReplaceOne;

                        findObject.Execute(ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref replaceAll, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                        newInterlinguaWord = frmInterlingua.selectedSuggestion;

                        //Set back the selection
                        start.Select();

                        //Set repeatcheck to true
                        if (frmInterlingua.changeAll)
                        {
                            stopcheck = false;
                            repeatcheck = true;
                            return;
                        }
                    }
                    else
                    {
                        var resultingText = selectedText.Replace(Interlinguaword, frmInterlingua.selectedSuggestion);

                        if (objecttype == "table")
                        {
                            Word.Range range = ((Word.Cell)wordobject).Range;
                            range.Text = resultingText;
                        }
                        else if (objecttype == "shape")
                        {
                            Word.Shape shape = (Word.Shape)wordobject;
                            shape.TextFrame.TextRange.Text = resultingText;
                        }

                        stopcheck = false;
                        repeatcheck = true;
                        return;
                    }
                }
                #endregion

                startposition += ((String.IsNullOrWhiteSpace(newInterlinguaWord) ? Interlinguaword.Length : newInterlinguaWord.Length) + 1);
            }
        }

        private string[] GetInterlinguaWords(string selectedText)
        {
            string[] Interlinguawords =
                selectedText
                    .Split(new char[] { '\u200B', '\u200C', ' ', '\r', '\a', '.', '\t', '\v' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(Interlinguaword => Interlinguaword.Trim(InterlinguaPunctuationsAndControls))
                    .Where(Interlinguaword => String.IsNullOrWhiteSpace(Interlinguaword) == false)
                    .Where(Interlinguaword => InterlinguaCharacters.Contains(Interlinguaword[0]))
                    .ToArray();
            return Interlinguawords;
        }
    }
}
