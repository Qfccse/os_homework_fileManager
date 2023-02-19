using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public partial class TxtEditor : Form
    {
        private BitMap bitmap;
        public File txtFile;
        public DelegateMethod.delegateFunction CallBack;

        public TxtEditor()
        {
            InitializeComponent();
        }

        public TxtEditor(ref BitMap bitMap, ref File file)
        {
            InitializeComponent();
            bitmap = bitMap;
            txtFile = file;
            showContent();
        }

        public void showContent()
        {
            richTextBox1.Text = txtFile.getFileContent();
        }
        private void callBack()
        {
            if (CallBack != null)
                this.CallBack();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("保存更改?", "提示信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                txtFile.writeFile(richTextBox1.Text, ref bitmap);
                txtFile.updatedTime = DateTime.Now;
            }
            //txtFile.writeFile(richTextBox1.Text, ref bitmap);
            callBack();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
