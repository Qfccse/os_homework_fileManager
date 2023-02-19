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
    public partial class FileCreator : Form
    {
        public Catalog curCatalog;
        String fileName;
        String fileType;
        public enum operationType { newfile, rename };
        public operationType operation_type;
        public DelegateMethod.delegateFunction CallBack;

        public FileCreator(ref Catalog currentCatalog, String name, String type, operationType otype)
        {
            InitializeComponent();
            curCatalog = currentCatalog;
            textBox1.Text = name;
            fileName = name;
            fileType = type;
            operation_type = otype;
        }


        private void callBack()
        {
            if (CallBack != null)
            {
                if (curCatalog.parenCatalog != null)
                {
                    curCatalog.parenCatalog.updatedTime = DateTime.Now;
                }
                this.CallBack();
            }
        }

        //检测重名
        private String nameCheck(String name, String type)
        {
            int counter = 0;
            if (type == "")
            {
                for (int i = 0; i < curCatalog.nodelist.Count(); i += 1)
                {
                    if (curCatalog.nodelist[i].nodeType == Node.NodeType.folder)
                    {
                        string[] sArray = curCatalog.nodelist[i].folder.name.Split('(');
                        if (sArray[0] == name)
                        {
                            counter++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < curCatalog.nodelist.Count(); i += 1)
                {
                    if (curCatalog.nodelist[i].nodeType == Node.NodeType.file)
                    {
                        string[] sArray = curCatalog.nodelist[i].file.name.Split('(');
                        if (sArray[0] == name)
                        {
                            counter++;
                        }
                    }
                }
            }
            if (counter > 0)
                name += "(" + counter.ToString() + ")";
            return name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String fatherPath = curCatalog.path;
            
            textBox1.Text = nameCheck(textBox1.Text, fileType);
            if (operation_type == operationType.newfile)
            {
                if (fileType == "")
                    curCatalog.addNode(curCatalog, textBox1.Text, fatherPath);
                else
                    curCatalog.addNode(textBox1.Text, fileType, fatherPath);
            }
            else if (operation_type == operationType.rename)
            {
                if (fileType == "")
                {
                    for (int i = 0; i < curCatalog.nodelist.Count(); i += 1)
                    {
                        if (curCatalog.nodelist[i].name == fileName && curCatalog.nodelist[i].nodeType == Node.NodeType.folder)
                        {
                            curCatalog.nodelist[i].reName(textBox1.Text);
                            break;
                        }
                    }
                }
                else if (fileType == "txt")
                    for (int i = 0; i < curCatalog.nodelist.Count(); i += 1)
                    {
                        if (curCatalog.nodelist[i].name == fileName && curCatalog.nodelist[i].nodeType == Node.NodeType.file)
                        {

                            curCatalog.nodelist[i].reName(textBox1.Text);
                            break;
                        }
                    }
            }
         
            callBack();
            this.Close();
        }
    }

    public class DelegateMethod
    {
        public delegate void delegateFunction();
    }
}
