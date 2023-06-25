using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace Text_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static String openFilePath = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        // new
        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            txtEditor.Document.Blocks.Clear();
            openFilePath = "";
        }

        // new window
        private void NewWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void NewWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
        }

        // open
        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            txtEditor.Document.Blocks.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtEditor.Document.Blocks.Add(new Paragraph(new Run(File.ReadAllText(openFileDialog.FileName))));
                openFilePath = openFileDialog.FileName;
            }
        }
        // save
        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (openFilePath != "")
            {
                File.WriteAllText(openFilePath, new TextRange(txtEditor.Document.ContentStart, txtEditor.Document.ContentEnd).Text);
            }
        }

        // save as
        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, new TextRange(txtEditor.Document.ContentStart, txtEditor.Document.ContentEnd).Text);
        }

        // print
        private void PrintCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if ((pd.ShowDialog() == true))
            {
                pd.PrintDocument((((IDocumentPaginatorSource)txtEditor.Document).DocumentPaginator), "printing as paginator");
            }
        }

        // exit
        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // predefined commands
        // paste
        // cut 
        // copy
        // undo
        // delete
        // select all

        // find
        private void FindCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void FindCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectWord(txtSearch.Text, LogicalDirection.Forward);
        }

        // find next
        private void FindNextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void FindNextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectWord(txtSearch.Text, LogicalDirection.Forward);
        }

        // find previous
        private void FindPreviousCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void FindPreviousCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectWord(txtSearch.Text, LogicalDirection.Backward);
        }


        // buttons

        // open button
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            txtEditor.Document.Blocks.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtEditor.Document.Blocks.Add(new Paragraph(new Run(File.ReadAllText(openFileDialog.FileName))));
                openFilePath = openFileDialog.FileName;
            }
        }

        // save button
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (openFilePath != "")
            {
                File.WriteAllText(openFilePath, new TextRange(txtEditor.Document.ContentStart, txtEditor.Document.ContentEnd).Text);
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                    File.WriteAllText(saveFileDialog.FileName, new TextRange(txtEditor.Document.ContentStart, txtEditor.Document.ContentEnd).Text);
            }
        }

        // find button
        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            SelectWord(txtSearch.Text, LogicalDirection.Forward);
        }

        // find next button
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            SelectWord(txtSearch.Text, LogicalDirection.Forward);
        }

        // find previous button
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            SelectWord(txtSearch.Text, LogicalDirection.Backward);
        }

        // select word with txtSearch
        private void SelectWord(string input, LogicalDirection direction)
        {
            RichTextBox rtb = txtEditor;

            TextPointer currentStartposition = rtb.Selection.Start;
            TextPointer currentEndposition = rtb.Selection.End;
            TextPointer position;
            TextPointer previousPosition;
            string textLine = null;

            if (direction == LogicalDirection.Forward)
            {
                position = currentStartposition.GetLineStartPosition(1);
                previousPosition = currentEndposition;
                if (position != null)
                    textLine = new TextRange(previousPosition, position).Text;
            }
            else
            {
                position = currentStartposition.GetLineStartPosition(0);
                previousPosition = currentStartposition;
                if (position != null)
                    textLine = new TextRange(position, previousPosition).Text;
            }

            while (position != null)
            {
                int indexInRun;
                if (direction == LogicalDirection.Forward)
                    indexInRun = textLine.IndexOf(input, StringComparison.CurrentCultureIgnoreCase);
                else
                    indexInRun = textLine.LastIndexOf(input, StringComparison.CurrentCultureIgnoreCase);

                if (indexInRun >= 0)
                {
                    TextPointer nextPointer = null;
                    if (direction == LogicalDirection.Forward)
                        position = previousPosition;

                    int inputLength = input.Length;
                    while (nextPointer == null)
                    {
                        if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text && nextPointer == null) //checks to see if textpointer is actually text
                        {
                            string textRun = position.GetTextInRun(LogicalDirection.Forward);
                            if (textRun.Length - 1 < indexInRun)
                                indexInRun -= textRun.Length;
                            else //found the start position of text pointer
                            {
                                position = position.GetPositionAtOffset(indexInRun);
                                nextPointer = position;
                                while (inputLength > 0)
                                {
                                    textRun = nextPointer.GetTextInRun(LogicalDirection.Forward);
                                    if (textRun.Length - indexInRun < inputLength)
                                    {
                                        inputLength -= textRun.Length;
                                        indexInRun = 0; //after the first pass, index in run is no longer relevant
                                    }
                                    else
                                    {
                                        nextPointer = nextPointer.GetPositionAtOffset(inputLength);
                                        rtb.Selection.Select(position, nextPointer);
                                        rtb.Focus();

                                        //moves the scrollbar to the selected text
                                        Rect r = position.GetCharacterRect(LogicalDirection.Forward);
                                        double totaloffset = r.Top + rtb.VerticalOffset;
                                        rtb.ScrollToVerticalOffset(totaloffset - rtb.ActualHeight / 2);
                                        return; //word is selected and scrolled to. Exit method
                                    }
                                    nextPointer = nextPointer.GetNextContextPosition(LogicalDirection.Forward);
                                }
                            }
                        }
                        position = position.GetNextContextPosition(LogicalDirection.Forward);
                    }
                }

                previousPosition = position;
                if (direction == LogicalDirection.Forward)
                {
                    position = position.GetLineStartPosition(1);
                    if (position != null)
                        textLine = new TextRange(previousPosition, position).Text;
                }
                else
                {
                    position = position.GetLineStartPosition(-1);
                    if (position != null)
                        textLine = new TextRange(position, previousPosition).Text;
                }
            }

            //if next/previous word is not found, leave the current selected word selected
            rtb.Selection.Select(currentStartposition, currentEndposition);
            rtb.Focus();
        }

    }
}
