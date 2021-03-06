﻿using System;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelFindAndReplace {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            InitStuff();
            
        }

        public void InitStuff() {
            openFileDialog1.Filter = "Excel XLSX (*.xlsx)|*.xlsx|" + "Excel All (*.xlsx;*.xls;*.xlsm)|*.xlsx;*.xls;*.xlsm";
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }

        //Preparation

        private void toolStripLabel1_Click(object sender, EventArgs e) {
            AddFiles();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            AddFiles();
        }

        //Add files to ListView
        public void AddFiles() {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                foreach (string file in openFileDialog1.FileNames) {
                    ListViewItem item = new ListViewItem(new[] { file });
                    listView1.Items.Add(item);
                }
                toolStripStatusLabel1.Text = "Files added: " + openFileDialog1.FileNames.Length.ToString();
            }
        }

        //Press Del to remove selected items
        private void listView1_KeyDown(object sender, KeyEventArgs e) {
            if (Keys.Delete == e.KeyCode) {
                statusLabel_Del();
                foreach (ListViewItem item in listView1.SelectedItems) {
                    listView1.Items.Remove(item);
                }  
            }
        }

        //Press Remove button in toolstrip to remove items
        private void toolStripLabel2_Click(object sender, EventArgs e) {
            statusLabel_Del();
            foreach (ListViewItem item in listView1.SelectedItems) {
                listView1.Items.Remove(item);
            }
        }

        public void statusLabel_Del() => toolStripStatusLabel1.Text = "Files removed: " + listView1.SelectedItems.Count.ToString();

        //Execution

        string findText = "";
        string replaceText = "";
        Excel.XlLookAt lookat;
        Excel.XlSearchOrder searchOrder;
        bool matchCase;

        public void SetVariables() {
            findText = textBox1.Text;
            replaceText = textBox2.Text;
            lookat = (comboBox1.SelectedIndex == 0) ? Excel.XlLookAt.xlPart : Excel.XlLookAt.xlWhole;
            searchOrder = (comboBox2.SelectedIndex == 0) ? Excel.XlSearchOrder.xlByRows : Excel.XlSearchOrder.xlByColumns;
            matchCase = checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e) {
            SetVariables();
            if (findText == "" || listView1.Items.Count == 0) 
                toolStripStatusLabel1.Text = "Error: No text entered or no files added.";
            else {
                foreach (ListViewItem file in listView1.Items)
                    FindAndReplace(file.Text);
            }
        }

        public void FindAndReplace(string file) {
            toolStripStatusLabel1.Text = "Editing " + file;

            Excel.Application excelApp = new Excel.Application() { Visible = false };
            Excel.Workbook wb = excelApp.Workbooks.Open(file, ReadOnly: false);

            //Need to loop through worksheets and unprotect them all
            foreach (Excel.Worksheet sheet in wb.Worksheets) {
                sheet.Unprotect();
            }

            Excel.Worksheet ws = (Excel.Worksheet)wb.ActiveSheet;
            Excel.Range rnge = ws.UsedRange;

            bool success = rnge.Replace(
                What: findText,
                Replacement: replaceText,
                LookAt: lookat, 
                SearchOrder: searchOrder,
                MatchCase: matchCase
                );

            foreach (Excel.Worksheet sheet in wb.Worksheets) {
                //sheet.Protect();
            }

            toolStripStatusLabel1.Text = "Finished editing " + file;
            wb.Save();
            excelApp.Quit();
        }
    }
}
