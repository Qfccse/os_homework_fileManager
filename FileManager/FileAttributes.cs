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
    public partial class FileAttributes : Form
    {
        public FileAttributes(Node node)
        {
            InitializeComponent();
            if(node.nodeType==Node.NodeType.file)
            {
                this.pictureBox1.Image = this.imageList1.Images[0];
                this.textBox1.Text = node.file.name + ".txt";
                this.textBox2.Text = ".txt";
                this.textBox3.Text = node.file.path.Substring(0, node.file.path.Length-node.file.name.Length);
                this.textBox4.Text = node.file.size.ToString() + " B";
                this.textBox5.Text = node.file.createdTime.ToString();
                this.textBox6.Text = node.file.updatedTime.ToString();
            }
            else
            {
                this.pictureBox1.Image = this.imageList1.Images[1];
                this.textBox1.Text = node.folder.name;
                this.textBox2.Text = "文件夹";
                this.textBox3.Text = node.folder.path.Substring(0, node.folder.path.Length - node.folder.name.Length);
                this.textBox4.Text = node.folder.childrenNum.ToString() + "个子项";
                this.textBox5.Text = node.folder.createdTime.ToString();
                this.textBox6.Text = node.folder.updatedTime.ToString();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
