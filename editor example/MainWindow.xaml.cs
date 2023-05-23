using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Navigation;
using editor_example;
using System.IO.Pipes;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Xml.Linq;
using System.Windows.Controls.Primitives;
using System.Text.RegularExpressions;

namespace WpfTutorialSamples.Rich_text_controls
{
    public partial class RichTextEditorSample : Window
    {
        string sFileName = "";
        string copied_text = "";
        public RichTextEditorSample()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            cmbFontSize.Text = temp.ToString();
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "Text Format (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Open);

                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

                range.Load(fileStream, DataFormats.Rtf);

            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Format (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create);

                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

                range.Save(fileStream, DataFormats.Rtf);

            }
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
        }

        private void About_btn_Click(object sender, RoutedEventArgs e)
        {
            About_window about_Window = new About_window();
            about_Window.Show();
        }
        private void Create(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Format (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                sFileName = saveFileDialog.FileName;
            }
        }

        private void Auto_save(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(sFileName);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream stream = new MemoryStream(byteArray);
            File.WriteAllText(sFileName, DataFormats.Rtf);
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            rtbEditor.SelectAll();

            rtbEditor.Selection.Text = "";
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            rtbEditor.Copy();
        }

        private void Cut(object sender, RoutedEventArgs e)
        {
            rtbEditor.Cut();
        }
        private void Paste(object sender, RoutedEventArgs e)
        {
            rtbEditor.Paste();
        }

        private void rtbEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextPointer tp1 = rtbEditor.Selection.Start.GetLineStartPosition(0);
            TextPointer tp2 = rtbEditor.Selection.Start;

            int column = tp1.GetOffsetToPosition(tp2);

            int someBigNumber = int.MaxValue;
            int lineMoved, currentLineNumber;
            rtbEditor.Selection.Start.GetLineStartPosition(-someBigNumber, out lineMoved);
            currentLineNumber = -lineMoved;
            TextRange textRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

            MatchCollection wordColl = Regex.Matches(textRange.Text, @"[\W]+");

            count_label.Content = "Line: " + currentLineNumber.ToString() + " Column: " + column.ToString() + " Symbols: " + ((textRange.Text.Length - 2 * currentLineNumber) - 2).ToString() + " Words: "+ wordColl.Count;
        }
    }
}